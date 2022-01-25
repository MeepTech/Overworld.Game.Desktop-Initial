using Overworld.Data;
using UnityEngine;

public class SelectedTileController : MonoBehaviour {

  /// <summary>
  /// The compiled dimensions
  /// </summary>
  public Vector2Int SelectedTileLocation {
    get => _selectedTileLocation;
  } [SerializeField, ReadOnly]
  Vector2Int _selectedTileLocation;

  /// <summary>
  /// The compiled dimensions
  /// </summary>
  public Vector2Int HoveredTileLocation {
    get => _hoveredTileLocation;
  } [SerializeField, ReadOnly]
  Vector2Int _hoveredTileLocation;

  /// <summary>
  /// The compiled dimensions
  /// </summary>
  public Vector3 MouseLocation {
    get => _mouseLocation;
  } [SerializeField, ReadOnly]
  Vector3 _mouseLocation;

  /// <summary>
  /// The currently selected tile
  /// </summary>
  public Tile? SelectedTile
    => _worldController.World.Boards.TryGetValue(_worldController.TileBoards.CurrentDominantTileBoardForUser.BoardKey, out var foundBoard)
      ? foundBoard[SelectedTileLocation]
      : _worldController.World.Boards.TryGetValue(TileBoardsController.BaseTileBackgroundsBoardKey, out foundBoard)
        ? foundBoard[SelectedTileLocation]
        : null;

  /// <summary>
  /// The current hovered tile
  /// </summary>
  public Tile? HoveredTile
    => _worldController.World.Boards.TryGetValue(_worldController.TileBoards.CurrentDominantTileBoardForUser.BoardKey, out var foundBoard)
      ? foundBoard[HoveredTileLocation]
      : _worldController.World.Boards.TryGetValue(TileBoardsController.BaseTileBackgroundsBoardKey, out foundBoard)
        ? foundBoard[HoveredTileLocation]
        : null;

  #region Unity Inspector Set

  [SerializeField]
  GameObject SelectedTileIndicator;

  [SerializeField]
  GameObject HoveredTileIndicator;

  #endregion

  WorldController _worldController
    => Demiurge.Self.WorldController;

  // Update is called once per frame
  void Update() {
    if(!_worldController.MouseIsOverUI) {
      // move hover
      _mouseLocation = Camera.main.ScreenToWorldPoint(Input.mousePosition);
      _hoveredTileLocation
        = new(Mathf.FloorToInt(_mouseLocation.x), Mathf.FloorToInt(_mouseLocation.z));
      HoveredTileIndicator.transform.position = new Vector3(
        _hoveredTileLocation.x + 0.5f,
        0.01f,
        _hoveredTileLocation.y + 0.5f
      );

      // on click, select:
      if(Input.GetMouseButtonDown(0)) {
        _selectedTileLocation = _hoveredTileLocation;
        SelectedTileIndicator.transform.position = new Vector3(
          _selectedTileLocation.x + 0.5f,
          0.01f,
          _selectedTileLocation.y + 0.5f
        );

      }
    }
  }
}
