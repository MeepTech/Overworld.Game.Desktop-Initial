using Meep.Tech.Collections;
using Meep.Tech.Data;
using System.Collections.Generic;

namespace Overworld.Objects.Models {

  public partial class World {

    /// <summary>
    /// Data specific to edit mode for a world.
    /// </summary>
    public class EditorData {

      /// <summary>
      /// All the valid tile types for this editor, indexed by menu id.
      /// </summary>
      HashSet<Overworld.Data.Tile.Type> _tileTypes
        = new();

      /// <summary>
      /// Add tiles
      /// </summary>
      internal void AddTileTypes(IEnumerable<Overworld.Data.Tile.Type> types)
        => types.ForEach(type => _tileTypes.Add(type));

    }
  }
}