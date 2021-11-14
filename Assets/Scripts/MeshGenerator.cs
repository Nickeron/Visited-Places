using UnityEngine;

[RequireComponent(typeof(MeshFilter), typeof(ObjectsGenerator))]
public class MeshGenerator : MonoBehaviour
{
    [Header("World Type")]
    public MeshDataSO MeshData;  

    Mesh _generatedMesh;

    Vector3[] _vertices;
    int[] _triangles;
    Color[] _colors;

    float minTerrainHeight, maxTerrainHeight;
    int xSize = 20, zSize = 20;

    public bool autoUpdate;

    void Start()
    {
        CreateShape();
        UpdateMesh();
        GetComponent<ObjectsGenerator>().PopulateWithSpawners(_vertices);
    }

    public void CreateShape()
    {
        CreateVertices();
        CreateTriangles();
        CreateColors();
    }

    public void UpdateMesh()
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
    }

    public Vector3 GetRandomVertex()
    {
        return _vertices[Random.Range(0, _vertices.Length)];
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

    private void CreateColors()
    {
        _colors = new Color[_vertices.Length];

        ForEachVertex((x, z, index) =>
        {
            _colors[index] = MeshData.worldColor.Evaluate(GetNormalHeight(index));
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
        System.Random prng = new System.Random(MeshData.seed);

        float amplitude = 1;
        float frequency = 1;
        float noiseHeight = 0;

        for (int i = 0; i < MeshData.octaves; i++)
        {
            float sampleX = x / MeshData.noiseScale * frequency + prng.Next(-100000, 100000);
            float sampleY = z / MeshData.noiseScale * frequency + prng.Next(-100000, 100000);

            float perlinValue = Mathf.PerlinNoise(sampleX, sampleY) * 2 - 1;
            noiseHeight += perlinValue * amplitude;

            amplitude *= MeshData.persistance;
            frequency *= MeshData.lacunarity;
        }
        return noiseHeight;
    }

    float GetNormalHeight(int index)
    {
        return Mathf.InverseLerp(minTerrainHeight, maxTerrainHeight, _vertices[index].y);
    }
    #endregion Helper Methods
}
