using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Tilemaps;


/// <summary>
/// Used to hold constants for, and set up base archetypes.
/// </summary>
public class BuiltInArchetypesInitializer : MonoBehaviour {

  [Header("Default Tile Height Archetypes")]
  [SerializeField]
  [Tooltip("The lowest available height for a tile")]
  float _minTileHight = -10;

  [SerializeField]
  [Tooltip("The highest available height for a tile")]
  float _maxTileHight = 10;

  [SerializeField]
  [Tooltip("The sprite sheet to cute the default tile height archetype tile icons from")]
  internal Sprite[] _spritesForHeightTilesFromLowToHigh;

  static internal IEnumerable<Overworld.Data.Tile.Type> _defaultHeightTiles;

  void Awake() {
    _loadBasicHeightMapTileArchetypes();
  }

  void _loadBasicHeightMapTileArchetypes() {
    /// initialize the heigt based tile archetypes
    List<Overworld.Data.Tile.Type> basicPositiveHeightMapTiles = new();
    List<Overworld.Data.Tile.Type> basicNegativeHeightMapTiles = new();
    /// gradient them with random-ish colors.
    Color lowestColor = new(7, 24, 108);
    Color zeroColor = new(128, 186, 181);
    Color highestColor = new(73, 51, 50);

    // negatives are first in the tile images
    int index = 0;
    float tileHeight = _minTileHight;
    while(tileHeight < 0) {
      UnityEngine.Tilemaps.Tile tile = ScriptableObject.CreateInstance<UnityEngine.Tilemaps.Tile>();
      tile.sprite = _spritesForHeightTilesFromLowToHigh[index++];
      tile.flags = TileFlags.None;
      tile.color = Color.Lerp(zeroColor, lowestColor, tileHeight / (float)_minTileHight);

      BasicHeightMapTile @new
        = new(tileHeight, tile);
      basicNegativeHeightMapTiles.Add(@new);

      tileHeight += 0.5f;
    }

    // 0, then positive
    tileHeight = 0;
    while(tileHeight <= _maxTileHight) {
      UnityEngine.Tilemaps.Tile tile = ScriptableObject.CreateInstance<UnityEngine.Tilemaps.Tile>();
      tile.sprite = _spritesForHeightTilesFromLowToHigh[index++];
      tile.flags = TileFlags.None;
      tile.color = Color.Lerp(zeroColor, highestColor, tileHeight / (float)_maxTileHight);

      BasicHeightMapTile @new
        = new(tileHeight, tile);
      basicNegativeHeightMapTiles.Add(@new);

      tileHeight += 0.5f;
    }

    // add them to the editor menu:
    _defaultHeightTiles = basicPositiveHeightMapTiles.Concat(
      (basicNegativeHeightMapTiles as IEnumerable<Overworld.Data.Tile.Type>)
        .Reverse());
  }
}