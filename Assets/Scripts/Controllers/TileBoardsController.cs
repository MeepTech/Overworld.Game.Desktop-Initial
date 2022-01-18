using System.Collections.Generic;
using UnityEngine;

public class TileBoardsController : MonoBehaviour {

  public const string BaseTileBackgroundsBoardKey = "__baseTileBackgrounds_";
  /*public const string BaseHeightMapBoardKey = "__baseHeightMap_";
  public const string BaseHooksBoardKey = "__baseHooksMap_";*/

  /// <summary>
  /// The currently rendered tile board's controller
  /// </summary>
  public TileBoardController CurrentDominantTileBoardForUser
    => _boards[_dominantBoardKey];

  [SerializeField, ReadOnly]
  string _dominantBoardKey
    = "";

  public TileBoardController this[string boardKey] 
    => _boards.TryGetValue(boardKey, out var found)
      ? found
      : null;

  Dictionary<string, TileBoardController> _boards
    = new();

  void Awake() {
    foreach(TileBoardController tileBoard in GetComponentsInChildren<TileBoardController>()) {
      _boards.Add(tileBoard._boardKey, tileBoard);
    }
    _dominantBoardKey = BaseTileBackgroundsBoardKey;
  }

  public void AddBoard(string key, TileBoardController tileBoard) {
    tileBoard._boardKey = key;
    _boards.Add(key, tileBoard);
  }
}