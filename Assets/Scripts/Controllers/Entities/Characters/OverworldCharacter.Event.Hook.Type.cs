public partial class OverworldCharacter {
  public new class Event {
    public struct Hook {

      public static Type OnKeyPress {
        get;
      } = new Type("On Key Press");

      public class Type : OverworldEntity.Event.Hook.Type {
        internal Type(string name) : base(name) { }
        public Type(string name, string @namespace) 
          : base(name, @namespace) { }
      }
    }
  }
}
