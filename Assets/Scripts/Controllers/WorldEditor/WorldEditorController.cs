using Overworld.Data;
using System.Collections.Generic;
using UnityEngine;
using Meep.Tech.Data;

[RequireComponent(typeof(WorldController))]
public class WorldEditorController : MonoBehaviour {

  #region Unity Investigator Game Level Settings

  /// <summary>
  /// The parent world editor controller.
  /// </summary>
  public WorldEditorMainMenuController WorldEditorEditorMainMenu
    => _worldEditorMainMenu; [SerializeField]
  WorldEditorMainMenuController _worldEditorMainMenu;

  /// <summary>
  /// The controller for the selected editor tool
  /// </summary>
  public WorldEditorToolController ToolController
    => _toolController; [SerializeField]
      WorldEditorToolController _toolController;

  /// <summary>
  /// The parent world editor controller.
  /// </summary>
  public WorldController WorldController
    => _worldController;
  [SerializeField] WorldController _worldController;

  /// <summary>
  /// Objects to toggle off when not in world edit mode.
  /// </summary>
  [SerializeField][Tooltip("Objects that are toggled on and off with Edit mode.")]
  GameObject[] _worldEditModeObjects;

  #endregion

  /// <summary>
  /// All porters, indexed by which type they port
  /// </summary>
  public IReadOnlyDictionary<System.Type, IPorter> Porters
    => _porters; readonly Dictionary<System.Type, IPorter> _porters
      = new();

  bool _initialized 
    = false;

  void Awake() {
    if(!_initialized) {
      _initialize();
    }
  }

  void OnEnable() {
    if(!_initialized) {
      _initialize();
    }
    _worldEditModeObjects.ForEach(gameObj => gameObj.SetActive(true));
  }

  void OnDisable() {
    _worldEditModeObjects?.ForEach(gameObj => gameObj?.SetActive(false));
  }

  void _initialize() {
    _porters.Add(typeof(Tile.Type), new Tile.Porter(WorldController.CurrentUser));
    _initialized = true;
  }
}