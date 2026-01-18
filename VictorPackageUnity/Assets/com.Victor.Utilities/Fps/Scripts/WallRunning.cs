
using UnityEngine;

namespace Fps_Handle.Scripts.Controller
{
    [RequireComponent(typeof(PlayerController))]
    [RequireComponent(typeof(Rigidbody))]
    public class WallRunning : MonoBehaviour
    {
        #region Serialized Fields

        [SerializeField] private WallRunningData data;

        #endregion

        #region Private Fields

        private float wallrunTimer;
        private float exitWallTimer;
        
        private float horizontalInput;
        private float verticalInput;
        private bool jumpPressed;
        private Vector2 moveInput;

        private RaycastHit leftWallhit;
        private RaycastHit rightWallhit;
        private bool wallLeft;
        private bool wallRight;

        private bool exitingWall;

        private PlayerController pc;
        private Rigidbody rb;
        private PlayerInputActions inputActions;
        private CameraController cameraController;

        #endregion

        #region Unity Lifecycle

        private void Awake()
        {
            InitializeComponents();
            SetupInputActions();
        }

        private void Start()
        {
            cameraController = CameraController.Instance;
        }

        private void Update()
        {
            CheckForWall();
            StateMachine();
        }

        private void FixedUpdate()
        {
            if (pc.GetWallRunning())
            {
                WallRunningMovement();
            }
        }

        private void OnDestroy()
        {
            CleanupInputActions();
        }

        #endregion

        #region Initialization

        private void InitializeComponents()
        {
            rb = GetComponent<Rigidbody>();
            pc = GetComponent<PlayerController>();
        }

        private void SetupInputActions()
        {
            inputActions = new PlayerInputActions();
            
            inputActions.Player.Move.performed += OnMovePerformed;
            inputActions.Player.Move.canceled += OnMoveCanceled;
            
            inputActions.Player.Jump.performed += OnJumpPerformed;
            inputActions.Player.Jump.canceled += OnJumpCanceled;
            
            inputActions.Enable();
        }

        private void CleanupInputActions()
        {
            if (inputActions != null)
            {
                inputActions.Player.Move.performed -= OnMovePerformed;
                inputActions.Player.Move.canceled -= OnMoveCanceled;
                
                inputActions.Player.Jump.performed -= OnJumpPerformed;
                inputActions.Player.Jump.canceled -= OnJumpCanceled;
                
                inputActions.Disable();
                inputActions.Dispose();
                inputActions = null;
            }
        }

        #endregion

        #region Input Callbacks

        private void OnMovePerformed(UnityEngine.InputSystem.InputAction.CallbackContext ctx) => moveInput = ctx.ReadValue<Vector2>();
        private void OnMoveCanceled(UnityEngine.InputSystem.InputAction.CallbackContext ctx) => moveInput = Vector2.zero;
        
        private void OnJumpPerformed(UnityEngine.InputSystem.InputAction.CallbackContext ctx) => jumpPressed = true;
        private void OnJumpCanceled(UnityEngine.InputSystem.InputAction.CallbackContext ctx) => jumpPressed = false;

        #endregion

        #region Wall Detection

        private void CheckForWall()
        {
            wallRight = Physics.Raycast(transform.position, transform.right,
                out rightWallhit, data.WallCheckDistance, data.WallLayer);
            
            wallLeft = Physics.Raycast(transform.position, -transform.right, 
                out leftWallhit, data.WallCheckDistance, data.WallLayer);
        }

        private bool AboveGround()
        {
            return !Physics.Raycast(transform.position, Vector3.down, 
                data.MinJumpHeight, data.GroundLayer);
        }

        #endregion

        #region State Machine

        private void StateMachine()
        {
            UpdateInput();

            if (CanStartWallRun())
            {
                if (!pc.GetWallRunning())
                {
                    StartWallRunning();
                }

                UpdateWallRunTimer();
                
                if (jumpPressed)
                {
                    WallJump();
                }
            }
            else if (exitingWall)
            {
                if (pc.GetWallRunning())
                {
                    StopWallRunning();
                }

                UpdateExitTimer();
            }
            else
            {
                if (pc.GetWallRunning())
                {
                    StopWallRunning();
                }
            }
        }

        private bool CanStartWallRun()
        {
            return (wallLeft || wallRight) 
                   && verticalInput > 0 
                   && AboveGround() 
                   && !exitingWall;
        }

        private void UpdateWallRunTimer()
        {
            if (wallrunTimer > 0)
            {
                wallrunTimer -= Time.deltaTime;
            }

            if (wallrunTimer <= 0 && pc.GetWallRunning())
            {
                exitingWall = true;
                exitWallTimer = data.ExitWallTime;
            }
        }

        private void UpdateExitTimer()
        {
            if (exitWallTimer > 0)
            {
                exitWallTimer -= Time.deltaTime;
            }

            if (exitWallTimer <= 0)
            {
                exitingWall = false;
            }
        }

        private void UpdateInput()
        {
            horizontalInput = moveInput.x;
            verticalInput = moveInput.y;
        }

        #endregion

        #region Wall Running

        private void StartWallRunning()
        {
            jumpPressed = false;
            wallrunTimer = data.MaxWallRunTime;
            pc.SetWallRunning(true);
            
            rb.linearVelocity = new Vector3(rb.linearVelocity.x, 0, rb.linearVelocity.z);
            
            if (cameraController != null)
            {
                cameraController.DoFov(data.WallRunFOV, data.CameraTransitionSpeed);
            }
        }

        private void StopWallRunning()
        {
            pc.SetWallRunning(false);
            
            if (cameraController != null)
            {
                cameraController.DoFov(data.NormalFOV, data.CameraTransitionSpeed);
            }
        }

        private void WallRunningMovement()
        {
            rb.useGravity = data.UseGravity;
            
            Vector3 wallNormal = wallRight ? rightWallhit.normal : leftWallhit.normal;
            Vector3 wallForward = Vector3.Cross(wallNormal, transform.up);

            if ((transform.forward - wallForward).magnitude > (transform.forward - -wallForward).magnitude)
            {
                wallForward = -wallForward;
            }
            
            rb.AddForce(wallForward * data.WallRunForce, ForceMode.Force);

            rb.linearVelocity = new Vector3(rb.linearVelocity.x, data.WallClimbSpeed, rb.linearVelocity.z);
            
            if (!(wallLeft && horizontalInput > 0) && !(wallRight && horizontalInput < 0))
            {
                rb.AddForce(-wallNormal * data.WallStickForce, ForceMode.Force);
            }

            if (data.UseGravity)
            {
                rb.AddForce(transform.up * data.GravityCounterForce, ForceMode.Force);
            }
        }

        #endregion

        #region Wall Jump

        private void WallJump()
        {
            exitingWall = true;
            exitWallTimer = data.ExitWallTime;
            
            Vector3 wallNormal = wallRight ? rightWallhit.normal : leftWallhit.normal;
            Vector3 forceToApply = transform.up * data.WallJumpUpForce + wallNormal * data.WallJumpSideForce;

            rb.linearVelocity = new Vector3(rb.linearVelocity.x, 0f, rb.linearVelocity.z);
            rb.AddForce(forceToApply, ForceMode.Impulse);
        }

        #endregion

        #region Gizmos

        private void OnDrawGizmosSelected()
        {
            if (!Application.isPlaying) return;

            Gizmos.color = wallRight ? Color.green : Color.red;
            Gizmos.DrawRay(transform.position, transform.right * data.WallCheckDistance);
            
            Gizmos.color = wallLeft ? Color.green : Color.red;
            Gizmos.DrawRay(transform.position, -transform.right * data.WallCheckDistance);
            
            Gizmos.color = Color.yellow;
            Gizmos.DrawRay(transform.position, Vector3.down * data.MinJumpHeight);
        }

        #endregion
    }
}