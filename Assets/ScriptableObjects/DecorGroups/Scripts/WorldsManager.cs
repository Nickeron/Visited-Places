using System;
using System.Linq;

using UnityEngine;

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
    System.Random rand;

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
        if (world == null) throw new NullReferenceException();

        rand = new(seed);

        DecorativeSet newDecorSet = GetDecorset();
        Decors decorPieces = newDecorSet.GetRandomDecors(rand);
        Population density = GetPopulation();

        world.GetComponent<PlaceGenerator>()
            .GenerateNew(seed,
                GetRandomMesh(),
                newDecorSet.worldColor,
                decorPieces.GetPrefabs(),
                density,

                skyboxes[rand.Next(skyboxes.Length)]);

        return DescriptionGenerator.GetWorldDescription(rand, decorPieces.GetTypes(), density);
    }

    private Population GetPopulation()
    {
        Array values = Enum.GetValues(typeof(Population));
        return (Population)values.GetValue(rand.Next(values.Length));
    }

    private MeshDataSO GetRandomMesh()
    {
        return meshDataSOs[rand.Next(meshDataSOs.Length)];
    }
    
    private DecorativeSet GetDecorset()
    {
        return decorationSets[rand.Next(decorationSets.Length)];
    }
}
