using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;


public class PlayerMovement : MonoBehaviour
{
    // references
    [SerializeField] PlayerStatsReference _playerStatsRef;
    
    [SerializeField] Animator _animator;
    [SerializeField] CharacterController _controller;
    [SerializeField] InputControl _input;
    [SerializeField] CameraMovement _cameraMovement;

    [SerializeField] AnimationScript _animation;

    [SerializeField] GameObject _mainCamera;
    [SerializeField] GameObject _playerCameraRoot;
    [SerializeField] GameObject _sheathedWeapon;
    [SerializeField] GameObject _unsheathedWeapon;
    [SerializeField] GameObject _playerHead;
    [SerializeField] GameObject _interactor;
    [SerializeField] GameObject _obstacleDetector;
    [SerializeField] GameObject _groundReference;
    [SerializeField] public GameObject _noiseSpot;

    Vector3 movement;
    Vector3 velocity;
    public float walkSpeed = 2f;
    public float sprintSpeed = 6f;
    public float sneakSpeed = 1f;
    public float climbSpeed = 3f;
    public float hangSpeed = 0.5f;

    public float targetSpeed;
    [Range(0.0f, 0.3f)]
    public float rotationSpeed = 0.12f;
    private float rotationVelocity;
    public float speedChangeRate = 10.0f;
    public float speed;
    private float animationBlend;
    public float verticalVelocity;
    private float terminalVelocity = 53.0f;

    public float attackCoolDown = 3.0f;
    public float attackRadius;
    public float attackAngle = 30.0f;

    public AudioClip LandingAudioClip;
    public AudioClip[] FootstepAudioClips;
    [Range(0, 1)] 
    public float FootstepAudioVolume = 0.5f;

    //physics
    [Space(10)]
    public float jumpHeight = 1.2f;
    public float gravity;
    [Space(10)]
    public float jumpTimeout = 0.50f;
    public float fallTimeout = 0.15f;
    public bool isGrounded = true;
    public float groundedOffset = -0.14f;
    public float groundedRadius = 0.28f;
    private float jumpTimeoutDelta;
    private float fallTimeoutDelta;

    public LayerMask GroundLayers;
    public LayerMask ObstacleLayers;
    RaycastHit objectCollider;
    public bool obstacleDetected;
    public float heightOfObstacle;



    //states
    public bool isMoving;
    public bool isSprinting;
    public bool isCrouched;
    public bool isVaultingOver;
    public bool isClimbing;
    public bool jumpToHang;
    public bool isBracedHanging;
    public bool isFreeHanging;
    public bool isFighting;
    public bool isAttacking;
    public bool isHidden;
    public bool isDead;
    
    //accessible states
    public bool canMove;
    public bool canSprint;
    public bool canCrouch;
    public bool canClimb;
    public bool canHang;
    public bool canVaultOver;
    public bool canVaultUnder;
    public bool canFight;
    public bool canAttack;


    public float totalStealth;
    public float baseStealth;
    public float crouchStealth;
    public float noise;
    public bool noiseGenerated;
    public Vector3 noiseLocation;

    public enum STATE
    {
        DEFAULT, 
        CROUCH, 
        SPRINT, 
        CLIMB,
        HANG,
        FREEHANG,
        COMBAT, 
        ATTACK,
        DEFEND, 
        DAMAGED, 
        DEAD
    }
    public STATE currentState = STATE.DEFAULT;

    public enum STATUS
    {
        ANONYMOUS, //enemies are not aware of the player
        SUSPICIOUS, //enemies will be aware of the player if they draw attention
        COMPROMISED //enemies are aware of the player and will engage them on sight
    }
    public STATUS currentStatus = STATUS.ANONYMOUS;

    private void Awake()
    {
        _playerStatsRef.PlayerMovement = this;
    }

    private void Start()
    {
        baseStealth = _playerStatsRef.baseStealth;
        crouchStealth = _playerStatsRef.crouchStealth;
        jumpToHang = false;
        jumpTimeoutDelta = jumpTimeout;
        fallTimeoutDelta = fallTimeout;
        gravity = -15.0f;

        isDead = false;
    }

    private void FixedUpdate()
    {
        _controller.Move(movement.normalized * (speed * Time.deltaTime));
        GroundedCheck();
        ObstacleCheck();
    }

    private void Update()
    {
        targetSpeed = walkSpeed;

        switch (currentState)
        {
            case STATE.DEFAULT:
                
                break;
            case STATE.SPRINT:
                
                break;
            case STATE.CROUCH:
                
                break;
            case STATE.CLIMB:

                break;
            case STATE.HANG:

                break;
            case STATE.COMBAT:

                break;
            case STATE.ATTACK:
                
                break;
            case STATE.DEAD:
                
                break;
            default:

                break;
        }

        totalStealth = baseStealth - noise;

        Input();
        JumpAndGravity();
        Death();

    }

    

    private void GroundedCheck()
    {
        Vector3 spherePosition = new Vector3(transform.position.x, transform.position.y - groundedOffset,
            transform.position.z);

        if (!isClimbing)
        {
            isGrounded = Physics.CheckSphere(spherePosition, groundedRadius, GroundLayers, QueryTriggerInteraction.Ignore);
            //isGrounded = Physics.Raycast(transform.position, transform.up * -0.5f, out var ground, 0.5f, GroundLayers);
        }
        else
        {
            isGrounded = true;
        }

        _animator.SetBool(_animation.grounded, isGrounded);
    }

    private void ObstacleCheck()
    {
        obstacleDetected = Physics.Raycast(_obstacleDetector.transform.position, _obstacleDetector.transform.up * -50f, out objectCollider, 50f, ObstacleLayers);
        if (obstacleDetected)
        {
            Debug.DrawRay(_obstacleDetector.transform.position, _obstacleDetector.transform.up * -50f, Color.green, 0.5f);
            Debug.Log("obstacle detected" + objectCollider.collider.name);

            /*Vector3 obstaclePosition = objectCollider.point;
            heightOfObstacle = Vector3.Distance(obstaclePosition, _groundReference.transform.position);*/

            if (objectCollider.collider.gameObject.layer == LayerMask.NameToLayer("Vault"))
                {
                    canHang = true;
                }
            if (objectCollider.collider.gameObject.layer == LayerMask.NameToLayer("Ledge"))
                {
                    canVaultOver = true;
                }
        }
        else
        {
            Debug.DrawRay(_obstacleDetector.transform.position, _obstacleDetector.transform.up * -50f, Color.yellow, 0.5f);
        }


        

        //to do list :
        //very small height : step over the obstacle
        //small height : hop over the obstacle
        //medium height : vault/jump over the obstacle
        //high height : climb over the obstacle
        //very high height : jump to climb over the obstacle
    }

    private void Input()
    {
        //default
        if (!isSprinting && !isCrouched && !isClimbing && !isBracedHanging && !isFighting)
        {
            ChangeState(STATE.DEFAULT);
        }

        //movement
        if (isClimbing)
        {
            movement = transform.up * _input.move.y;
        }
        else if (isBracedHanging)
        {
            movement = transform.right * _input.move.x;
        }
        else
        {
            movement = transform.right * _input.move.x + transform.forward * _input.move.y;
        }

        //idle
        if (_input.move == Vector2.zero)
        {
            targetSpeed = 0;
        }

        //rotate on move
        if (_input.canMove && _input.move != Vector2.zero && !isClimbing && !isBracedHanging)
        {
            float rotation = Mathf.SmoothDampAngle(transform.eulerAngles.y, _mainCamera.transform.eulerAngles.y, ref rotationVelocity, rotationSpeed);
            Quaternion targetRotation = Quaternion.Euler(0, rotation, 0);
            transform.rotation = targetRotation;
        }

        //freerun
        if (_input.sprint)
        {
            if (canClimb) //climb
            {
                ChangeState(STATE.CLIMB);
                isClimbing = true;
                _input.crouch = false;
                _input.combat = false;

                if (_input.move != Vector2.zero)
                {
                    targetSpeed = climbSpeed;
                }
            }
            else if (canHang) //hang
            {
                ChangeState(STATE.HANG);
                isBracedHanging = true;
                _input.crouch = false;
                _input.combat = false;

                if (_input.move != Vector2.zero)
                {
                    targetSpeed = hangSpeed;
                }
            }
            else if (canVaultOver) //vault over
            {
                
                isVaultingOver = true;
                _input.crouch = false;
                _input.combat = false;

                if (_input.move != Vector2.zero)
                {
                    targetSpeed = hangSpeed;
                }
            }
            else if (_input.move != Vector2.zero) //sprint
            {
                ChangeState(STATE.SPRINT);
                isSprinting = true;
                targetSpeed = sprintSpeed;
                _input.crouch = false;

                if (!noiseGenerated)
                {
                    StartCoroutine(CreateNoise());
                    Debug.Log("noise created");
                }
            }
            else
            {
                isClimbing = false;
                isBracedHanging = false;
                isSprinting = false;
            }
        }
        else
        {
            isClimbing = false;
            isBracedHanging = false;
            isSprinting = false;
        }

        //crouch
        if (_input.crouch)
        {
            ChangeState(STATE.CROUCH);
            isCrouched = true;
            targetSpeed = _input.move != Vector2.zero ? sneakSpeed : 0;
            _playerCameraRoot.transform.position = new Vector3(transform.position.x, transform.position.y + 0.8f, transform.position.z);
            _interactor.transform.position = new Vector3(transform.position.x, transform.position.y + 0.8f, transform.position.z);
            _playerHead.transform.position = new Vector3(transform.position.x, transform.position.y + 0.8f, transform.position.z);
        }
        else
        {
            isCrouched = false;
            _playerCameraRoot.transform.position = new Vector3(transform.position.x, transform.position.y + 1.2f, transform.position.z);
            _interactor.transform.position = new Vector3(transform.position.x, transform.position.y + 1.5f, transform.position.z);
            _playerHead.transform.position = new Vector3(transform.position.x, transform.position.y + 1.5f, transform.position.z);
        }

        //combat
        if (_input.combat && _input.canFight)
        {
            ChangeState(STATE.COMBAT);
            isFighting = true;
            _unsheathedWeapon.SetActive(true);
            _input.crouch = false;
            _input.canAttack = true;
            if (_input.sprint)
            {
                if (_input.move != Vector2.zero)
                {
                    targetSpeed = sprintSpeed;
                    canAttack = false;
                    _input.attack = false;
                }
                else
                {
                    targetSpeed = 0;
                    canAttack = true;
                }
            }     
        }
        else
        {
            _input.canAttack = false;
            isFighting = false;
            _unsheathedWeapon.SetActive(false);
        }

        //attack
        if (_input.attack)
        {
            if (!isAttacking)
            {
                float rotation = Mathf.SmoothDampAngle(transform.eulerAngles.y, _mainCamera.transform.eulerAngles.y, ref rotationVelocity, rotationSpeed);
                Quaternion targetRotation = Quaternion.Euler(0, rotation, 0);
                transform.rotation = targetRotation;
                StartCoroutine(Attack());
            }
        }

        //smooth speed change
        float currentHorizontalSpeed = new Vector3(_controller.velocity.x, 0.0f, _controller.velocity.z).magnitude;
        float speedOffset = 0.1f;
        float inputMagnitude = _input.analogMovement ? _input.move.magnitude : 1f;
        if (_input.canMove)
        {
            if (currentHorizontalSpeed < targetSpeed - speedOffset
                || currentHorizontalSpeed > targetSpeed + speedOffset)
            {
                speed = Mathf.Lerp(currentHorizontalSpeed, targetSpeed * inputMagnitude,
                    Time.deltaTime * speedChangeRate);
                speed = Mathf.Round(speed * 1000f) / 1000f;
            }
            else
            {
                speed = targetSpeed;
            }
        }
        else
        {
            speed = 0.0f;
        }

        //animations

        animationBlend = Mathf.Lerp(animationBlend, targetSpeed, Time.deltaTime * speedChangeRate);
        if (animationBlend < 0.01f) animationBlend = 0f;
        
        _animator.SetFloat(_animation.speed, animationBlend);
        _animator.SetFloat(_animation.xAxis, _input.move.x);
        _animator.SetFloat(_animation.yAxis, _input.move.y);
        _animator.SetFloat(_animation.motionSpeed, inputMagnitude);

        if (isSprinting) _animator.SetBool(_animation.sprint, true);
        else _animator.SetBool(_animation.sprint, false);

        if (isClimbing) _animator.SetBool(_animation.climbLadder, true);
        else _animator.SetBool(_animation.climbLadder, false);

        if (isBracedHanging) _animator.SetBool(_animation.bracedHang, true);   
        else _animator.SetBool(_animation.bracedHang, false);

        if (isCrouched) _animator.SetBool(_animation.crouch, true);
        else _animator.SetBool(_animation.crouch, false);

        if (isFighting)
        {
            if (_input.sprint && _input.move != Vector2.zero)
            {
                _animator.SetBool(_animation.sprint, true);
                _animator.SetBool(_animation.combat, false);
            }
            else
            {
                _animator.SetBool(_animation.combat, true);
                _animator.SetBool(_animation.sprint, false);
            }
        }
        else
        {
            _animator.SetBool(_animation.combat, false);
        }
    }

    IEnumerator Attack()
    {
        isAttacking = true;
        canAttack = false;
        ChangeState(STATE.ATTACK);

        _animator.SetTrigger(_animation.attack1);
        Debug.Log("attack 1");

        yield return new WaitForSeconds(attackCoolDown);

        isAttacking = false;
        canAttack = true;
        ChangeState(STATE.COMBAT);
    }

    IEnumerator CreateNoise()
    {
        noiseGenerated = true;
        Instantiate(_noiseSpot, transform.position, transform.rotation);
        noiseLocation = _noiseSpot.transform.position;
        yield return new WaitForSeconds(0.2f);
        noiseGenerated = false;
    }
    

    private void JumpAndGravity()
    {

        if (isGrounded || isClimbing)
        {

            fallTimeoutDelta = fallTimeout;
            _animator.SetBool(_animation.jump, false);
            _animator.SetBool(_animation.freeFall, false);
            // stop our velocity dropping infinitely when grounded
            if (verticalVelocity < 0.0f)
            {
                verticalVelocity = -2f;
            }
            if (_input.jump && jumpTimeoutDelta <= 0.0f)
            {
                // the square root of H * -2 * G = how much velocity needed to reach desired height
                verticalVelocity = Mathf.Sqrt(jumpHeight * -2f * gravity);
                _animator.SetBool(_animation.jump, true);
            }
            if (jumpTimeoutDelta >= 0.0f)
            {
                jumpTimeoutDelta -= Time.deltaTime;
            }
        }

        if (!isGrounded && !isClimbing)
        {
            velocity.y += gravity * Time.deltaTime;
            _controller.Move(velocity * Time.deltaTime);

            _input.jump = false;
            jumpTimeoutDelta = jumpTimeout;
            if (fallTimeoutDelta >= 0.0f)
            {
                fallTimeoutDelta -= Time.deltaTime;
            }
            else
            {
                _animator.SetBool(_animation.freeFall, true);
            }

        }
        
        if (isClimbing)
        {
            if (_input.move.y > 0)
            {
                verticalVelocity = 1;
            }
            else if (_input.move.y > 0)
            {
                verticalVelocity = -1;
            }
            else if (_input.move.y == 0)
            {
                verticalVelocity = 0;
            }


        }

        if (verticalVelocity < terminalVelocity && !isClimbing)
        {
            verticalVelocity += gravity * Time.deltaTime;
        }

    }

    public void Death()
    {
        if (_playerStatsRef.currentHealth <= 0)
            if (!isDead)
            {
                ChangeState(STATE.DEAD);
                _animator.SetTrigger(_animation.death);
                isDead = true;
            }
    }

    public void ChangeState(STATE newState)
    {
        currentState = newState;
    }

    public void ChangeStatus(STATUS newStatus)
    {
        currentStatus = newStatus;
    }

    private void OnDrawGizmosSelected()
    {
        Color transparentGreen = new Color(0.0f, 1.0f, 0.0f, 0.35f);
        Color transparentRed = new Color(1.0f, 0.0f, 0.0f, 0.35f);

        if (isGrounded) Gizmos.color = transparentGreen;
        else Gizmos.color = transparentRed;

        // when selected, draw a gizmo in the position of, and matching radius of, the grounded collider
        Gizmos.DrawSphere(
            new Vector3(transform.position.x, transform.position.y - groundedOffset, transform.position.z),
            groundedRadius);
    }

    /*private void OnFootstep(AnimationEvent animationEvent)
    {
        if (animationEvent.animatorClipInfo.weight > 0.5f)
        {
            if (FootstepAudioClips.Length > 0)
            {
                var index = Random.Range(0, FootstepAudioClips.Length);
                AudioSource.PlayClipAtPoint(FootstepAudioClips[index], transform.TransformPoint(_controller.center), FootstepAudioVolume);
            }
        }
    }*/

    /*private void OnLand(AnimationEvent animationEvent)
    {
        if (animationEvent.animatorClipInfo.weight > 0.5f)
        {
            AudioSource.PlayClipAtPoint(LandingAudioClip, transform.TransformPoint(_controller.center), FootstepAudioVolume);
        }
    }*/
}
