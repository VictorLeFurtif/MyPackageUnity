using UnityEngine;
using UnityEngine.Serialization;

public class MeshHelper : MonoBehaviour
{
    [FormerlySerializedAs("mesh")] [SerializeField] private MeshFilter meshF;
    [SerializeField] private float height;

    
    [ContextMenu("Height")]
    public void Height()
    {
        Mesh mesh = meshF.mesh;

        Vector3[] vertices = mesh.vertices;
        int iteration = 0;

        for (int i = 0; i < vertices.Length; i++)
        {
            if (vertices[i].y <= 0f) continue;

            vertices[i].y += height;
            iteration++;
        }

        mesh.vertices = vertices;
        mesh.RecalculateBounds();
        mesh.RecalculateNormals();

        Debug.Log($"Calcul Height Done Apagnan,\nnumber iteration : {iteration}");
    }

}

