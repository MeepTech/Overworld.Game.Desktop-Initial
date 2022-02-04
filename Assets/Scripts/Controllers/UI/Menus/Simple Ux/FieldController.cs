using Overworld.Ux.Simple;
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Overworld.Controllers.SimpleUx {

  /// <summary>
  /// The base class for simple UX field controllers.
  /// This should be placed on the field.
  /// To impliment this properly there should be a call to OnFieldChanged() in the child class somewhere.
  /// </summary>
  public abstract class FieldController : MonoBehaviour, IFieldController {

    /// <summary>
    /// The default background color for the input of the field when it's value is valid
    /// </summary>
    public static readonly Color ValidFieldInputBackgroundColor = new(0, 230, 255, 150);

    /// <summary>
    /// The default background color for the input of the field when it's value is invalid
    /// </summary>
    public static readonly Color InvalidFieldInputBackgroundColor = new(255, 192, 203, 150);

    /// <summary>
    /// The object for the title of this field.
    /// </summary>
    protected GameObject TitleObject
      => Title.gameObject;

    /// <summary>
    /// The title of this field.
    /// </summary>
    public abstract TitleController Title {
      get;
    }

    ///<summary><inheritdoc/></summary>
    public abstract DataField.DisplayType DisplayType {
      get;
    }

    ///<summary><inheritdoc/></summary>
    public virtual HashSet<Type> ValidFieldDataTypes {
      get;
    } = new();

    ///<summary><inheritdoc/></summary>
    public ViewController View {
      get;
      internal set;
    }

    ///<summary><inheritdoc/></summary>
    public DataField FieldData {
      get;
      protected set;
    }

    ///<summary><inheritdoc/></summary>
    public IUxViewElement Element 
      => FieldData;

    ///<summary><inheritdoc/></summary>
    public ColumnController Column {
      get;
      internal set;
    }

    ///<summary><inheritdoc/></summary>
    public RectTransform RectTransform
      => _rectTransfrom ??= GetComponent<RectTransform>();
    RectTransform _rectTransfrom;

    /// <summary>
    /// Used to initialize the field for the applied FieldData.
    /// </summary>
    protected abstract void _intializeForFieldData();

    /// <summary>
    /// Used to attach OnFieldChanged to a listener.
    /// </summary>
    protected abstract void _addOnChangeListener(DataField dataField);

    /// <summary>
    /// Should be called when the field is changed.
    /// </summary>
    protected virtual void OnFieldChanged() {
      DataField original = FieldData.Copy();
      if(!FieldData.TryToSetValue(GetCurrentValue(), out string message)) {
        Debug.LogError(message);
      } else {
        View._onUpdated(original);
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
    protected internal virtual void _onThisFieldUpdated(DataField originalFieldData) {}

    /// <summary>
    /// Called when any other field in the view is updated, including this one.
    /// updatedElement may also be null (this happens when the view first finishes initializing).
    /// </summary>
    protected internal virtual void _onOtherFieldUpdated(View view, IUxViewElement updatedElement = null) {
      _updateFieldEnabledState();
    }

    /// <summary>
    /// Used to refresh the currently displayed value to the internal one.
    /// For use in the full revert, and for you to initialize the displayed value in InitializeFor
    /// </summary>
    protected abstract void RefreshCurrentDisplayForCurrentValue(object newValue);

    /// <summary>
    /// Can be used to help validate the field type.
    /// </summary>
    protected void ValidateFieldType(Type fieldType, HashSet<Type> validFieldDataTypes) {
      if(!validFieldDataTypes.Any(validType => validType.IsAssignableFrom(fieldType))) {
        throw new System.ArgumentException($"Data Field of Type: {FieldData.GetType()}, does not match one of the the required types for the Field Controller:{GetType()}\nValid Types:\n of {string.Join('\t', validFieldDataTypes.Select(x => $"\n{x.Name}"))}");
      }
    }

    /// <summary>
    /// Used to refresh the currently displayed value to the internal one
    /// </summary>
    internal virtual void _refreshCurrentValue() {
      var original = GetCurrentValue();
      var @new = FieldData.Value;
      if(original != @new) {
        RefreshCurrentDisplayForCurrentValue(@new);
        OnFieldChanged();
      }
    }

    internal void _intializeFor(DataField dataField) {
      if(dataField.Type == DisplayType) {
        FieldData = dataField;
      } else
        throw new System.NotSupportedException($"SimpleUx Field Controller of type {GetType().Name} cannot handle display type of {dataField.Type}. The controller requires a field of type {DisplayType}");
      
      _initializeTooltip();
      _initializeTitle();
      if(ValidFieldDataTypes?.Any() ?? false) {
        ValidateFieldType(FieldData.GetType(), ValidFieldDataTypes);
      }
      _intializeForFieldData();
      _addOnChangeListener(dataField);
    }

    /// <summary>
    /// Can be used to check if the field should be enabled, and update the field to the correct state.
    /// </summary>
    internal void _updateFieldEnabledState() {
      /// check if the field should still be enabled:
      if(FieldData.Enable is not null)
        if (!FieldData.Enable(FieldData, View.Data)) {
          _setFieldEnabled(false);
        } else
          _setFieldEnabled(true);
    }

    void _initializeTooltip() {
      if(!string.IsNullOrWhiteSpace(FieldData.Tooltip)) {
        Tooltip tooltip = TitleObject.AddComponent<Tooltip>();
        tooltip.TooltipStylePrefab = SimpleUxGlobalManager.DefaultTooltipPrefab;
        tooltip.Text = FieldData.Tooltip;
      }
    }

    void _initializeTitle() {
      // set up the title if we have one
      if(!string.IsNullOrWhiteSpace(FieldData.Name)) {
        Title._initializeFor(new Title(GetTitleText(), FieldData.Tooltip));
      } // hide if no title 
      else {
        TitleObject.SetActive(false);
      }
    }

    /// <summary>
    /// Used to get the title text from the field data.
    /// </summary>
    protected virtual string GetTitleText()
      => FieldData.Name + ":";
  }
}
