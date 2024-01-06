// Import necessary namespaces
using System.Collections;
using UnityEngine;
using UnityEngine.EventSystems;

/// <summary>
/// This script enables dragging a UI element within a canvas and instantiates a prefab when the drag ends.
/// </summary>
public class DragUIItem : MonoBehaviour, IBeginDragHandler, IDragHandler, IEndDragHandler
{
  // Reference to the prefab to instantiate on drag end
  [SerializeField] private GameObject PrefabToInstantiate;

  // Reference to the RectTransform of the UI element to be dragged
  [SerializeField] private RectTransform UIDragElement;

  // Reference to the RectTransform of the canvas containing the UI element
  [SerializeField] private RectTransform Canvas;

  // Variables for tracking original positions during the drag
  private Vector2 mOriginalLocalPointerPosition;
  private Vector3 mOriginalPanelLocalPosition;
  private Vector2 mOriginalPosition;

  /// <summary>
  /// Initializes the original position of the UI element.
  /// </summary>
  private void Start()
  {
    mOriginalPosition = UIDragElement.localPosition;
  }

  /// <summary>
  /// Handles the beginning of a drag operation.
  /// </summary>
  /// <param name="data">Event data associated with the drag operation.</param>
  public void OnBeginDrag(PointerEventData data)
  {
    // Record the original local pointer position when the drag starts
    mOriginalPanelLocalPosition = UIDragElement.localPosition;

    // Convert screen point to local point in canvas space
    RectTransformUtility.ScreenPointToLocalPointInRectangle(
        Canvas,
        data.position,
        data.pressEventCamera,
        out mOriginalLocalPointerPosition);
  }

  /// <summary>
  /// Handles the ongoing drag operation, updating the UI element's position.
  /// </summary>
  /// <param name="data">Event data associated with the drag operation.</param>
  public void OnDrag(PointerEventData data)
  {
    // Convert screen point to local point in canvas space during the drag
    Vector2 localPointerPosition;
    if (RectTransformUtility.ScreenPointToLocalPointInRectangle(
        Canvas,
        data.position,
        data.pressEventCamera,
        out localPointerPosition))
    {
      // Calculate the offset from the original position
      Vector3 offsetToOriginal = localPointerPosition - mOriginalLocalPointerPosition;

      // Update the UI element's position based on the drag movement
      UIDragElement.localPosition = mOriginalPanelLocalPosition + offsetToOriginal;
    }

    // Uncomment the next line to enable clamping of the UI element's position within a specified area.
    // ClampToArea();
  }

  /// <summary>
  /// Coroutine to smoothly move the UI element to a target position over a specified duration.
  /// </summary>
  /// <param name="r">RectTransform to be moved.</param>
  /// <param name="targetPosition">Target position to move towards.</param>
  /// <param name="duration">Duration of the movement animation.</param>
  /// <returns>Coroutine enumerator.</returns>
  public IEnumerator Coroutine_MoveUIElement(RectTransform r, Vector2 targetPosition, float duration = 0.1f)
  {
    float elapsedTime = 0;
    Vector2 startingPos = r.localPosition;

    // Lerp (linear interpolation) to smoothly move the UI element
    while (elapsedTime < duration)
    {
      r.localPosition = Vector2.Lerp(startingPos, targetPosition, (elapsedTime / duration));
      elapsedTime += Time.deltaTime;

      // Wait until the end of the frame before updating again
      yield return new WaitForEndOfFrame();
    }

    // Ensure the UI element reaches the exact target position
    r.localPosition = targetPosition;
  }

  // Uncomment the next method to enable clamping of the UI element's position within a specified area.
  // /// <summary>
  // /// Clamps the panel's position within a specified area.
  // /// </summary>
  // private void ClampToArea()
  // {
  //     Vector3 pos = UIDragElement.localPosition;
  //     Vector3 minPosition = Canvas.rect.min - UIDragElement.rect.min;
  //     Vector3 maxPosition = Canvas.rect.max - UIDragElement.rect.max;
  //
  //     pos.x = Mathf.Clamp(UIDragElement.localPosition.x, minPosition.x, maxPosition.x);
  //     pos.y = Mathf.Clamp(UIDragElement.localPosition.y, minPosition.y, maxPosition.y);
  //
  //     UIDragElement.localPosition = pos;
  // }

  /// <summary>
  /// Handles the end of a drag operation, initiating UI element snap-back and prefab instantiation.
  /// </summary>
  /// <param name="eventData">Event data associated with the drag operation.</param>
  public void OnEndDrag(PointerEventData eventData)
  {
    // Snap the UI element back to its original position using a coroutine
    StartCoroutine(Coroutine_MoveUIElement(UIDragElement, mOriginalPosition, 0.5f));

    // Perform a raycast to determine the position where the drag ended
    RaycastHit hit;
    Ray ray = Camera.main.ScreenPointToRay(eventData.position);

    // Check if the ray hits something within the specified range
    if (Physics.Raycast(ray, out hit, 1000.0f))
    {
      Vector3 worldPoint = hit.point;

      // Uncomment the next line to display the world point in the console.
      // Debug.Log(worldPoint);

      // Create an object at the hit position if it satisfies certain criteria
      CreateObject(worldPoint);
    }
  }

  /// <summary>
  /// Instantiates a prefab at the specified position if the conditions are met.
  /// </summary>
  /// <param name="position">Position at which the prefab will be instantiated.</param>
  public void CreateObject(Vector3 position)
  {
    // Check if the prefab is defined
    if (PrefabToInstantiate == null)
    {
      Debug.Log("No prefab to instantiate");
      return;
    }

    // Check if the position meets certain criteria (Placeholder logic in PositionWithinCell method)
    if (PositionWithinCell(position))
    {
      // Instantiate the prefab at the specified position
      GameObject obj = Instantiate(PrefabToInstantiate, position, Quaternion.identity);
    }
  }

  /// <summary>
  /// Placeholder method to determine if a position is within a specific cell or meets certain criteria.
  /// </summary>
  /// <param name="pos">Position to be checked.</param>
  /// <returns>True if the position meets the criteria, false otherwise.</returns>
  private bool PositionWithinCell(Vector3 pos)
  {
    // Placeholder logic: Always return true. Implement your own logic here.
    return true;
  }
}
