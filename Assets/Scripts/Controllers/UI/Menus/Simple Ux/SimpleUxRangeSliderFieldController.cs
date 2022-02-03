using Overworld.Ux.Simple;
using UnityEngine;
using UnityEngine.UI;

namespace Overworld.Controllers.SimpleUx {
  /// <summary>
  /// A simple ux controller for a range slider field
  /// </summary>
  public class SimpleUxRangeSliderFieldController : SimpleUxFieldController {

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

    public override DataField.DisplayType DisplayType
      => DataField.DisplayType.RangeSlider;

    public override SimpleUxTitleController Title
      => _title ??= _titleTextField.GetComponent<SimpleUxTitleController>(); SimpleUxTitleController _title;

    public override object GetCurrentValue()
      => _valueIndicator?.text;

    protected override void _intializeForFieldData() {
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

      _refreshCurrentDisplayForCurrentValue(FieldData.Value);

      if(FieldData.IsReadOnly) {
        _slider.interactable = false;
        var newColors = _slider.colors;
        _valueIndicator.color = Color.grey + Color.blue + Color.white;
        newColors.disabledColor = Color.blue;
        _sliderKnob.sprite = _disabledKnob;
        _slider.colors = newColors;
      }
    }

    protected override void _addOnChangeListener(DataField dataField) {
      _slider.onValueChanged.AddListener(_ => OnFieldChanged());
    }

    protected override void _setFieldValid(bool toValid = true) {
      _sliderKnob.color = toValid ? ValidFieldInputBackgroundColor : InvalidFieldInputBackgroundColor;
    }

    protected override void _setFieldEnabled(bool toEnabled = true) {
      _slider.interactable = !FieldData.IsReadOnly && toEnabled;
    }

    protected override void OnFieldChanged() {
      _valueIndicator.text = _slider.value.ToString();
      base.OnFieldChanged();
      _valueIndicator.text = FieldData.Value.ToString();
    }

    protected override void _refreshCurrentDisplayForCurrentValue(object newValue) {
      _valueIndicator.text = newValue?.ToString() ?? "NULL";
    }
  }
}
