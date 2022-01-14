using Meep.Tech.Data;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class TileBoardController : MonoBehaviour {

  [SerializeField][Tooltip("The tilemap's unique board key")]
  internal string _boardKey;

  [SerializeField][Tooltip("The tilemap this controls")]
  Tilemap _tilemap;

  Dictionary<Hash128, Tile> _tileTypes
    = new();

  #region Import Logic

  /// <summary>
  /// Add tiles
  /// </summary>
  internal void SetTile(Vector3Int location, Tile tile) {
    _tilemap.SetTile(location, tile);
  }

  /// <summary>
  /// Add tiles
  /// </summary>
  internal void AddTiles(IReadOnlyDictionary<Hash128, Tile> tiles)
    => _tileTypes = _tileTypes.MergeIn(tiles);

  #endregion
}
