using Meep.Tech.Data;

namespace Overworld.Data {
  public partial class Entity {

    /// <summary>
    /// A type of entity
    /// </summary>
    public abstract partial class Type : Archetype<Entity, Entity.Type> {

      /// <summary>
      /// The name of this type of entity
      /// </summary>
      public string Name {
        get;
      }

      protected Type(Identity id, string name)
        : base(id) {
        Name = name;
      }
    }
  }
}