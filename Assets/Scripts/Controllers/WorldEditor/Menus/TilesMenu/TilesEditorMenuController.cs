using Overworld.Controllers.World;
using Overworld.Data;
using Overworld.Utilities;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Tilemaps;

namespace Overworld.Controllers.Editor {
  public class TilesEditorMenuController : MonoBehaviour {

    /// <summary>
    /// The layers of the tilemap visible in the editor.
    /// </summary>
    public enum TileMapLayer {
      TileBackgrounds,
      HeightMap,
      Hooks
    }

    /// <summary>
    /// IDK? remove this?
    /// </summary>
    Tilemap FocusedTileGridLayer
      => _tileGrids.Length > (_tabsController.enabledTab?.id ?? int.MaxValue)
        ? _tileGrids[_tabsController.enabledTab.id]
        : null;

    /// <summary>
    /// The currently enabled tile enabled sub menu
    /// </summary>
    public TilesMenuSubMenuController EnabledTileSubMenu
      => _tilesSubMenuControllers.Length > (_tabsController.enabledTab?.id ?? int.MaxValue)
        ? _tilesSubMenuControllers[_tabsController.enabledTab.id]
        : null;

    /// <summary>
    /// Parent Menu
    /// </summary>
    public WorldEditorMainMenuController MainEditorMenu
      => Demiurge.Self.WorldController.WorldEditor.WorldEditorEditorMainMenu;

    /// <summary>
    /// All tile sub menus, in order
    /// </summary>
    public IReadOnlyList<TilesMenuSubMenuController> SubMenus
      => _tilesSubMenuControllers;

    #region Unity inspector set

    public Tilemap OverlayGrid
      => _overlayGrid;[SerializeField] Tilemap _overlayGrid;

    [SerializeField]
    TilesEditorMenuOptionController _tileOptionPrefab;

    [SerializeField]
    TabbedMenuController _tabsController;

    [SerializeField]
    TilesMenuSubMenuController[] _tilesSubMenuControllers;

    [SerializeField]
    Tilemap[] _tileGrids;

    #endregion

    void Start() {
      /// populate the height tiles options menu
      Demiurge.Self.WorldController.WorldEditor.WorldEditorEditorMainMenu
        .TilesMenu.AddTileHeightMapOptions(BuiltInArchetypesInitializer._defaultHeightTiles);

      /// initialize the heightmap visualizer tiles for each board.
      foreach(TileBoard board in MainEditorMenu.WorldController.World.Boards.Values) {
        for(int x = board.Bounds.minBottomLeft.x;
          x < board.Bounds.maxTopRight.x;
          x++
        ) {
          for(int y = board.Bounds.minBottomLeft.y;
            y < board.Bounds.maxTopRight.y;
            y++
          ) {
            float currentTileHeight = (board[x,y]?.Height ?? 0);
            MainEditorMenu.WorldController.TileBoards.CurrentDominantTileBoardForUser._heightMap
              .SetTile(new Vector3Int(x, y, 0), BasicHeightMapTile.TypesByHeight[currentTileHeight].DefaultBackground);
          }
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

    public void AddTileHeightMapOptions(IEnumerable<Overworld.Data.Tile.Type> heightTileArchetypes) {
      TilesMenuSubMenuController backgroundTilesMenuController =  _tilesSubMenuControllers[1];
      backgroundTilesMenuController.AddTileOptions(heightTileArchetypes, _tileOptionPrefab);
    }
  }
}