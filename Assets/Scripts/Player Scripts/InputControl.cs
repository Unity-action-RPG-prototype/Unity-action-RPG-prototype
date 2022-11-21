using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;



public class InputControl : MonoBehaviour
{
    [SerializeField] PlayerInput _playerInput;
    [SerializeField] Animator _animator;
    [SerializeField] AnimationScript _animation;

    //inputs
    public Vector2 move;
    public Vector2 look;
    public bool sprint;
    public bool crouch;
    public bool climb;
    public bool interact;
    public bool pickpocket;
    public bool assassinate;
    public bool combat;
    public bool weapon;
    public bool jump;
    public bool attack;


    //accessible inputs
    public bool canMove;
    public bool canSprint;
    public bool canCrouch;
    public bool canClimb;
    public bool canJump;
    public bool canFight;
    public bool canAttack;
    
    //movement settings
    public bool analogMovement;

    //mouse cursor settings
    public bool cursorLocked = true;
    public bool cursorInputForLook = true;

    private void Start()
    {
        canMove = true;
        canSprint = true;
        canCrouch = true;
        canClimb = true;
        canJump = true;
        canFight = true;
        canAttack = false;
    }

    //input methods

    public void OnMove(InputValue inputValue)
    {
        if (canMove) Move(inputValue.Get<Vector2>());
  
    }

    public void OnLook(InputValue inputValue)
    {
        if (cursorInputForLook) Look(inputValue.Get<Vector2>());
    }
    public void OnAttack(InputValue inputValue)
    {
        if (canAttack) Attack(inputValue.isPressed);
    }

    public void OnJump(InputValue inputValue)
    {
        if (canJump) Jump(inputValue.isPressed);
    }

    public void OnSprint(InputValue inputValue)
    {
        if (canSprint) Sprint(inputValue.isPressed);
    }

    public void OnCrouch(InputValue inputValue)
    {
        if (canCrouch) Crouch(inputValue.isPressed);
    }

    public void OnClimb(InputValue inputValue)
    {
        StartCoroutine(Climb());
    }

    public void OnInteract(InputValue inputValue)
    {
        StartCoroutine(Interact());
    }

    public void OnPickpocket(InputValue inputValue)
    {
        StartCoroutine(Pickpocket());
    }

    public void OnAssassinate(InputValue inputValue)
    {
        StartCoroutine(Assassinate());
    }

    public void OnCombat(InputValue inputValue)
    {
        if (canFight) Combat(inputValue.isPressed);
    }

    //input called

    public void Move(Vector2 newMoveDirection)
    {
        move = newMoveDirection;
    }

    public void Look(Vector2 newLookDirection)
    {
        look = newLookDirection;
    }
    public void Attack(bool newAttackState)
    {
        attack = newAttackState;
        Debug.Log("attack is pressed");
    }

    public void Jump(bool newJumpState)
    {
        jump = newJumpState;
        Debug.Log("jump is pressed");
    }

    public void Sprint(bool newSprintState)
    {
        sprint = newSprintState;
        Debug.Log("sprint is pressed");
    }

    public void Crouch(bool newCrouchState)
    {
        crouch = !crouch;
        Debug.Log("crouch is pressed");
    }

    IEnumerator Climb()
    {
        climb = true;
        Debug.Log("climb is pressed");
        yield return new WaitForSeconds(0.5f);
        climb = false;
    }

    IEnumerator Interact()
    {
        interact = true;
        Debug.Log("interact is pressed");
        yield return new WaitForSeconds(0.5f);
        interact = false;
    }

    IEnumerator Pickpocket()
    {
        pickpocket = true;
        Debug.Log("pickpocket is pressed");
        yield return new WaitForSeconds(0.5f);
        pickpocket = false;
    }

    IEnumerator Assassinate()
    {
        assassinate = true;
        Debug.Log("assassinate is pressed");
        yield return new WaitForSeconds(0.5f);
        assassinate = false;
    }

    public void Combat(bool newCombatState)
    {
        combat = !combat;
        Debug.Log("combat is pressed");
    }

    private void OnApplicationFocus(bool hasFocus)
    {
        SetCursorState(cursorLocked);
    }

    private void SetCursorState(bool newState)
    {
        Cursor.lockState = newState ? CursorLockMode.Locked : CursorLockMode.None;
    }
}