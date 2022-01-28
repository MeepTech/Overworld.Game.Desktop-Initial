using Overworld.Ux.Simple;
using UnityEngine;
using UnityEngine.UI;

namespace Overworld.Controllers.SimpleUx {

  /// <summary>
  /// A simple ux controller for a text field
  /// </summary>
  public partial class SimpleUxTextFieldController : SimpleUxFieldController {

    [SerializeField]
    TMPro.TextMeshProUGUI _titleTextField;

    [SerializeField]
    TMPro.TMP_InputField _inputTextController;

    [SerializeField]
    TMPro.TextMeshProUGUI _inputTextField;

    [SerializeField]
    TMPro.TextMeshProUGUI _inputTextPlaceholder;

    [SerializeField]
    Image _inputTextBackground;

    public override UxDataField.DisplayType DisplayType
      => UxDataField.DisplayType.Text;

    public override GameObject FieldTitle
      => _titleTextField.gameObject;

    public override object GetCurrentValue()
      => _inputTextField.text;

    protected override void _intializeForFieldData() {
      _inputTextField.text = FieldData.Value as string;
      _titleTextField.text = FieldData.Name + ":";
      if(FieldData.IsReadOnly) {
        _inputTextField.color = Color.white;
        _inputTextBackground.enabled = false;
        _inputTextPlaceholder.gameObject.SetActive(false);
      }
    }

    protected override void _addOnChangeListener(UxDataField dataField) {
      _inputTextController.onValueChanged.AddListener(_ => OnFieldChanged());
    }

    protected override void _setFieldValid(bool toValid = true) {
      _inputTextBackground.color = toValid ? ValidFieldInputBackgroundColor : InvalidFieldInputBackgroundColor;
    }

    protected override void _setFieldEnabled(bool toEnabled = true) {
      _inputTextController.readOnly = !FieldData.IsReadOnly && !toEnabled;
    }
  }
}
