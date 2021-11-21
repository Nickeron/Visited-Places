using UnityEngine;

[CreateAssetMenu]
public class MeshDataSO : ScriptableObject
{
    [Header("Noise Distribution")]

    [Tooltip("How zoomed in the noise is")]
    public float noiseScale = 10;

    [Tooltip("How many layers of noise to add")]
    public int octaves = 3;

    [Header("Decrease in amplitude of octaves")]
    [Tooltip("How much small features affect the overall shape")]
    [Range(0, 1)]
    public float persistance = 0.5f;

    [Header("Increase in frequency of octaves")]
    [Tooltip("Increases the number of small features")]
    public float lacunarity = 3;
}
