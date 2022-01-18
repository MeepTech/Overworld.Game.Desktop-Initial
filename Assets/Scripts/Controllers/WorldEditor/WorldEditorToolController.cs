using UnityEngine;

public class WorldEditorToolController : MonoBehaviour {

  /// <summary>
  /// The parent world editor controller
  /// </summary>
  public WorldEditorController WorldEditor {
    get => _worldEditor;
  } [SerializeField] WorldEditorController _worldEditor;

  /// <summary>
  /// The tool currently enabled in the editor.
  /// Null if none are enabled.
  /// </summary>
  public WorldEditorTool CurrentlyEnabledTool {
    get => _currentlyEnabledTool;
    private set {
      _currentlyEnabledTool = value;
#if UNITY_EDITOR
      _currentTool = CurrentlyEnabledTool?.Name;
#endif
    }
  } [SerializeField, ReadOnly]
  WorldEditorTool _currentlyEnabledTool;

#if UNITY_EDITOR
  [SerializeField, ReadOnly]
  string _currentTool;
#endif

  void Update() {
    if(!_worldEditor.WorldController.MouseIsOverUI) {
      CurrentlyEnabledTool?.WhileEquipedDo(_worldEditor);
    }
  }

  /// <summary>
  /// Called to enable a tool in the editor
  /// </summary>
  /// <param name="tool"></param>
  public void EnableTool(WorldEditorTool tool) {
    //CurrentlyEnabledTool?.OnDisable(_worldEditor);
    CurrentlyEnabledTool?.FromMenu?
      .OnToolDissabled(CurrentlyEnabledTool);
    CurrentlyEnabledTool = tool;
    //CurrentlyEnabledTool.OnEnable(_worldEditor);
  }

  /// <summary>
  /// called to disable a tool in the editor.
  /// </summary>
  public void DissableEnabledTool(WorldEditorTool tool) {
    if(tool == CurrentlyEnabledTool) {
      //CurrentlyEnabledTool.OnDisable(_worldEditor);
      CurrentlyEnabledTool = null;
    }
  }
}