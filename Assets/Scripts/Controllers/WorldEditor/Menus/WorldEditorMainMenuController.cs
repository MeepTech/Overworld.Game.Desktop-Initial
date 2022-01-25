using Overworld.Utilities;
using UnityEngine;

[RequireComponent(typeof(TabbedMenuController))]
public partial class WorldEditorMainMenuController : MonoBehaviour {

  /// <summary>
  /// The parent world controller
  /// </summary>
  public WorldController WorldController
    => Demiurge.Self.WorldController;

  /// <summary>
  /// The main tabbed menu controller of the world editor
  /// </summary>
  public TabbedMenuController TabbedMenuController {
    get;
    internal set;
  }

  /// <summary>
  /// The tiles menu.
  /// </summary>
  public TilesEditorMenuController TilesMenu {
    get => _tilesMenu;
  } [SerializeField] TilesEditorMenuController _tilesMenu;

  /// <summary>
  /// The options currently active for the menu
  /// </summary>
  public Settings ActiveOptions {
    get;
    private set;
  }

  void Start() {
    ActiveOptions = new Settings();
    TabbedMenuController = GetComponent<TabbedMenuController>();
  }
}
  