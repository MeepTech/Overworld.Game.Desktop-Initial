using Overworld.Data;
using System.Collections.Generic;

public partial class OverworldEntity {
  public partial class Event {
    /// <summary>
    /// Context for an event.
    /// </summary>
    public struct Context {

      /// <summary>
      /// The entity that has the Event.Hook attached.
      /// </summary>
      public Entity AttachedTo {
        get;
      }

      /// <summary>
      /// 
      /// </summary>
      public Character Executor {
        get;
      }

      /// <summary>
      /// The hook that was called
      /// </summary>
      public Hook? Hook {
        get;
      }

      public IReadOnlyDictionary<string, object> OtherParam
        => _otherParams;

      readonly Dictionary<string, object> _otherParams;

      public Context(
        Entity attachedTo,
        Character executor,
        Hook? hook,
        Dictionary<string, object> otherParams = null
      ) {
        AttachedTo = attachedTo;
        Executor = executor;
        Hook = hook;
        _otherParams = otherParams;
      }
    }
  }
}
