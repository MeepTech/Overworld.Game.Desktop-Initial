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
        _slider.minValue = rangeSliderData.ValidRange.min;
        _slider.maxValue = rangeSliderData.ValidRange.max;
        _slider.wholeNumbers = rangeSliderData.IsClampedToWholeNumbers;
      } else if(FieldData.Validation is (int minClamped, int maxClamped)) {
        _slider.wholeNumbers = true;
        _slider.minValue = minClamped;
        _slider.maxValue = maxClamped;
      } else if(FieldData.Validation is (float min, float max)) {
        _slider.minValue = min;
        _slider.maxValue = max;
      } // fallback:
      else {
        _slider.minValue = 0f;
        _slider.maxValue = 1f;
      }

      _valueIndicator.text = FieldData.Value?.ToString() ?? "NULL";

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
      base.OnFieldChanged();
      _valueIndicator.text = FieldData.Value.ToString();
    }
  }
}
