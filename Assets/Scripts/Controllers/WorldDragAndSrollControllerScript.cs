using UnityEngine;

[RequireComponent(typeof(WorldController))]
public class WorldDragAndSrollControllerScript : MonoBehaviour {

  #region Unity Inspector Set

  /// <summary>
  /// TODO: should be a user setting.
  /// </summary>
  [Tooltip("The speed of the drag hand.")]
  [SerializeField]
  float _dragSpeed
    = 1f;

  /// <summary>
  /// TODO: should be a user setting.
  /// </summary>
  [Tooltip("Invert the dragging controls.")]
  [SerializeField]
  bool _invertDrag
    = false;

  #endregion

  /// <summary>
  /// If the camera is locked on to the current user's player-character.
  /// </summary>
  public bool CameraIsLockedOnCharacter {
    get;
    private set;
  }

  /// <summary>
  /// If player is currently dragging the view in this frame
  /// </summary>
  public bool IsDragging {
    get;
    private set;
  }

  /// <summary>
  /// If the player was dragging the view the last frame
  /// </summary>
  public bool WasDragging {
    get;
    private set;
  }

  Vector3? _dragFromPosition;

  void Update() {
    WorldController worldController = Demiurge.Self.WorldController;
    if(worldController.World.Options.AllowDragging || worldController.IsInEditMode) {
      /// drag can be prevented in a tool by overriding the right click button
      if(worldController.IsInEditMode && (worldController.WorldEditor.ToolController.CurrentlyEnabledTool?.HotKeys?.Contains(KeyCode.Mouse1) ?? false)) {
        return;
      }

      DragUpdateLogic();
    }
  }

  /// <summary>
  /// This is what needs to run in update to enable dragging.
  /// </summary>
  public void DragUpdateLogic() {
    WasDragging = IsDragging;
    if(Input.GetMouseButtonDown(1)) {
      _dragFromPosition = Input.mousePosition;
      return;
    }
    if(Input.GetMouseButton(1)) {
      if(Input.mousePosition != _dragFromPosition.Value) {
        IsDragging = true;
      }
    } else {
      IsDragging = false;
    }

    if(IsDragging) {
      Vector3 moveDiff = Camera.main.ScreenToViewportPoint(Input.mousePosition - _dragFromPosition.Value);
      Vector3 move = new Vector3(moveDiff.x * _dragSpeed, 0, moveDiff.y * _dragSpeed) * (_invertDrag ? -1 : 1);

      Camera.main.transform.Translate(move, Space.World);
    }
  }
}
