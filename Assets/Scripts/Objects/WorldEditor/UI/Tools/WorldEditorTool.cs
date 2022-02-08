using Overworld.Controllers.Editor;
using Simple.Ux.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.InputSystem;

namespace Overworld.Objects.Editor {

  /// <summary>
  /// The base class for world editor tools
  /// </summary>
  public abstract class WorldEditorTool : ScriptableObject {

    /// <summary>
    /// The status of the Use Tool action
    /// </summary>
    public enum ActionStatus {
      None,
      Down,
      Held,
      Other,
      Up
    }

    /// <summary>
    /// The other buttons and keys this overrides.
    /// </summary>
    public virtual InputActionMap ExtraBindings {
      get;
    }

    /// <summary>
    /// The other buttons and keys this overrides.
    /// </summary>
    public virtual IEnumerable<string> ReservedControlPaths 
      => _reservedControlPaths ??= ExtraBindings?
        .SelectMany(action => action.bindings
          .Select(binding => binding.path))
        ?? Enumerable.Empty<string>();

    /// <summary>
    /// Name of the tool
    /// </summary>
    public virtual string Name
      => _name;[SerializeField] string _name;

    /// <summary>
    /// the tool description.
    /// Used as the tooltip
    /// </summary>
    public virtual string Description
      => _description;[SerializeField] string _description;

    /// <summary>
    /// the button icon
    /// </summary>
    public virtual Sprite Icon
      => _icon;[SerializeField] Sprite _icon;

    /// <summary>
    /// Can be overriden to get a background preview
    /// </summary>
    public virtual Func<WorldEditorController, Sprite> GetBackgroundPreview {
      get;
    } = null;

    /// <summary>
    /// The world editor this tool is part of
    /// </summary>
    public WorldEditorController WorldEditor {
      get;
      set;
    }

    /// <summary>
    /// If the tool's alt mode is enabled.
    /// </summary>
    public bool AltModeEnabled {
      get => _altModeEnabled;
      internal protected set => _altModeEnabled = value;
    }

    /// <summary>
    /// If the tool's controled mode is enabled.
    /// </summary>
    public bool ControledModeEnabled {
      get => _controledModeEnabled;
      internal protected set => _controledModeEnabled = value;
    }

    /// <summary>
    /// If the tool's controled mode is enabled.
    /// </summary>
    public bool ShiftedModeEnabled {
      get => _shiftedModeEnabled;
      internal protected set => _shiftedModeEnabled = value;
    }

    /// <summary>
    /// The menu this came from
    /// </summary>
    public IWorldEditorToolContainerMenu FromMenu;

    /// <summary>
    /// The compiled settings window for this tool.
    /// This will be null until GetSettingsWindow is called.
    /// </summary>
    protected View SettingsWindow {
      get;
      private set;
    } bool _hasSettingsWindow 
      = true;

    [SerializeField, ReadOnly]
    ActionStatus _previousPhase;
    [SerializeField, ReadOnly]
    bool _altModeEnabled;
    [SerializeField, ReadOnly]
    bool _controledModeEnabled;
    [SerializeField, ReadOnly]
    bool _shiftedModeEnabled;
    IEnumerable<string> _reservedControlPaths;

    internal void _initialize() {
      _hasSettingsWindow = true;
      if(ExtraBindings is not null && ExtraBindings.Any()) {
        if(WorldEditor.Controls.actions.FindActionMap(ExtraBindings.name) != null) {
          WorldEditor.Controls.actions.RemoveActionMap(ExtraBindings.name);
        }

        WorldEditor.Controls.actions.AddActionMap(ExtraBindings);
        ExtraBindings.Enable();
      }

      WorldEditor.Controls.onActionTriggered += (callbackContext) => {
        if(WorldEditor.ToolController.CurrentlyEnabledTool?.Name == Name) {
          switch(callbackContext.action.name) {
            case "Use Selected Tool":
              ActionStatus phase = _previousPhase == ActionStatus.Held 
                ? ActionStatus.Up 
                : ActionStatus.None;
              switch(callbackContext.phase) {
                case UnityEngine.InputSystem.InputActionPhase.Started:
                  phase = ActionStatus.Down;
                  break;
                case UnityEngine.InputSystem.InputActionPhase.Performed:
                  phase = ActionStatus.Held;
                  break;
                case UnityEngine.InputSystem.InputActionPhase.Canceled:
                  phase = ActionStatus.Up;
                  break;
              }
              if(phase != ActionStatus.None) {
                UseTool(phase);
              }
              _previousPhase = phase;
              break;
            case "Tool Alternate Mode Modifier":
              bool enable = callbackContext.action.phase == UnityEngine.InputSystem.InputActionPhase.Started
                ||callbackContext.action.phase == UnityEngine.InputSystem.InputActionPhase.Performed;
              if(enable != AltModeEnabled) {
                OnToggleAltMode(ShiftedModeEnabled = enable);
              }
              break;
            case "Tool Control Mode Modifier":
              bool _enable = callbackContext.action.phase == UnityEngine.InputSystem.InputActionPhase.Started
                || callbackContext.action.phase == UnityEngine.InputSystem.InputActionPhase.Performed;
              if(_enable != ControledModeEnabled) {
                OnToggleControledMode(ControledModeEnabled = _enable);
              }
              break;
            case "Tool Shifted Mode Modifier":
              bool __enable = callbackContext.action.phase == UnityEngine.InputSystem.InputActionPhase.Started
                || callbackContext.action.phase == UnityEngine.InputSystem.InputActionPhase.Performed;
              if(__enable != ShiftedModeEnabled) {
                OnToggleShiftedMode(ShiftedModeEnabled = __enable);
              }
              break;
          }
        }
      };

      OnInitialize();
    }

    internal void _onUpdate() {
      switch(_previousPhase) {
        case ActionStatus.Down:
          _previousPhase = ActionStatus.Held;
          UseTool(_previousPhase);
          break;
        case ActionStatus.Held:
          UseTool(_previousPhase);
          break;
        case ActionStatus.Up:
          _previousPhase = ActionStatus.None;
          break;
      }
    }

    void OnDestroy() {
      if(ExtraBindings is not null && ExtraBindings.Any()) {
        ExtraBindings.Disable();
        WorldEditor.Controls.actions.RemoveActionMap(ExtraBindings);
      }
      OnDeinitialize();
    }

    /// <summary>
    /// Called on destroy.
    /// </summary>
    protected virtual void OnDeinitialize() {}

    /// <summary>
    /// Things to do on initialization of this tool
    /// </summary>
    protected virtual void OnInitialize() {
    }

    /// <summary>
    /// Performed when this tool is equiped.
    /// Previous tool may be null.
    /// </summary>
    protected internal virtual void OnEquip(WorldEditorTool previous = null) {
    }

    /// <summary>
    /// Performed when this tool is de-equiped
    /// Next tool may be null.
    /// </summary>
    protected internal virtual void OnDequip(WorldEditorTool next = null) {
      if(next is not null) {
        next.AltModeEnabled = AltModeEnabled;
        next.ShiftedModeEnabled = ShiftedModeEnabled;
        next.ControledModeEnabled = ControledModeEnabled;
      }

      AltModeEnabled = false;
      ShiftedModeEnabled = false;
      ControledModeEnabled = false;
      _previousPhase = ActionStatus.None;
    }

    /// <summary>
    /// Get the settings window for this tool.
    /// </summary>
    public View GetSettingsWindow() {
      if(SettingsWindow is null && _hasSettingsWindow) {
        var builder = new ViewBuilder(Name)
          .StartNewPannel(new("Settings"));
        BuildSettingsView(builder);
        if(ExtraBindings?.Any() ?? false) {
          if(builder.FinishedPannelCountForCurrentBuilder == 0
            && builder.ColumnCountForCurrentPannel == 0
            && builder.FieldCountForCurrentColumn == 0
          ) {
            builder.SetCurrentPannelTab(new("Bindings"));
          } else 
            builder.StartNewPannel(new("Bindings"));

          foreach(var action in ExtraBindings.actions) {
            builder.AddField(new ReadOnlyTextField(action.bindings.First().path, action.name));
          }
        }
        if(builder.FinishedPannelCountForCurrentBuilder == 0
           && builder.ColumnCountForCurrentPannel == 0
           && builder.FieldCountForCurrentColumn == 0
         ) {
          _hasSettingsWindow = false;
        } else
          SettingsWindow = builder.Build();
      }

      return SettingsWindow;
    }

    /// <summary>
    /// What to do when the use tool button is clicked or held.
    /// </summary>
    protected internal abstract void UseTool(ActionStatus actionStatus);

    /// <summary>
    /// executes on this tool being enabled.
    /// </summary>
    protected internal virtual void OnUpdateWhileEquiped() {
    }

    /// <summary>
    /// Things to do when alt mode is enabled or disabled for this tool
    /// </summary>
    protected virtual void OnToggleAltMode(bool toEnabled) {
    }

    /// <summary>
    /// Things to do when shifted mode is enabled or disabled for this tool
    /// </summary>
    protected virtual void OnToggleShiftedMode(bool toEnabled) {
    }

    /// <summary>
    /// Things to do when controled mode is enabled or disabled for this tool
    /// </summary>
    protected virtual void OnToggleControledMode(bool toEnabled) {
    }

    /// <summary>
    /// Can be used to customize the builder.
    /// </summary>
    protected virtual ViewBuilder BuildSettingsView(ViewBuilder builder)
      => builder;
  }
}