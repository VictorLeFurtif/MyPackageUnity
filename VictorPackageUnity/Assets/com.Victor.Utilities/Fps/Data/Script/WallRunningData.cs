using UnityEngine;

/// <summary>
/// Configuration pour le wall running
/// </summary>
[CreateAssetMenu(fileName = "WallRunningData", menuName = "FPS/Wall Running Data")]
public class WallRunningData : ScriptableObject
{
    [Header("Detection")]
    [Tooltip("Distance de détection des murs")]
    public float WallCheckDistance = 0.7f;
        
    [Tooltip("Hauteur minimale pour wall run")]
    public float MinJumpHeight = 1.5f;
        
    [Tooltip("Layer des murs")]
    public LayerMask WallLayer;
        
    [Tooltip("Layer du sol")]
    public LayerMask GroundLayer;

    [Header("Movement")]
    [Tooltip("Force de déplacement le long du mur")]
    public float WallRunForce = 200f;
        
    [Tooltip("Vitesse de montée sur le mur (contre la gravité)")]
    public float WallClimbSpeed = 3f;
        
    [Tooltip("Force pour coller au mur")]
    public float WallStickForce = 100f;

    [Header("Gravity")]
    [Tooltip("Utiliser la gravité pendant le wall run")]
    public bool UseGravity = true;
        
    [Tooltip("Force pour contrer la gravité")]
    public float GravityCounterForce = 300f;

    [Header("Timing")]
    [Tooltip("Durée maximale du wall run")]
    public float MaxWallRunTime = 1f;
        
    [Tooltip("Temps avant de pouvoir re-wall run")]
    public float ExitWallTime = 0.2f;

    [Header("Wall Jump")]
    [Tooltip("Force verticale du wall jump")]
    public float WallJumpUpForce = 7f;
        
    [Tooltip("Force horizontale du wall jump (éloignement du mur)")]
    public float WallJumpSideForce = 12f;

    [Header("Camera")]
    [Tooltip("FOV pendant le wall run")]
    public float WallRunFOV = 90f;
        
    [Tooltip("FOV normal")]
    public float NormalFOV = 80f;
        
    [Tooltip("Vitesse de transition de la caméra")]
    public float CameraTransitionSpeed = 10f;
}