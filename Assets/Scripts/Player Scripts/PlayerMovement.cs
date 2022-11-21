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
    public LayerMask ClimbLayers;
    public LayerMask ObstacleLayers;
    RaycastHit objectCollider;
    public bool obstacleDetected;
    public float heightOfObstacle;

    //inputs
    public bool inputMovePressed;
    public bool inputClimbTrue;
    public bool inputCrouchTrue;    
    public bool inputSprintTrue;
    public bool inputCombatTrue;
    public bool inputAttackTrue;

    //states
    public bool isMoving;
    public bool isSprinting;
    public bool isCrouched;
    public bool isClimbing;
    bool jumpToHang;
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
        GroundedCheck();
        ClimbCheck();
        ObstacleCheck();
    }

    private void Update()
    {
        targetSpeed = walkSpeed;

        //input accessibility
        canMove = _input.canMove;
        canSprint = _input.canSprint;
        canCrouch = _input.canCrouch;
        canFight = _input.canFight;
        canAttack = _input.canAttack;

        //input called
        inputMovePressed = _input.move != Vector2.zero;
        inputCombatTrue = _input.combat; //toggle
        inputCrouchTrue = _input.crouch; //toggle
        inputClimbTrue = _input.climb;
        inputSprintTrue = _input.sprint; //hold
        inputAttackTrue = _input.attack; //hold

        switch (currentState)
        {
            case STATE.DEFAULT:
                
                break;
            case STATE.SPRINT:
                
                break;
            case STATE.CROUCH:
                
                break;
            case STATE.CLIMB:
                canSprint = false;
                break;
            case STATE.HANG:
                canSprint = false;
                break;
            case STATE.COMBAT:
                _input.canAttack = true;
                break;
            case STATE.ATTACK:
                
                break;
            case STATE.DEAD:
                
                break;
            default:

                break;
        }

        totalStealth = baseStealth - noise;

        Move();
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

    private void ClimbCheck()
    {
        canClimb = Physics.Raycast(transform.position, transform.forward * 0.5f, out objectCollider, 0.5f, ClimbLayers);
        if (canClimb)
        {
            Debug.DrawRay(transform.position, transform.forward * 0.5f, Color.green, 0.5f);
            Debug.Log("can climb" + objectCollider.collider.name);
        }
        else
        {
            Debug.DrawRay(transform.position, transform.forward * 0.5f, Color.yellow, 0.5f);
        }
    }

    private void ObstacleCheck()
    {
        obstacleDetected = Physics.Raycast(_obstacleDetector.transform.position, _obstacleDetector.transform.up * -50f, out objectCollider, 50f, ObstacleLayers);
        if (obstacleDetected)
        {
            Debug.DrawRay(_obstacleDetector.transform.position, _obstacleDetector.transform.up * -50f, Color.green, 0.5f);
            Debug.Log("obstacle detected" + objectCollider.collider.name);

            Vector3 obstaclePosition = objectCollider.point;
            heightOfObstacle = Vector3.Distance(obstaclePosition, _groundReference.transform.position);
        }
        else
        {
            Debug.DrawRay(_obstacleDetector.transform.position, _obstacleDetector.transform.up * -50f, Color.yellow, 0.5f);
        }

        canHang = obstacleDetected && heightOfObstacle > 1f && heightOfObstacle < 3f;

        //to do list :
        //very small height : step over the obstacle
        //small height : hop over the obstacle
        //medium height : vault/jump over the obstacle
        //high height : climb over the obstacle
        //very high height : jump to climb over the obstacle
    }

    private void Move()
    {
        //move
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

        _controller.Move(movement.normalized * (speed * Time.deltaTime));

        //idle
        if (!inputMovePressed)
        {
            targetSpeed = 0;
        }

        //default
        if (!isSprinting && !isCrouched && !isFighting && !isClimbing)
        {
            ChangeState(STATE.DEFAULT);
        }

        //rotate on move
        if (canMove && inputMovePressed && !isClimbing && !isBracedHanging)
        {
            float rotation = Mathf.SmoothDampAngle(transform.eulerAngles.y, _mainCamera.transform.eulerAngles.y, ref rotationVelocity, rotationSpeed);
            Quaternion targetRotation = Quaternion.Euler(0, rotation, 0);
            transform.rotation = targetRotation;
        }

        //sprint
        if (canSprint && inputSprintTrue && !isClimbing && !inputCombatTrue)
        {
            if (inputMovePressed)
            {
                if(!noiseGenerated)
                {
                    StartCoroutine(CreateNoise());
                    Debug.Log("noise created");
                }
                ChangeState(STATE.SPRINT);
                isSprinting = true;
                targetSpeed = sprintSpeed;
                _input.crouch = false;
            }
            else
            {
                ChangeState(STATE.DEFAULT);
                isSprinting=false;
            }
        }

        //crouch
        if (canCrouch && inputCrouchTrue)
        {
            ChangeState(STATE.CROUCH);
            isCrouched = true;
            targetSpeed = inputMovePressed ? sneakSpeed : 0;
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
        
        //climb
        if (canClimb && inputSprintTrue)
        {
            ChangeState(STATE.CLIMB);
            isClimbing = true;
            _input.crouch = false;
            _input.combat = false;

            if (inputMovePressed)
            {
                targetSpeed = climbSpeed;
            }

        }
        else
        {
            ChangeState(STATE.DEFAULT);
            isClimbing = false;
        }

        //hang
        if (canHang)
        {
            canSprint = false;
            if (inputSprintTrue)
            {
                ChangeState(STATE.HANG);
                _input.crouch = false;
                _input.combat = false;

                if (inputMovePressed)
                {
                    targetSpeed = hangSpeed;
                }
            }
            //isBracedHanging = true;
        }
        else
        {
            ChangeState(STATE.DEFAULT);
            canSprint = true;
            //isBracedHanging = false;
        }

        //combat
        if (canFight && inputCombatTrue)
        {
            _unsheathedWeapon.SetActive(true);
            ChangeState(STATE.COMBAT);
            isFighting = true;
            _input.crouch = false;

            if(inputSprintTrue)
            {
                if (inputMovePressed)
                {
                    targetSpeed = sprintSpeed;
                    canAttack = false;
                    inputAttackTrue = false;
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
            _unsheathedWeapon.SetActive(false);
            isFighting = false;
        }

        //attack
        if (canAttack && inputAttackTrue && !isAttacking)
        {
            float rotation = Mathf.SmoothDampAngle(transform.eulerAngles.y, _mainCamera.transform.eulerAngles.y, ref rotationVelocity, rotationSpeed);
            Quaternion targetRotation = Quaternion.Euler(0, rotation, 0);
            transform.rotation = targetRotation;
            StartCoroutine(Attack());
        }

        //smooth speed change
        float currentHorizontalSpeed = new Vector3(_controller.velocity.x, 0.0f, _controller.velocity.z).magnitude;
        float speedOffset = 0.1f;
        float inputMagnitude = _input.analogMovement ? _input.move.magnitude : 1f;
        if (canMove)
        {
            if (currentHorizontalSpeed < targetSpeed - speedOffset ||
                currentHorizontalSpeed > targetSpeed + speedOffset)
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

        

        animationBlend = Mathf.Lerp(animationBlend, targetSpeed, Time.deltaTime * speedChangeRate);
        if (animationBlend < 0.01f) animationBlend = 0f;

        //animations
        _animator.SetFloat(_animation.speed, animationBlend);
        _animator.SetFloat(_animation.xAxis, _input.move.x);
        _animator.SetFloat(_animation.yAxis, _input.move.y);
        _animator.SetFloat(_animation.motionSpeed, inputMagnitude);
        if (canSprint && inputSprintTrue) _animator.SetBool(_animation.sprint, true);
        else _animator.SetBool(_animation.sprint, false);
        if (canClimb && inputSprintTrue) _animator.SetBool(_animation.climbLadder, true);
        else _animator.SetBool(_animation.climbLadder, false);
        if (canHang && inputSprintTrue)
        {
            isBracedHanging = true;
            _animator.SetBool(_animation.bracedHang, true);
            /*if (!isBracedHanging)
            {
                isBracedHanging = true;
                _animator.SetTrigger(_animation.jumpToBracedHang);
            }
            else
            {
                _animator.SetBool(_animation.bracedHang, true);
            }*/
        }
        else
        {
            _animator.SetBool(_animation.bracedHang, false);
            isBracedHanging = false;
        }
        
        if (canCrouch && inputCrouchTrue) _animator.SetBool(_animation.crouch, true);
        else _animator.SetBool(_animation.crouch, false);
        if (canFight && inputCombatTrue)
        {
            if (inputSprintTrue && inputMovePressed)
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
