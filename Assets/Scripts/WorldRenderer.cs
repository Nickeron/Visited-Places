using UnityEngine;

using Text = TMPro.TextMeshProUGUI;

public class WorldRenderer : MonoBehaviour
{
    [SerializeField]
    Text Title, Body;

    [SerializeField]
    UnityEngine.UI.RawImage rawImage;

    GameObject connectedPlace;

    public static System.Func<GameObject, int, Description> onRedecorateWorld;
    public static System.Func<Texture, GameObject> onDemandNewWorld;

    public void OnListRecycle() 
    {
        if (connectedPlace == null) 
        {
            connectedPlace = onDemandNewWorld?.Invoke(rawImage.texture);
            if (connectedPlace == null) return;
        }

        Description worldDescription = onRedecorateWorld?.Invoke(connectedPlace, (int)transform.position.y) ?? new Description { Title = "Not subscribed", Body = "WorldsManager did not subscribe"};

        Title.text = worldDescription.Title;
        Body.text = worldDescription.Body;
    }
}

public struct Description
{
    public string Title;
    public string Body;
}
