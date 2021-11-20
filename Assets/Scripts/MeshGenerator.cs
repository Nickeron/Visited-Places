using System.Collections;

using UnityEngine;

[RequireComponent(typeof(MeshFilter), typeof(PropsGenerator))]
public class MeshGenerator : MonoBehaviour
{
    MeshDataSO _meshData;  

    Mesh _generatedMesh;

    Vector3[] _vertices;
    int[] _triangles;
    Color[] _colors;

    float minTerrainHeight, maxTerrainHeight;
    int xSize = 20, zSize = 20;

    private System.Random _rndg;

    public void ConstructMesh(int seed, MeshDataSO meshParameters, Gradient worldColor, System.Action<Vector3[]> onMeshConstructed)
    {
        StopAllCoroutines();

        _meshData = meshParameters;

        // Initialize Random Generator with a specific seed value.
        // So we can control what is generated through one variable.
        _rndg = new System.Random(seed);

        // Create the world
        StartCoroutine(CreateShape(worldColor, onMeshConstructed));
    }

    IEnumerator CreateShape(Gradient worldColor, System.Action<Vector3[]> onFinish)
    {
        CreateVertices();
        CreateTriangles();
        CreateColors(worldColor);

        yield return UpdateMesh(onFinish);
    }

    IEnumerator UpdateMesh(System.Action<Vector3[]> onFinish)
    {
        _generatedMesh = new Mesh();
        GetComponent<MeshFilter>().mesh = _generatedMesh;

        // Reset the Mesh of any parameters
        _generatedMesh.Clear();

        _generatedMesh.vertices = _vertices;
        _generatedMesh.triangles = _triangles;
        _generatedMesh.colors = _colors;

        // Ask Unity to set the normals properly, so that our mesh is lit right
        _generatedMesh.RecalculateNormals();

        onFinish?.Invoke(_vertices);
        yield return null;
    }

    #region Creation Methods
    private void CreateVertices()
    {
        _vertices = new Vector3[(xSize + 1) * (zSize + 1)];

        ForEachVertex((x, z, index) =>
        {
            // Add Perlin Noise to fluctuate height
            float y = GetNoiseSample(x, z);

            // Save height range
            if (y < minTerrainHeight) minTerrainHeight = y;
            if (y > maxTerrainHeight) maxTerrainHeight = y;

            // Create new vertex
            _vertices[index] = new Vector3(x, y, z);
        });
    }

    private void CreateTriangles()
    {
        _triangles = new int[xSize * zSize * 6];

        ForEachTriangle((x, z, vert, tris) =>
        {
            _triangles[tris + 0] = vert + 0;
            _triangles[tris + 1] = vert + xSize + 1;
            _triangles[tris + 2] = vert + 1;
            _triangles[tris + 3] = vert + 1;
            _triangles[tris + 4] = vert + xSize + 1;
            _triangles[tris + 5] = vert + xSize + 2;
        });
    }

    private void CreateColors(Gradient worldColor)
    {
        _colors = new Color[_vertices.Length];

        ForEachVertex((x, z, index) =>
        {
            _colors[index] = worldColor.Evaluate(GetNormalHeight(index));
        });
    }
    #endregion Creation Methods

    #region Helper Methods
    void ForEachVertex(System.Action<int, int, int> method)
    {
        for (int index = 0, z = 0; z <= zSize; z++)
        {
            for (int x = 0; x <= xSize; x++)
            {
                method(x, z, index);
                index++;
            }
        }
    }

    void ForEachTriangle(System.Action<int, int, int, int> method)
    {
        int vert = 0;
        int tris = 0;

        for (int z = 0; z < zSize; z++)
        {
            for (int x = 0; x < xSize; x++)
            {
                method(x, z, vert, tris);

                vert++;
                tris += 6;
            }
            vert++;
        }
    }

    private float GetNoiseSample(int x, int z)
    {
        float amplitude = 1;
        float frequency = 1;
        float noiseHeight = 0;

        for (int i = 0; i < _meshData.octaves; i++)
        {
            float sampleX = x / _meshData.noiseScale * frequency + _rndg.Next(-100000, 100000);
            float sampleY = z / _meshData.noiseScale * frequency + _rndg.Next(-100000, 100000);

            float perlinValue = Mathf.PerlinNoise(sampleX, sampleY) * 2 - 1;
            noiseHeight += perlinValue * amplitude;

            amplitude *= _meshData.persistance;
            frequency *= _meshData.lacunarity;
        }
        return noiseHeight;
    }

    float GetNormalHeight(int index)
    {
        return Mathf.InverseLerp(minTerrainHeight, maxTerrainHeight, _vertices[index].y);
    }
    #endregion Helper Methods
}
