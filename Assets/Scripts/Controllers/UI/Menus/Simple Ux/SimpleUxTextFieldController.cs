using Overworld.Utilities;
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

    public override DataField.DisplayType DisplayType
      => DataField.DisplayType.Text;

    public override SimpleUxTitleController Title
      => _title ??= _titleTextField.GetComponent<SimpleUxTitleController>(); SimpleUxTitleController _title;

    /*public override float ItemHeight 
      => _itemHeight ?? base.ItemHeight;
    float? _itemHeight;*/

    /*RectTransform _rectTransform 
      => __rectTransform ??= GetComponent<RectTransform>(); RectTransform __rectTransform;*/

    bool _sizeIsDirty;
    bool _waitedWhileDirty;

    public override object GetCurrentValue()
      => _inputTextController.text;

    protected override void _intializeForFieldData() {
      _inputTextController.text = FieldData.Value as string;
      if(FieldData.IsReadOnly) {
        _inputTextField.color = Color.white;
        _inputTextBackground.enabled = false;
        _inputTextPlaceholder.gameObject.SetActive(false);
        _inputTextController.richText = true;
        _inputTextController.lineType = TMPro.TMP_InputField.LineType.MultiLineNewline;
        _sizeIsDirty = true;

        RectTransform.pivot = RectTransform.pivot.ReplaceY(1);
        RectTransform.anchorMin = new Vector2(0, 1);
        RectTransform.anchorMax = new Vector2(1, 1);
        _inputTextController.readOnly = true;
        _inputTextController.isRichTextEditingAllowed = false;
      } else {
        if(FieldData is TextField textField) {
          _inputTextPlaceholder.text = textField.PlaceholderText;
        }
      }
    }

    void Update() {
      /*if(_sizeIsDirty && _waitedWhileDirty) {
        _inputTextController.textComponent.ForceMeshUpdate();
        _itemHeight = _inputTextController.textComponent.preferredHeight;
        float localMin = 56;
        if(!TitleObject.activeSelf) {
          float titleHeight = _inputTextField.rectTransform.rect.height;
          _itemHeight -= titleHeight;
          localMin -= titleHeight;
        }
        _itemHeight *= (3f / 4f);
        _itemHeight = Mathf.Max(_itemHeight.Value, localMin, 36);
        _rectTransform.sizeDelta = _rectTransform.sizeDelta.ReplaceY(ItemHeight);
        View._waitingOnDirtyChildren--;
        _sizeIsDirty = false;
        _waitedWhileDirty = false;
      } else if(_sizeIsDirty) {
        View._waitingOnDirtyChildren++;
        _waitedWhileDirty = true;
      }*/
    }

    protected override void _addOnChangeListener(DataField dataField) {
      _inputTextController.onValueChanged.AddListener(_ => OnFieldChanged());
    }

    protected override void _setFieldValid(bool toValid = true) {
      _inputTextBackground.color = toValid ? ValidFieldInputBackgroundColor : InvalidFieldInputBackgroundColor;
    }

    protected override void _setFieldEnabled(bool toEnabled = true) {
      _inputTextController.readOnly = FieldData.IsReadOnly || !toEnabled;
    }
  }
}
