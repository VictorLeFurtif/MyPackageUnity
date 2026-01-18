using UnityEngine;

/// <summary>
/// Configuration pour le sliding
/// </summary>
[CreateAssetMenu(fileName = "SlidingData", menuName = "FPS/Sliding Data")]
public class SlidingData : ScriptableObject
{
    [Header("Scale")]
    [Tooltip("Échelle Y pendant le slide")]
    [Range(0.1f, 1f)]
    public float SlideYScale = 0.5f;

    [Header("Forces")]
    [Tooltip("Force de slide")]
    public float SlideForce = 400f;
        
    [Tooltip("Force vers le bas au début du slide")]
    public float SlideDownForce = 5f;

    [Header("Timing")]
    [Tooltip("Durée maximale du slide")]
    public float MaxSlideTime = 1f;

    [Header("Detection")]
    [Tooltip("Distance de check pour le slide")]
    public float SlideCheckDistance = 3f;

    [Header("Camera")]
    [Tooltip("FOV pendant le slide")]
    public float SlideFOV = 100f;
        
    [Tooltip("FOV normal")]
    public float NormalFOV = 80f;
        
    [Tooltip("Vitesse de transition de la caméra")]
    public float CameraTransitionSpeed = 10f;
}