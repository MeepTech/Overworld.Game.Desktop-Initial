using Overworld.Utilities;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class WorldEditorTilesSelectorGridController : MonoBehaviour {
  [SerializeField] RuleTile _outlineTile;
  [SerializeField] Tile _fillTile;

  [SerializeField] Tilemap _selectionOutline;
  [SerializeField] Tilemap _selectionFill;
  [SerializeField] Tilemap _newSelectionOutline;
  [SerializeField] Tilemap _newSelectionFill;

  public void ClearTempSelection() {
    _newSelectionOutline.ClearAllTiles();
    _newSelectionFill.ClearAllTiles();
  }

  public void ClearSelection() {
    _selectionFill.ClearAllTiles();
    _selectionOutline.ClearAllTiles();
  }

  public void ToggleSelected(TilesSelectorEditorTool toolData, Vector2Int tileLocation, bool? setIsSelectedTo = null, bool newlySelected = false) {
    Tilemap outlineMap, fillMap;
    if(newlySelected) {
      outlineMap = _newSelectionOutline;
      fillMap = _newSelectionFill;
    } else {
      outlineMap = _selectionOutline;
      fillMap = _selectionFill;
    }

    bool setTo = setIsSelectedTo
      ?? !toolData.IsSelected(tileLocation);

    if(setTo) {
      outlineMap.SetTile(tileLocation.X_Y_0(), _outlineTile);
      fillMap.SetTile(tileLocation.X_Y_0(), _fillTile);
      if(!newlySelected) {
        toolData.
          _selectedTiles
          .Add(tileLocation);
      }
    } else {
      outlineMap.SetTile(tileLocation.X_Y_0(), null);
      fillMap.SetTile(tileLocation.X_Y_0(), null);
      if(!newlySelected) {
        toolData.
        _selectedTiles
        .Remove(tileLocation);
      }
    }
  }
}
