using Meep.Tech.Collections.Generic;
using Meep.Tech.Data;
using Simple.Ux.Data;
using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace Overworld.Controllers.SimpleUx {

  /// <summary>
  /// A simple ux controller for a checkbox toggle field
  /// </summary>
  public class CheckboxFieldController : FieldController {

    [SerializeField]
    TMPro.TextMeshProUGUI _titleTextField;

    [SerializeField]
    Toggle _toggle;

    [SerializeField]
    Image _toggleBackground;

    public override HashSet<Type> ValidFieldDataTypes
      => base.ValidFieldDataTypes
        .Append(typeof(ToggleField));

    public override DataField.DisplayType DisplayType
      => DataField.DisplayType.Toggle;

    public override TitleController Title
      => _title ??= _titleTextField.GetComponent<TitleController>(); TitleController _title;

    public override object GetCurrentlyDisplayedValue()
      => _toggle.isOn;

    protected override void IntializeForFieldData() {
      RefreshCurrentDisplayForCurrentValue((bool)FieldData.Value);
      if(FieldData.IsReadOnly) {
        _toggle.interactable = false;
        var newColors = _toggle.colors;
        newColors.disabledColor = Color.blue;
        _toggle.colors = newColors;
      }
    }

    protected override string GetTitleText()
      => ": " + FieldData.Name;

    protected override void AddOnChangeListener(DataField dataField) {
      _toggle.onValueChanged.AddListener(_ => OnFieldChanged());
    }

    protected override void SetFieldValid(bool toValid = true) {
      _toggleBackground.color = toValid ? ValidFieldInputBackgroundColor : InvalidFieldInputBackgroundColor;
    }

    protected override void SetFieldEnabled(bool toEnabled = true) {
      _toggle.interactable = !FieldData.IsReadOnly && toEnabled;
    }

    protected override void RefreshCurrentDisplayForCurrentValue(object newValue) {
      _toggle.isOn = (bool)newValue;
    }
  }
}
