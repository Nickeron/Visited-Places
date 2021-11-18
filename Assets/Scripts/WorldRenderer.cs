using UnityEngine;

using Text = TMPro.TextMeshProUGUI;

public class WorldRenderer : MonoBehaviour
{
    [SerializeField]
    Text Title, Body;

    GameObject connectedPlace;

    public static System.Func<GameObject, int, Description> onRedecorateWorld;
    public static System.Func<GameObject> onDemandNewWorld;

    private void Start()
    {
        OnListRecycle();
    }

    public void OnListRecycle() 
    {
        if (connectedPlace == null) 
        {
            connectedPlace = onDemandNewWorld?.Invoke();
            if (connectedPlace == null) return;
        }

        Description worldDescription = onRedecorateWorld?.Invoke(connectedPlace, (int)transform.position.y / 100) ?? new Description { Title = "Not subscribed", Body = "WorldsManager did not subscribe"};

        Title.text = worldDescription.Title;
        Body.text = worldDescription.Body;
    }
}

public struct Description
{
    public string Title;
    public string Body;
}
