using System;
using System.Collections;
using UnityEngine;

// TODO: resolve all JetBrains warnings
// TODO: comment code and make terminology consistent

public class PlayerController : MonoBehaviour
{
    [Header("Objects")] public CharacterController controller;
    public Animator animator;
    public Camera camera;
    public InputHandler inputHandler;
    
    // private SurfaceSampler _SurfaceSampler;

    [Header("Controls")] public float playerSpeed = 5.0f;
    public float jumpHeight = 10f;
    public float gravityMultiplier = 3.0f;
    public float maxFallSpeed = -25f;

    [Header("Grapple")]
    private Ray _tetherRaycast;
    private LineRenderer _lineRenderer;
    private Material _lineMaterial;
    [SerializeField] private RandomAudioClip _crystalChimeRandomClip;
    
    private const float TetherMinLength = 1f;
    private const float TetherMaxLength = 15f;
    private const float TetherRetractExtendSpeed = 10f;
    private const float TetherShootPauseSeconds = 0.1f;

    private const float AimAssistRadius = 8f;

    private readonly Color _maxTensionColor = Color.saddleBrown;
    private readonly Color _slackColor = Color.softYellow;
    private const float TetherLengthToFullSlack = 2.5f;
    
    private float _tetherCurrentLength;
    private float _distanceToGrapplePoint;
    private Vector3 _grapplePoint;
    private bool _tetherExtending;

    [Header("Smoothing")]
    // [Range(0, 1)] 
    // public float speedDampTime = 0.1f;
    // [Range(0, 1)] 
    // public float velocityDampTime = 0.9f;
    // [Range(0, 1)] 
    // public float rotationDampTime = 0.2f;
    [Range(0, 1)]
    public float airControl = 0.75f;

    [Header("State Machine")] public StateMachine movementSm;

    public IdleState idleState;
    public MoveState moveState;
    public JumpState jumpState;
    public FallState fallState;
    public InteractState interactState;
    public SwingState swingState;

    [HideInInspector] public float gravityValue = -9.8f;
    [HideInInspector] public Vector3 direction;

    public Vector3 velocity;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    private void Start()
    {
        _tetherCurrentLength = TetherMaxLength;
        camera = GetComponentInChildren<Camera>();
        _lineRenderer = GetComponent<LineRenderer>();
        _lineMaterial = _lineRenderer.material;
        _lineRenderer.enabled = false;

        // _SurfaceSampler = GetComponent<SurfaceSampler>();
    
        _tetherRaycast = new Ray(transform.position, camera.transform.forward);
        
        velocity.y = -2.0f;
        movementSm.Initialize(idleState);
    }

    private void Update()
    {
        // Update line render visuals
        if (_lineRenderer.enabled)
        {
            UpdateLineColor();
            _lineRenderer.SetPosition(0, transform.position);
            if (_tetherExtending)
            {
                ExtendTether();
            }
        }

        // Handle slowmo
        if (Time.timeScale != 0f)
        {
            Time.timeScale = inputHandler.WantsToSlowmo() ? 0.2f : 1.0f;
        }
        
        // Grapple controls
        if (inputHandler.WantsToGrapple())
        {
            ToggleGrapple();
        }

        if (inputHandler.WantsToRetract())
        {
            ExtendOrRetract(-Time.deltaTime * TetherRetractExtendSpeed);
        } else if (inputHandler.WantsToExtend())
        {
            ExtendOrRetract(Time.deltaTime * TetherRetractExtendSpeed);
        }

        LeashSelf();

        print(SurfaceSampler.GetSurfaceTypeFromTerrain(gameObject.transform.position, Terrain.activeTerrain));
    }

    void UpdateLineColor()
    {
        _lineMaterial.color = Color.Lerp(_maxTensionColor, _slackColor, (_tetherCurrentLength - _distanceToGrapplePoint)/TetherLengthToFullSlack);
    }

    public void ToggleGrapple()
    {
        if (_lineRenderer.enabled)
        {
            _grapplePoint = Vector3.negativeInfinity;
            _lineRenderer.enabled = false;
            _tetherExtending = false;
            return;
        }
        _tetherRaycast.origin = transform.position;
        _tetherRaycast.direction = camera.transform.forward;
        // Debug.DrawRay(transform.position, _tetherRaycast.direction * TetherMaxLength, Color.red);

        if (!SearchForGrapplePoint(out _grapplePoint))
        {
            return;
        }
        AudioSource.PlayClipAtPoint(_crystalChimeRandomClip.GetClip(), _grapplePoint);
        _lineRenderer.enabled = true;
        _tetherExtending = true;
        _lineRenderer.SetPosition(0, transform.position);
        _lineRenderer.SetPosition(1, transform.position);
        _tetherCurrentLength = (_grapplePoint - transform.position).magnitude;
        if (Time.timeScale == 0f)
        {
            return;
        }
        Time.timeScale = 0f;
        StartCoroutine(PauseGame(TetherShootPauseSeconds));
    }

    private bool SearchForGrapplePoint(out Vector3 grapplePoint)
    {
        grapplePoint = Vector3.zero;

        Vector3 searchEndPoint;
        
        if (Physics.Raycast(_tetherRaycast, out RaycastHit hit, TetherMaxLength, 1 | 1 << 6))
        {
            if (hit.collider.gameObject.layer == 6)
            { // We hit a crystal, so return the hit point as is.
                grapplePoint = hit.point;
                return true;
            }
            
            if (SearchInSphereForCollider(out Collider c, hit.point, 1 << 6))
            { // There's a collider in range at the hit point, so return that.
                grapplePoint = c.gameObject.transform.position;
                return true;
            }

            searchEndPoint = hit.point; 
        }
        else
        {
            searchEndPoint = gameObject.transform.position + (_tetherRaycast.direction * TetherMaxLength);
        }
        
        // Finally, try an iterative search from player to end point for last aim assist check.
        Vector3 searchPoint = gameObject.transform.position;
        Vector3 searchDirection = _tetherRaycast.direction;

        int searchSteps = (int) ( (searchPoint - searchEndPoint).magnitude / AimAssistRadius );
        
        for (int i = 0; i < searchSteps; i++)
        {
            searchPoint += searchDirection * (AimAssistRadius * 2);
            if (SearchInSphereForCollider(out Collider c, searchPoint, 1 << 6))
            {
                grapplePoint = c.gameObject.transform.position;
                return true;
            }
        }
        
        return false;
    }

    private bool SearchInSphereForCollider(out Collider collider, Vector3 searchPosition, int layerMask)
    {
        collider = null;
        Collider[] nearbyColliders = new Collider[10];
        int colliderCount = Physics.OverlapSphereNonAlloc(searchPosition, AimAssistRadius, nearbyColliders, layerMask);
            
        if (colliderCount == 0)
        {
            return false;
        }


        Collider closestCollider = nearbyColliders[0];
        float closestDistance = float.PositiveInfinity;

        for (int i = 0; i < colliderCount; i++)
        {
            Collider c = nearbyColliders[i];
            float distance = (searchPosition - c.gameObject.transform.position).magnitude;
            if (distance < closestDistance)
            {
                closestCollider = c;
                closestDistance = distance;
            }
        }

        collider = closestCollider;
        return true;
    }

    /**
     * Extends the line renderer until it reaches the grapple point.
     */
    private void ExtendTether()
    {
        _lineRenderer.SetPosition(1, 
            Vector3.Lerp(_lineRenderer.GetPosition(1), _grapplePoint, Time.unscaledDeltaTime * 10f)
            );
        if ((_lineRenderer.GetPosition(1) - _grapplePoint).magnitude < 0.5)
        {
            _tetherExtending = false;
        }
    }


    private static IEnumerator PauseGame(float seconds)
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
