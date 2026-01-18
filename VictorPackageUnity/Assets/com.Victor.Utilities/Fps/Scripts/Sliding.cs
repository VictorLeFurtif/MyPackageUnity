
using UnityEngine;

namespace Fps_Handle.Scripts.Controller
{
    [RequireComponent(typeof(PlayerController))]
    [RequireComponent(typeof(Rigidbody))]
    public class Sliding : MonoBehaviour
    {
        #region Serialized Fields

        [Header("References")]
        [SerializeField] private Transform orientation;
        [SerializeField] private Transform playerObj;
        [SerializeField] private SlidingData data;

        #endregion

        #region Private Fields

        private Rigidbody rb;
        private PlayerController pc;
        private CameraController cameraController;
        
        private PlayerInputActions inputActions;
        private Vector2 moveInput;
        private float horizontalInput;
        private float verticalInput;
        
        private float slideTimer;
        private float startYScale;

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
            UpdateInput();
        }

        private void FixedUpdate()
        {
            if (pc.GetSliding())
            {
                SlidingMovement();
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
            startYScale = playerObj.localScale.y;
        }

        private void SetupInputActions()
        {
            inputActions = new PlayerInputActions();
            
            inputActions.Player.Move.performed += OnMovePerformed;
            inputActions.Player.Move.canceled += OnMoveCanceled;
            
            inputActions.Player.Slide.performed += OnSlidePerformed;
            inputActions.Player.Slide.canceled += OnSlideCanceled;
            
            inputActions.Enable();
        }

        private void CleanupInputActions()
        {
            if (inputActions != null)
            {
                inputActions.Player.Move.performed -= OnMovePerformed;
                inputActions.Player.Move.canceled -= OnMoveCanceled;
                
                inputActions.Player.Slide.performed -= OnSlidePerformed;
                inputActions.Player.Slide.canceled -= OnSlideCanceled;
                
                inputActions.Disable();
                inputActions.Dispose();
                inputActions = null;
            }
        }

        #endregion

        #region Input Callbacks

        private void OnMovePerformed(UnityEngine.InputSystem.InputAction.CallbackContext ctx) => moveInput = ctx.ReadValue<Vector2>();
        private void OnMoveCanceled(UnityEngine.InputSystem.InputAction.CallbackContext ctx) => moveInput = Vector2.zero;
        
        private void OnSlidePerformed(UnityEngine.InputSystem.InputAction.CallbackContext ctx)
        {
            if (CanStartSlide())
            {
                StartSlide();
            }
        }
        
        private void OnSlideCanceled(UnityEngine.InputSystem.InputAction.CallbackContext ctx)
        {
            if (pc.GetSliding())
            {
                StopSlide();
            }
        }

        #endregion

        #region Input Handling

        private void UpdateInput()
        {
            horizontalInput = moveInput.x;
            verticalInput = moveInput.y;
        }

        private bool CanStartSlide()
        {
            return (horizontalInput != 0 || verticalInput != 0) 
                   && !pc.GetWallRunning() 
                   && pc.CanSlide();
        }

        #endregion

        #region Sliding Logic

        private void StartSlide()
        {
            pc.SetSliding(true);
            
            playerObj.localScale = new Vector3(playerObj.localScale.x, data.SlideYScale, playerObj.localScale.z);
            
            rb.AddForce(Vector3.down * data.SlideDownForce, ForceMode.Impulse);
            
            slideTimer = data.MaxSlideTime;

            if (cameraController != null)
            {
                cameraController.DoFov(data.SlideFOV, data.CameraTransitionSpeed);
            }
        }

        private void SlidingMovement()
        {
            Vector3 inputDirection = orientation.forward * verticalInput + orientation.right * horizontalInput;

            if (!pc.OnSlope() || rb.linearVelocity.y > -0.1f)
            {
                rb.AddForce(inputDirection.normalized * data.SlideForce, ForceMode.Force);
                slideTimer -= Time.deltaTime;
            }
            else
            {
                rb.AddForce(pc.GetSlopeMoveDirection(inputDirection) * data.SlideForce, ForceMode.Force);
            }
            
            if (slideTimer <= 0)
            {
                StopSlide();
            }
        }
        
        private void StopSlide()
        {
            pc.SetSliding(false);
            
            playerObj.localScale = new Vector3(playerObj.localScale.x, startYScale, playerObj.localScale.z);

            if (cameraController != null)
            {
                cameraController.DoFov(data.NormalFOV, data.CameraTransitionSpeed);
            }
        }

        #endregion

        #region Gizmos

        private void OnDrawGizmosSelected()
        {
            if (!Application.isPlaying || pc == null) return;

            Gizmos.color = pc.CanSlide() ? Color.green : Color.red;
            Gizmos.DrawRay(transform.position, Vector3.down * (data.SlideCheckDistance + 1f));
        }

        #endregion
    }
}