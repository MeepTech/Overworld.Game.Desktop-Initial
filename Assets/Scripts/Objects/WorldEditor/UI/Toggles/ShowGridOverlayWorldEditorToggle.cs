using UnityEditor;

public class ShowGridOverlayWorldEditorToggle : WorldEditorToggle {

  public override string Name 
    => "Show Grid Overlay";

  public override string Description
    => "Toggle the grid overlay on or off.";

  public override void Disable(WorldEditorController editor) {
    editor.WorldEditorEditorMainMenu.TilesMenu.OverlayGrid.gameObject.SetActive(false);
  }

  public override void Enable(WorldEditorController editor) {
    editor.WorldEditorEditorMainMenu.TilesMenu.OverlayGrid.gameObject.SetActive(true);
  }

#if UNITY_EDITOR
  [MenuItem("Assets/Create/Overworld/UI/Toggles/Show Grid Overlay")]
  static void CreateMyAsset() {
    ShowGridOverlayWorldEditorToggle asset 
      = UnityEngine.ScriptableObject.CreateInstance<ShowGridOverlayWorldEditorToggle>();

    AssetDatabase.CreateAsset(asset, System.IO.Path.Combine(UnityEditorUtilities.GetSelectedPathOrFallback(), "ShowGridOverlayWorldEditorToggle.asset"));
    AssetDatabase.SaveAssets();

    EditorUtility.FocusProjectWindow();

    Selection.activeObject = asset;
  }
#endif
}