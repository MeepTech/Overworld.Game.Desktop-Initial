using Meep.Tech.Data;
using System.Collections.Generic;

namespace Overworld.Data {
  public partial class Entity : Model<Entity, Entity.Type>, IUnique {

    static Dictionary<string, int> _entityUsedNames
      = new Dictionary<string, int>();

    /// <summary>
    /// Unique entity id
    /// </summary>
    public string Id {
      get;
      internal set;
    } string IUnique.Id {
      get => Id;
      set => Id = value;
    }

    /// <summary>
    /// The display name of an entity
    /// </summary>
    public string Name {
      get;
      internal set;
    }

    /// <summary>
    /// Availible sprite animations by tag
    /// </summary>
    public TagedCollection<Animation.Tag, Animation> Animations {
      get;
    } = new TagedCollection<Animation.Tag, Animation>();

    /// <summary>
    /// X Bam Builder
    /// </summary>
    /// <param name="builder"></param>
    Entity(IBuilder<Entity> builder) 
      : this(
        builder?.GetParam(nameof(Name), (builder.Archetype as Type).Name) 
          ?? (builder.Archetype as Type).Name,
        (builder?.GetParam(nameof(IUnique.Id), (builder.Archetype as Type).Name))
    ){}

    /// <summary>
    /// Make a new entity
    /// </summary>
    public Entity(string name, string uniqueName = null) : base() {
      Name = name;
      string key = uniqueName ?? Name;
      if(_entityUsedNames.ContainsKey(key ?? "__none_")) {
        _entityUsedNames[uniqueName] = _entityUsedNames[uniqueName] + 1;
        key += _entityUsedNames[uniqueName].ToString();
      }

      Id = key;
    }
  }
}