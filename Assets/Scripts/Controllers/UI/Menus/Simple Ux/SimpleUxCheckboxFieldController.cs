using Overworld.Ux.Simple;
using UnityEngine;
using UnityEngine.UI;

namespace Overworld.Controllers.SimpleUx {
  /// <summary>
  /// A simple ux controller for a checkbox toggle field
  /// </summary>
  public class SimpleUxCheckboxFieldController : SimpleUxFieldController {

    [SerializeField]
    TMPro.TextMeshProUGUI _titleTextField;

    [SerializeField]
    Toggle _toggle;

    [SerializeField]
    Image _toggleBackground;

    public override UxDataField.DisplayType DisplayType
      => UxDataField.DisplayType.Toggle;

    public override GameObject FieldTitle
      => _titleTextField.gameObject;

    public override object GetCurrentValue()
      => _toggle.isOn;

    protected override void _intializeForFieldData() {
      _titleTextField.text = ": " + FieldData.Name;
      if(FieldData.IsReadOnly) {
        _toggle.interactable = false;
        var newColors = _toggle.colors;
        newColors.disabledColor = Color.blue;
        _toggle.colors = newColors;
      }
    }

    protected override void _addOnChangeListener(UxDataField dataField) {
      _toggle.onValueChanged.AddListener(_ => OnFieldChanged());
    }

    protected override void _setFieldValid(bool toValid = true) {
      _toggleBackground.color = toValid ? ValidFieldInputBackgroundColor : InvalidFieldInputBackgroundColor;
    }

    protected override void _setFieldEnabled(bool toEnabled = true) {
      _toggle.interactable = !FieldData.IsReadOnly && !toEnabled;
    }
  }
}
