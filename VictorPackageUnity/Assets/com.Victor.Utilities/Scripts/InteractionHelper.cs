using System;
using UnityEngine;

namespace com.Victor.Utilities.Scripts
{
    public static class InteractionHelper
    {
        public static T GetInteractableInFront<T>(Transform player, float distance, LayerMask layer)
        {
            return Physics.Raycast(player.position, player.forward,out RaycastHit hit,distance,layer ) 
                ? hit.collider.GetComponent<T>() : default;
        }

        public static void HighlightObject(GameObject obj, Color color, float intensity)
        {
            if (obj == null) throw new Exception("GameObject null");

            Renderer renderer = obj.GetComponent<Renderer>();
            if (renderer == null) throw new Exception("No Renderer attached to this GO");

            Material material = renderer.material;

            material.EnableKeyword("_EMISSION");
            material.SetColor("_EmissionColor", color * intensity);
        }
        
        public static void RemoveHighlight(GameObject obj)
        {
            if (obj == null) throw new Exception("GameObject null");

            Renderer renderer = obj.GetComponent<Renderer>();
            if (renderer == null) throw new Exception("No Renderer attached to this GO");

            Material material = renderer.material;

            material.DisableKeyword("_EMISSION");
        }

        public static bool HasLineOfSight(Vector3 from, Vector3 to, LayerMask obstacles)
        {
            Vector3 direction = to - from;
            return !Physics.Raycast(from, direction, direction.magnitude, obstacles);
        }
    }
}