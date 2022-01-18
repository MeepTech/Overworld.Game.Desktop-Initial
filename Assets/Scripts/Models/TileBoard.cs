using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Overworld.Data {

  /// <summary>
  /// A collection of tiles for a board.
  /// </summary>
  public class TileBoard: IEnumerable<(Vector2Int location, Tile data)> {

    /// <summary>
    /// Required tile archetypes needed to load this board.
    /// </summary>
    public HashSet<Tile.Type> RequiredTileTypes {
      get => _requiredTileTypes;
    } HashSet<Tile.Type> _requiredTileTypes;

    /// <summary>
    /// The raw tile data for this board
    /// </summary>
    Dictionary<Vector2Int, Tile> _rawTileData
      = new Dictionary<Vector2Int, Tile>();

    /// <summary>
    /// Get a tile via world location from above
    /// </summary>
    public Tile? this[Vector2Int tileWorldLocation] {
      get => _rawTileData.TryGetValue(tileWorldLocation, out var found)
        ? found
        : null;
      internal set {
        if(value.HasValue)
          _rawTileData[tileWorldLocation] = value.Value;
        else
          _rawTileData.Remove(tileWorldLocation);
      }
    }

    /// <summary>
    /// Get a tile via world location from above
    /// </summary>
    public Tile? this[int x, int y_z] {
      get => _rawTileData.TryGetValue(new Vector2Int(x, y_z), out var found)
        ? found
        : null;
      internal set {
        if(value.HasValue)
          _rawTileData[new Vector2Int(x, y_z)] = value.Value;
        else
          _rawTileData.Remove(new Vector2Int(x, y_z));
      }
    }

    public IEnumerator<(Vector2Int location, Tile data)> GetEnumerator()
      => _rawTileData.Select(e => (e.Key, e.Value)).GetEnumerator();

    IEnumerator IEnumerable.GetEnumerator()
      => GetEnumerator();
  }
}