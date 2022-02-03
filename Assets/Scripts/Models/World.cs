using Overworld.Script;
using System.Collections.Generic;
using UnityEngine;

namespace Overworld.Objects.Models {

  /// <summary>
  /// Game//Unity specific constants for the world.
  /// </summary>
  public partial class World : Overworld.Data.World {

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
      get => _interpreter ??= new(OwsContext);
    } Ows.Interpreter _interpreter;

    /// <summary>
    /// The editor specific data.
    /// This should be lazy loaded when the editor is opened for a world.
    /// </summary>
    public EditorData Editor {
      get;
    } = new();
  }
}