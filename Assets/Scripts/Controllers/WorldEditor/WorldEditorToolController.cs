using Overworld.Controllers.World;
using Overworld.Objects.Editor;
using System.Collections.Generic;
using UnityEngine;

namespace Overworld.Controllers.Editor {

  /// <summary>
  /// Controls the tools that can edit the world.
  /// </summary>
  public partial class WorldEditorToolController : MonoBehaviour {

    /// <summary>
    /// The parent world editor controller
    /// </summary>
    public WorldEditorController WorldEditor
      => Demiurge.Self.WorldController.WorldEditor;

    /// <summary>
    /// Tile selection tool data
    /// </summary>
    public TilesSelectorEditorTool SelectionData
      => _selectionToolData;
    [SerializeField]
    TilesSelectorEditorTool _selectionToolData;

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
    }
    [SerializeField, ReadOnly]
    WorldEditorTool _currentlyEnabledTool;
    [SerializeField, ReadOnly]
    WorldEditorTool _tempHiddenTool;
    [SerializeField, ReadOnly]
    WorldEditorTool _previouslyEnabledTool;

#if UNITY_EDITOR
    [SerializeField, ReadOnly]
    string _currentTool;
#endif

    Stack<HistoricalAction> _history
    = new();

    Stack<HistoricalAction> _rolledBackHistory
    = new();

    void Start() {
      WorldEditor.Controls.onActionTriggered += callbackContext => {
        if(callbackContext.action.actionMap.name != "UI") {
          Debug.Log(callbackContext.action);
        }

        // Switch to the select tool on hot key click
        switch(callbackContext.action.name) {
          case "Temp-Enable Select Tool Hot-Key": 
            if(CurrentlyEnabledTool is not null && CurrentlyEnabledTool.Name != SelectionData.Name && callbackContext.action.phase == UnityEngine.InputSystem.InputActionPhase.Started) {
              _tempHiddenTool = CurrentlyEnabledTool;
              CurrentlyEnabledTool = SelectionData;
            } else if (_tempHiddenTool is not null && callbackContext.action.phase != UnityEngine.InputSystem.InputActionPhase.Performed) {
              CurrentlyEnabledTool = _tempHiddenTool;
              _tempHiddenTool = null;
            }
          break;
          case "Undo":
            if(callbackContext.phase == UnityEngine.InputSystem.InputActionPhase.Performed) {
              UndoAction();
            }
            break;
          case "Redo":
            if(callbackContext.phase == UnityEngine.InputSystem.InputActionPhase.Performed) {
              RedoAction();
            }
            break;
          case "Unequip Current Tool":
            if(callbackContext.phase == UnityEngine.InputSystem.InputActionPhase.Performed) {
              DissableEnabledTool(CurrentlyEnabledTool);
            }
            break;
        }
      };
    }

    void Update() {
      if(!WorldEditor.WorldController.MouseIsOverUI) {
        CurrentlyEnabledTool?._onUpdate();
        CurrentlyEnabledTool?.OnUpdateWhileEquiped();
      }
      /*if(Input.GetKeyDown(KeyCode.Y)
#if !UNITY_EDITOR
      && (Input.GetKeyDown(KeyCode.LeftControl) || Input.GetKeyDown(KeyCode.RightControl))
#endif
    ) {
        RedoAction();
      }
      if(Input.GetKeyDown(KeyCode.Z)
#if !UNITY_EDITOR
      && (Input.GetKeyDown(KeyCode.LeftControl) || Input.GetKeyDown(KeyCode.RightControl))
#endif
    ) {
        UndoAction();
      }*/

      /// While space is held down with any other tool being active, we switch to the selection tool:
      /*if(CurrentlyEnabledTool is not null && Input.GetKeyDown(KeyCode.Space)) {
        _tempHiddenTool = CurrentlyEnabledTool;
        _currentlyEnabledTool = SelectionData;
      } else if(CurrentlyEnabledTool is not null && Input.GetKeyUp(KeyCode.Space)) {
        _currentlyEnabledTool = _tempHiddenTool;
        _tempHiddenTool = null;
      } else if(_tempHiddenTool is not null && !Input.GetKey(KeyCode.Space)) {
        _currentlyEnabledTool = _tempHiddenTool;
        _tempHiddenTool = null;
      }*/
    }

    /// <summary>
    /// Called to enable a tool in the editor
    /// </summary>
    public void EnableTool(WorldEditorTool tool) {
      CurrentlyEnabledTool?.OnDequip(tool);
      CurrentlyEnabledTool?.FromMenu?
        .OnToolDissabled(CurrentlyEnabledTool);
      _previouslyEnabledTool = CurrentlyEnabledTool;
      CurrentlyEnabledTool = tool;
      CurrentlyEnabledTool.OnEquip(_previouslyEnabledTool);
    }

    /// <summary>
    /// called to disable a tool in the editor.
    /// </summary>
    public void DissableEnabledTool(WorldEditorTool tool) {
      if(tool == CurrentlyEnabledTool) {
        CurrentlyEnabledTool?.OnDequip(null);
        _previouslyEnabledTool = CurrentlyEnabledTool;
        CurrentlyEnabledTool = null;
      }
    }

    /// <summary>
    /// Add an action to the history
    /// </summary>
    public void AppendHistoryAction(HistoricalAction action) {
      _rolledBackHistory.Clear();
      Debug.Log("HISTORY ACTION RECORDED FOR:" + action.DoneByTool);
      _history.Push(action);
    }

    /// <summary>
    /// Undo the last action on the stack
    /// </summary>
    public void UndoAction() {
      if(_history.TryPop(out HistoricalAction action)) {
        _rolledBackHistory.Push(action);
        Debug.Log("UNDO ACTION FOR:" + action.DoneByTool);
        action.Undo(WorldEditor);
      }
    }

    /// <summary>
    /// Redo the last action on the stack.
    /// </summary>
    public void RedoAction() {
      if(_rolledBackHistory.TryPop(out HistoricalAction action)) {
        _history.Push(action);
        Debug.Log("REDO ACTION FOR:" + action.DoneByTool);
        action.Redo(WorldEditor);
      }
    }
  }
}