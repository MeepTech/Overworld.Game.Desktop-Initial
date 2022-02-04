using Meep.Tech.Collections;
using Meep.Tech.Collections.Generic;
using Overworld.Controllers.Editor;
using Overworld.Utilities;
using Simple.Ux.Data;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEngine;

namespace Overworld.Objects.Editor {
  public class TilesSelectorEditorTool : WorldEditorTool, IHasAnOpenableSettingsWindow {

    public override HashSet<KeyCode> HotKeys
      => new() {
        KeyCode.Space,
        KeyCode.Mouse0,
        KeyCode.LeftShift,
        KeyCode.RightShift,
        KeyCode.LeftControl,
        KeyCode.RightControl,
        KeyCode.LeftAlt,
        KeyCode.RightAlt
      };

    (Vector2Int init, Vector2Int end)? _currentAreaSelection;

    Vector2Int? _lastModifiedLocation
    = null;

    internal HashSet<Vector2Int>
      _selectedTiles
        = new();

    Vector2Int?
    _currentSelectedTile
      = null;

    bool _altDrawModeEnabled
    = false;

    bool? _specialSetToEnabled
    = false;

    bool _stickySelect
    = false;

    bool _selectSpecialAltSingleClickActive
    = false;

    public override string Description
      => $"Select tiles with the mouse. Drag to select a square area of tiles."
        + $"\n\t- Holding shift while using most tools will override to this tool"
        + $"\n\t- CTRL+Click to add or subtract from the current selection (uses the first clicked and toggled tile to determine whether to add or remove). Shift+Click also does this when not in override mode."
        + $"\n\t- ALT+Click to draw by dragging, add or subtract is determined same as above"
        + $"\n\t- Shift+ALT+Click to draw in a square from the last placed tile (can be used to draw lines too). If CTRL is also held it add or subtract."
        + $"\n\t- ESC to clear current selection. Right Click also does if you don't drag the mouse too much.";


    /// <summary>
    /// Check if the selection is active
    /// </summary>
    public bool SelectionIsActive
      => _selectedTiles.Any();

    Vector2Int? _rightClickLocation;
    View _settingsWindow;

    /// <summary>
    /// Check if a tile is selected by the tool
    /// </summary>
    public bool IsSelected(Vector2Int vector2Int)
      => _selectedTiles.Contains(vector2Int);

    public override void WhileEquipedDo(WorldEditorController editor) {
      Vector2Int mouseoverTileLocation = editor.WorldController.TileSelector.HoveredTileLocation;

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
          _dismissCurrentSelection(editor);
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
              _stickySelect = true;
              _specialSetToEnabled = !_selectedTiles.Contains(mouseoverTileLocation);
            }
          } // if not clicking alt and not in override mode
          else {
            _stickySelect = true;
            _specialSetToEnabled = !_selectedTiles.Contains(mouseoverTileLocation);
          }

          /// Control keeps the items in sticky select
        } else if(Input.GetKey(KeyCode.LeftControl) || Input.GetKey(KeyCode.RightControl)) {
          _stickySelect = true;
          _specialSetToEnabled = !_selectedTiles.Contains(mouseoverTileLocation);
        } // enable alt draw mode =
        else if(Input.GetKey(KeyCode.LeftAlt) || Input.GetKey(KeyCode.RightAlt)) {
          _specialSetToEnabled = !_selectedTiles.Contains(mouseoverTileLocation);
          _altDrawModeEnabled = true;
          _stickySelect = true;
        }  // toggle the clicked tile otherwise in a new select:
        else {
          _specialSetToEnabled = true;
          _stickySelect = false;
        }

        _currentSelectedTile = mouseoverTileLocation;
        _lastModifiedLocation = _selectSpecialAltSingleClickActive ? _lastModifiedLocation : mouseoverTileLocation;
      } else if(Input.GetMouseButton(0)) {
        if(_lastModifiedLocation.HasValue && mouseoverTileLocation != _lastModifiedLocation.Value) {
          // no alt draw mode, on drag use area selection:
          if(!_altDrawModeEnabled) {
            if(_currentAreaSelection is null) {
              _currentAreaSelection = (_lastModifiedLocation.Value, mouseoverTileLocation);
            } else {
              _currentAreaSelection = (_currentAreaSelection.Value.init, mouseoverTileLocation);
            }

            _selectSpecialAltSingleClickActive = false;
            _currentSelectedTile = null;
            editor.TilesSelectionController.ClearTempSelection();
            _currentAreaSelection.Value.init.Until(
              _currentAreaSelection.Value.end,
              tempSelectedTile => editor.TilesSelectionController.ToggleSelected(this, tempSelectedTile, true, true)
            );
          } // alt draw mode is enabled, change only the drawn on tile to what we originally chose:
          else {
            editor.TilesSelectionController.ToggleSelected(this, mouseoverTileLocation, _specialSetToEnabled.Value);
          }

          _currentSelectedTile = null;
          _selectSpecialAltSingleClickActive = false;
          _lastModifiedLocation = mouseoverTileLocation;
        }
      } // on release mouse, clear setings etc:
        else if(Input.GetMouseButtonUp(0)) {
        var previousTilesSelected = _selectedTiles.ToArray();
        _onSelectDone(editor, mouseoverTileLocation);
        ClearMetaSettings();
        _lastModifiedLocation = mouseoverTileLocation;

        // Undo, Redo:
        var currentlySelectedTiles = _selectedTiles.ToArray();
        editor.ToolController.AppendHistoryAction(new WorldEditorToolController.HistoricalAction(
          this,
          editor => {
            editor.TilesSelectionController.ClearSelection();
            _selectedTiles.Clear();
            previousTilesSelected.ForEach(previousTile => {
              _selectedTiles.Add(previousTile);
              editor.TilesSelectionController.ToggleSelected(this, previousTile, true);
            });
            _updateTileCount(_selectedTiles.Count);
            Debug.Log($"Undoing selection. Applying {previousTilesSelected.Length} previously selected tiles.");
          },
          editor => {
            editor.TilesSelectionController.ClearSelection();
            _selectedTiles.Clear();
            currentlySelectedTiles.ForEach(currentTile => {
              _selectedTiles.Add(currentTile);
              editor.TilesSelectionController.ToggleSelected(this, currentTile, true);
            });
      _updateTileCount(_selectedTiles.Count);
            Debug.Log($"Redoing selection. Re-applying {currentlySelectedTiles.Length} previously un-done selected tiles.");
          }
        ));
      } // if the mouse isn't being pressed and we missed the mouse button up:

      /// Escape clears the current selection:
      if(Input.GetKeyDown(KeyCode.Escape)) {
        _dismissCurrentSelection(editor);
      }
    }

    void _dismissCurrentSelection(WorldEditorController editor) {
      if(!_selectedTiles.Any()) {
        return;
      }

      var previousTilesSelected = _selectedTiles.ToArray();

      _selectedTiles.Clear();
      editor.TilesSelectionController.ClearSelection();

      // Undo, Redo:
      var currentlySelectedTiles = _selectedTiles.ToArray();
      _updateTileCount(0);
      editor.ToolController.AppendHistoryAction(new WorldEditorToolController.HistoricalAction(
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

    /// <summary>
    /// Clear the meta settings for the temp select
    /// </summary>
    public void ClearMetaSettings() {
      _specialSetToEnabled = null;
      _currentSelectedTile = null;
      _selectSpecialAltSingleClickActive = false;
      _currentAreaSelection = null;
      _stickySelect = false;
      _altDrawModeEnabled = false;
      _lastModifiedLocation = null;
    }

    void OnDestroy() {
      ClearMetaSettings();
      _selectedTiles.Clear();
    }

    void _onSelectDone(WorldEditorController editor, Vector2Int mouseoverTileLocation) {
      if(_selectSpecialAltSingleClickActive) {
        if(!_stickySelect) {
          _selectedTiles.Clear();
          editor.TilesSelectionController.ClearSelection();
        }

        // uses last clicked location if there's no location stored in this tool:
        (_lastModifiedLocation ?? editor.WorldController.TileSelector.SelectedTileLocation).Until(mouseoverTileLocation, selectionTile =>
          editor.TilesSelectionController.ToggleSelected(this, selectionTile, true)
        );

        return;
      }

      if(!_stickySelect) {
        _selectedTiles.Clear();
        editor.TilesSelectionController.ClearSelection();
      }

      if(_currentSelectedTile.HasValue) {
        editor.TilesSelectionController.ToggleSelected(this, _currentSelectedTile.Value, _specialSetToEnabled ?? true);
      } else if(_currentAreaSelection.HasValue) {
        _currentAreaSelection.Value.init.Until(
          _currentAreaSelection.Value.end,
          tempSelectedTile => editor.TilesSelectionController.ToggleSelected(this, tempSelectedTile, _stickySelect ? _specialSetToEnabled : true)
        );
      }

      editor.TilesSelectionController.ClearTempSelection();
      _updateTileCount(_selectedTiles.Count);
    }

    private void _updateTileCount(int count) {
      _settingsWindow?.GetField("Selected Tile Count").TryToSetValue(count.ToString(), out _);
    }

    /// <summary>
    /// Edit modes for the tile selector tool
    /// </summary>
    public enum EditModes {
      Add,
      Subtract
    }

    public View GetSettingsWindow()
      => _settingsWindow ??= new ViewBuilder("Tile Selector")
        .AddField(new ReadOnlyTextField(_selectedTiles.Count.ToString(), "Selected Tile Count"))
        .AddField(new ToggleField("Sticky", "If a new select shouldn't be created every time you click, and values should be added to, or removed from the current selection."))
        .AddField(new ToggleField("Lock Mode", "Locks the select mode to one of the below modes."))
        .AddField(new DropdownSelectField<EditModes>("Mode") {
          EnabledIfCheckers = new() {
            {
              "ifLocked",
              (field, view) => view.GetFieldValue<bool>("Lock Mode")
            }
          }
        })
      .Build();

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