using Meep.Tech.Data;
using Overworld.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEngine;

public class AddBrushWorldEditorTool : WorldEditorTool {

  (string, Vector2Int)? _lastEditedLocation 
    = null;

  public override Func<WorldEditorController, Sprite> GetBackgroundPreview {
    get;
  } = worldEditor =>
    _getSelectedTileType(worldEditor)?
      .DefaultBackground?.sprite;

  public override HashSet<KeyCode> HotKeys 
    => new() {
      KeyCode.Mouse0
    };

  bool _clicked
    = false;

  Dictionary<Vector2Int, (Tile? old, Tile? @new)> _editedTiles
    = new ();

  string _activeBoard
    = null;

  public override void WhileEquipedDo(WorldEditorController editor) {
    if(Input.GetMouseButtonDown(0)) {
      /// if there's a selection check if it's in the selection first.
      if(editor.ToolController.SelectionData?.SelectionIsActive ?? false) {
        Vector2Int clickedLocation = editor.WorldController.TileSelector.HoveredTileLocation;
        if(!editor.ToolController.SelectionData.IsSelected(clickedLocation)) {
          return;
        }
      }
      _clicked = true;
      _activeBoard = editor.WorldController.TileBoards.CurrentDominantTileBoardForUser.BoardKey;
    }

    if(Input.GetMouseButton(0) && _clicked) {
      // get where we want to edit
      Vector2Int editTileLocation = editor.WorldController.TileSelector.HoveredTileLocation;

      /// if there's a selection check if it's in the selection first.
      if(editor.ToolController.SelectionData?.SelectionIsActive ?? false) {
        if(!editor.ToolController.SelectionData.IsSelected(editTileLocation)) {
          return;
        }
      }

      var locationKey = (
        _activeBoard, 
        editTileLocation
      );

      Tile.Type activeTileType
        = _getSelectedTileType(editor);
      if(activeTileType is null) {
        return;
      }

      // if the brush hasn't moved, don't edit the same tile twice:
      if(_lastEditedLocation != null) {
        if(_lastEditedLocation == locationKey) {
          return;
        }
      }

      var beforeTile = editor.WorldController.TileSelector.HoveredTile;
      editor.WorldController.TileBoards[_activeBoard].UpdateTile(
        editTileLocation, 
        activeTileType
      );
      var afterTile = editor.WorldController.TileSelector.HoveredTile;
      if(!(beforeTile is null && afterTile is null) && !_editedTiles.ContainsKey(editTileLocation)) {
        _editedTiles[editTileLocation] = (beforeTile, afterTile);
        //Debug.Log($"Add tile {editTileLocation.x},{editTileLocation.y}");
      }

      _lastEditedLocation = locationKey;
    }

    if(Input.GetMouseButtonUp(0) && _clicked) {
      _lastEditedLocation = null;
      _clicked = false;

      // Undo, Redo:
      var editedTiles = _editedTiles.ToArray();
      var activeBoard = _activeBoard;
      if(_editedTiles.Any()) {
        editor.ToolController.AppendHistoryAction(new WorldEditorToolController.HistoricalAction(
          this,
          editor => editedTiles.ForEach(editedTileData
            => editor.WorldController.TileBoards[activeBoard].SetTile(editedTileData.Key, editedTileData.Value.old)),
          editor => editedTiles.ForEach(editedTileData
            => editor.WorldController.TileBoards[activeBoard].SetTile(editedTileData.Key, editedTileData.Value.@new))
        ));

        _activeBoard = null;
        _editedTiles.Clear();
      }
    }
  }

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
