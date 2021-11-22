using UnityEngine;

/// <summary>
/// Generates a place and connects its camera output to the UI
/// </summary>
[RequireComponent(typeof(MeshGenerator), typeof(PropsGenerator))]
public class PlaceGenerator : MonoBehaviour
{
    [SerializeField]
    CameraHandler cameraHandler;

    System.Action<int, MeshDataSO, Gradient, System.Action<Vector3[]>> CreateMesh;
    System.Action<int, Vector3[], GameObject[], Plane[], Population> PopulatePlace;

    /// <summary>
    /// Runs right after the GameObject is enabled. It keeps a reference to ConstructMesh and PopulateMeshWithProps
    /// </summary>
    private void OnEnable()
    {
        CreateMesh = GetComponent<MeshGenerator>().ConstructMesh;
        PopulatePlace = GetComponent<PropsGenerator>().PopulateMeshWithProps;
    }

    /// <summary>
    /// Connects this place's camera output to the given RenderTexture
    /// </summary>
    /// <param name="cardRawImage">RenderTexture to set on camera's output</param>
    public void ConnectBroadCast(RenderTexture cardRawImage)
    {
        cameraHandler.SetRenderTexture(cardRawImage);
    }

    /// <summary>
    /// Generates a new place.
    /// </summary>
    /// <param name="seed">For the random generator of every object</param>
    /// <param name="meshParameters">For Perlin Noise</param>
    /// <param name="meshColor">Place's color</param>
    /// <param name="decorPrefabs">GameObjects to populate the place with</param>
    /// <param name="density">Amount of spawners on this place</param>
    /// <param name="skybox">Material to use as skybox of this place</param>
    public void GenerateNew(int seed, MeshDataSO meshParameters, Gradient meshColor, GameObject[] decorPrefabs, Population density, Material skybox)
    {
        CreateMesh(seed, meshParameters, meshColor, 
            meshVertices => PopulatePlace(seed, meshVertices, decorPrefabs, cameraHandler.GetFrustumPlanes(), density));

        cameraHandler.SetCameraSky(skybox);       
    }
}
