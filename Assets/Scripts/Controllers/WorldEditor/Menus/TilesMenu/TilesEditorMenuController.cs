using SpiritWorlds.Controllers;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Tilemaps;

public class TilesEditorMenuController : MonoBehaviour {

  Tilemap FocusedTileGridLayer
    => _tileGrids.Length > _tileSubMenusTabController.enabledTab.id
      ? _tileGrids[_tileSubMenusTabController.enabledTab.id]
      : null;

  public TilesMenuSubMenuController EnabledTileSubMenu
    => _tilesSubMenuControllers.Length > _tileSubMenusTabController.enabledTab.id
      ? _tilesSubMenuControllers[_tileSubMenusTabController.enabledTab.id]
      : null;

  #region Unity inspector set

  [SerializeField]
  int _minTileHight = -8;

  [SerializeField]
  int _maxTileHight = 11;

  [SerializeField]
  TilesEditorMenuOptionController _tileOptionPrefab;

  [SerializeField]
  TabbedMenuController _tileSubMenusTabController;

  [SerializeField]
  TilesMenuSubMenuController[] _tilesSubMenuControllers;

  [SerializeField]
  Tilemap[] _tileGrids;

  [SerializeField]
  UnityEngine.Tilemaps.Tile[] _spritesForHeightTilesFromLowToHigh;

  #endregion

  void Awake() {
    /// initialize the heigt based tile archetypes
    // TODO: can these be loaded in the World editor's tile menu's awake function instead?
    List<Overworld.Data.Tile.Type> basicPositiveHeightMapTiles = new();
    List<Overworld.Data.Tile.Type> basicNegativeHeightMapTiles = new();

    // negatives are first in the tile images
    int index = 0;
    int tileHeight = 0;
    while(tileHeight-- > _minTileHight) {
      BasicHeightMapTile @new
        = new(tileHeight, _spritesForHeightTilesFromLowToHigh[index++]);
      basicNegativeHeightMapTiles.Add(@new);
    }

    // 0, then positive
    tileHeight = -1;
    while(tileHeight++ < _maxTileHight) {
      BasicHeightMapTile @new
        = new(tileHeight, _spritesForHeightTilesFromLowToHigh[index++]);
      basicPositiveHeightMapTiles.Add(@new);
    }

    _tilesSubMenuControllers[1].AddTileOptions(
      basicPositiveHeightMapTiles.Concat(basicNegativeHeightMapTiles),
      _tileOptionPrefab
    );
  }

  public void AddTileBackgroundOptions(IEnumerable<Overworld.Data.Tile.Type> backgroundTileArchetypes) {
    TilesMenuSubMenuController backgroundTilesMenuController =  _tilesSubMenuControllers.First();
    backgroundTilesMenuController.AddTileOptions(backgroundTileArchetypes, _tileOptionPrefab);
  }
}
