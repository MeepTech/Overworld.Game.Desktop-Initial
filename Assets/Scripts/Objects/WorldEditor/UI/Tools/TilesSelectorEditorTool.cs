using Meep.Tech.Collections;
using Meep.Tech.Collections.Generic;
using Overworld.Controllers.Editor;
using Overworld.Utilities;
using Simple.Ux.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEngine;
using UnityEngine.InputSystem;

namespace Overworld.Objects.Editor {
  public class TilesSelectorEditorTool : WorldEditorTool, IHasAnOpenableSettingsWindow {

    const string SelectedTileCountFieldKey = "Selected Tile Count";
    const string LockModeFieldKey = "Lock Additive Mode";
    const string ModeFieldKey = "Additive Mode";
    const string StickyFieldKey = "Sticky";
    const string UseBrushFieldKey = "Use Brush To Draw";
    const string SnapDrawModeFieldKey = "Snap Draw Mode";

    const string ClearSelectionActionName
      = "Clear Select Tool Selection";

    public override InputActionMap ExtraBindings {
      get;
    } = new InputActionMap(nameof(TilesSelectorEditorTool))
      .AppendAction(ClearSelectionActionName, InputActionType.Button, "<Keyboard>/0");

    /// <summary>
    /// Edit modes for the tile selector tool
    /// </summary>
    public enum EditModes {
      Add,
      Subtract
    }

    public override string Description
      => $"Select tiles with the mouse. Drag to select a square area of tiles."
        + $"\n\t- Holding shift while using most tools will override to this tool. Snap mode is not available in override mode."
        + $"\n\t- CTRL+Click to add or subtract from the current selection (uses the first clicked and toggled tile to determine whether to add or remove)."
        + $"\n\t- ALT+Click to draw by dragging, add or subtract is determined same as above"
        + $"\n\t- Shift+Click to snap draw in a square from the last clicked tile (can be used to draw lines too)"
        + $"\n\t- ESC to clear current selection.";

    /// <summary>
    /// Check if the selection is active
    /// </summary>
    public bool SelectionIsActive
      => _selectedTiles.Any();

    internal HashSet<Vector2Int> _selectedTiles= new();
    (Vector2Int init, Vector2Int end)? _currentAreaSelection;
    [SerializeField, ReadOnly]
    Vector2Int? _lastModifiedLocation = null;
    [SerializeField, ReadOnly]
    Vector2Int? _currentSelectedTile = null;
    [SerializeField, ReadOnly]
    Vector2Int? _rightClickLocation;
    [SerializeField, ReadOnly]
    bool? _specialSetToEnabled;
    [SerializeField, ReadOnly]
    bool _altBrushModeIsEnabled;
    [SerializeField, ReadOnly]
    bool _stickySelectIsEnabled;
    [SerializeField, ReadOnly]
    bool _selectSpecialAltSingleClickActive;
    [SerializeField, ReadOnly]
    bool _modeLockActive;
    [SerializeField, ReadOnly]
    bool _brushLockActive;
    [SerializeField, ReadOnly]
    bool _stickyLockActive;
    [SerializeField, ReadOnly]
    bool _snapModeLockActive;
    [SerializeField, ReadOnly]
    bool _snapModeLockIsEnabled;

    /// <summary>
    /// Check if a tile is selected by the tool
    /// </summary>
    public bool IsSelected(Vector2Int vector2Int)
      => _selectedTiles.Contains(vector2Int);

    /// <summary>
    /// Clear the meta settings for the temp select
    /// </summary>
    public void ClearMetaSettings() {
      _currentSelectedTile = null;
      _selectSpecialAltSingleClickActive = false;
      _currentAreaSelection = null;
      _lastModifiedLocation = null;

      _clearSpecialSetTo();
      _dissableBrushMode();
      _setStickyInactive();
      _snapModeLockIsEnabled 
        = _snapModeLockActive;
    }

    protected override void OnInitialize() {
      WorldEditor.Controls.onActionTriggered += context => {
        if(context.action.name == ClearSelectionActionName) {
          _dismissCurrentSelection();
        }
      };
    }

    protected override ViewBuilder BuildSettingsView(ViewBuilder builder)
      => builder
        .AddField(new ReadOnlyTextField(_selectedTiles.Count.ToString(), SelectedTileCountFieldKey))
        .AddField(new ToggleField(StickyFieldKey, "If a new select shouldn't be created every time you click, and values should be added to, or removed from the current selection."))
        .AddField(new ToggleField(LockModeFieldKey, "Locks the select mode to one of the below modes."))
        .AddField(new DropdownSelectField<EditModes>(ModeFieldKey) {
          EnabledIfCheckers = new() {
            {
              "unLocked",
              (field, view) => view.GetFieldValue<bool>(LockModeFieldKey)
            }
          }
        })
      .AddField(new ToggleField(UseBrushFieldKey, "Locks to brush add/subtract mode.") {
        EnabledIfCheckers = new() {
          {
            "unLocked",
            (field, view) => !view.GetFieldValue<bool>(SnapDrawModeFieldKey)
          }
        }
      })
      .AddField(new ToggleField(SnapDrawModeFieldKey, "Locks to Snap add/subtract mode.") {
        EnabledIfCheckers = new() {
          {
            "unLocked",
            (field, view) => !view.GetFieldValue<bool>(UseBrushFieldKey)
          }
        }
      });

    void OnDestroy() {
      ClearMetaSettings();
      _selectedTiles.Clear();
    }

    protected internal override void UseTool(ActionStatus actionStatus) {
      Vector2Int mouseoverTileLocation;
      switch(actionStatus) {
        case ActionStatus.Down:
          mouseoverTileLocation = WorldEditor.WorldController.TileSelector.HoveredTileLocation;
          bool specialModeEnabled = ControledModeEnabled || AltModeEnabled || ShiftedModeEnabled;
          if((_stickyLockActive = SettingsWindow?.Data.GetFieldValue<bool>(StickyFieldKey) ?? false) || ControledModeEnabled) {
            _setStickyActive();
          }
          if((_modeLockActive = SettingsWindow?.Data.GetFieldValue<bool>(LockModeFieldKey) ?? false) || specialModeEnabled) {
            _setSpecialSetAdditiveType();
          }
          // brush:
          if((_brushLockActive = SettingsWindow?.Data.GetFieldValue<bool>(UseBrushFieldKey) ?? false) || AltModeEnabled) {
            _selectSpecialAltSingleClickActive = true;
            _enableBrushMode();
          } // snap mode:
          else if((_snapModeLockActive = SettingsWindow?.Data.GetFieldValue<bool>(SnapDrawModeFieldKey) ?? false) || ShiftedModeEnabled) {
            if(_lastModifiedLocation is not null) {

              // This prevents the snap functionaliy from making a new empty selection.
              if(_specialSetToEnabled.HasValue && !_specialSetToEnabled.Value) {
                if(!_stickySelectIsEnabled) {
                  _specialSetToEnabled = true;
                }
              }
              _snapModeLockIsEnabled = true;
              _currentSelectedTile = mouseoverTileLocation;
              _lastModifiedLocation = _selectSpecialAltSingleClickActive ? _lastModifiedLocation : mouseoverTileLocation;
              _currentAreaSelection = (_lastModifiedLocation.Value, mouseoverTileLocation);
              _onSelectDone(mouseoverTileLocation);

              return;
            }
          }

          _currentSelectedTile = mouseoverTileLocation;
          _lastModifiedLocation = _selectSpecialAltSingleClickActive ? _lastModifiedLocation : mouseoverTileLocation;
          break;
        case ActionStatus.Held:
          mouseoverTileLocation = WorldEditor.WorldController.TileSelector.HoveredTileLocation;
          // If we dragged the mouse at all:
          if(_lastModifiedLocation.HasValue && mouseoverTileLocation != _lastModifiedLocation.Value) {
            // no alt draw mode, on drag use area selection:
            if(!_altBrushModeIsEnabled) {
              if(_currentAreaSelection is null) {
                _currentAreaSelection = (_lastModifiedLocation.Value, mouseoverTileLocation);
              } else {
                _currentAreaSelection = (_currentAreaSelection.Value.init, mouseoverTileLocation);
              }

              //_selectSpecialAltSingleClickActive = false;
              //_currentSelectedTile = null;
              WorldEditor.TilesSelectionController.ClearTempSelection();
              _currentAreaSelection.Value.init.Until(
                _currentAreaSelection.Value.end,
                tempSelectedTile => WorldEditor.TilesSelectionController.ToggleSelected(this, tempSelectedTile, true, true)
              );
            } // alt draw mode is enabled, change only the drawn on tile to what we originally chose:
            else {
              WorldEditor.TilesSelectionController.ToggleSelected(this, mouseoverTileLocation, _specialSetToEnabled.Value);
            }

            _currentSelectedTile = null;
            _selectSpecialAltSingleClickActive = false;
            _lastModifiedLocation = mouseoverTileLocation;
          }

          break;
        case ActionStatus.Up:
          if(!_snapModeLockIsEnabled) {
            mouseoverTileLocation = WorldEditor.WorldController.TileSelector.HoveredTileLocation;
            _onSelectDone(mouseoverTileLocation);
          } else if (!_snapModeLockActive) {
            _snapModeLockIsEnabled = false;
          }
          break;
      }
    }

    protected internal override void OnDequip(WorldEditorTool next = null) {
      ClearMetaSettings();
    }

    protected internal override void OnUpdateWhileEquiped() {
      /*Vector2Int mouseoverTileLocation = WorldEditor.WorldController.TileSelector.HoveredTileLocation;

      // right click to dismiss (unless you move too far)
      if(Input.GetMouseButtonDown(1)) {
        _rightClickLocation = mouseoverTileLocation;
      }
      if(Input.GetMouseButton(1)) {
        if(_rightClickLocation.HasValue && _rightClickLocation.Value == mouseoverTileLocation) {
          _rightClickLocation = mouseoverTileLocation;
        } else
          _rightClickLocation = null;
      }
      if(Input.GetMouseButtonUp(1)) {
        if(_rightClickLocation is not null) {
          _dismissCurrentSelection(WorldEditor);
        }
      }

      // left click to select
      if(Input.GetMouseButtonDown(0)) {
        // This doesn't work when using the selector in override mode. 
        if(Input.GetKey(KeyCode.LeftShift) || Input.GetKey(KeyCode.RightShift)) {
          // shift and alt lets you draw boxes and lines with a click
          // works in override mode too
          if(Input.GetKey(KeyCode.LeftAlt) || Input.GetKey(KeyCode.RightAlt)) {
            _selectSpecialAltSingleClickActive = true;
            if(Input.GetKey(KeyCode.LeftControl) || Input.GetKey(KeyCode.RightControl)) {
              _stickySelectIsEnabled = true;
              _specialSetToEnabled = !_selectedTiles.Contains(mouseoverTileLocation);
            }
          } // if not clicking alt and not in override mode
          else {
            _stickySelectIsEnabled = true;
            _specialSetToEnabled = !_selectedTiles.Contains(mouseoverTileLocation);
          }

          /// Control keeps the items in sticky select
        } else if(Input.GetKey(KeyCode.LeftControl) || Input.GetKey(KeyCode.RightControl)) {
          _stickySelectIsEnabled = true;
          _specialSetToEnabled = !_selectedTiles.Contains(mouseoverTileLocation);
        } // enable alt draw mode =
        else if(Input.GetKey(KeyCode.LeftAlt) || Input.GetKey(KeyCode.RightAlt)) {
          _specialSetToEnabled = !_selectedTiles.Contains(mouseoverTileLocation);
          _altBrushModeIsEnabled = true;
          _stickySelectIsEnabled = true;
        }  // toggle the clicked tile otherwise in a new select:
        else {
          _specialSetToEnabled = true;
          _stickySelectIsEnabled = false;
        }

        _currentSelectedTile = mouseoverTileLocation;
        _lastModifiedLocation = _selectSpecialAltSingleClickActive ? _lastModifiedLocation : mouseoverTileLocation;
      } else if(Input.GetMouseButton(0)) {
        if(_lastModifiedLocation.HasValue && mouseoverTileLocation != _lastModifiedLocation.Value) {
          // no alt draw mode, on drag use area selection:
          if(!_altBrushModeIsEnabled) {
            if(_currentAreaSelection is null) {
              _currentAreaSelection = (_lastModifiedLocation.Value, mouseoverTileLocation);
            } else {
              _currentAreaSelection = (_currentAreaSelection.Value.init, mouseoverTileLocation);
            }

            _selectSpecialAltSingleClickActive = false;
            _currentSelectedTile = null;
            WorldEditor.TilesSelectionController.ClearTempSelection();
            _currentAreaSelection.Value.init.Until(
              _currentAreaSelection.Value.end,
              tempSelectedTile => WorldEditor.TilesSelectionController.ToggleSelected(this, tempSelectedTile, true, true)
            );
          } // alt draw mode is enabled, change only the drawn on tile to what we originally chose:
          else {
            WorldEditor.TilesSelectionController.ToggleSelected(this, mouseoverTileLocation, _specialSetToEnabled.Value);
          }

          _currentSelectedTile = null;
          _selectSpecialAltSingleClickActive = false;
          _lastModifiedLocation = mouseoverTileLocation;
        }
      } // on release mouse, clear setings etc:
        else if(Input.GetMouseButtonUp(0)) {
        var previousTilesSelected = _selectedTiles.ToArray();
        _onSelectDone(WorldEditor, mouseoverTileLocation);
        ClearMetaSettings();
        _lastModifiedLocation = mouseoverTileLocation;

        // Undo, Redo:
        var currentlySelectedTiles = _selectedTiles.ToArray();
        WorldEditor.ToolController.AppendHistoryAction(new WorldEditorToolController.HistoricalAction(
          this,
          WorldEditor => {
            WorldEditor.TilesSelectionController.ClearSelection();
            _selectedTiles.Clear();
            previousTilesSelected.ForEach(previousTile => {
              _selectedTiles.Add(previousTile);
              WorldEditor.TilesSelectionController.ToggleSelected(this, previousTile, true);
            });
            _updateTileCount(_selectedTiles.Count);
            Debug.Log($"Undoing selection. Applying {previousTilesSelected.Length} previously selected tiles.");
          },
          WorldEditor => {
            WorldEditor.TilesSelectionController.ClearSelection();
            _selectedTiles.Clear();
            currentlySelectedTiles.ForEach(currentTile => {
              _selectedTiles.Add(currentTile);
              WorldEditor.TilesSelectionController.ToggleSelected(this, currentTile, true);
            });
            _updateTileCount(_selectedTiles.Count);
            Debug.Log($"Redoing selection. Re-applying {currentlySelectedTiles.Length} previously un-done selected tiles.");
          }
        ));
      } // if the mouse isn't being pressed and we missed the mouse button up:

      /// Escape clears the current selection:
      if(Input.GetKeyDown(KeyCode.Escape)) {
        _dismissCurrentSelection(WorldEditor);
      }*/
    }

    void _dismissCurrentSelection() {
      if(!_selectedTiles.Any()) {
        return;
      }

      var previousTilesSelected = _selectedTiles.ToArray();

      _selectedTiles.Clear();
      WorldEditor.TilesSelectionController.ClearSelection();

      // Undo, Redo:
      var currentlySelectedTiles = _selectedTiles.ToArray();
      _updateTileCount(0);
      WorldEditor.ToolController.AppendHistoryAction(new WorldEditorToolController.HistoricalAction(
        this,
        editor => {
          editor.TilesSelectionController.ClearSelection();
          previousTilesSelected.ForEach(previousTile => {
            _selectedTiles.Add(previousTile);
            editor.TilesSelectionController.ToggleSelected(this, previousTile, true);
          });
          _updateTileCount(_selectedTiles.Count);
          Debug.Log($"Undoing selection. Applying {previousTilesSelected.Length} previously selected tiles.");
        },
        editor => {
          editor.TilesSelectionController.ClearSelection();
          currentlySelectedTiles.ForEach(currentTile => {
            _selectedTiles.Add(currentTile);
            editor.TilesSelectionController.ToggleSelected(this, currentTile, true);
          });
          _updateTileCount(_selectedTiles.Count);
          Debug.Log($"Redoing selection. Re-applying {currentlySelectedTiles.Length} previously un-done selected tiles.");
        }
      ));
    }

    void _onSelectDone(Vector2Int mouseoverTileLocation) {
      var previousTilesSelected = _selectedTiles.ToArray();
      if(_selectSpecialAltSingleClickActive) {
        if(!_stickySelectIsEnabled) {
          _selectedTiles.Clear();
          WorldEditor.TilesSelectionController.ClearSelection();
        }

        // uses last clicked location if there's no location stored in this tool:
        (_lastModifiedLocation ?? WorldEditor.WorldController.TileSelector.SelectedTileLocation).Until(mouseoverTileLocation, selectionTile =>
          WorldEditor.TilesSelectionController.ToggleSelected(this, selectionTile, true)
        );

        return;
      } else {

        if(!_stickySelectIsEnabled) {
          _selectedTiles.Clear();
          WorldEditor.TilesSelectionController.ClearSelection();
        }

        if(_currentSelectedTile.HasValue) {
          WorldEditor.TilesSelectionController.ToggleSelected(this, _currentSelectedTile.Value, _specialSetToEnabled ?? true);
        } else if(_currentAreaSelection.HasValue) {
          _currentAreaSelection.Value.init.Until(
            _currentAreaSelection.Value.end,
            tempSelectedTile => WorldEditor.TilesSelectionController.ToggleSelected(this, tempSelectedTile, _stickySelectIsEnabled ? _specialSetToEnabled : true)
          );
        }

        WorldEditor.TilesSelectionController.ClearTempSelection();
      }

      // Undo, Redo:
      var currentlySelectedTiles = _selectedTiles.ToArray();
      WorldEditor.ToolController.AppendHistoryAction(new WorldEditorToolController.HistoricalAction(
        this,
        WorldEditor => {
          WorldEditor.TilesSelectionController.ClearSelection();
          _selectedTiles.Clear();
          previousTilesSelected.ForEach(previousTile => {
            _selectedTiles.Add(previousTile);
            WorldEditor.TilesSelectionController.ToggleSelected(this, previousTile, true);
          });
          _updateTileCount(_selectedTiles.Count);
          Debug.Log($"Undoing selection. Applying {previousTilesSelected.Length} previously selected tiles.");
        },
        WorldEditor => {
          WorldEditor.TilesSelectionController.ClearSelection();
          _selectedTiles.Clear();
          currentlySelectedTiles.ForEach(currentTile => {
            _selectedTiles.Add(currentTile);
            WorldEditor.TilesSelectionController.ToggleSelected(this, currentTile, true);
          });
          _updateTileCount(_selectedTiles.Count);
          Debug.Log($"Redoing selection. Re-applying {currentlySelectedTiles.Length} previously un-done selected tiles.");
        }
      ));

      ClearMetaSettings();
      _lastModifiedLocation = mouseoverTileLocation;
      _updateTileCount(_selectedTiles.Count);
    }

    void _updateTileCount(int count) {
      SettingsWindow?.UpdateFieldValue(SelectedTileCountFieldKey, count.ToString());
    }

    void _setStickyActive() {
      _stickySelectIsEnabled = true;
      if(!_stickyLockActive) {
        SettingsWindow?.UpdateFieldValue(StickyFieldKey, true);
      }
    }

    void _setStickyInactive() {
      if(!_stickyLockActive) {
        _stickySelectIsEnabled = false;
        SettingsWindow?.UpdateFieldValue(StickyFieldKey, false);
      }
    }

    void _enableBrushMode() {
      _altBrushModeIsEnabled = true;
      if(!_brushLockActive) {
        SettingsWindow?.UpdateFieldValue(UseBrushFieldKey, true);
      }
    }

    void _dissableBrushMode() {
      if(!_brushLockActive) {
        _altBrushModeIsEnabled = false;
        SettingsWindow?.UpdateFieldValue(UseBrushFieldKey, false);
      }
    }

    void _setSpecialSetAdditiveType() {
      Vector2Int mouseoverTileLocation = WorldEditor.WorldController.TileSelector.HoveredTileLocation;
      EditModes? overrideMode = _modeLockActive ? SettingsWindow?.Data.GetFieldValue<EditModes>(ModeFieldKey) : null;
      _specialSetToEnabled = overrideMode?.Equals(EditModes.Add) ?? !_selectedTiles.Contains(mouseoverTileLocation);
      if(overrideMode != null) {
        SettingsWindow?.UpdateFieldValue(ModeFieldKey, overrideMode ?? (_specialSetToEnabled.Value ? EditModes.Add : EditModes.Subtract));
        SettingsWindow?.UpdateFieldValue(LockModeFieldKey, true);
      }
    }

    void _clearSpecialSetTo() {
      if(!_modeLockActive) {
        _specialSetToEnabled = null;
        SettingsWindow?.UpdateFieldValue(LockModeFieldKey, false);
      }
    }

#if UNITY_EDITOR
    [MenuItem("Assets/Create/Overworld/UI/Tools/Tiles Selector Editor Tool")]
    static void CreateMyAsset() {
      WorldEditorTool asset = ScriptableObject.CreateInstance<TilesSelectorEditorTool>();

      AssetDatabase.CreateAsset(asset, "Assets/TilesSelectorEditorTool.asset");
      AssetDatabase.SaveAssets();

      EditorUtility.FocusProjectWindow();

      Selection.activeObject = asset;
    }
#endif
  }
}