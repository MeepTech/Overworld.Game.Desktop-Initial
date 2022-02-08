using Meep.Tech.Collections.Generic;
using Overworld.Controllers.Editor;
using Overworld.Data;
using Simple.Ux.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEngine;

namespace Overworld.Objects.Editor {
  public class AddBrushWorldEditorTool : WorldEditorTool, IHasAnOpenableSettingsWindow {

    /// <summary>
    /// The shapes for the brush tip
    /// </summary>
    public enum Shapes {
      Round,
      Square
    }

    (string, Vector2Int)? _lastEditedLocation
      = null;

    public override Func<WorldEditorController, Sprite> GetBackgroundPreview {
      get;
    } = worldEditor =>
      _getSelectedTileType(worldEditor)?
        .DefaultBackground?.sprite;

    [SerializeField, ReadOnly]
    bool _clicked
    = false;

    Dictionary<Vector2Int, (Tile? old, Tile? @new)> _editedTiles
      = new ();

    [SerializeField, ReadOnly]
    string _activeBoard
      = null;
    View _settingsWindow;
    [SerializeField, ReadOnly]
    Shapes _currentShape 
      = Shapes.Round;
    [SerializeField, ReadOnly]
    int _brushSize 
      = 1;

    protected internal override void UseTool(ActionStatus actionStatus) {
      switch(actionStatus) {
        case ActionStatus.Down:
          /// if there's a selection check if it's in the selection first.
          if(WorldEditor.ToolController.SelectionData?.SelectionIsActive ?? false) {
            Vector2Int clickedLocation = WorldEditor.WorldController.TileSelector.HoveredTileLocation;
            if(!WorldEditor.ToolController.SelectionData.IsSelected(clickedLocation)) {
              return;
            }
          }
          _clicked = true;
          _activeBoard = WorldEditor.WorldController.TileBoards.CurrentDominantTileBoardForUser.BoardKey;
          break;
        case ActionStatus.Held:// get where we want to edit
          Vector2Int editTileLocation = WorldEditor.WorldController.TileSelector.HoveredTileLocation;

          /// if there's a selection check if it's in the selection first.
          if(WorldEditor.ToolController.SelectionData?.SelectionIsActive ?? false) {
            if(!WorldEditor.ToolController.SelectionData.IsSelected(editTileLocation)) {
              return;
            }
          }
          _clicked = true;

          var locationKey = (
            _activeBoard,
            editTileLocation
          );

          Tile.Type activeTileType
            = _getSelectedTileType(WorldEditor);

          if(activeTileType is null) {
            return;
          }

          // if the brush hasn't moved, don't edit the same tile twice:
          if(_lastEditedLocation != null) {
            if(_lastEditedLocation == locationKey) {
              return;
            }
          }

          var beforeTile = WorldEditor.WorldController.TileSelector.HoveredTile;
          WorldEditor.WorldController.TileBoards[_activeBoard].UpdateTile(
            editTileLocation,
            activeTileType
          );
          var afterTile = WorldEditor.WorldController.TileSelector.HoveredTile;
          if(!(beforeTile is null && afterTile is null) && !_editedTiles.ContainsKey(editTileLocation)) {
            _editedTiles[editTileLocation] = (beforeTile, afterTile);
          }

          _lastEditedLocation = locationKey;
          break;
        case ActionStatus.Up:
          if(_clicked) {
            _lastEditedLocation = null;
            _clicked = false;

            // Undo, Redo:
            var editedTiles = _editedTiles.ToArray();
            var activeBoard = _activeBoard;
            if(_editedTiles.Any()) {
              WorldEditor.ToolController.AppendHistoryAction(new WorldEditorToolController.HistoricalAction(
                this,
                WorldEditor => editedTiles.ForEach(editedTileData
                  => WorldEditor.WorldController.TileBoards[activeBoard].SetTile(editedTileData.Key, editedTileData.Value.old)),
                WorldEditor => editedTiles.ForEach(editedTileData
                  => WorldEditor.WorldController.TileBoards[activeBoard].SetTile(editedTileData.Key, editedTileData.Value.@new))
              ));

              _activeBoard = null;
              _editedTiles.Clear();
            }
          }
          break;
      }
    }

    protected override ViewBuilder BuildSettingsView(ViewBuilder builder)
      => builder
        .AddField(new RangeSliderField(
            name: "Size",
            min: 1,
            max: 30,
            clampedToWholeNumbers: true
          ) { OnValueChangedListeners = (Action<DataField, double>)((updatedField, _)
             => this._brushSize = (int)updatedField.Value)
        })
        .AddField(new DropdownSelectField<Shapes>(
            name: "Shape"
          ) { OnValueChangedListeners = (Action<DataField, List<KeyValuePair<string, object>>>)((updatedField, _)
            => {
              this._currentShape
                = (Shapes)((updatedField as DropdownSelectField)?.Value?.FirstOrDefault().Value ?? 0);
            })
          });

    static Tile.Type _getSelectedTileType(WorldEditorController editor)
      => editor.WorldEditorEditorMainMenu.TilesMenu.EnabledTileSubMenu?.SelectedTileTypeOption;

#if UNITY_EDITOR
    [MenuItem("Assets/Create/Overworld/UI/Tools/Brush Editor Tool")]
    static void CreateMyAsset() {
      WorldEditorTool asset = ScriptableObject.CreateInstance<AddBrushWorldEditorTool>();

      AssetDatabase.CreateAsset(asset, "Assets/AddBrushWorldEditorTool.asset");
      AssetDatabase.SaveAssets();

      EditorUtility.FocusProjectWindow();

      Selection.activeObject = asset;
    }
#endif
  }
}