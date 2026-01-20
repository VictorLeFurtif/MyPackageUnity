using System;
using UnityEngine;

public class InputManager : MonoBehaviour
{
    #region Private Variables
    private PlayerControls m_playerControls;
    private Vector2 m_movementInput;
    private Vector2 m_cameraInput;
    private bool m_sprintingInputPressed;
    private bool m_jumpInput;
    #endregion

    #region Component References
    [Header("Component References")]
    [Space(5)]
    [SerializeField, Tooltip("Reference to the AnimatorManager component")]
    private AnimatorManager m_animatorManager;
    
    [SerializeField, Tooltip("Reference to the PlayerLocomotion component")]
    private PlayerLocomotion m_playerLocomotion;
    #endregion

    #region Public Properties
    public float MoveAmount { get; private set; }
    public float VerticalInput { get; private set; }
    public float HorizontalInput { get; private set; }
    public float CameraInputX { get; private set; }
    public float CameraInputY { get; private set; }
    #endregion
    
    private void OnEnable()
    {
        if (m_playerControls == null)
        {
            m_playerControls = new PlayerControls();
            m_playerControls.PlayerMovement.Movement.performed += i => m_movementInput = i.ReadValue<Vector2>();
            m_playerControls.PlayerMovement.Camera.performed += i => m_cameraInput = i.ReadValue<Vector2>();

            m_playerControls.PlayerActions.SprintsButton.performed += i => m_sprintingInputPressed = true;
            m_playerControls.PlayerActions.SprintsButton.canceled += i => m_sprintingInputPressed = false;
            
            m_playerControls.PlayerActions.Jump.performed += i => m_jumpInput = true;
        }
        
        m_playerControls.Enable();
    }
    private void OnDisable()
    {
        m_playerControls.Disable();
    }

    private void HandleMovementInput()
    {
        VerticalInput = m_movementInput.y;
        HorizontalInput = m_movementInput.x;

        CameraInputY = m_cameraInput.y;
        CameraInputX = m_cameraInput.x;
        
        
        MoveAmount = Mathf.Clamp01(Mathf.Abs(HorizontalInput) + Mathf.Abs(VerticalInput));
        m_animatorManager.UpdateAnimatorValues(0,MoveAmount,m_playerLocomotion.IsSprinting);
    }

    private void HandleSprintingInput()
    {
        if (m_sprintingInputPressed && MoveAmount > 0.5f)
        {
            m_playerLocomotion.IsSprinting = true;
        }
        else
        {
            m_playerLocomotion.IsSprinting = false;
        }
    }

    public void HandleAllInputs()
    {
        HandleMovementInput();
        HandleSprintingInput();
        HandleJumpingInput();
    }

    private void HandleJumpingInput()
    {
        if (m_jumpInput)
        {
            m_jumpInput = false;
            m_playerLocomotion.HandleJump();
        }
    }
}
