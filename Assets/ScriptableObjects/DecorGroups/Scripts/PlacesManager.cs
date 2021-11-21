using System;
using UnityEngine;

/// <summary>
/// Manages the creation and redecoration of all the place.
/// </summary>
public class PlacesManager : MonoBehaviour
{
    [Tooltip("A prefab to initialize for every place we want to create")]
    public GameObject PlaceGeneratorPrefab;

    [Tooltip("A variety of different decoration sets to decorate the generated worlds")]
    [SerializeField]
    DecorativeSet[] decorationSets;

    [Tooltip("A variety of different sets of parameters for different effects on mesh generation")]
    [SerializeField]
    MeshDataSO[] meshDataSOs;

    [Tooltip("A variety of skyboxes to use for the generated worlds")]
    [SerializeField]
    Material[] skyboxes;

    // Used to vary the height of each place in the world.
    private float _placeHeightPos = 0;

    private System.Random _randomGenerator;

    /// <summary>
    /// Method invoked from event, to create a new place,
    /// and provide it back to the caller.
    /// </summary>
    /// <param name="rawImageFeed">RenderTexture to be used by the newly created place's camera to broadcast there.</param>
    /// <returns>The newly instantiated place as a GameObject.</returns>
    public GameObject CreateNewPlace(Texture rawImageFeed)
    {
        GameObject newWorld = Instantiate(PlaceGeneratorPrefab, new Vector3(0, _placeHeightPos), Quaternion.identity);
        _placeHeightPos += 20;
        newWorld.GetComponent<PlaceGenerator>().ConnectBroadCast(rawImageFeed);
        return newWorld;
    }

    /// <summary>
    /// Redecorates a provided place with a new color and props. Makes it like brand new :P
    /// </summary>
    /// <param name="place">The GameObject to redecorate</param>
    /// <param name="seed">int to use as seed in the random generator.</param>
    /// <returns>The description of the redecorated place.</returns>
    /// <exception cref="NullReferenceException">If the place is null</exception>
    public Description RedecoratePlace(GameObject place, int seed)
    {
        if (place == null) throw new NullReferenceException("Provided place is null. Event fired from place renderer.");

        _randomGenerator = new(seed);

        DecorativeSet newDecorSet = GetDecorset();
        Decors decorPieces = newDecorSet.GetRandomDecors(_randomGenerator);
        Population density = GetPopulation();

        place.GetComponent<PlaceGenerator>().GenerateNew(
                seed,
                GetMesh(),
                newDecorSet.worldColor,
                decorPieces.GetPrefabs(),
                density,
                GetSkybox());

        return DescriptionGenerator.GetWorldDescription(_randomGenerator, decorPieces.GetTypes(), density);
    }

    #region Event Subscription
    private void OnEnable()
    {
        PlaceRenderer.onDemandNewWorld += CreateNewPlace;
        PlaceRenderer.onRedecorateWorld += RedecoratePlace;
    }

    private void OnDisable()
    {
        PlaceRenderer.onDemandNewWorld -= CreateNewPlace;
        PlaceRenderer.onRedecorateWorld -= RedecoratePlace;
    }
    #endregion Event Subscription

    #region Helper Methods
    /// <summary>
    /// Randomly chooses a Population Density value
    /// </summary>
    /// <returns>Enum of type Population</returns>
    private Population GetPopulation()
    {
        Array values = Enum.GetValues(typeof(Population));
        return (Population)values.GetValue(_randomGenerator.Next(values.Length));
    }
    
    /// <summary>
    /// Randomly chooses a MeshData object
    /// </summary>
    /// <returns>MeshData scriptable object</returns>
    private MeshDataSO GetMesh()
    {
        return meshDataSOs[_randomGenerator.Next(meshDataSOs.Length)];
    }
    
    /// <summary>
    /// Randomly chooses a DecorativeSet
    /// </summary>
    /// <returns>DecorativeSet object</returns>
    private DecorativeSet GetDecorset()
    {
        return decorationSets[_randomGenerator.Next(decorationSets.Length)];
    }

    /// <summary>
    /// Randomly chooses a skybox material
    /// </summary>
    /// <returns>Skybox material</returns>
    private Material GetSkybox()
    {
        return skyboxes[_randomGenerator.Next(skyboxes.Length)];
    }
    #endregion Helper Methods
}
