using Overworld.Objects.Editor;
using System;

namespace Overworld.Controllers.Editor {
  public partial class WorldEditorToolController {

    /// <summary>
    /// An undoable and redoable action by a tool.
    /// </summary>
    public class HistoricalAction {

      public WorldEditorTool DoneByTool {
        get;
      }

      public Action<WorldEditorController> Undo {
        get;
      }

      public Action<WorldEditorController> Redo {
        get;
      }

      public string Description {
        get;
      }

      public HistoricalAction(WorldEditorTool tool, Action<WorldEditorController> undo, Action<WorldEditorController> redo, string description = null) {
        DoneByTool = tool;
        Undo = undo;
        Redo = redo;
        Description = description;
      }
    }
  }
}