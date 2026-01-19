using System;
using UnityEngine;
using UnityEngine.Rendering;

public class Portal : MonoBehaviour
{
    [SerializeField] private Camera _camera;
    [SerializeField] private Portal otherPortal;
    [SerializeField] private Transform playerCamera;

    private void RotateCamera(ScriptableRenderContext context, Camera camera)
    {
        float angle = Quaternion.Angle(transform.rotation, otherPortal.transform.rotation);

        Quaternion angleToQuaternion = Quaternion.AngleAxis(angle, Vector3.up);

        Vector3 dir = angleToQuaternion * -playerCamera.forward;

        _camera.transform.rotation = Quaternion.LookRotation(new Vector3(dir.x,-dir.y,dir.z), Vector3.up);
        
    }

    private void MoveCamera(ScriptableRenderContext context, Camera camera)
    {
        Vector3 offset = playerCamera.position - otherPortal.transform.position;

        _camera.transform.position = transform.position + offset;
        
    }

    private void OnEnable()
    {
        RenderPipelineManager.beginCameraRendering += RotateCamera;
        RenderPipelineManager.beginCameraRendering += MoveCamera;
    }

    private void OnDisable()
    {
        RenderPipelineManager.beginCameraRendering -= RotateCamera;
        RenderPipelineManager.beginCameraRendering -= MoveCamera;
    }
}
