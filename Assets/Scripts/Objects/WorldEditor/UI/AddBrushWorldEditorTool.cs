using System;
using UnityEditor;
using UnityEngine;

public class AddBrushWorldEditorTool : WorldEditorTool {

  public override Func<WorldEditorController, Sprite> GetBackgroundPreview {
    get;
  } = worldEditor =>
    worldEditor.WorldEditorEditorMainMenu.TilesMenu.EnabledTileSubMenu?.SelectedTileTypeOption?.DefaultBackground?.sprite;

#if UNITY_EDITOR
  [MenuItem("Assets/Create/Overworld/UI/Brush Editor Tool")]
  static void CreateMyAsset() {
    WorldEditorTool asset = ScriptableObject.CreateInstance<AddBrushWorldEditorTool>();

    AssetDatabase.CreateAsset(asset, "Assets/AddBrushWorldEditorTool.asset");
    AssetDatabase.SaveAssets();

    EditorUtility.FocusProjectWindow();

    Selection.activeObject = asset;
  }
#endif
}
