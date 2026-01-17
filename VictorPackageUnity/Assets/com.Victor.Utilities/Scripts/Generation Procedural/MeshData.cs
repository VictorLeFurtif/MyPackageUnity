using UnityEngine;

namespace com.Victor.Utilities.Scripts.Generation_Procedural
{
    public class MeshData
    {
        public Vector3[] Verticles;
        public int[] Triangles;
        public Vector2[] Uvs;

        private int triangleIndex;
        
        public MeshData(int meshWidth, int meshHeight)
        {
            Verticles = new Vector3[meshWidth * meshHeight];
            Uvs = new Vector2[meshWidth * meshHeight];
            Triangles = new int[(meshWidth - 1) * (meshHeight - 1) * 6];
        }

        public void AddTriangle(int a, int b, int c)
        {
            Triangles[triangleIndex] = a;
            Triangles[triangleIndex + 1] = b;
            Triangles[triangleIndex + 2] = c;
            triangleIndex += 3;
        }

        public Mesh CreateMesh()
        {
            Mesh mesh = new Mesh
            {
                vertices = Verticles,
                triangles = Triangles,
                uv = Uvs
            };
            mesh.RecalculateNormals();
            return mesh;
        }
    }
}