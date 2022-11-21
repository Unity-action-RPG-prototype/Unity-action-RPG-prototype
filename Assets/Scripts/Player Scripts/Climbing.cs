/*using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Windows;

public class Climbing : MonoBehaviour
{
    [Header("References")]
    public Transform orientation;
    public InputControl _input;
    public CharacterController _controller;
    public PlayerMovement _playerMovement;
    public LayerMask ClimbableLayers;

    [Header("Climbing")]
    public float climbSpeed;
    public float maxClimbTime;
    private float climbTimer;

    private bool isClimbing;

    [Header("ClimbJumping")]
    public float climbJumpUpForce;
    public float climbJumpBackForce;

    public KeyCode jumpKey = KeyCode.Space;
    public int climbJumps;
    private int climbJumpsLeft;

    [Header("Detection")]
    public float detectionLength;
    public float sphereCastRadius;
    public float maxWallLookAngle;
    private float wallLookAngle;

    private RaycastHit frontWallHit;
    private bool wallFront;

    private Transform lastWall;
    private Vector3 lastWallNormal;
    public float minWallNormalAngleChange;

    [Header("Exiting")]
    public bool exitingWall;
    public float exitWallTime;
    private float exitWallTimer;

    private void Update()
    {
        WallCheck();
        StateMachine();

        if (isClimbing && !exitingWall) ClimbingMovement();
    }

    private void StateMachine()
    {
        // State 1 - Climbing
        if (wallFront && _input.climb && wallLookAngle < maxWallLookAngle && !exitingWall)
        {
            if (!isClimbing && climbTimer > 0) StartClimbing();

            // timer
            if (climbTimer > 0) climbTimer -= Time.deltaTime;
            if (climbTimer < 0) StopClimbing();
        }

        // State 2 - Exiting
        else if (exitingWall)
        {
            if (isClimbing) StopClimbing();

            if (exitWallTimer > 0) exitWallTimer -= Time.deltaTime;
            if (exitWallTimer < 0) exitingWall = false;
        }

        // State 3 - None
        else
        {
            if (isClimbing) StopClimbing();
        }

        if (wallFront && _input.jump && climbJumpsLeft > 0) ClimbJump();
    }

    private void WallCheck()
    {
        wallFront = Physics.SphereCast(transform.position, sphereCastRadius, orientation.forward, out frontWallHit, detectionLength, ClimbableLayers);
        wallLookAngle = Vector3.Angle(orientation.forward, -frontWallHit.normal);

        bool newWall = frontWallHit.transform != lastWall || Mathf.Abs(Vector3.Angle(lastWallNormal, frontWallHit.normal)) > minWallNormalAngleChange;

        if ((wallFront && newWall) || _playerMovement.isGrounded)
        {
            climbTimer = maxClimbTime;
            climbJumpsLeft = climbJumps;
        }
    }

    private void StartClimbing()
    {
        isClimbing = true;
        _playerMovement.isClimbing = true;

        lastWall = frontWallHit.transform;
        lastWallNormal = frontWallHit.normal;

        /// idea - camera fov change
    }

    private void ClimbingMovement()
    {
        //_controller.velocity = new Vector3(_controller.velocity.x, climbSpeed, _controller.velocity.z);
        Vector3 direction = transform.right * _input.move.x + transform.up * _input.move.y;
        /// idea - sound effect
    }

    private void StopClimbing()
    {
        isClimbing = false;
        _playerMovement.isClimbing = false;

        /// idea - particle effect
        /// idea - sound effect
    }

    private void ClimbJump()
    {
        exitingWall = true;
        exitWallTimer = exitWallTime;

        Vector3 forceToApply = transform.up * climbJumpUpForce + frontWallHit.normal * climbJumpBackForce;

        //_controller.velocity = new Vector3(_controller.velocity.x, 0f, _controller.velocity.z);
        //_controller.AddForce(forceToApply, ForceMode.Impulse);

        climbJumpsLeft--;
    }
}

*/