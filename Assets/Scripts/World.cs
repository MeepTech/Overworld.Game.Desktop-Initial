using Overworld.Script;
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
    public Ows.Program.ContextData OwsContext {
      get;
      internal set;
    } = new();

    /// <summary>
    /// The built in Ows interpeter that this world uses.
    /// Is built with OwsContext
    /// </summary>
    public Ows.Interpreter OwsInterpreter {
      get => _interpreter ??= new Ows.Interpreter(OwsContext);
    } Ows.Interpreter _interpreter;
  }
}