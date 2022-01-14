using Overworld.Script;

public partial class OverworldEntity {

  /// <summary>
  /// An event that can take place
  /// </summary>
  public partial class Event {

    readonly Ows.Program _program;

    internal Event(Ows.Program program) {
      _program = program;
    }

    protected Event() {}

    public virtual void ExecuteFor(Context context) {
      _program?.ExecuteFrom(context.AttachedTo, context.Executor, new System.Collections.Generic.Dictionary<string, Ows.IParameter> {
        {"CALLING-EVENT-HOOK-EXISTS", new Ows.Boolean(_program, context.Hook.HasValue) },
        {"CALLING-EVENT-HOOK-NAME", new Ows.String(_program, context.Hook?.ToString()) },
        {"CALLING-EVENT-HOOK-TYPE-NAME", new Ows.String(_program, context.Hook?.SystemType.ToString()) }
      });
    }
  }
}
