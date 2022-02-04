using Overworld.Data;
using System.Collections.Generic;
using UnityEngine;
using Meep.Tech.Data;
using Overworld.Controllers.World;
using Meep.Tech.Collections;

namespace Overworld.Controllers.Editor {

  [DefaultExecutionOrder(10)]
  [RequireComponent(typeof(WorldController))]
  public class WorldEditorController : MonoBehaviour {

    /// <summary>
    /// The parent world controller.
    /// </summary>
    public WorldController WorldController
      => Demiurge.Self.WorldController;

    #region Unity Investigator Game Level Settings

    /// <summary>
    /// The parent world editor controller.
    /// </summary>
    public WorldEditorMainMenuController WorldEditorEditorMainMenu
      => _worldEditorMainMenu;[SerializeField]
    WorldEditorMainMenuController _worldEditorMainMenu;

    /// <summary>
    /// The controller for the selected editor tool
    /// </summary>
    public WorldEditorToolController ToolController
      => _toolController;[SerializeField]
    WorldEditorToolController _toolController;

    /// <summary>
    /// The controller for the selected tiles display
    /// </summary>
    public WorldEditorTilesSelectorGridController TilesSelectionController
      => _tilesSelectionController;[SerializeField]
    WorldEditorTilesSelectorGridController _tilesSelectionController;

    /// <summary>
    /// Objects to toggle off when not in world edit mode.
    /// </summary>
    [SerializeField]
    [Tooltip("Objects that are toggled on and off with Edit mode.")]
    GameObject[] _worldEditModeObjects;

    #endregion

    /// <summary>
    /// All porters, indexed by which type they port
    /// </summary>
    public IReadOnlyDictionary<System.Type, IArchetypePorter> Porters
      => _porters; readonly Dictionary<System.Type, IArchetypePorter> _porters
      = new();

    bool _initialized
    = false;

    void Start() {
      if(WorldController.IsInEditMode) {
        _activateEditMode();
      }
    }

    void OnDisable() {
      _deActivateEditMode();
    }

    void _activateEditMode() {
      if(!_initialized) {
        _initialize();
      }
      _worldEditModeObjects.ForEach(gameObj => gameObj.SetActive(true));
    }

    void _deActivateEditMode() {
      _worldEditModeObjects?.ForEach(gameObj => gameObj?.SetActive(false));
    }

    void _initialize() {
      _porters.Add(typeof(Tile.Type), new Tile.Porter(WorldController.CurrentUser));
      _initialized = true;
    }
  }
}