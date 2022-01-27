using Overworld.Ux.Simple;
using System;
using UnityEngine;

namespace Overworld.Controllers.SimpleUx {

  /// <summary>
  /// The base class for simple UX field controllers.
  /// This should be placed on the field.
  /// To impliment this properly there should be a call to OnFieldChanged() in the child class somewhere.
  /// </summary>
  public abstract class SimpleUxFieldController : MonoBehaviour, ISimpleUxFieldController {

    /// <summary>
    /// The default background color for the input of the field when it's value is valid
    /// </summary>
    public static readonly Color ValidFieldInputBackgroundColor = new(0, 230, 255, 150);

    /// <summary>
    /// The default background color for the input of the field when it's value is invalid
    /// </summary>
    public static readonly Color InvalidFieldInputBackgroundColor = new(255, 192, 203, 150);

    public abstract GameObject FieldTitle {
      get;
    }

    public abstract UxDataField.DisplayType DisplayType {
      get;
    }

    public SimpleUxViewController View {
      get;
      internal set;
    }

    public UxDataField FieldData {
      get;
      protected set;
    }

    public void IntializeFor(UxDataField dataField) {
      if(dataField.Type != DisplayType) {
        FieldData = dataField;
      } else throw new System.NotSupportedException($"SimpleUx Field Controller of type {GetType().Name} cannot handle display type of {dataField.Type}. The controller requires a field of type {DisplayType}");
      _initializeTooltip();
      _intializeForFieldData();
      _addOnChangeListener(dataField);
    }

    /// <summary>
    /// Used to initialize the field for the applied FieldData.
    /// </summary>
    protected abstract void _intializeForFieldData();

    /// <summary>
    /// Used to attach OnFieldChanged to a listener.
    /// </summary>
    protected abstract void _addOnChangeListener(UxDataField dataField);

    /// <summary>
    /// Should be called when the field is changed.
    /// </summary>
    protected virtual void OnFieldChanged() {
      UxDataField original = FieldData.Copy();
      if(!FieldData.TryToSetValue(GetCurrentValue(), out string message)) {
        Debug.LogError(message);
      } else {
        View.OnUpdated(original);
      }
    }

    public abstract object GetCurrentValue();

    /// <summary>
    /// Used to set the field as enabled or disbaled.
    /// </summary>
    protected abstract void _setFieldEnabled(bool toEnabled = true);

    /// <summary>
    /// Used to set the field as valid or invalid.
    /// </summary>
    protected abstract void _setFieldValid(bool toValid = true);

    /// <summary>
    /// Can add extra logic when this field specifically is updated.
    /// </summary>
    protected internal virtual void OnThisFieldUpdated(UxDataField originalFieldData) {}

    /// <summary>
    /// Called when any other field in the view is updated, including this one.
    /// </summary>
    protected internal virtual void OnViewUpdated(UxView view, IUxViewElement updatedElement = null) {
      _updateFieldEnabledState();
    }

    /// <summary>
    /// Can be used to check if the field should be enabled, and update the field to the correct state.
    /// </summary>
    internal void _updateFieldEnabledState() {
      /// check if the field should still be enabled:
      if(FieldData.Enable is not null && !FieldData.Enable(FieldData, View.View)) {
        _setFieldEnabled(false);
      } else
        _setFieldEnabled(true);
    }

    void _initializeTooltip() {
      if(!string.IsNullOrWhiteSpace(FieldData.Tooltip)) {
        Tooltip tooltip = FieldTitle.AddComponent<Tooltip>();
        tooltip.Text = FieldData.Tooltip;
      }
    }
  }
}
