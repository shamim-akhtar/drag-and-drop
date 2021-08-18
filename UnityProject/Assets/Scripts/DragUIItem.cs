using System.Collections;
using UnityEngine;
using UnityEngine.EventSystems;

public class DragUIItem : 
  MonoBehaviour, 
  IBeginDragHandler, 
  IDragHandler, 
  IEndDragHandler
{
  [SerializeField]
  GameObject PrefabToInstantiate;

  [SerializeField]
  RectTransform UIDragElement;

  [SerializeField]
  RectTransform Canvas;

  private Vector2 mOriginalLocalPointerPosition;
  private Vector3 mOriginalPanelLocalPosition;
  private Vector2 mOriginalPosition;

  private void Start()
  {
    mOriginalPosition = UIDragElement.localPosition;
  }

  public void OnBeginDrag(PointerEventData data)
  {
    mOriginalPanelLocalPosition = UIDragElement.localPosition;
    RectTransformUtility.ScreenPointToLocalPointInRectangle(
      Canvas, 
      data.position, 
      data.pressEventCamera, 
      out mOriginalLocalPointerPosition);
  }

  public void OnDrag(PointerEventData data)
  {
    Vector2 localPointerPosition;
    if (RectTransformUtility.ScreenPointToLocalPointInRectangle(
      Canvas, 
      data.position, 
      data.pressEventCamera, 
      out localPointerPosition))
    {
      Vector3 offsetToOriginal =
        localPointerPosition - 
        mOriginalLocalPointerPosition;

      UIDragElement.localPosition = 
        mOriginalPanelLocalPosition + 
        offsetToOriginal;
    }

    //ClampToArea();
  }

  public IEnumerator Coroutine_MoveUIElement(
    RectTransform r, 
    Vector2 targetPosition, 
    float duration = 0.1f)
  {
    float elapsedTime = 0;
    Vector2 startingPos = r.localPosition;
    while (elapsedTime < duration)
    {
      r.localPosition =
        Vector2.Lerp(
          startingPos,
          targetPosition, 
          (elapsedTime / duration));
      elapsedTime += Time.deltaTime;

      yield return new WaitForEndOfFrame();
    }
    r.localPosition = targetPosition;
  }

  // Clamp panel to dragArea
  private void ClampToArea()
  {
    Vector3 pos = UIDragElement.localPosition;

    Vector3 minPosition = 
      Canvas.rect.min - 
      UIDragElement.rect.min;

    Vector3 maxPosition = 
      Canvas.rect.max - 
      UIDragElement.rect.max;

    pos.x = Mathf.Clamp(
      UIDragElement.localPosition.x, 
      minPosition.x, 
      maxPosition.x);

    pos.y = Mathf.Clamp(
      UIDragElement.localPosition.y, 
      minPosition.y, 
      maxPosition.y);

    UIDragElement.localPosition = pos;
  }

  public void OnEndDrag(PointerEventData eventData)
  {
    StartCoroutine(
      Coroutine_MoveUIElement(      
        UIDragElement,       
        mOriginalPosition,       
        0.5f));

    RaycastHit hit;
    Ray ray = Camera.main.ScreenPointToRay(
      Input.mousePosition);

    if (Physics.Raycast(ray, out hit, 1000.0f))
    {
      Vector3 worldPoint = hit.point;

      //Debug.Log(worldPoint);
      CreateObject(worldPoint);
    }
  }

  public void CreateObject(Vector3 position)
  {
    if (PrefabToInstantiate == null)
    {
      Debug.Log("No prefab to instantiate");
      return;
    }

    GameObject obj = Instantiate(
      PrefabToInstantiate, 
      position, 
      Quaternion.identity);
  }
}