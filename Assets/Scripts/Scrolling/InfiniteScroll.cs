using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class InfiniteScroll : MonoBehaviour, IBeginDragHandler, IDragHandler, IScrollHandler
{
    #region Private Members
    /// <summary>
    /// The ScrollContent component that belongs to the scroll content GameObject.
    /// </summary>
    [SerializeField]
    private float _screenHeightReference = 1080;

    /// <summary>
    /// The ScrollContent component that belongs to the scroll content GameObject.
    /// </summary>
    [SerializeField]
    private ScrollContent _scrollContent;

    /// <summary>
    /// How far the items will travel outside of the scroll view before being repositioned.
    /// </summary>
    [SerializeField]
    private float _outOfBoundsThreshold;

    /// <summary>
    /// The ScrollRect component for this GameObject.
    /// </summary>
    private ScrollRect _scrollRect;

    /// <summary>
    /// The last position where the user has dragged.
    /// </summary>
    private Vector2 _lastDragPosition;

    /// <summary>
    /// Is the user dragging in the positive axis or the negative axis?
    /// </summary>
    private bool _isPositiveScroll;

    #endregion

    private void Start()
    {
        // For safety we make the area scrollable without restrictions, to avoid wrong selection on UI
        _scrollRect = GetComponent<ScrollRect>();
        _scrollRect.movementType = ScrollRect.MovementType.Unrestricted;
    }

    /// <summary>
    /// Called when the user starts to drag the scroll view.
    /// </summary>
    /// <param name="eventData">The data related to the drag event.</param>
    public void OnBeginDrag(PointerEventData eventData)
    {
        _lastDragPosition = eventData.position;
    }

    /// <summary>
    /// Called while the user is dragging the scroll view.
    /// </summary>
    /// <param name="dragData">The data related to the drag event.</param>
    public void OnDrag(PointerEventData dragData)
    {
        _isPositiveScroll = dragData.position.y > _lastDragPosition.y;

        _lastDragPosition = dragData.position;
    }

    /// <summary>
    /// Called when the user starts to scroll with their mouse wheel in the scroll view.
    /// </summary>
    /// <param name="wheelData">The data related to the scroll event.</param>
    public void OnScroll(PointerEventData wheelData)
    {
        _isPositiveScroll = wheelData.scrollDelta.y < 0;
    }

    /// <summary>
    /// Called when the user is dragging/scrolling the scroll view.
    /// </summary>
    public void OnViewScroll()
    {
        // Get the current item 
        Transform currItem = _scrollRect.content.GetChild(GetCurrentIndex());

        if (!ReachedThreshold(currItem))
        {
            return;
        }

        SwitchItems(currItem);
    }

    /// <summary>
    /// Switches the current item with the last or the first item on the list, depending on the direction.
    /// </summary>
    /// <param name="currItem">The item to switch places with the end one.</param>
    private void SwitchItems(Transform currItem)
    {
        Transform endItem = _scrollRect.content.GetChild(GetEndIndex());
        Vector2 newPos = endItem.position;

        Debug.Log($"Item Distance:{ItemDistance()}");
        if (_isPositiveScroll)
        {
            newPos.y = endItem.position.y - ItemDistance();
        }
        else
        {
            newPos.y = endItem.position.y + ItemDistance();
        }
        currItem.gameObject.GetComponent<WorldRenderer>().OnListRecycle();
        currItem.position = newPos;
        currItem.SetSiblingIndex(GetEndIndex());
    }

    private float ItemDistance() => RelativeHeight(_scrollContent.ChildHeight) + RelativeHeight(_scrollContent.ItemSpacing);

    private float ClipThreshold() => (RelativeHeight(_scrollContent.Height) + RelativeHeight(_scrollContent.ChildHeight)) * 0.5f + RelativeHeight(_outOfBoundsThreshold);

    private float RelativeHeight(float itemHeight) => itemHeight * Screen.height /_screenHeightReference;    

    /// <summary>
    /// Get the index of the End Item.
    /// </summary>
    /// <returns>The index of the first or the last item, depending on the direction of scroll</returns>
    int GetEndIndex()
    {
        return _isPositiveScroll ? 0 : _scrollRect.content.childCount - 1;
    }

    /// <summary>
    /// Get the index of the Current Item.
    /// </summary>
    /// <returns>The index of the first or the last item, depending on the direction of scroll</returns>
    int GetCurrentIndex()
    {
        return _isPositiveScroll ? _scrollRect.content.childCount - 1 : 0;
    }

    /// <summary>
    /// Checks if an item has the reached the out of bounds threshold for the scroll view.
    /// </summary>
    /// <param name="item">The item to be checked.</param>
    /// <returns>True if the item has reached the threshold for either ends of the scroll view, false otherwise.</returns>
    private bool ReachedThreshold(Transform item)
    {
        float posYThreshold = transform.position.y + ClipThreshold();
        float negYThreshold = transform.position.y - ClipThreshold();

        return _isPositiveScroll ?
            item.position.y > posYThreshold :
            item.position.y < negYThreshold;
    }
}
