using System;
using UnityEngine;
using UnityEngine.Serialization;

public class CameraManager : MonoBehaviour
{
    #region Private Variables
    private float m_defaultPosition;
    private Vector3 m_cameraFollowVelocity = Vector3.zero;
    private Vector3 m_cameraVectorPosition;
    #endregion

    #region Transform References
    [Header("Transform References")]
    [Space(5)]
    [SerializeField, Tooltip("Target that the camera will follow (usually the player)")]
    private Transform m_targetTransform;

    [SerializeField, Tooltip("Pivot point for vertical camera rotation")]
    private Transform m_cameraPivot;

    [SerializeField, Tooltip("The actual camera transform")]
    private Transform m_cameraTransform;
    #endregion

    #region Component References
    [Header("Component References")]
    [Space(5)]
    [SerializeField, Tooltip("Reference to the InputManager component")]
    private InputManager m_inputManager;
    #endregion

    #region Camera Follow Settings
    [Header("Camera Follow Settings")]
    [Space(5)]
    [SerializeField, Tooltip("Smoothness of camera following (lower = smoother)")]
    private float cameraFollowSpeed = 0.2f;
    #endregion

    #region Camera Collision Settings
    [Header("Camera Collision Settings")]
    [Space(5)]
    [SerializeField, Tooltip("Radius of the sphere used for collision detection")]
    private float m_cameraCollisionRadius = 0.2f;

    [SerializeField, Tooltip("Offset applied during collision detection")]
    private float m_cameraCollisionOffSet = 0.2f;

    [SerializeField, Tooltip("Minimum distance the camera can be pushed forward")]
    private float m_minimumCollisionOffSet = 0.2f;

    [SerializeField, Tooltip("Layers that the camera will collide with")]
    private LayerMask m_collisionLayer;
    #endregion

    #region Camera Rotation Settings
    [Header("Camera Rotation Settings")]
    [Space(5)]
    [SerializeField, Tooltip("Speed of horizontal camera rotation")]
    private float m_cameraLookSpeed;

    [SerializeField, Tooltip("Speed of vertical camera rotation")]
    private float m_cameraPivotSpeed;

    [Space(10)]
    [SerializeField, Range(-90f, 0f), Tooltip("Maximum upward camera angle")]
    private float _minClampCameraPivot = -35f;

    [SerializeField, Range(0f, 90f), Tooltip("Maximum downward camera angle")]
    private float _maxClampCameraPivot = 35f;

    [Space(10)]
    [SerializeField, Tooltip("Current horizontal rotation angle")]
    private float lookAngle;

    [SerializeField, Tooltip("Current vertical rotation angle")]
    private float pivotAngle;
    #endregion

    private void Awake()
    {
        m_defaultPosition = m_cameraTransform.localPosition.z;
    }

    private void FollowTarget()
    {
        Vector3 targetPosition = Vector3.SmoothDamp(transform.position, m_targetTransform.position,
            ref m_cameraFollowVelocity, cameraFollowSpeed);
        transform.position = targetPosition;
    }

    private void RotateCamera()
    {
        Vector3 rotation;
        Quaternion targetRotation;
        lookAngle += (m_inputManager.CameraInputX * m_cameraLookSpeed);
        pivotAngle -= (m_inputManager.CameraInputY * m_cameraPivotSpeed);

        pivotAngle = Mathf.Clamp(pivotAngle, _minClampCameraPivot, _maxClampCameraPivot);
        
        rotation = Vector3.zero;
        rotation.y = lookAngle;
        targetRotation = Quaternion.Euler(rotation);
        transform.rotation = targetRotation;

        rotation = Vector3.zero;
        rotation.x = pivotAngle;
        targetRotation = Quaternion.Euler(rotation);

        m_cameraPivot.localRotation = targetRotation;
    }

    private void HandleCameraCollisions()
    {
        float targetPosition = m_defaultPosition;
        RaycastHit hit;
        Vector3 direction = m_cameraTransform.position - m_cameraPivot.position;
        direction.Normalize();

        if (Physics.SphereCast(m_cameraPivot.position, m_cameraCollisionRadius,direction, out hit, Mathf.Abs(targetPosition),m_collisionLayer))
        {
            float distance = Vector3.Distance(m_cameraPivot.position, hit.point);
            targetPosition =- (distance - m_cameraCollisionOffSet);
        }

        if (Mathf.Abs(targetPosition) < m_minimumCollisionOffSet)
        {
            targetPosition -= m_minimumCollisionOffSet;
        }

        m_cameraVectorPosition.z = Mathf.Lerp(m_cameraTransform.localPosition.z, targetPosition, 0.2f);
        m_cameraTransform.localPosition = m_cameraVectorPosition;
    }

    public void HandleAllCameraMovement()
    {
        FollowTarget();
        RotateCamera();
        HandleCameraCollisions();
    }
}
