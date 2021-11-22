using System.Collections;

using UnityEngine;

/// <summary>
/// Generates a new Mesh
/// </summary>
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

    /// <summary>
    /// Creates a Mesh using the provided seed for controlling the randomness.
    /// </summary>
    /// <param name="seed">For the random generator to produce consistent results</param>
    /// <param name="meshParameters">For Perlin Noise</param>
    /// <param name="placeColor">Gradient to use for the color of the mesh</param>
    /// <param name="onMeshConstructed">Callback Action with the Positions as parameter</param>
    public void ConstructMesh(int seed, MeshDataSO meshParameters, Gradient placeColor, System.Action<Vector3[]> onMeshConstructed)
    {
        // Stop the creation of any previous Mesh and do a new one.
        StopAllCoroutines();

        _meshData = meshParameters;

        _rndg = new System.Random(seed);

        // Create the Mesh in the background.
        StartCoroutine(CreateShape(placeColor, onMeshConstructed));
    }

    /// <summary>
    /// Creates the Mesh shape: Vertices, Triangles, Colors
    /// </summary>
    /// <param name="placeColor">To use for the colors of the Mesh</param>
    /// <param name="onFinish">Callback Action to pass on</param>
    /// <returns>The UpdateMesh co-routine, with the callback as parameter</returns>
    IEnumerator CreateShape(Gradient placeColor, System.Action<Vector3[]> onFinish)
    {
        CreateVertices();
        CreateTriangles();
        CreateColors(placeColor);

        yield return UpdateMesh(onFinish);
    }

    /// <summary>
    /// Sets the vertices, triangles and colors on the Mesh, of this GameObject's MeshFilter.
    /// Then calls the provided Action passing the vertices of this Mesh.
    /// </summary>
    /// <param name="onFinish">To call after this Mesh is updated</param>
    /// <returns>null</returns>
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
    /// <summary>
    /// Creates an Array of vertices [(xSize + 1) * (zSize + 1)], for the Mesh
    /// </summary>
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

    /// <summary>
    /// Creates an Array of triangles [xSize * zSize * 6], for the Mesh
    /// </summary>
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

    /// <summary>
    /// Creates a color for each vertex in the Vertices array, out of the provided gradient and adds it to the Colors array
    /// </summary>
    /// <param name="placeColor">used for evaluating each vertex color</param>
    private void CreateColors(Gradient placeColor)
    {
        _colors = new Color[_vertices.Length];

        ForEachVertex((x, z, index) =>
        {
            _colors[index] = placeColor.Evaluate(GetNormalHeight(index));
        });
    }
    #endregion Creation Methods

    #region Helper Methods
    /// <summary>
    /// Calls a method for each vertex of the Mesh
    /// </summary>
    /// <param name="method">Method to call</param>
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

    /// <summary>
    /// Calls a method for each triangle of the Mesh
    /// </summary>
    /// <param name="method">Method to call</param>
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

    /// <summary>
    /// Generates a PerlinNoise sample to fluctuate the height of each vertex
    /// </summary>
    /// <param name="x">Coordinate X</param>
    /// <param name="z">Coordinate Z</param>
    /// <returns>Perlin Noise sample for the height of the vertex</returns>
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

    /// <summary>
    /// Normalizes the vertex on the Vertices array at the provided index, between 0 and 1.
    /// minTerrainHeight and maxTerrainHeight act as the boundaries.
    /// </summary>
    /// <param name="index">of the Vertex in the Vertices array</param>
    /// <returns></returns>
    float GetNormalHeight(int index)
    {
        return Mathf.InverseLerp(minTerrainHeight, maxTerrainHeight, _vertices[index].y);
    }
    #endregion Helper Methods
}
