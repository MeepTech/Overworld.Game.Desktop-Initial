using Overworld.Script;
using System.Collections.Generic;
using UnityEngine;

namespace Overworld.Data {

  /// <summary>
  /// A loadable/joinable world with it's multuple layers
  /// </summary>
  public partial class World {

    /// <summary>
    /// The world origin
    /// </summary>
    public static Vector2Int Origin 
      = Vector2Int.zero;

    /// <summary>
    /// The owner-set options for this world.
    /// </summary>
    public Settings Options {
      get;
    } = new();

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
      get => _interpreter ??= new(OwsContext);
    } Ows.Interpreter _interpreter;

    /// <summary>
    /// The editor specific data.
    /// This should be lazy loaded when the editor is opened for a world.
    /// </summary>
    public EditorData Editor {
      get;
    } = new();

    /// <summary>
    /// The editor specific data.
    /// This should be lazy loaded when the editor is opened for a world.
    /// </summary>
    public Dictionary<string, TileBoard> Boards {
      get;
    } = new();

    /// <summary>
    /// The world boundaries
    /// </summary>
    public (Vector2Int minBottomLeft, Vector2Int maxTopRight) Bounds {
      get => _bounds ??= _calculateWorldBounds();
    } (Vector2Int minBottomLeft, Vector2Int maxTopRight)? _bounds;

    (Vector2Int minBottomLeft, Vector2Int maxTopRight) _calculateWorldBounds()
      => (
        new(
          Origin.x - Options.Dimensions.x / 2,
          Origin.y - Options.Dimensions.x / 2),
        new(
          Origin.x + Options.Dimensions.x / 2,
          Origin.y + Options.Dimensions.x / 2
        )
      );
  }
}