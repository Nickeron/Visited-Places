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

    float index = 0;

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

    public GameObject InstantiateWorld(Texture rawImageFeed)
    {
        GameObject newWorld = Instantiate(WorldGeneratorPrefab, new Vector3(0, index), Quaternion.identity);
        index += 20;
        newWorld.GetComponent<PlaceGenerator>().ConnectBroadCast(rawImageFeed);
        return newWorld;
    }

    public Description RedecorateWorld(GameObject world, int seed)
    {
        //Debug.Log($"Seed {seed}");
        if (world == null) throw new NullReferenceException();

        System.Random rand = new(seed);

        DecorPiece[] newDecorSet = decorationSets[rand.Next(decorationSets.Length)].GetRandomDecors(5, rand);
        Population density = GetRandomPopulation(rand);

        world.GetComponent<PlaceGenerator>()
            .GenerateNew(seed,
                meshDataSOs[rand.Next(meshDataSOs.Length)],
                GetDecorPrefabs(newDecorSet),
                density,
                skyboxes[rand.Next(skyboxes.Length)]);

        return DescriptionGenerator.GetWorldDescription(rand, newDecorSet, density);
    }

    private Population GetRandomPopulation(System.Random rand)
    {
        Array values = Enum.GetValues(typeof(Population));
        return (Population) values.GetValue(rand.Next(values.Length));
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
