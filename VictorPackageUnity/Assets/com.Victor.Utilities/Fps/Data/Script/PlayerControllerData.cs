using UnityEngine;

/// <summary>
    /// Configuration pour le contrôleur principal du joueur
    /// </summary>
    [CreateAssetMenu(fileName = "PlayerControllerData", menuName = "FPS/Player Controller Data")]
    public class PlayerControllerData : ScriptableObject
    {
        [Header("Movement Speeds")]
        [Tooltip("Vitesse de marche normale")]
        public float WalkSpeed = 7f;
        
        [Tooltip("Vitesse en sprint")]
        public float SprintSpeed = 10f;
        
        [Tooltip("Vitesse en crouch")]
        public float CrouchSpeed = 3.5f;
        
        [Tooltip("Vitesse de slide")]
        public float SlideSpeed = 15f;
        
        [Tooltip("Vitesse de wall running")]
        public float WallRunningSpeed = 8.5f;

        [Header("Acceleration")]
        [Tooltip("Accélération au sol (recommandé: 10-30)")]
        [Range(1f, 50f)]
        public float GroundAcceleration = 20f;
        
        [Tooltip("Accélération en l'air (recommandé: 5-15)")]
        [Range(1f, 50f)]
        public float AirAcceleration = 10f;

        [Header("Jump Settings")]
        [Tooltip("Force du saut")]
        public float JumpForce = 12f;
        
        [Tooltip("Cooldown entre les sauts")]
        public float JumpCooldown = 0.25f;

        [Header("Ground Detection")]
        [Tooltip("Hauteur du joueur pour les checks")]
        public float PlayerHeight = 2f;
        
        [Tooltip("Distance additionnelle pour le ground check")]
        public float GroundCheckDistance = 0.2f;
        
        [Tooltip("Distance pour vérifier si on peut slide")]
        public float SlideCheckDistance = 3f;
        
        [Tooltip("Layer du sol")]
        public LayerMask GroundLayer;

        [Header("Physics")]
        [Tooltip("Multiplicateur de force en l'air (DEPRECATED - non utilisé)")]
        [Range(0f, 1f)]
        public float AirMultiplier = 0.4f;
        
        [Tooltip("Drag au sol (DEPRECATED - doit être à 0)")]
        public float GroundDrag = 0f;
        
        [Tooltip("Multiplicateur de force au sol (DEPRECATED - non utilisé)")]
        public float GroundForceMultiplier = 9f;
        
        [Tooltip("Multiplicateur de force sur les pentes (DEPRECATED - non utilisé)")]
        public float SlopeForceMultiplier = 10f;

        [Header("Slope Settings")]
        [Tooltip("Angle maximum des pentes praticables")]
        [Range(0f, 90f)]
        public float MaxSlopeAngle = 40f;
        
        [Tooltip("Multiplicateur d'augmentation de vitesse sur pente")]
        public float SlopeIncreaseMultiplier = 2.5f;

        [Header("Crouch")]
        [Tooltip("Échelle Y en crouch")]
        [Range(0.1f, 1f)]
        public float CrouchYScale = 0.5f;
        
        [Tooltip("Force vers le bas en crouch")]
        public float CrouchDownForce = 5f;

        [Header("Speed Transitions")]
        [Tooltip("Multiplicateur de vitesse de transition")]
        public float SpeedIncreaseMultiplier = 10f;
        
        [Tooltip("Seuil de changement de vitesse pour transition smooth")]
        public float SpeedChangeThreshold = 4f;

        [Header("Camera")]
        [Tooltip("Sensibilité horizontale")]
        public float SensX = 400f;
        
        [Tooltip("Sensibilité verticale")]
        public float SensY = 400f;
    }