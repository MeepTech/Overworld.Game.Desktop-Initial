using System;
using UnityEditor;
using UnityEngine;

public class WorldEditorTool : ScriptableObject {

  /// <summary>
  /// Name of the tool
  /// </summary>
  public virtual string Name
    => _name; [SerializeField] string _name;

  /// <summary>
  /// the tool description.
  /// Used as the tooltip
  /// </summary>
  public virtual string Description 
    => _description; [SerializeField] string _description;

  /// <summary>
  /// the button icon
  /// </summary>
  public virtual Sprite Icon
    => _icon; [SerializeField] Sprite _icon;

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
  /// The menu this came from
  /// </summary>
  public IWorldEditorToolContainerMenu FromMenu;

#if UNITY_EDITOR
  [MenuItem("Assets/Create/Overworld/UI/World Editor Tool")]
  static void CreateMyAsset() {
    WorldEditorTool asset = ScriptableObject.CreateInstance<WorldEditorTool>();

    AssetDatabase.CreateAsset(asset, "Assets/NewScripableObject.asset");
    AssetDatabase.SaveAssets();

    EditorUtility.FocusProjectWindow();

    Selection.activeObject = asset;
  }
#endif
}