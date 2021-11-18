using System;
using System.Linq;

using UnityEngine;
using UnityEngine.Pool;

public class WorldsManager : MonoBehaviour
{
    [Tooltip("A prefab to initialize for every place we want to create")]
    public GameObject WorldGeneratorPrefab;    

    [Tooltip("A variety of different decoration sets to decorate the generated worlds")]
    [SerializeField]
    DecorativeSet[] decorationSets;

    [Tooltip("A variety of different sets of parameters for different effects on mesh generation")]
    [SerializeField]
    MeshDataSO[] meshDataSOs;

    [Tooltip("A variety of skyboxes to use for the generated worlds")]
    [SerializeField]
    Material[] skyboxes;

    private void OnEnable()
    {
        WorldRenderer.onDemandNewWorld += InstantiateWorld;
        WorldRenderer.onRedecorateWorld += RedecorateWorld;
    }

    private void OnDisable()
    {
        WorldRenderer.onDemandNewWorld -= InstantiateWorld;
        WorldRenderer.onRedecorateWorld -= RedecorateWorld;
    } 

    public GameObject InstantiateWorld()
    {
        GameObject newWorld = Instantiate(WorldGeneratorPrefab);
        
        return newWorld;
    }

    public Description RedecorateWorld(GameObject world, int seed)
    {
        if (world == null) throw new NullReferenceException();

        System.Random rand = new(seed);

        DecorPiece[] newDecorSet = decorationSets[seed].GetRandomDecors(5, rand);
        Population density = (Population)rand.Next(2);

        world.GetComponent<PlaceGenerator>()
            .GenerateNew(seed,
            meshDataSOs[rand.Next(meshDataSOs.Length)],
            GetDecorPrefabs(newDecorSet),
            density,
            skyboxes[rand.Next(skyboxes.Length)]);

        return DescriptionGenerator.GetWorldDescription(rand, newDecorSet, density);
    }

    GameObject[] GetDecorPrefabs(DecorPiece[] decorSet)
    {
        return decorSet.Select(piece => piece.prefab).ToArray();
    }
    
    private void OnReleaseWorld(GameObject releasedWorld)
    {
        try
        {
            int worldIndex = int.Parse(releasedWorld.name);
            releasedWorld.SetActive(false);
        }
        catch (FormatException)
        {
            Debug.Log($"Unable to parse world: '{releasedWorld.name}'");
        }        
    }

    private void OnGetWorld(GameObject world)
    {
        world.SetActive(true);
    }
}
