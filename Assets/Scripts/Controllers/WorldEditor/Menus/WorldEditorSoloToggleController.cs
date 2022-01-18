using UnityEngine.UI;

public class WorldEditorSoloToggleController : UnityEngine.MonoBehaviour {

  [UnityEngine.SerializeField]
  WorldEditorController _worldEditor;

  [UnityEngine.SerializeField]
  UnityEngine.UI.Toggle _toggle;

  [UnityEngine.SerializeField]
  Image _icon;

  [UnityEngine.SerializeField]
  WorldEditorToggle _data;

  void Awake() {
    Tooltip tooltip = GetComponent<Tooltip>();
    tooltip.Text = _data.Description;
    tooltip.Title = _data.Name;

    _icon.sprite = _data.Icon;

    _toggle.onValueChanged.AddListener(toTrue => {
      if(toTrue) {
        _data.Enable(_worldEditor);
      } else
        _data.Disable(_worldEditor);
    });
  }
}
