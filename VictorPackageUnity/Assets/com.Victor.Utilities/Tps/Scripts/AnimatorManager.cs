using System.Collections.Generic;
using UnityEngine;

public class AnimatorManager : MonoBehaviour
{
    #region Private Variables
    private int m_horizontalHashId;
    private int m_verticalHashId;
    #endregion

    [field: SerializeField]
    [field: Tooltip("Reference to the Animator component")]
    public Animator AnimatorEntity { get; private set; }

    #region Animation Settings
    [Header("Animation Settings")]
    [Space(5)]
    [SerializeField, Range(0f, 1f), Tooltip("Smoothness of animation blending (lower = smoother)")]
    private float m_blendTime = 0.1f;
    
    [Space(5)]
    [SerializeField, Tooltip("Enable snapping for discrete animation states")]
    private bool m_snappingAnimation;
    
    [SerializeField, Range(0.1f, 0.9f), Tooltip("Threshold for animation snapping")]
    private float m_snapThreshold = 0.55f;
    #endregion

    private void Awake()
    {
        m_horizontalHashId = Animator.StringToHash("Horizontal");
        m_verticalHashId = Animator.StringToHash("Vertical");
    }

    public void UpdateAnimatorValues(float horizontalMovement, float verticalMovement, bool isSprinting)
    {
        float snappedHorizontal = horizontalMovement;
        float snappedVertical = verticalMovement;

        if (isSprinting)
        {
            snappedHorizontal = horizontalMovement;
            snappedVertical = 2;
        }
        else if (m_snappingAnimation)
        {
            snappedHorizontal = SnappedAnimation(horizontalMovement);
            snappedVertical = SnappedAnimation(verticalMovement);
        }
        
        AnimatorEntity.SetFloat(m_horizontalHashId, snappedHorizontal, m_blendTime, Time.deltaTime);
        AnimatorEntity.SetFloat(m_verticalHashId, snappedVertical, m_blendTime, Time.deltaTime);
    }

    public void PlayTargetAnimation(string targetAnimation, bool isInteracting)
    {
        AnimatorEntity.SetBool("IsInteracting", isInteracting);
        AnimatorEntity.CrossFade(targetAnimation, 0.2f);
    }

    private float SnappedAnimation(float value)
    {
        if (value > m_snapThreshold) return 1f;
        if (value > 0) return 0.5f;
        if (value < -m_snapThreshold) return -1f;
        if (value < 0) return -0.5f;
        return 0f;
    }

}