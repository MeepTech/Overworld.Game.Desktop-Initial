using Overworld.Controllers.Editor;
using UnityEngine;

namespace Overworld.Objects.Editor {
  public abstract class WorldEditorToggle : ScriptableObject {

    public Sprite Icon => _icon;
    [SerializeField] protected Sprite _icon;

    public abstract string Name {
      get;
    }

    public abstract string Description {
      get;
    }

    /// <summary>
    /// The logic to override existing click and mouse logic while equipped.
    /// </summary>
    public abstract void Enable(WorldEditorController editor);

    /// <summary>
    /// executes on this tool being diabled
    /// </summary>
    public abstract void Disable(WorldEditorController editor);
  }
}