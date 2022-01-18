using Overworld.Data;
using System;
using System.Collections.Generic;
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

  public override HashSet<KeyCode> OverridenButtons 
    => new() {
      KeyCode.Mouse0
    };

  public override void WhileEquipedDo(WorldEditorController editor) {
    if(Input.GetMouseButton(0)) {
      // get where we want to edit
      Vector2Int editTileLocation = editor.WorldController.TileSelector.HoveredTileLocation;
      var locationKey = (
        editor.WorldController.TileBoards.CurrentDominantTileBoardForUser._boardKey, 
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

      editor.WorldController.TileBoards.CurrentDominantTileBoardForUser.UpdateTile(
        editTileLocation, 
        activeTileType
      );

      _lastEditedLocation = locationKey;
    }

    if(Input.GetMouseButtonUp(0)) {
      _lastEditedLocation = null;
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
