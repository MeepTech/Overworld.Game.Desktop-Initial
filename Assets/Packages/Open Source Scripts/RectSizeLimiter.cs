using Overworld.Utilities;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

/// <summary>
/// From: https://bitbucket.org/snippets/Democritus/5ex7n4
/// https://forum.unity.com/threads/rect-transform-size-limiter.620860/
/// </summary>
[ExecuteInEditMode]
public class RectSizeLimiter : UIBehaviour, ILayoutSelfController {

  [SerializeField]
  RectTransform _rectTransform;

  [SerializeField]
  bool _useCurrentAnchors
    = true;

  [SerializeField]
  protected Vector2 m_maxSize 
    = Vector2.zero;

  public Vector2 maxSize {
    get {
      return m_maxSize;
    }
    set {
      if(m_maxSize != value) {
        m_maxSize = value;
        SetDirty();
      }
    }
  }

  private DrivenRectTransformTracker m_Tracker;

  protected override void OnEnable() {
    base.OnEnable();
    SetDirty();
  }

  protected override void OnDisable() {
    m_Tracker.Clear();
    LayoutRebuilder.MarkLayoutForRebuild(_rectTransform);
    base.OnDisable();
  }

  protected void SetDirty() {
    if(!IsActive())
      return;

    LayoutRebuilder.MarkLayoutForRebuild(_rectTransform);
  }

  public void SetLayoutHorizontal() {
    if(m_maxSize.x > 0f && _rectTransform.rect.width > m_maxSize.x) {
      if(_useCurrentAnchors) {
        _rectTransform.SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, maxSize.x);
      } else {
        Vector3 originalPos = _rectTransform.localPosition;
        _rectTransform.sizeDelta = _rectTransform.sizeDelta.ReplaceX(maxSize.x);
        _rectTransform.localPosition = originalPos;
      }
      m_Tracker.Add(this, _rectTransform, DrivenTransformProperties.SizeDeltaX);
    }
  }

  public void SetLayoutVertical() {
    if(m_maxSize.y > 0f && _rectTransform.rect.height > m_maxSize.y) {
      if(_useCurrentAnchors) {
        _rectTransform.SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, maxSize.y);
      } else {
        Vector3 originalPos = _rectTransform.localPosition;
        _rectTransform.sizeDelta = _rectTransform.sizeDelta.ReplaceY(maxSize.y);
        _rectTransform.localPosition = originalPos;
      }
      m_Tracker.Add(this, _rectTransform, DrivenTransformProperties.SizeDeltaY);
    }
  }

#if UNITY_EDITOR
  protected override void OnValidate() {
    base.OnValidate();
    SetDirty();
  }
#endif

}