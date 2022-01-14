using UnityEngine;

public partial class OverworldEntity {
  public partial class Event {

    /// <summary>
    /// An in game action or event of some kind that you can hook onto
    /// </summary>
    public partial struct Hook {
      readonly object _hook;

      /// <summary>
      /// The type of hook
      /// </summary>
      public System.Type SystemType
        => _hook.GetType();

      internal Hook(object hook) {
        _hook = hook;
      }

      public override string ToString()
        => _hook.ToString();

      public override int GetHashCode() {
        Hash128 hash = new();
        hash.Append(_hook.ToString());
        hash.Append(_hook.GetType().ToString());
        return hash.GetHashCode();
      }
    }
  }
}
