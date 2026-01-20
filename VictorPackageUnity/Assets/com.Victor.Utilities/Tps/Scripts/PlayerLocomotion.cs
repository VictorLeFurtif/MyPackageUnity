using System;
using Manager;
using UnityEditor;
using UnityEngine;
using UnityEngine.Serialization;
using Object = UnityEngine.Object;

public class PlayerLocomotion : MonoBehaviour
{
    #region Private Variables
    private Vector3 m_moveDirection;
    private Transform m_cameraObject;
    private float m_inAirTimer;
    #endregion

    #region Public Properties
    public bool IsSprinting = false;
    public bool IsGrounded = false;
    public bool IsJumping = false;
    #endregion

    #region Component References
    [Header("Component References")]
    [Space(5)]
    [SerializeField, Tooltip("Reference to the InputManager component")]
    private InputManager m_inputManager;

    [SerializeField, Tooltip("Reference to the PlayerManager component")]
    private PlayerManager m_playerManager;

    [SerializeField, Tooltip("Reference to the Animator component")]
    private AnimatorManager m_animatorManager;

    [SerializeField, Tooltip("Reference to the main camera")]
    private Camera m_camera;

    [SerializeField, Tooltip("Reference to the Rigidbody component")]
    private Rigidbody m_rb;
    #endregion

    #region Movement Settings
    [Header("Movement Settings")]
    [Space(5)]
    [SerializeField, Range(0f, 5f), Tooltip("Walking speed in m/s")]
    private float m_walkingSpeed = 1.5f;

    [SerializeField, Range(0f, 10f), Tooltip("Running speed in m/s")]
    private float m_runningSpeed = 5f;

    [SerializeField, Range(0f, 15f), Tooltip("Sprinting speed in m/s")]
    private float m_sprintingSpeed = 7f;

    [Space(10)]
    [SerializeField, Range(1f, 30f), Tooltip("Speed of character rotation")]
    private float m_rotationSpeed = 15f;
    #endregion

    #region Jump & Gravity Settings
    [Header("Jump & Gravity Settings")]
    [Space(5)]
    [SerializeField, Range(-50f, 0f), Tooltip("Gravity intensity (higher = falls faster)")]
    private float m_gravityIntensity = -15f;

    [SerializeField, Range(0f, 10f), Tooltip("Maximum jump height in meters")]
    private float m_jumpHeight = 3f;

    [Space(10)]
    [SerializeField, Tooltip("Horizontal velocity while in air")]
    private float m_leapingVelocity;

    [SerializeField, Range(0f, 200f), Tooltip("Additional falling speed multiplier")]
    private float m_fallingSeed;
    #endregion

    #region Ground Detection Settings
    [Header("Ground Detection Settings")]
    [Space(5)]
    [SerializeField, Tooltip("Layer(s) considered as ground")]
    private LayerMask groundLayer;

    [SerializeField, Range(0f, 2f), Tooltip("Height offset for ground detection raycast origin")]
    private float raycastOriginHeightOffSet = 1.5f;

    [SerializeField, Range(0f, 2f), Tooltip("Maximum distance to detect ground")]
    private float m_maxDistance = 2f;
    #endregion

    private void Awake()
    {
        m_cameraObject = m_camera.transform;
    }

    private void HandleMovement()
    {
        if (IsJumping)
        {
            return;
        }
        
        m_moveDirection = m_cameraObject.forward * m_inputManager.VerticalInput;
        m_moveDirection += m_cameraObject.right * m_inputManager.HorizontalInput;
        m_moveDirection.Normalize();
        m_moveDirection.y = 0;

        if (IsSprinting)
        {
            m_moveDirection *= m_sprintingSpeed;
        }
        else
        {
            if (m_inputManager.MoveAmount >= 0.5f)
            {
                m_moveDirection *= m_runningSpeed;
            }
            else
            {
                m_moveDirection *= m_walkingSpeed;
            }
        }
        
        Vector3 movementVelocity = m_moveDirection;
        m_rb.linearVelocity = movementVelocity;
    }

    private void HandleRotation()
    {
        Vector3 targetDirection = Vector3.zero;

        targetDirection = m_cameraObject.forward * m_inputManager.VerticalInput;
        targetDirection += m_cameraObject.right * m_inputManager.HorizontalInput;
        targetDirection.Normalize();
        targetDirection.y = 0;

        if (targetDirection == Vector3.zero)
        {
            targetDirection = transform.forward;
        }
        
        Quaternion targetRotation = Quaternion.LookRotation(targetDirection);
        Quaternion playerRotation = Quaternion.Slerp
            (transform.rotation, targetRotation, m_rotationSpeed * Time.deltaTime);

        transform.rotation = playerRotation;
    }

    private void HandleFallingAndLanding()
    {
        RaycastHit hit;
        Vector3 raycastOrigin = transform.position;
        Vector3 targetPosition = transform.position;
        raycastOrigin.y += raycastOriginHeightOffSet;

        if (!IsGrounded && !IsJumping)
        {
            if (!m_playerManager.isInteracting)
            {
                m_animatorManager.PlayTargetAnimation("falling",true);
            }

            m_inAirTimer += Time.deltaTime;
            m_rb.AddForce(transform.forward * m_leapingVelocity);
            m_rb.AddForce(-Vector3.up * m_fallingSeed * m_inAirTimer);
        }

        if (Physics.SphereCast(raycastOrigin, 0.2f, -Vector3.up, out hit, m_maxDistance,groundLayer))
        {
            if (!IsGrounded && !m_playerManager.isInteracting)
            {
                m_animatorManager.PlayTargetAnimation("landing",true);
            }

            Vector3 rayCastHitPoint = hit.point;
            targetPosition.y = rayCastHitPoint.y;
            m_inAirTimer = 0;
            IsGrounded = true;
        }
        else
        {
            IsGrounded = false;
        }

        if (IsGrounded && !IsJumping)
        {
            if (m_playerManager.isInteracting || m_inputManager.MoveAmount > 0)
            {
                transform.position = Vector3.Lerp(transform.position, targetPosition, Time.deltaTime/0.1f);
            }
            else
            {
                transform.position = targetPosition;
            }
        }
    }

    public void HandleAllMovement()
    {
        HandleFallingAndLanding();
        
        if (m_playerManager.isInteracting)
        {
            return;
        }
        
        HandleMovement();
        HandleRotation();
    }

    public void HandleJump()
    {
        if (IsGrounded)
        {
            m_animatorManager.AnimatorEntity.SetBool("IsJumping",true);
            m_animatorManager.PlayTargetAnimation("jump",false);
            
            float jumpingVelocity = Mathf.Sqrt(-2 * m_gravityIntensity * m_jumpHeight);
            Vector3 playerVelocity = m_moveDirection;
            playerVelocity.y = jumpingVelocity;
            m_rb.linearVelocity = playerVelocity;
        }
    }
}
