using Meep.Tech.Data;
using Meep.Tech.Collections.Generic;
using Simple.Ux.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

namespace Overworld.Controllers.SimpleUx {

  /// <summary>
  /// A simple ux controller for a range slider field
  /// </summary>
  public class DropdownFieldController : FieldController {

    [SerializeField]
    TMPro.TextMeshProUGUI _titleTextField;

    [SerializeField]
    TMPro.TextMeshProUGUI _dropdownSelectedItemText;

    [SerializeField]
    TMPro.TMP_Dropdown _dropdown;

    [SerializeField]
    Image _dropdownBackgroundImage;

    public override HashSet<Type> ValidFieldDataTypes 
      => base.ValidFieldDataTypes
        .Append(typeof(DropdownSelectField));

    public override DataField.DisplayType DisplayType
      => DataField.DisplayType.Dropdown;

    public override TitleController Title
      => _title ??= _titleTextField.GetComponent<TitleController>(); TitleController _title;

    public override object GetCurrentValue()
      => (FieldData as DropdownSelectField).Options.TryToGetPairAtIndex(_dropdown.value).AsSingleItemEnumerable().ToList();

    protected override void _intializeForFieldData() {

      RefreshCurrentDisplayForCurrentValue(FieldData.Value);
      if(FieldData.IsReadOnly) {
        _dropdown.interactable = false;
        _dropdownBackgroundImage.enabled = false;
        _dropdownSelectedItemText.color = Color.white;
      }
    }

    protected override void RefreshCurrentDisplayForCurrentValue(object newValue) {
      // re-set the options
      _dropdown.options = (FieldData as DropdownSelectField).Options
        .Select(option => new TMPro.TMP_Dropdown.OptionData(option.Key)).ToList();

      // swap out the value:
      var value = newValue as List<KeyValuePair<string, object>>;
      if(value?.Any() ?? false) {
        int index = (FieldData as DropdownSelectField).Options.IndexOf(value.First().Key);
        _dropdown.value = index;
      } else if (newValue is not null && value is null)
        throw new ArgumentException($"Value[={newValue}] of type: {newValue.GetType().FullName}, is not a List of KeyValuePair<string, object> with a single entry.");
    }

    protected override void _addOnChangeListener(DataField dataField) {
      _dropdown.onValueChanged.AddListener(_ => OnFieldChanged());
    }

    protected override void _setFieldValid(bool toValid = true) {
      ColorBlock colorBlock = _dropdown.colors;
      colorBlock.normalColor = toValid ? Color.white : InvalidFieldInputBackgroundColor;
      _dropdown.colors = colorBlock;
    }

    protected override void _setFieldEnabled(bool toEnabled = true) {
      _dropdown.interactable = !FieldData.IsReadOnly && toEnabled;
    }
  }
}
