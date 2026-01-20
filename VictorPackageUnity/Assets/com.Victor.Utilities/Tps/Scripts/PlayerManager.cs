using System;
using UnityEngine;

namespace Manager
{
    public class PlayerManager : MonoBehaviour
    {
        #region Component References
        [Header("Component References")]
        [Space(5)]
        [SerializeField, Tooltip("Reference to the InputManager component")]
        private InputManager m_inputManager;

        [SerializeField, Tooltip("Reference to the PlayerLocomotion component")]
        private PlayerLocomotion m_playerLocomotion;

        [SerializeField, Tooltip("Reference to the CameraManager component")]
        private CameraManager m_cameraManager;

        [SerializeField, Tooltip("Reference to the Animator component")]
        private Animator m_animator;
        #endregion

        #region Public Properties
        public bool isInteracting { get; private set; }
        #endregion

        private void Update()
        {
            m_inputManager.HandleAllInputs();
        }

        private void FixedUpdate()
        {
            m_playerLocomotion.HandleAllMovement();
        }

        private void LateUpdate()
        {
            m_cameraManager.HandleAllCameraMovement();

            isInteracting = m_animator.GetBool("IsInteracting");
            m_playerLocomotion.IsJumping = m_animator.GetBool("IsJumping");
            m_animator.SetBool("IsGrounded",m_playerLocomotion.IsGrounded);
        }
    }
}