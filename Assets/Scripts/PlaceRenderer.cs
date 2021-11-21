using UnityEngine;

using Text = TMPro.TextMeshProUGUI;

public class PlaceRenderer : MonoBehaviour
{
    [SerializeField]
    Text Title, Body;

    [SerializeField]
    UnityEngine.UI.RawImage rawImage;

    GameObject connectedPlace;

    public static System.Func<GameObject, int, Description> onRedecoratePlace;
    public static System.Func<Texture, GameObject> onDemandNewPlace;

    /// <summary>
    /// Called when the current PostCard item has reached the threshold and needs to be reordered in the list
    /// Or at the first ordering of the list's items. 
    /// Firstly it requests to show a new Place from its observer and passes on its RenderTexture to connect to the place's camera.
    /// Then it keeps the passed GameObject-Place in connectedPlace to redecorate it on the next reorder.
    /// Then asks for a description of the redecoration from its observer, and sets the text of the title and the body of the description on the UI.
    /// </summary>
    /// <exception cref="System.NullReferenceException"></exception>
    public void OnReorder() 
    {
        if (connectedPlace == null) 
        {
            connectedPlace = onDemandNewPlace?.Invoke(rawImage.texture);
            if (connectedPlace == null) throw new System.NullReferenceException("Asked for new place from subscriber, but got null.");
        }

        Description placeDescription = onRedecoratePlace?.Invoke(connectedPlace, (int)transform.position.y) ?? new Description { Title = "Not subscribed", Body = "Subscribers did not subscribe"};

        Title.text = placeDescription.Title;
        Body.text = placeDescription.Body;
    }
}
