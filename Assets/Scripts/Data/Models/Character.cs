using Meep.Tech.Data;
using System;
using System.Collections.Generic;
using static Meep.Tech.Data.Configuration.Loader.Settings;

namespace Overworld.Data {
  /// <summary>
  /// A player controlled entity
  /// </summary>
  public class Character : Entity {

    [Branch]
    public new class Type : Entity.Type {

      protected override Dictionary<string, object> DefaultTestParams
        => new Dictionary<string, object> {
          {nameof(Character.UniqueName), "Test" }
        };

      /// <summary>
      /// The Character Type Id
      /// </summary>
      public static new Identity Id {
        get;
      } = new Identity("Character");

      /// <summary>
      /// For X Bam
      /// </summary>
      /// <param name="id"></param>
      Type(Identity id) 
        : base(id ?? Id, "Character") {}

      /// <summary>
      /// for inheritance
      /// </summary>
      protected Type(
        Identity id,
        string name
      ) : base(id, name) {}
    }

    /// <summary>
    /// The unique, human readable name of a character. Like their username
    /// </summary>
    public string UniqueName {
      get;
      internal set;
    }

    /// <summary>
    /// X Bam Builder
    /// </summary>
    /// <param name="builder"></param>
    Character(IBuilder<Entity> builder)
     : this(
        builder?.GetParam(nameof(Name), (builder?.Archetype as Type).Name)
          ?? (builder?.Archetype as Type)?.Name,
       builder?.GetAndValidateParamAs<string>(nameof(UniqueName))
         ?? new Guid().ToString()
    ) {}
    
    /// <summary>
    /// Make a new character
    /// </summary>
    public Character(string name, string uniqueName) : base(name, null) {
      Name = name;
      UniqueName = uniqueName;
      Id = new Guid().ToString();
    }
  }
}