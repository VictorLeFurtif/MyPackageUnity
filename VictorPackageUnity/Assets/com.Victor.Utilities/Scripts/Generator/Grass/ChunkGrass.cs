using System.Collections.Generic;
using UnityEngine;

public class ChunkGrass
{
    public List<Matrix4x4> MatricesGrass { get; private set; }
    public Vector3 CenterChunk { get; private set; }
    public Bounds Bounds { get; private set; }

    public void AddMatrice4x4(Matrix4x4 matrice)
    {
        MatricesGrass.Add(matrice);
    }

    public ChunkGrass(Vector3 center,float chunkSize)
    {
        MatricesGrass = new List<Matrix4x4>();
        CenterChunk = center;
        Bounds = new Bounds(CenterChunk, new Vector3(chunkSize, 100f, chunkSize));
    }
        
}