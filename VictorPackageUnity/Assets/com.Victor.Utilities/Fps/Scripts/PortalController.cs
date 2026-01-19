using System;
using UnityEngine;
using UnityEngine.InputSystem;

public class PortalController : MonoBehaviour
{
    private PlayerInputActions inputActions = new PlayerInputActions();
    
    private void OnEnable()
    {
        inputActions.Player.Fire1.performed += SpawnPortal;
        inputActions.Player.Fire2.performed += SpawnPortal;
        inputActions.Enable();
    }

    private void SpawnPortal(InputAction.CallbackContext ctx)
    {
        
    }
}
