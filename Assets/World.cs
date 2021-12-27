using System.Collections.Generic;

namespace Overworld.Data {
  /// <summary>
  /// A loadable/joinable world with it's multuple layers
  /// </summary>
  public class World {

    /// <summary>
    /// All of the entities in the current world by id
    /// </summary>
    public IReadOnlyDictionary<string, Entity> Entities 
      => _entities;
    readonly Dictionary<string, Entity> _entities
      = new();

    /// <summary>
    /// The generial Ows commands loaded into this world
    /// </summary>
    public Dictionary<string, Ows.Command.Type> Commands {
      get;
      internal set;
    } = new();
  }
}