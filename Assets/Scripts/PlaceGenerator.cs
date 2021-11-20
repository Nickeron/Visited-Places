using UnityEngine;

[RequireComponent(typeof(MeshGenerator), typeof(PropsGenerator))]
public class PlaceGenerator : MonoBehaviour
{
    [SerializeField]
    CameraHandler cameraHandler;

    System.Action<int, MeshDataSO, Gradient, System.Action<Vector3[]>> CreateMesh;
    System.Action<int, Vector3[], GameObject[], Plane[], Population> PopulatePlace;


    private void OnEnable()
    {
        CreateMesh = GetComponent<MeshGenerator>().ConstructMesh;
        PopulatePlace = GetComponent<PropsGenerator>().PopulateMeshWithProps;
    }

    public void ConnectBroadCast(Texture cardRawImage)
    {
        cameraHandler.SetRenderTexture(cardRawImage);
    }

    public void GenerateNew(int seed, MeshDataSO meshParameters, Gradient meshColor, GameObject[] decorPrefabs, Population density, Material skybox)
    {
        CreateMesh(seed, meshParameters, meshColor, meshVertices => PopulatePlace(seed, meshVertices, decorPrefabs, cameraHandler.GetFrustumPlanes(), density));

        cameraHandler.SetCameraSky(skybox);       
    }
}