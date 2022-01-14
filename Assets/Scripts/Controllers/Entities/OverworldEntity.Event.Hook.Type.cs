using Meep.Tech.Data;

public partial class OverworldEntity {
  public partial class Event {

    public partial struct Hook {

      public static Type OnInteract {
        get;
      } = new Type("On Interaction");

      public static Type OnCollide {
        get;
      } = new Type("On Collision");

      public static Type WhileTouching {
        get;
      } = new Type("While Touching");

      public static Type OnApproach {
        get;
      } = new Type("On Approach");

      public static Type OnDepatrute {
        get;
      } = new Type("On Departure");

      public static Type WhileNearby {
        get;
      } = new Type("While Nearby");

      /// <summary>
      /// A type of event specific to overworld entities.
      /// </summary>
      public class Type : Enumeration<Type> {
        public Type(string name, string @namespace)
          : base($"{@namespace ?? ""}.{name}") { }
        internal Type(string name)
          : base($"BuiltInEventHookTypes.{name}") { }
      }
    }
  }
}
