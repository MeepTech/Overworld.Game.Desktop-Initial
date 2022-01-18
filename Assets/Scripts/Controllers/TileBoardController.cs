using Meep.Tech.Data;
using UnityEngine;
using UnityEngine.Tilemaps;

public class TileBoardController : MonoBehaviour {

  public WorldController WorldController 
    => _worldController;[SerializeField] WorldController _worldController;

  [SerializeField][Tooltip("The tilemap's unique board key")]
  internal string _boardKey;

  [SerializeField][Tooltip("The tilemap this controls")]
  Tilemap _tilemap;

  [SerializeField][Tooltip("The heigthmap for the tilemap this controls")]
  internal Tilemap _heightMap;

  [SerializeField][Tooltip("The hookMap for the tilemap this controls")]
  internal Tilemap _hookMap;

  #region Import Logic

  void Awake() {
    _tryToLoadBoardByName(_boardKey);
  }

  /// <summary>
  /// Add tiles
  /// </summary>
  internal Overworld.Data.Tile SetTile(Vector2Int location, Overworld.Data.Tile.Type tile)
    => SetTile(location, tile.Make()).Value;

  /// <summary>
  /// Add tiles
  /// </summary>
  internal Overworld.Data.Tile? SetTile(Vector2Int location, Overworld.Data.Tile? tile) {
    _tilemap.SetTile(new Vector3Int(location.x, location.y, 0), null);
    _tilemap.SetTile(new Vector3Int(location.x, location.y, 0), tile.Value.Background);
    return tile;
  }

  /// <summary>
  /// Update a tile using an archetype.
  /// </summary>
  public void UpdateTile(Vector2Int location, Overworld.Data.Tile.Type withArchetype) {
    Overworld.Data.Tile toSet;
    Overworld.Data.Tile? existing = _worldController.World.Boards
      [_worldController.ActiveBoardKey]
      [location];

    /// set the tile on the map graphically
    if(existing.HasValue) {
      // update an existing tile with a new archetype:
      float originalHeight = existing.Value.Height;
      toSet = existing.Value;
      toSet.Archetype = withArchetype;
      _worldController.TileBoards.CurrentDominantTileBoardForUser
        .SetTile(location, toSet);
      // update the height map value if the height changed:
      if(originalHeight != toSet.Height) {
        _worldController.TileBoards.CurrentDominantTileBoardForUser._heightMap
          .SetTile(new Vector3Int(location.x, location.y, 0), BasicHeightMapTile.TypesByHeight[toSet.Height].DefaultBackground);
      }
    } // add a new tile at the given location
    else {
      toSet = _worldController.TileBoards.CurrentDominantTileBoardForUser
        .SetTile(location, withArchetype);
      _worldController.TileBoards.CurrentDominantTileBoardForUser._heightMap
        .SetTile(new Vector3Int(location.x, location.y, 0), BasicHeightMapTile.TypesByHeight[toSet.Height].DefaultBackground);
    }

    /// update the tile data:
    _worldController.World.Boards
      [_worldController.ActiveBoardKey]
      [location]
        = toSet;
  }

  void _tryToLoadBoardByName(string name) {
    if(_worldController.World.Boards.TryGetValue(name, out var boardData)) {
      _loadFromBoard(boardData);
    } else {
      _worldController.World.Boards.Add(name, new Overworld.Data.TileBoard());
    }
  }

  void _loadFromBoard(Overworld.Data.TileBoard boardData) {
    boardData.ForEach(tile => 
      SetTile(tile.location, tile.data));
  }

  #endregion
}
