using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.Serialization;

// TODO: resolve all JetBrains warnings
// TODO: comment code and make terminology consistent
// TODO: optional slowmo (hold RMB) for easier aiming
// TODO: disable mouse

public class PlayerController : MonoBehaviour
{
    [Header("Objects")] public CharacterController controller;
    public Animator animator;
    public Camera camera;
    public InputHandler inputHandler;

    [Header("Controls")] public float playerSpeed = 5.0f;
    public float jumpHeight = 10f;
    public float gravityMultiplier = 3.0f;
    public float maxFallSpeed = -25f;

    [Header("Grapple")]
    private Ray _tetherRaycast;
    private LineRenderer _lineRenderer;
    
    private const float TetherMinLength = 1f;
    private const float TetherMaxLength = 10f;
    private const float TetherRetractExtendSpeed = 10f;
    private const float TetherShootPauseSeconds = 0.1f;
    
    private float _tetherCurrentLength;
    private float _distanceToGrapplePoint;
    private Vector3 _grapplePoint;
    private bool _tetherExtending = false;

    [Header("Smoothing")]
    // [Range(0, 1)] 
    // public float speedDampTime = 0.1f;
    // [Range(0, 1)] 
    // public float velocityDampTime = 0.9f;
    // [Range(0, 1)] 
    // public float rotationDampTime = 0.2f;
    [Range(0, 1)]
    public float airControl = 0.75f;

    [Header("State Machine")] public StateMachine movementSM;

    public IdleState idleState;
    public MoveState moveState;
    public JumpState jumpState;
    public FallState fallState;
    public InteractState interactState;
    public SwingState swingState;

    [HideInInspector] public float gravityValue = -9.8f;
    [HideInInspector] public Vector3 direction;

    // TODO: seperate internal and external velocity so the player can jump out of swing and air move without canceling momentum.
    public Vector3 velocity;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    private void Start()
    {
        _tetherCurrentLength = TetherMaxLength;
        camera = GetComponentInChildren<Camera>();
        _lineRenderer = GetComponent<LineRenderer>();
        _lineRenderer.enabled = false;
    
        _tetherRaycast = new Ray(transform.position, camera.transform.forward);
        
        velocity.y = -2.0f;
        movementSM.Initialize(idleState);
    }

    private void Update()
    {
        if (_lineRenderer.enabled)
        {
            _lineRenderer.SetPosition(0, transform.position);
            if (_tetherExtending)
            {
                ExtendTether();
            }
        }

        if (inputHandler.WantsToGrapple())
        {
            TryGrapple();
        }

        if (inputHandler.WantsToRetract())
        {
            ExtendOrRetract(-Time.deltaTime * TetherRetractExtendSpeed);
        } else if (inputHandler.WantsToExtend())
        {
            ExtendOrRetract(Time.deltaTime * TetherRetractExtendSpeed);
        }

        LeashSelf();
    }

    public void TryGrapple()
    {
        if (_lineRenderer.enabled)
        {
            _grapplePoint = Vector3.zero;
            _lineRenderer.enabled = false;
            _tetherExtending = false;
            return;
        }
        _tetherRaycast.origin = transform.position;
        _tetherRaycast.direction = camera.transform.forward;
        Debug.DrawRay(transform.position, _tetherRaycast.direction * TetherMaxLength, Color.red);
        if (Physics.Raycast(_tetherRaycast, out RaycastHit hit, TetherMaxLength, 1))
        {
            _grapplePoint = hit.point;
            _lineRenderer.enabled = true;
            _lineRenderer.SetPosition(0, transform.position);
            _lineRenderer.SetPosition(1, transform.position);
            _tetherCurrentLength = (_grapplePoint - transform.position).magnitude;
            _tetherExtending = true;
            if (Time.timeScale == 0f)
            {
                return;
            }
            Time.timeScale = 0f;
            StartCoroutine(PauseGame(TetherShootPauseSeconds));
            

        }
    }

    public void ExtendTether()
    {
        _lineRenderer.SetPosition(1, 
            Vector3.Lerp(_lineRenderer.GetPosition(1), _grapplePoint, .05f)
            );
        if ((_lineRenderer.GetPosition(1) - _grapplePoint).magnitude < 0.5)
        {
            _tetherExtending = false;
        }
    }


    IEnumerator PauseGame(float seconds)
    {
        yield return new WaitForSecondsRealtime(seconds);
        Time.timeScale = 1f;
    }
     
    /**
     * Limits the player's position based on the current length of their tether, if enabled.
     */
    private void LeashSelf()
    {
        if (!_lineRenderer.enabled)
            return;

        Vector3 offset = transform.position - _grapplePoint;
        _distanceToGrapplePoint = offset.magnitude;

        if (_distanceToGrapplePoint > _tetherCurrentLength)
        {
            Vector3 targetPosition = _grapplePoint + offset.normalized * _tetherCurrentLength;

            Vector3 correction = targetPosition - transform.position;

            controller.Move(correction);
        }
    }

    /**
     * Extends or retracts the player's tether by the amount specified, capping out at a minimum/maximum length.
     */
    private void ExtendOrRetract(float distance)
    {
        _tetherCurrentLength += distance;
        if (_tetherCurrentLength < TetherMinLength)
        {
            _tetherCurrentLength = TetherMinLength;
        } else if (_tetherCurrentLength > TetherMaxLength)
        {
            _tetherCurrentLength = TetherMaxLength;
        }
    }

    public bool IsTethered()
    {
        return _lineRenderer.enabled;
    }

    public Vector3 GetGrapplePoint()
    {
        return _grapplePoint;
    }

    public bool TetherIsMaxLength()
    {
        return _lineRenderer.enabled && _distanceToGrapplePoint >= _tetherCurrentLength;
    }

    private void OnApplicationFocus(bool hasFocus)
    {
        Cursor.lockState = hasFocus ? CursorLockMode.Locked : CursorLockMode.None;
    }
}
