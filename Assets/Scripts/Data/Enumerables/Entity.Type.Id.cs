namespace Overworld.Data {
  public partial class Entity {
    public abstract partial class Type {

      /// <summary>
      /// Ids for entity types
      /// </summary>
      public new class Identity : Meep.Tech.Data.Archetype<Entity, Type>.Identity {
        public Identity(string name, string keyPrefixEndingAdditions = null)
          : base(name, keyPrefixEndingAdditions) { }
      }
    }
  }
}