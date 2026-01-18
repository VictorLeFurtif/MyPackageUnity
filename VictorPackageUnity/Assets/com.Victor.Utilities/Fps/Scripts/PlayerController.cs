using System.Collections;
using UnityEngine;

namespace Fps_Handle.Scripts.Controller
{
    public class PlayerController : MonoBehaviour
    {
        #region Serialized Fields

        [Header("Data")]
        [SerializeField] private PlayerControllerData data;
        
        [Header("Components")]
        [SerializeField] private Rigidbody rb;
        [SerializeField] private Collider playerCollider;
        [SerializeField] private Transform cameraPosition;
        
        [Header("Module Toggles")]
        [SerializeField] private bool enableWallRunning = true;
        [SerializeField] private bool enableSliding = true;

        #endregion

        #region Private Fields

        private float moveSpeed;
        private float desiredMoveSpeed;
        private float lastDesiredMoveSpeed;
        private Vector3 moveDirection;
        private float horizontalInput;
        private float verticalInput;

        private MovementState currentMovementState = MovementState.Walking;
        private bool grounded;
        private bool canSlide;
        private bool canMove = true;
        
        private bool readyToJump = true;
        private float jumpCooldownTimer;
        
        private RaycastHit slopeHit;
        private bool exitingSlope;
        
        private float startYScale;
        
        private PlayerInputActions inputActions;
        private Vector2 moveInput;
        private bool jumpPressed;
        private bool sprintHeld;
        private bool crouchHeld;
        private Vector2 lookInput;
        
        private CameraController cameraController;
        
        private bool frozen;
        private bool sliding;
        private bool wallRunning;

        #endregion

        #region Enums

        public enum MovementState
        {
            Walking,
            Sprinting,
            Air,
            Crouching,
            Sliding,
            WallRunning,
            Freeze
        }

        #endregion

        #region Unity Lifecycle

        private void Awake()
        {
            InitializeComponents();
            SetupInputActions();
        }

        private void Start()
        {
            InitializeReferences();
            ToggleCursor(true);
            ValidateModules();
        }

        private void Update()
        {
            Debug.Log($"your speed is {rb.linearVelocity}");
            
            if (!canMove) return;
            
            HandleInput();

            IsGrounded();
            UpdateCameraInput();
            StateHandler();
            UpdateJumpCooldown();
        }

        private void LateUpdate()
        {
            UpdateCamera();
        }

        private void FixedUpdate()
        {
            MovePlayer();
        }

        private void OnDestroy()
        {
            CleanupInputActions();
            ToggleCursor(false);
        }

        #endregion

        #region Initialization

        private void InitializeComponents()
        {
            if (rb == null)
                rb = GetComponent<Rigidbody>();

            if (playerCollider == null)
                playerCollider = GetComponentInChildren<Collider>();

            rb.freezeRotation = true;
            rb.linearDamping = 0f;
            startYScale = transform.localScale.y;
        }

        private void InitializeReferences()
        {
            cameraController = CameraController.Instance;
            
            if (cameraController != null)
            {
                cameraController.Initialize(cameraPosition);
            }
            else
            {
                Debug.LogError("[PlayerController] CameraController not found!");
            }
        }

        private void ValidateModules()
        {
            if (!enableWallRunning)
            {
                WallRunning wallRun = GetComponent<WallRunning>();
                if (wallRun != null)
                {
                    wallRun.enabled = false;
                    Debug.Log("[PlayerController] Wall Running module disabled");
                }
            }

            if (!enableSliding)
            {
                Sliding slide = GetComponent<Sliding>();
                if (slide != null)
                {
                    slide.enabled = false;
                    Debug.Log("[PlayerController] Sliding module disabled");
                }
            }
        }

        private void SetupInputActions()
        {
            inputActions = new PlayerInputActions();
            
            inputActions.Player.Move.performed += OnMovePerformed;
            inputActions.Player.Move.canceled += OnMoveCanceled;
            
            inputActions.Player.Look.performed += OnLookPerformed;
            inputActions.Player.Look.canceled += OnLookCanceled;

            inputActions.Player.Jump.performed += OnJumpPerformed;
            inputActions.Player.Jump.canceled += OnJumpCanceled;

            inputActions.Player.Sprint.performed += OnSprintPerformed;
            inputActions.Player.Sprint.canceled += OnSprintCanceled;

            inputActions.Player.Crouch.performed += OnCrouchPerformed;
            inputActions.Player.Crouch.canceled += OnCrouchCanceled;
            
            inputActions.Enable();
        }

        private void CleanupInputActions()
        {
            if (inputActions != null)
            {
                inputActions.Player.Move.performed -= OnMovePerformed;
                inputActions.Player.Move.canceled -= OnMoveCanceled;
                
                inputActions.Player.Look.performed -= OnLookPerformed;
                inputActions.Player.Look.canceled -= OnLookCanceled;

                inputActions.Player.Jump.performed -= OnJumpPerformed;
                inputActions.Player.Jump.canceled -= OnJumpCanceled;

                inputActions.Player.Sprint.performed -= OnSprintPerformed;
                inputActions.Player.Sprint.canceled -= OnSprintCanceled;

                inputActions.Player.Crouch.performed -= OnCrouchPerformed;
                inputActions.Player.Crouch.canceled -= OnCrouchCanceled;
                
                inputActions.Disable();
                inputActions.Dispose();
                inputActions = null;
            }
        }

        private void ToggleCursor(bool lockCursor)
        {
            Cursor.lockState = lockCursor ? CursorLockMode.Locked : CursorLockMode.None;
            Cursor.visible = !lockCursor;
        }

        #endregion

        #region Input Callbacks

        private void OnMovePerformed(UnityEngine.InputSystem.InputAction.CallbackContext ctx) => moveInput = ctx.ReadValue<Vector2>();
        private void OnMoveCanceled(UnityEngine.InputSystem.InputAction.CallbackContext ctx) => moveInput = Vector2.zero;
        
        private void OnLookPerformed(UnityEngine.InputSystem.InputAction.CallbackContext ctx) => lookInput = ctx.ReadValue<Vector2>();
        private void OnLookCanceled(UnityEngine.InputSystem.InputAction.CallbackContext ctx) => lookInput = Vector2.zero;
        
        private void OnJumpPerformed(UnityEngine.InputSystem.InputAction.CallbackContext ctx) => jumpPressed = true;
        private void OnJumpCanceled(UnityEngine.InputSystem.InputAction.CallbackContext ctx) => jumpPressed = false;
        
        private void OnSprintPerformed(UnityEngine.InputSystem.InputAction.CallbackContext ctx) => sprintHeld = true;
        private void OnSprintCanceled(UnityEngine.InputSystem.InputAction.CallbackContext ctx) => sprintHeld = false;
        
        private void OnCrouchPerformed(UnityEngine.InputSystem.InputAction.CallbackContext ctx)
        {
            crouchHeld = true;
            transform.localScale = new Vector3(transform.localScale.x, data.CrouchYScale, transform.localScale.z);
            rb.AddForce(Vector3.down * data.CrouchDownForce, ForceMode.Impulse);
        }
        
        private void OnCrouchCanceled(UnityEngine.InputSystem.InputAction.CallbackContext ctx)
        {
            crouchHeld = false;
            transform.localScale = new Vector3(transform.localScale.x, startYScale, transform.localScale.z);
        }

        #endregion

        #region Input Handling

        private void HandleInput()
        {
            horizontalInput = moveInput.x;
            verticalInput = moveInput.y;

            if (jumpPressed && CanJump())
            {
                Jump();
            }
        }

        private bool CanJump()
        {
            return readyToJump 
                   && grounded 
                   && currentMovementState != MovementState.Crouching
                   && jumpCooldownTimer <= 0f;
        }

        private void UpdateJumpCooldown()
        {
            if (jumpCooldownTimer > 0f)
            {
                jumpCooldownTimer -= Time.deltaTime;
                
                if (jumpCooldownTimer <= 0f)
                {
                    readyToJump = true;
                    exitingSlope = false;
                }
            }
        }

        #endregion

        #region Movement

        private void MovePlayer()
        {
            moveDirection = transform.forward * verticalInput + transform.right * horizontalInput;
            
            Vector3 targetVelocity = CalculateTargetVelocity();
            Vector3 currentVelocity = rb.linearVelocity;
            
            float acceleration = grounded ? data.GroundAcceleration : data.AirAcceleration;
            
            Vector3 newVelocity = new Vector3(
                Mathf.Lerp(currentVelocity.x, targetVelocity.x, acceleration * Time.fixedDeltaTime),
                currentVelocity.y,
                Mathf.Lerp(currentVelocity.z, targetVelocity.z, acceleration * Time.fixedDeltaTime)
            );
            
            rb.linearVelocity = newVelocity;
            
            //UpdateSpeedEffect(new Vector3(rb.linearVelocity.x, 0f, rb.linearVelocity.z).magnitude);
        }

        private Vector3 CalculateTargetVelocity()
        {
            if (OnSlope() && !exitingSlope)
            {
                Vector3 slopeDirection = GetSlopeMoveDirection(moveDirection);
                return slopeDirection * moveSpeed;
            }
            
            return moveDirection.normalized * moveSpeed;
        }

        private void UpdateSpeedEffect(float currentSpeed)
        {
            if (cameraController == null) return;

            if (currentSpeed > data.SprintSpeed)
            {
                if (!cameraController.EffectSpeedActive())
                {
                    cameraController.ToggleSpeedEffect(true);
                }
            }
            else
            {
                cameraController.ToggleSpeedEffect(false);
            }
        }

        private void Jump()
        {
            exitingSlope = true;
            readyToJump = false;
            jumpPressed = false;
            jumpCooldownTimer = data.JumpCooldown;
            
            rb.linearVelocity = new Vector3(rb.linearVelocity.x, 0f, rb.linearVelocity.z);
            rb.AddForce(transform.up * data.JumpForce, ForceMode.Impulse);
        }

        #endregion

        #region State Management

        private void StateHandler()
        {
            if (frozen)
            {
                currentMovementState = MovementState.Freeze;
                desiredMoveSpeed = 0;
                rb.linearVelocity = Vector3.zero;
            }
            else if (wallRunning && enableWallRunning)
            {
                currentMovementState = MovementState.WallRunning;
                desiredMoveSpeed = data.WallRunningSpeed;
            }
            else if (sliding && enableSliding)
            {
                currentMovementState = MovementState.Sliding;
                desiredMoveSpeed = OnSlope() && rb.linearVelocity.y < 0.1f 
                    ? data.SlideSpeed 
                    : data.SprintSpeed;
            }
            else if (crouchHeld)
            {
                currentMovementState = MovementState.Crouching;
                desiredMoveSpeed = data.CrouchSpeed;
            }
            else if (grounded && sprintHeld)
            {
                currentMovementState = MovementState.Sprinting;
                desiredMoveSpeed = data.SprintSpeed;
            }
            else if (grounded)
            {
                currentMovementState = MovementState.Walking;
                desiredMoveSpeed = data.WalkSpeed;
            }
            else
            {
                currentMovementState = MovementState.Air;
            }

            if (Mathf.Abs(desiredMoveSpeed - lastDesiredMoveSpeed) > data.SpeedChangeThreshold && moveSpeed != 0)
            {
                StopAllCoroutines();
                StartCoroutine(SmoothlyLerpMoveSpeed());
            }
            else
            {
                moveSpeed = desiredMoveSpeed;
            }

            lastDesiredMoveSpeed = desiredMoveSpeed;
        }

        private IEnumerator SmoothlyLerpMoveSpeed()
        {
            float time = 0;
            float difference = Mathf.Abs(desiredMoveSpeed - moveSpeed);
            float startValue = moveSpeed;

            while (time < difference)
            {
                moveSpeed = Mathf.Lerp(startValue, desiredMoveSpeed, time / difference);

                if (OnSlope())
                {
                    float slopeAngle = Vector3.Angle(Vector3.up, slopeHit.normal);
                    float slopeAngleIncrease = 1 + (slopeAngle / 90f);
                    time += Time.deltaTime * data.SpeedIncreaseMultiplier * data.SlopeIncreaseMultiplier * slopeAngleIncrease;
                }
                else
                {
                    time += Time.deltaTime * data.SpeedIncreaseMultiplier;
                }
                
                yield return null;
            }
            
            moveSpeed = desiredMoveSpeed;
        }

        #endregion

        #region Ground & Slope Detection

        public bool IsGrounded() 
        {
            grounded = Physics.Raycast(transform.position, Vector3.down, 
                data.PlayerHeight * 0.5f + data.GroundCheckDistance, data.GroundLayer);
            return grounded;
        }
        
        public bool CanSlide() 
        {
            canSlide = Physics.Raycast(transform.position, Vector3.down, 
                data.PlayerHeight + data.SlideCheckDistance, data.GroundLayer);
            return canSlide;
        }

        public bool OnSlope()
        { 
            if (Physics.Raycast(transform.position, Vector3.down, out slopeHit, 
                data.PlayerHeight * 0.5f + data.GroundCheckDistance))
            {
                float angle = Vector3.Angle(Vector3.up, slopeHit.normal);
                return angle < data.MaxSlopeAngle && angle != 0;
            }

            return false;
        }

        public Vector3 GetSlopeMoveDirection(Vector3 direction)
        {
            return Vector3.ProjectOnPlane(direction, slopeHit.normal).normalized;
        }

        #endregion

        #region Camera

        private void UpdateCameraInput()
        {
            if (cameraController != null)
            {
                cameraController.InputMouse(lookInput, data.SensX, data.SensY);
            }
        }

        private void UpdateCamera()
        {
            if (cameraController != null)
            {
                cameraController.MouseController(transform);
            }
        }

        #endregion

        #region Public API

        public void SetMove(bool value) => canMove = value;
        public void SetSliding(bool value) => sliding = value;
        public void SetWallRunning(bool value) => wallRunning = value;
        public void SetCollider(bool value) => playerCollider.enabled = value;
        
        public bool GetSliding() => sliding;
        public bool GetWallRunning() => wallRunning;
        public MovementState GetMovementState() => currentMovementState;
        public Rigidbody GetPlayerRigidbody() => rb;
        
        public bool IsWallRunningEnabled() => enableWallRunning;
        public bool IsSlidingEnabled() => enableSliding;

        public void ResetVelocity()
        {
            rb.linearVelocity = Vector3.zero;
            rb.angularVelocity = Vector3.zero;
        }

        #endregion
    }
}