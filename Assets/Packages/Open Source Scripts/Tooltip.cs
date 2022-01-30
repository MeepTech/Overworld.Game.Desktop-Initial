using Overworld.Utilities;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

/// <summary>
/// Modifed by Meep, original taken from: https://answers.unity.com/questions/1253570/creating-a-tooltip-when-hovering-over-a-ui-button.html
/// </summary>
public class Tooltip : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler {

  #region Unity Inspector Settable
  public bool IsActive 
    = true;

  public RectTransform TooltipStylePrefab;

  [SerializeField]
  float _mouseHoverTimeRequired
    = 2f;

  [SerializeField]
  Vector2 _hoverOffset
    = new(20, 20);

  public string Title {
    get => _title;
    set {
      _title = value;
      if(_titleUi != null) {
        _titleUi.text = _title;
        if(!string.IsNullOrWhiteSpace(_title)) {
          _titleUi.gameObject.SetActive(true);
        }
      }
    }
  } [SerializeField] string _title;

  public string Text {
    get => _text;
    set {
      _text = value;
      if(_textUi != null) {
        _textUi.text = _text;
        if(!string.IsNullOrWhiteSpace(_text)) {
          _textUi.gameObject.SetActive(true);
        }
      }
    }
  } [SerializeField] string _text;

  #endregion

  Vector3 _min, _max;
  bool _initialized;
  Canvas _canvas;
  RectTransform _tooltip;
  TMPro.TextMeshProUGUI _textUi;
  TMPro.TextMeshProUGUI _titleUi;
  HorizontalLayoutGroup _horizontalLayoutGroup;
  bool _mousedOver;
  float _mouseOverTime;
  Vector2 _offset;

  // Start is called before the first frame update
  void OnEnable() {
    if(!_initialized) {
      _tryToInitialize();
    }
  }

  void _tryToInitialize() {
    if(!IsActive || TooltipStylePrefab is null) {
      return;
    }

    _canvas = GetComponentInParent<Canvas>();
    _tooltip = Instantiate(TooltipStylePrefab, _canvas.transform);
    _tooltip.pivot = new Vector2(0, 1);
    _tooltip.gameObject.SetActive(false);

    _horizontalLayoutGroup
      = _tooltip.GetComponent<HorizontalLayoutGroup>();
    TMPro.TextMeshProUGUI[] textUiElements
      = _tooltip.GetComponentsInChildren<TMPro.TextMeshProUGUI>();
    _textUi = textUiElements[1];
    _titleUi = textUiElements[0];
    if(string.IsNullOrWhiteSpace(_text)) {
      _textUi.gameObject.SetActive(false);
    } else
      _textUi.text = _text;
    if(string.IsNullOrWhiteSpace(_title)) {
      _titleUi.gameObject.SetActive(false);
    } else
      _titleUi.text = _title;

    _min = new Vector3(0, 0, 0);
    _max = new Vector3(Camera.main.pixelWidth, Camera.main.pixelHeight, 0);
    _initialized = true;
  }

  // Update is called once per frame
  void Update() {
    if(IsActive) {
      if(!_initialized) {
        _tryToInitialize();
      }

      if(_mousedOver) {
        _mouseOverTime += Time.deltaTime;
        if(_mouseOverTime >= _mouseHoverTimeRequired) {
          _tooltip.gameObject.SetActive(true);
          _offset = new(_hoverOffset.x, -_hoverOffset.y);
        }
      }

      if(_tooltip?.gameObject.activeInHierarchy ?? false) {
        RectTransformUtility.ScreenPointToLocalPointInRectangle(
          _canvas.transform as RectTransform,
          Input.mousePosition,
          _canvas.worldCamera,
          out Vector2 tooltipScreenTarget
        );

        // TODO: these offsets need to be fixed and tested especially for the top of the screen.
        // flip to below, if it's going to go above the screen
        if(tooltipScreenTarget.y + 10 + _tooltip.rect.height >= _max.y) {
          //_offset.ReplaceY(_offset.y - _hoverOffset.y  * 2);
          _offset = _offset.ReplaceY(_offset.y - _hoverOffset.y  * 2 - _tooltip.rect.height);
          //_tooltip.pivot = new Vector2(_tooltip.pivot.x, 1);
          _horizontalLayoutGroup.childAlignment 
            = _horizontalLayoutGroup.childAlignment == TextAnchor.UpperLeft 
              || _horizontalLayoutGroup.childAlignment == TextAnchor.LowerLeft
                ? TextAnchor.UpperLeft
                : TextAnchor.UpperRight;
        }

        // flip to below if it's above already and would go off the screen
        if(_tooltip.pivot.y == 1 && tooltipScreenTarget.y - _tooltip.rect.height <= 0) {
          _offset = _offset.ReplaceY(_offset.y + _hoverOffset.y * 2);
          _tooltip.pivot = new Vector2(_tooltip.pivot.x, 0);
          _horizontalLayoutGroup.childAlignment
            = _horizontalLayoutGroup.childAlignment == TextAnchor.UpperLeft
              || _horizontalLayoutGroup.childAlignment == TextAnchor.LowerLeft
                ? TextAnchor.LowerLeft
                : TextAnchor.LowerRight;
        }

        if(_tooltip.pivot.x == 0 && tooltipScreenTarget.x + _tooltip.rect.width >= _max.x) {
          _offset = _offset.ReplaceX(_offset.x - _hoverOffset.x * 2);
          _tooltip.pivot = new Vector2(1, _tooltip.pivot.y);
          _horizontalLayoutGroup.childAlignment
            = _horizontalLayoutGroup.childAlignment == TextAnchor.UpperLeft
              || _horizontalLayoutGroup.childAlignment == TextAnchor.UpperRight
                ? TextAnchor.UpperRight
                : TextAnchor.LowerRight;
        }

        if(_tooltip.pivot.x == 1 && tooltipScreenTarget.x - _tooltip.rect.width <= 0) {
          _offset = _offset.ReplaceX(_offset.x + _hoverOffset.x * 2);
          _tooltip.pivot = new Vector2(0, _tooltip.pivot.y);
          _horizontalLayoutGroup.childAlignment
            = _horizontalLayoutGroup.childAlignment == TextAnchor.UpperLeft
              || _horizontalLayoutGroup.childAlignment == TextAnchor.UpperRight
                ? TextAnchor.UpperLeft
                : TextAnchor.LowerLeft;
        }

        tooltipScreenTarget += _offset;
        _tooltip.transform.position = _canvas.transform.TransformPoint(tooltipScreenTarget);
      }
    }
  }

  public void OnPointerEnter(PointerEventData eventData) {
    _mousedOver = true;
  }

  public void OnPointerExit(PointerEventData eventData) {
    _mousedOver = false;
    _mouseOverTime = 0;
    _tooltip.gameObject.SetActive(false);
  }
}
