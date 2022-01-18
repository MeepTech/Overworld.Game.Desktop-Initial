using SpiritWorlds.Controllers;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Tilemaps;

public class TilesEditorMenuController : MonoBehaviour {

  /// <summary>
  /// The layers of the tilemap visible in the editor.
  /// </summary>
  public enum TileMapLayer {
    TileBackgrounds,
    HeightMap,
    Hooks
  }

  Tilemap FocusedTileGridLayer
    => _tileGrids.Length > (_tabsController.enabledTab?.id ?? int.MaxValue)
      ? _tileGrids[_tabsController.enabledTab.id]
      : null;

  public TilesMenuSubMenuController EnabledTileSubMenu
    => _tilesSubMenuControllers.Length > (_tabsController.enabledTab?.id ?? int.MaxValue)
      ? _tilesSubMenuControllers[_tabsController.enabledTab.id]
      : null;

  #region Unity inspector set

  [SerializeField]
  int _minTileHight = -10;

  [SerializeField]
  int _maxTileHight = 10;

  public Tilemap OverlayGrid 
    => _overlayGrid;[SerializeField] Tilemap _overlayGrid;

  public WorldEditorMainMenuController MainEditorMenu 
    => _mainEditorMenu;

  [SerializeField] WorldEditorMainMenuController _mainEditorMenu;

  [SerializeField]
  TilesEditorMenuOptionController _tileOptionPrefab;

  [SerializeField]
  TabbedMenuController _tabsController;

  [SerializeField]
  TilesMenuSubMenuController[] _tilesSubMenuControllers;

  [SerializeField]
  Tilemap[] _tileGrids;

  [SerializeField]
  Sprite[] _spritesForHeightTilesFromLowToHigh;

  #endregion

  void Awake() {
    /// initialize the heigt based tile archetypes
    List<Overworld.Data.Tile.Type> basicPositiveHeightMapTiles = new();
    List<Overworld.Data.Tile.Type> basicNegativeHeightMapTiles = new();
    Color lowestColor = new(7, 24, 108);
    Color zeroColor = new(128, 186, 181);
    Color highestColor = new(73, 51, 50);

    // negatives are first in the tile images
    int index = 0;
    float tileHeight = _minTileHight;
    while(tileHeight < 0) {
      Tile tile = new() {
        sprite = _spritesForHeightTilesFromLowToHigh[index++],
        flags = TileFlags.None,
        color = Color.Lerp(zeroColor, lowestColor, tileHeight / (float)_minTileHight)
      };

      BasicHeightMapTile @new
        = new(tileHeight, tile);
      basicNegativeHeightMapTiles.Add(@new);

      tileHeight += 0.5f;
    }

    // 0, then positive
    tileHeight = 0;
    while(tileHeight <= _maxTileHight) {
      Tile tile = new() {
        sprite = _spritesForHeightTilesFromLowToHigh[index++],
        flags = TileFlags.None,
        color = Color.Lerp(zeroColor, highestColor, tileHeight / (float)_maxTileHight)
      };

      BasicHeightMapTile @new
        = new(tileHeight, tile);
      basicNegativeHeightMapTiles.Add(@new);

      tileHeight += 0.5f;
    }

    _tilesSubMenuControllers[1].AddTileOptions(
      basicPositiveHeightMapTiles.Concat(
        (basicNegativeHeightMapTiles as IEnumerable<Overworld.Data.Tile.Type>).Reverse()),
      _tileOptionPrefab
    );

    /// initialize the heightmap to 0 if there isn't one.
    // TODO: this should check and load the existing heightmap first obviously.
    for(int x = MainEditorMenu.WorldController.World.Bounds.minBottomLeft.x;
      x < MainEditorMenu.WorldController.World.Bounds.maxTopRight.x;
      x++
    ) {
      for(int y = MainEditorMenu.WorldController.World.Bounds.minBottomLeft.y;
        y < MainEditorMenu.WorldController.World.Bounds.maxTopRight.y;
        y++
      ) {
        MainEditorMenu.WorldController.TileBoards.CurrentDominantTileBoardForUser._heightMap
          .SetTile(new Vector3Int(x, y, 0), BasicHeightMapTile.TypesByHeight[0].DefaultBackground);
      }
    }

    /// set up tab onclick to toggle on and off heightmap:
    _tabsController.OnTabChanged = changedTo => {
      if(changedTo == 1) {
        MainEditorMenu.WorldController.TileBoards.CurrentDominantTileBoardForUser
          ._heightMap.gameObject.SetActive(true);
      } else
        MainEditorMenu.WorldController.TileBoards.CurrentDominantTileBoardForUser
          ._heightMap.gameObject.SetActive(false);
    };
  }

  public void AddTileBackgroundOptions(IEnumerable<Overworld.Data.Tile.Type> backgroundTileArchetypes) {
    TilesMenuSubMenuController backgroundTilesMenuController =  _tilesSubMenuControllers.First();
    backgroundTilesMenuController.AddTileOptions(backgroundTileArchetypes, _tileOptionPrefab);
  }
}
