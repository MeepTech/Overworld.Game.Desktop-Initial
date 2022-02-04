using Meep.Tech.Collections;
using Meep.Tech.Data;
using Overworld.Data;
using Overworld.Utilities;
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Tilemaps;

namespace Overworld.Controllers.World {
  public class TileBoardController : MonoBehaviour {

    /// <summary>
    /// The current tile board this controls
    /// </summary>
    public TileBoard Board
      => _worldController.World.Boards
        [BoardKey];

    /// <summary>
    /// The height map chunk size in tiles (x,z)
    /// </summary>
    [Tooltip("he height map chunk size in tiles. Should be an even number.")]
    public Vector2Int HeightMapChunkSize
      => _heightMapChunkSize;[SerializeField] Vector2Int _heightMapChunkSize;

    public WorldController WorldController
      => _worldController;[SerializeField] WorldController _worldController;

    /// <summary>
    /// TODO: this type should be on a prefab that the boards controler creates for each board as needed.
    /// </summary>
    public string BoardKey {
      get => _boardKey;
    }
    [SerializeField]
    [Tooltip("The tilemap's unique board key")]
    internal string _boardKey;

    [SerializeField]
    [Tooltip("The main tilemap this controls")]
    Tilemap _tilemap;

    [SerializeField]
    [Tooltip("The heigthmap for the tilemap this controls")]
    internal Tilemap _heightMap;

    [SerializeField]
    [Tooltip("The hookMap for the tilemap this controls")]
    internal Tilemap _hookMap;

    Dictionary<Vector2Int, MeshCollider> _heightMapCollisionChunks
    = new();

    // game object generation
    void Start() {
      _tryToLoadBoardByName(BoardKey);
      _initializeHeightMap();
    }

    /// <summary>
    /// Update a tile on this board using an archetype.
    /// </summary>
    public void UpdateTile(Vector2Int location, Overworld.Data.Tile.Type withArchetype) {
      Overworld.Data.Tile toSet;
      Overworld.Data.Tile? existing = Board[location];

      /// set the tile background on the map graphically
      if(existing.HasValue) {
        toSet = existing.Value;
        toSet.Archetype = withArchetype;
        _worldController.TileBoards.CurrentDominantTileBoardForUser
          .SetTile(location, toSet);
        return;
      } // add a new tile at the given location
      else {
        toSet = _worldController.TileBoards.CurrentDominantTileBoardForUser
          ._setTile(location, withArchetype);

        /// update the tile data:
        _updateHeightMapForTile(location, toSet.Height);
      }
    }

    Overworld.Data.Tile _setTile(Vector2Int location, Overworld.Data.Tile.Type tile) {
      var toSet = SetTile(location, tile.Make()).Value;
      Board[location] = toSet;
      return toSet;
    }

    /// <summary>
    /// Set a tile completely to a new one.
    /// </summary>
    public Overworld.Data.Tile? SetTile(Vector2Int location, Overworld.Data.Tile? tile) {
      // update an existing tile with a new archetype:
      Overworld.Data.Tile? existing = Board[location];
      float? originalHeight = existing?.Height;

      _tilemap.SetTile(new Vector3Int(location.x, location.y, 0), null);
      _tilemap.SetTile(new Vector3Int(location.x, location.y, 0), tile?.Background);

      // update the height map value if the height changed:
      if(originalHeight != tile?.Height) {
        _updateHeightMapForTile(location, tile?.Height ?? 0);
      }

      Board[location] = tile;
      return tile;
    }

    void _updateHeightMapForTile(Vector2Int tileLocation, float newHeight) {
      _heightMap
        .SetTile(
          new Vector3Int(tileLocation.x, tileLocation.y, 0),
          BasicHeightMapTile.TypesByHeight[newHeight].DefaultBackground
        );

      // TODO: get all entities within the tile and move them up how much we need to
      _updateHeightChunk(tileLocation.AsPersise().DividedBy(HeightMapChunkSize.AsPersise()).Snap());
    }

    void _tryToLoadBoardByName(string name) {
      if(_worldController.World.Boards.TryGetValue(name, out var boardData)) {
        _loadFromBoard(boardData);
      } else {
        _worldController.World.Boards.Add(name, new TileBoard(new Vector2Int(60, 60)));
      }
    }

    void _loadFromBoard(TileBoard boardData) {
      boardData.ForEach(tile =>
        SetTile(tile.location, tile.data));
    }

    void _initializeHeightMap() {
      // for each chunk
      Board.Bounds.minBottomLeft.X_0_Y().Until(
        Board.Bounds.maxTopRight.X_0_Y() - 1.SquareVector().X_0_Y(),
        step: HeightMapChunkSize.X_0_Y().ReplaceY(1),
        @do: heightMapChunkStart => {
        /// Set up the collider and object for this chunk:
        Vector2Int chunkKey = (heightMapChunkStart).X_Z().DividedBy(HeightMapChunkSize);
          _updateHeightChunk(chunkKey);
        }
      );

      // test
      UpdateTile(new Vector2Int(0, 0), BasicHeightMapTile.TypesByHeight[1]);
      UpdateTile(new Vector2Int(1, 0), BasicHeightMapTile.TypesByHeight[1]);
      UpdateTile(new Vector2Int(0, 1), BasicHeightMapTile.TypesByHeight[1]);
      UpdateTile(new Vector2Int(1, 1), BasicHeightMapTile.TypesByHeight[2]);
      UpdateTile(new Vector2Int(-1, -1), BasicHeightMapTile.TypesByHeight[1]);
      UpdateTile(new Vector2Int(1, -2), BasicHeightMapTile.TypesByHeight[3]);
      UpdateTile(new Vector2Int(-3, -5), BasicHeightMapTile.TypesByHeight[1.5f]);
      UpdateTile(new Vector2Int(-1, 1), BasicHeightMapTile.TypesByHeight[0.5f]);
    }

    void _updateHeightChunk(Vector2Int chunkKey) {
      Vector3Int chunkStart = (chunkKey * HeightMapChunkSize).X_0_Y();
      GameObject heightChunk;
      MeshCollider meshCollider;
      if(_heightMapCollisionChunks.TryGetValue(chunkKey, out MeshCollider found)) {
        meshCollider = found;
        heightChunk = found.gameObject;
      } else {
        heightChunk = new($"Height Chunk: {chunkKey.x}, {chunkKey.y}");
        heightChunk.transform.parent = transform;
        meshCollider
          = _heightMapCollisionChunks[chunkKey]
          = heightChunk.AddComponent<MeshCollider>();
      }

      List<Vector3> verticies = new();
      List<int> triangles = new();
      int currentTriangleVertexIndex = 0;

      /// iterate though all tiles in the chunk and build box colliders for them with MATH
      chunkStart.Until(
        chunkStart + HeightMapChunkSize.X_0_Y() - 1.SquareVector().X_0_Y(),
        tileLocationKey => {
          Overworld.Data.Tile? currentTile = Board[tileLocationKey.X_Z()];
          if(tileLocationKey.x == 1 && tileLocationKey.z == 1) {
            int x = 1;
            x++;
          }
          float tileHeight = currentTile?.Height ?? 0;
        // if it's a bottomless pit, skip the tile:
        if(tileHeight == TileBoard.PitDepth) {
          // continue:
          return;
          }

        // actually, fuck math, lets use logic:
        Vector3 previousCornerTop = ((Vector3)tileLocationKey).ReplaceY(tileHeight);
          Overworld.Data.World.CardinalDirection currentSide = Overworld.Data.World.CardinalDirection.South;
          Overworld.Data.World.CardinalCorner currentCorner = Overworld.Data.World.CardinalCorner.SouthWest;
          Vector3[] topOfBoxCorners = new Vector3[4];
          Vector3[][] bottomsOFTheSidesOfTheBox = new Vector3[4][];

        /// For each cardinal direction, we'll build part of the top and the given box side:
        foreach(Overworld.Data.World.CardinalDirection direction in Enum.GetValues(typeof(Overworld.Data.World.CardinalDirection))) {
            Vector3 currentTopCornerVertex = previousCornerTop + Overworld.Data.World.CardinalOffsets[direction].X_0_Y();
            currentSide = currentSide.TurnClockwise();
            currentCorner = currentCorner.TurnClockwise();
            topOfBoxCorners[(int)currentCorner] = currentTopCornerVertex;

            Overworld.Data.Tile? neighbor = Board[tileLocationKey.X_Z() + Overworld.Data.World.CardinalOffsets[currentSide]];
          // if the neighbor is higher or the same height we don't need to make a side face in this direction for this tile's collider.
          // if the current tile is taller though, we will need to add the bottom corners to draw this face of the box
          // also ignore pits.
          float neighborHeight = neighbor?.Height ?? 0;
            if(neighborHeight < tileHeight && neighborHeight != TileBoard.PitDepth) {
              Vector3[] bottomCorners = new Vector3[2];
              bottomCorners[0] = currentTopCornerVertex.ReplaceY(neighborHeight);
              bottomCorners[1] = bottomCorners[0] + Overworld.Data.World.CardinalOffsets[currentSide.TurnCounterClockwise()].X_0_Y();

              bottomsOFTheSidesOfTheBox[(int)currentSide] = bottomCorners;
            }

            previousCornerTop = currentTopCornerVertex;
          }

        /// Construct the top of the box:
        int topCornerTriIndex = currentTriangleVertexIndex;
          _constructColliderBoxTopFace(topOfBoxCorners, ref verticies, ref triangles, ref currentTriangleVertexIndex, out int[] tris);

        /// Foreach side that exists, construct it in a similar manner to the top:
        foreach(Overworld.Data.World.CardinalDirection direction in Enum.GetValues(typeof(Overworld.Data.World.CardinalDirection))) {
            Vector3[] sideOfBoxBottomCorners = bottomsOFTheSidesOfTheBox[(int)direction];
            if(sideOfBoxBottomCorners is not null) {
              _constructColliderBoxSideFace(
                // South Corners:
                sideOfBoxBottomCorners
                  // right
                  .Prepend(topOfBoxCorners[(int)direction.TurnCounterClockwise()])
                  // left
                  .Prepend(topOfBoxCorners[(int)direction])
                  .ToArray(),
                ref verticies,
                ref triangles,
                ref currentTriangleVertexIndex,
                // left
                tris[(int)direction.TurnClockwise()],
                // right
                tris[(int)direction]
              );
            }
          }
        }
      );

      meshCollider.sharedMesh = new() {
        vertices = verticies.ToArray(),
        triangles = triangles.ToArray()
      };
    }

    void _constructColliderBoxTopFace(Vector3[] BoxFaceCorners, ref List<Vector3> verticies, ref List<int> triangles, ref int currentTriangleVertexIndex, out int[] triVertIndexes) {
      // Spacial Verts:
      verticies.AddRange(BoxFaceCorners);

      // Tri Verts:
      // NW
      int a = currentTriangleVertexIndex++;
      // NE
      int b= currentTriangleVertexIndex++;
      // SE
      int c = currentTriangleVertexIndex++;
      // SW
      int d = currentTriangleVertexIndex++;

      // NE tri
      triangles.AddRange(
        new[] { a, b, c }
      );
      // SW tri
      triangles.AddRange(
        new[] { a, c, d }
      );

      triVertIndexes = new[] { a, b, c, d };
    }

    void _constructColliderBoxSideFace(
      Vector3[] BoxFaceCorners,
      ref List<Vector3> verticies,
      ref List<int> triangles,
      ref int currentTriangleVertexIndex,
      int topLeftCornerTriangleVertexIndex,
      int topRightCornerTriangleVertexIndex
    ) {
      // Spacial Verts:
      // skip 2 here because they were already added to the top
      verticies.AddRange(BoxFaceCorners.Skip(2));

      // Tri Verts:
      // The top two are taken from the existing box top:
      // NW
      int a = topLeftCornerTriangleVertexIndex;
      // NE
      int b = topRightCornerTriangleVertexIndex;
      // These two are new:
      // SE
      int c = currentTriangleVertexIndex++;
      // SW
      int d = currentTriangleVertexIndex++;

      // NE tri
      triangles.AddRange(
        new[] { a, b, c }
      );
      // SW tri
      triangles.AddRange(
        new[] { a, c, d }
      );
    }
  }
}