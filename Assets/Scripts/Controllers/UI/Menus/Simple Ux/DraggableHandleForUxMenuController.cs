using UnityEngine;
using UnityEngine.EventSystems;

/// <summary>
/// A handle for dragging a UX menu. Ususually used on titles.
/// </summary>
public class DraggableHandleForUxMenuController : MonoBehaviour, IDragHandler {

  /// <summary>
  /// What should be dragged when this is clicked.
  /// </summary>
  public RectTransform Target;
  float _timeCount = 0.0f;

  // Drag the selected item.
  public void OnDrag(PointerEventData data) {
    if(data.dragging) {
      // Object is being dragged.
      _timeCount += Time.deltaTime;
      if(_timeCount > 0.25f) {
        _timeCount = 0.0f;
      }
    }

    Target.localPosition 
      = data.position 
        - new Vector2(Screen.width, Screen.height)/2 
        - new Vector2(0, Target.rect.height/2-15);
  }
}