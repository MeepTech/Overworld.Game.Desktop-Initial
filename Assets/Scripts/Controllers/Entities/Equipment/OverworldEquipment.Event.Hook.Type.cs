public partial class OverworldEquipment : OverworldEntity {
  public new class Event {
    public struct Hook {

      public static Type OnClickOverride {
        get;
      } = new Type("On Click Override");

      public static Type OnOppositeClickOverride {
        get;
      } = new Type("On Opposite Click Override");

      /// <summary>
      /// Cannot override the DEL key
      /// </summary>
      public static Type OnKeyPressOverride {
        get;
      } = new Type("On Key Press Override");

      public static Type OnEquip {
        get;
      } = new Type("On Equip");

      public static Type OnUnequip {
        get;
      } = new Type("On Un-Equip");

      public static Type WhileAttached {
        get;
      } = new Type("While Attached");

      public class Type : OverworldEntity.Event.Hook.Type {
        internal Type(string name) : base(name) { }
        public Type(string name, string @namespace)
          : base(name, @namespace) { }
      }
    }
  }
}
