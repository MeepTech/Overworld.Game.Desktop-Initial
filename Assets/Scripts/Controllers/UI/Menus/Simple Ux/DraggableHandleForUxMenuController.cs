using UnityEngine;
using UnityEngine.EventSystems;

/// <summary>
/// A handle for dragging a UX menu. Ususually used on titles.
/// </summary>
public class DraggableHandleForUxMenuController : MonoBehaviour, IDragHandler, IPointerClickHandler {

  /// <summary>
  /// What should be dragged when this is clicked.
  /// </summary>
  [SerializeField]
  RectTransform _target;
  float _timeCount = 0.0f;

  // Drag the selected item.
  public void OnDrag(PointerEventData data) {
    _target.SetAsLastSibling();
    if(data.dragging) {
      // Object is being dragged.
      _timeCount += Time.deltaTime;
      if(_timeCount > 0.25f) {
        _timeCount = 0.0f;
      }
    }

    _target.localPosition 
      = data.position 
        - new Vector2(Screen.width, Screen.height) / 2 
        - new Vector2(0, -15);
  }

  public void OnPointerClick(PointerEventData eventData) {
    _target.SetAsLastSibling();
  }
}