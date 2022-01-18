using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(Tooltip))]
public class WorldEditorGeneralToolButtonController : MonoBehaviour {
  [SerializeField]
  internal Image _icon;
  [SerializeField]

  Image _backgroundImage;
  internal Toggle _toggle;
  internal Tooltip _tooltip;

  internal void UpdateBackgroundImage(Sprite sprite) {
    if(sprite == null) {
      if(_backgroundImage.sprite != null) {
        _backgroundImage.sprite = null;
        _backgroundImage.enabled = false;
      }

      return;
    }

    if(!_backgroundImage.enabled) {
      _backgroundImage.enabled = true;
    }

    if(_backgroundImage.sprite == null || _backgroundImage.sprite != sprite) {
      _backgroundImage.sprite = sprite;
    }
  }
}
