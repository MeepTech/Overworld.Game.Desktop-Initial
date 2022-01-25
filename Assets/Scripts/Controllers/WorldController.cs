using Overworld.Data;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

[DefaultExecutionOrder(-99)]
public class WorldController : MonoBehaviour {

  /// <summary>
  /// The parent world editor controller.
  /// </summary>
  public WorldEditorController WorldEditor
    => _worldEditor;[SerializeField]
  WorldEditorController _worldEditor;

  #region Child Controllers

  /// <summary>
  /// The tile selecto specific to the editor
  /// </summary>
  public SelectedTileController TileSelector
    => _tileSelector; [SerializeField]
  SelectedTileController _tileSelector;

  /// <summary>
  /// The currently rendered tile board's controller
  /// </summary>
  public TileBoardsController TileBoards
    => _tileBoards; [SerializeField]
  TileBoardsController _tileBoards;

  #endregion

  /// <summary>
  /// The parent transform of all entities
  /// </summary>
  public Transform EntitiesParent
    => _entitiesParent;
  [SerializeField]
  Transform _entitiesParent;

  #region Game State

  /// <summary>
  /// If this world is in edit mode
  /// </summary>
  [Tooltip("Can be used to start the world in edit mode.")]
  public bool IsInEditMode {
    get => _editModeEnabled;
  } [SerializeField] bool _editModeEnabled;

  /// <summary>
  /// If the mouse is currently over a ui element.
  /// </summary>
  public bool MouseIsOverUI
    => _mouseIsOverUI;
  [SerializeField, ReadOnly]
  bool _mouseIsOverUI;

  /// <summary>
  /// The current user in control of the game.
  /// TODO: impliment
  /// </summary>
  public static Overworld.Data.User CurrentUser {
    get;
  }

  #endregion

  #region World State

  /// <summary>
  /// The world data
  /// </summary>
  public Overworld.Game.World World {
    get;
    internal set;
  }

  /// <summary>
  /// The key of the active board froe the current user
  /// </summary>
  public string ActiveBoardKey {
    get;
  } = TileBoardsController.BaseTileBackgroundsBoardKey;

  #endregion

  void Update() {
    _mouseIsOverUI = IsPointerOverUIElement();
  }

  //Returns 'true' if we touched or hovering on Unity UI element.
  public bool IsPointerOverUIElement() {
    return IsPointerOverUIElement(GetEventSystemRaycastResults());
  }

  //Returns 'true' if we touched or hovering on Unity UI element.
  private bool IsPointerOverUIElement(List<RaycastResult> eventSystemRaysastResults) {
    for(int index = 0; index < eventSystemRaysastResults.Count; index++) {
      RaycastResult curRaysastResult = eventSystemRaysastResults[index];
      if(curRaysastResult.gameObject.layer == 5)
        return true;
    }
    return false;
  }


  //Gets all event system raycast results of current mouse or touch position.
  static List<RaycastResult> GetEventSystemRaycastResults() {
    PointerEventData eventData = new(EventSystem.current);
    eventData.position = Input.mousePosition;
    List<RaycastResult> raysastResults = new();
    EventSystem.current.RaycastAll(eventData, raysastResults);
    return raysastResults;
  }
}
