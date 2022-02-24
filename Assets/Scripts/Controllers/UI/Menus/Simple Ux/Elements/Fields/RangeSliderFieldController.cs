using Meep.Tech.Collections.Generic;
using Meep.Tech.Data;
using Simple.Ux.Data;
using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace Overworld.Controllers.SimpleUx {
  /// <summary>
  /// A simple ux controller for a range slider field
  /// </summary>
  public class RangeSliderFieldController : FieldController {

    [SerializeField]
    TMPro.TextMeshProUGUI _titleTextField;

    [SerializeField]
    Sprite _disabledKnob;

    [SerializeField]
    TMPro.TextMeshProUGUI _valueIndicator;

    [SerializeField]
    Slider _slider;

    [SerializeField]
    Image _sliderKnob;

    public override HashSet<Type> ValidFieldDataTypes
      => base.ValidFieldDataTypes
        .Append(typeof(RangeSliderField));

    public override DataField.DisplayType DisplayType
      => DataField.DisplayType.RangeSlider;

    public override TitleController Title
      => _title ??= _titleTextField.GetComponent<TitleController>(); TitleController _title;

    public override object GetCurrentlyDisplayedValue()
      => _valueIndicator?.text;

    protected override void IntializeForFieldData() {
      if(FieldData is RangeSliderField rangeSliderData) {
        _slider.minValue = (float)rangeSliderData.ValidRange.min;
        _slider.maxValue = (float)rangeSliderData.ValidRange.max;
        _slider.wholeNumbers = rangeSliderData.IsClampedToWholeNumbers;
      } // fallback:
      else {
        _slider.minValue = 0f;
        _slider.maxValue = 1f;
        _slider.wholeNumbers = false;
      }

      RefreshCurrentDisplayForCurrentValue(FieldData.Value);

      if(FieldData.IsReadOnly) {
        _slider.interactable = false;
        var newColors = _slider.colors;
        _valueIndicator.color = Color.grey + Color.blue + Color.white;
        newColors.disabledColor = Color.blue;
        _sliderKnob.sprite = _disabledKnob;
        _slider.colors = newColors;
      }
    }

    protected override void AddOnChangeListener(DataField dataField) {
      _slider.onValueChanged.AddListener(_ => OnFieldChanged());
    }

    protected override void SetFieldValid(bool toValid = true) {
      _sliderKnob.color = toValid ? ValidFieldInputBackgroundColor : InvalidFieldInputBackgroundColor;
    }

    protected override void SetFieldEnabled(bool toEnabled = true) {
      _slider.interactable = !FieldData.IsReadOnly && toEnabled;
    }

    protected override void OnFieldChanged() {
      _valueIndicator.text = _slider.value.ToString();
      base.OnFieldChanged();
      _valueIndicator.text = FieldData.Value.ToString();
    }

    protected override void RefreshCurrentDisplayForCurrentValue(object newValue) {
      _valueIndicator.text = newValue?.ToString() ?? "NULL";
    }
  }
}
