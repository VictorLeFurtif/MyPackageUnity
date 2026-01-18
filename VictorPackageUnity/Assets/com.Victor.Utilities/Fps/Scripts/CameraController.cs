using UnityEngine;

namespace Fps_Handle.Scripts.Controller
{
    public class CameraController : MonoBehaviour
    {
        #region Singleton

        private static CameraController instance;
        public static CameraController Instance
        {
            get
            {
                if (instance == null)
                {
                    instance = FindObjectsByType<CameraController>(FindObjectsSortMode.None)[0];
                    
                    if (instance == null)
                    {
                        Debug.LogError("[CameraController] Aucune instance trouvée dans la scène !");
                    }
                }
                return instance;
            }
        }

        #endregion
        
        #region Serialized Fields

        [Header("Camera Components")]
        [SerializeField] private Camera playerCamera;
        [SerializeField] private Transform cameraHolder;
        
        [Header("Visual Effects")]
        [SerializeField] private GameObject cameraSpeedEffect;
        
        [Header("Rotation Settings")]
        [SerializeField] private float verticalLimit = 80f;

        [Header("FOV Settings")]
        [SerializeField] private float defaultFOV = 80f;
        [SerializeField] private float fovTransitionSpeed = 10f;

        #endregion
        
        #region Private Fields

        private float yaw;
        private float pitch;
        
        private Transform cameraTarget;
        
        private bool isEffectSpeedActive = false;
        
        private float targetFOV;
        private float currentFOV;

        #endregion
        
        #region Unity Lifecycle

        private void Awake()
        {
            SetupSingleton();
            ValidateComponents();
        }

        private void Start()
        {
            InitializeCamera();
        }

        private void Update()
        {
            UpdateFOV();
        }

        #endregion

        #region Initialization

        private void SetupSingleton()
        {
            if (instance == null)
            {
                instance = this;
            }
            else if (instance != this)
            {
                Debug.LogWarning("[CameraController] Instance dupliquée détectée, destruction de " + gameObject.name);
                Destroy(gameObject);
            }
        }

        private void ValidateComponents()
        {
            if (playerCamera == null)
            {
                playerCamera = GetComponentInChildren<Camera>();
                if (playerCamera == null)
                {
                    Debug.LogError("[CameraController] Aucune Camera trouvée !");
                }
            }

            if (cameraHolder == null)
            {
                cameraHolder = transform;
                Debug.LogWarning("[CameraController] CameraHolder non assigné, utilisation du transform principal");
            }
        }

        private void InitializeCamera()
        {
            ToggleSpeedEffect(false);
            
            if (playerCamera != null)
            {
                currentFOV = defaultFOV;
                targetFOV = defaultFOV;
                playerCamera.fieldOfView = defaultFOV;
            }
        }

        public void Initialize(Transform target)
        {
            cameraTarget = target;
            
            if (cameraTarget != null)
            {
                cameraHolder.position = cameraTarget.position;
            }
        }

        #endregion

        #region Input Handling

        public void InputMouse(Vector2 lookInput, float sensX, float sensY) 
        {
            float mouseX = lookInput.x * sensX * Time.deltaTime;
            float mouseY = lookInput.y * sensY * Time.deltaTime;

            yaw += mouseX;
            pitch -= mouseY;
            pitch = Mathf.Clamp(pitch, -verticalLimit, verticalLimit);
        }

        #endregion

        #region Camera Control

        public void MouseController(Transform orientation)
        {
            if (orientation == null || cameraTarget == null) return;

            orientation.rotation = Quaternion.Euler(0f, yaw, 0f);
            
            cameraHolder.rotation = Quaternion.Euler(pitch, yaw, 0f);
            
            cameraHolder.position = cameraTarget.position;
        }

        #endregion

        #region Camera Effects - FOV

        public void DoFov(float newTargetFOV, float transitionSpeed = -1f)
        {
            if (playerCamera == null) return;

            targetFOV = newTargetFOV;
            
            if (transitionSpeed > 0)
            {
                fovTransitionSpeed = transitionSpeed;
            }
        }

        private void UpdateFOV()
        {
            if (playerCamera == null) return;
            
            currentFOV = Mathf.Lerp(currentFOV, targetFOV, Time.deltaTime * fovTransitionSpeed);
            playerCamera.fieldOfView = currentFOV;
        }

        #endregion

        #region Speed Effect

        public void ToggleSpeedEffect(bool active)
        {
            if (cameraSpeedEffect != null)
            {
                cameraSpeedEffect.SetActive(active);
                isEffectSpeedActive = active;
            }
        }

        #endregion

        #region Reset

        public void ResetEffects()
        {
            DoFov(defaultFOV);
            ToggleSpeedEffect(false);
        }

        #endregion

        #region Utility

        public bool EffectSpeedActive() => isEffectSpeedActive;
        public Transform CameraTransform() => cameraHolder;
        public Camera GetCamera() => playerCamera;
        public float GetCurrentFOV() => currentFOV;
        public float GetTargetFOV() => targetFOV;

        #endregion
    }
}