using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;

public class GrassSpawner : MonoBehaviour
{
    [Header("Grass Fields")]
    [SerializeField] private Mesh grassMesh;
    [SerializeField] private Material grassMaterial;
    [SerializeField] private Grass[] arrayGrass = new Grass[] { };
    
    [Header("Spawning Parameters")]
    [SerializeField] private int grassCount = 1000;
    [SerializeField] private float spawnRadius = 50f;
    [SerializeField] private LayerMask terrainLayer;
    [SerializeField] private float maxDistanceRaycast = 50f;

    [Header("Grass parameters")]
    [SerializeField, Range(0.5f, 2f)] private float minHeight = 0.8f;
    [SerializeField, Range(0.5f, 2f)] private float maxHeight = 1.2f;
    [SerializeField, Range(0f, 2f)] private float offSetGrassNormal ;

    [Header("Chunk Settings")] [SerializeField, Range(5f, 500f), Tooltip("Size of each chunk in meters")]
    private float chunkSize = 10f;
    
    [Header("Camera Settings")] 
    [SerializeField] private Camera cameraRendering;

    [SerializeField] private float cameraMaxDistanceRendering;
    
    private Dictionary<Vector2Int, ChunkGrass> chunks = new Dictionary<Vector2Int, ChunkGrass>();
    private Plane[] m_frustumPlanes;
    private int m_frameCounter = 0;
    private Matrix4x4[] m_batchPool = new Matrix4x4[1023];
    
    [ContextMenu("Regenerate Grass")]
    private void RegenerateGrass()
    {
        chunks.Clear();
        Start(); 
    }

    private void Start()
    {
        for (int i = 0; i < grassCount; i++)
        {
            Vector2 randomCircle = Random.insideUnitCircle * spawnRadius;
            Vector3 randomPosition = transform.position + new Vector3(randomCircle.x, 0, randomCircle.y);
            
            RaycastHit hit;
            Vector3 rayOrigin = randomPosition + Vector3.up * 100f;
            
            if (Physics.Raycast(rayOrigin,Vector3.down,out hit, maxDistanceRaycast,terrainLayer))
            {
                Vector3 groundPosition = hit.point + hit.normal * offSetGrassNormal;
    
                Quaternion randomRotation = Quaternion.Euler(0, Random.Range(0f, 360f), 0);
                Quaternion alignToNormal = Quaternion.FromToRotation(Vector3.up, hit.normal); 
                Quaternion finalRotation = alignToNormal * randomRotation;

                Vector3 scale = Vector3.one * Random.Range(minHeight, maxHeight);
                Matrix4x4 newMatrice = Matrix4x4.TRS(groundPosition, finalRotation, scale);


                int chunkX = Mathf.FloorToInt(groundPosition.x/ chunkSize);
                int chunkZ = Mathf.FloorToInt(groundPosition.z/ chunkSize);
                
                Vector2Int chunkKey = new Vector2Int(chunkX, chunkZ);

                if (!chunks.ContainsKey(chunkKey))
                {
                    Vector3 chunkCenter = new Vector3(chunkX * chunkSize + chunkSize * 0.5f,
                        0,
                        chunkZ * chunkSize + chunkSize * 0.5f);
                    
                    ChunkGrass newChunk = new ChunkGrass(chunkCenter, chunkSize);
                    
                    chunks[chunkKey] = newChunk;
                }
                
                chunks[chunkKey].AddMatrice4x4(newMatrice);
            }
        }
    }

    private void Update()
    {
        if (chunks.Count == 0 || cameraRendering == null) return;
        
        m_frameCounter++;
        
        if (m_frameCounter >= 5 || m_frustumPlanes == null)
        {
            m_frameCounter = 0;
            m_frustumPlanes = GeometryUtility.CalculateFrustumPlanes(cameraRendering);
        }
        
        Vector3 cameraPos = cameraRendering.transform.position;
        int maxBatchSize = 1023;

        foreach (KeyValuePair<Vector2Int,ChunkGrass> pair in chunks)
        {
            
            if (!GeometryUtility.TestPlanesAABB(m_frustumPlanes,pair.Value.Bounds)) continue;
            
            float distance = Vector3.Distance(cameraPos, pair.Value.CenterChunk);
            
            if (distance > cameraMaxDistanceRendering) continue;
            
            
            
            int totalMatrices = pair.Value.MatricesGrass.Count;
            
            int batches = Mathf.CeilToInt((float)totalMatrices / maxBatchSize);
            
            for (int i = 0; i < batches; i++)
            {
                int startIndex = i * maxBatchSize;
                int count = Mathf.Min(maxBatchSize, totalMatrices - startIndex);
        
                pair.Value.MatricesGrass.CopyTo(startIndex, m_batchPool, 0, count);
        
                Graphics.DrawMeshInstanced(grassMesh, 0, grassMaterial, m_batchPool, count);
            }
        }
    }

    void ChooseRandomMeshGrass(Matrix4x4[] batch)
    {
        int randomIndex = Random.Range(0, arrayGrass.Length);
        
        Graphics.DrawMeshInstanced(
            arrayGrass[randomIndex].grassMesh,
            0,
            arrayGrass[randomIndex].grassMaterial,
            batch);
    }
    
    void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.green;
        Gizmos.DrawWireSphere(transform.position, spawnRadius);
    }
    
    void OnDrawGizmos()
    {
        if (chunks == null) return;
    
        foreach (var chunk in chunks)
        {
            Gizmos.color = Color.yellow;
            Gizmos.DrawWireCube(chunk.Value.CenterChunk, chunk.Value.Bounds.size);
            
            #if UNITY_EDITOR
            UnityEditor.Handles.Label(
                chunk.Value.CenterChunk, 
                $"{chunk.Value.MatricesGrass.Count} grass"
            );
            #endif
        }
    }
}
