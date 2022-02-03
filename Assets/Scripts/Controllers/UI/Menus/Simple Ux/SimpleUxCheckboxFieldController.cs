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

    /*public override float ItemHeight
      => Mathf.Max(InputFieldMinHeight, Mathf.Abs(Title.RectTransform.parent.GetComponent<RectTransform>().rect.height));*/

    public override DataField.DisplayType DisplayType
      => DataField.DisplayType.Toggle;

    public override SimpleUxTitleController Title
      => _title ??= _titleTextField.GetComponent<SimpleUxTitleController>(); SimpleUxTitleController _title;

    public override object GetCurrentValue()
      => _toggle.isOn;

    protected override void _intializeForFieldData() {
      _refreshCurrentDisplayForCurrentValue((bool)FieldData.Value);
      if(FieldData.IsReadOnly) {
        _toggle.interactable = false;
        var newColors = _toggle.colors;
        newColors.disabledColor = Color.blue;
        _toggle.colors = newColors;
      }
    }

    protected override string GetTitleText()
      => ": " + FieldData.Name;

    protected override void _addOnChangeListener(DataField dataField) {
      _toggle.onValueChanged.AddListener(_ => OnFieldChanged());
    }

    protected override void _setFieldValid(bool toValid = true) {
      _toggleBackground.color = toValid ? ValidFieldInputBackgroundColor : InvalidFieldInputBackgroundColor;
    }

    protected override void _setFieldEnabled(bool toEnabled = true) {
      _toggle.interactable = !FieldData.IsReadOnly && toEnabled;
    }

    protected override void _refreshCurrentDisplayForCurrentValue(object newValue) {
      _toggle.isOn = (bool)newValue;
    }
  }
}
