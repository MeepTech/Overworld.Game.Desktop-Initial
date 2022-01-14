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

  [SerializeField]
  WorldController _worldController;

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
    if(_worldController.AllowDragging || _worldController.IsInEditMode) {
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
        Vector3 move = new(moveDiff.x * _dragSpeed, 0, moveDiff.y * _dragSpeed);

        Camera.main.transform.Translate(move, Space.World);
      }
    }
  }
}

[RequireComponent(typeof(WorldController))]
public class WorldZoomControllerScript : MonoBehaviour {

  /// <summary>
  /// How fast the scrollwheel should zoom.
  /// </summary>
  [SerializeField]
  float _zoomSpeed;

  /// <summary>
  /// How fast the scrollwheel should move the camera when zooming in.
  /// </summary>
  [SerializeField]
  float _zoomInLarpSpeed
    = 0.05f;

  /// <summary>
  /// How fast the scrollwheel should move the camera when zooming in.
  /// </summary>
  [SerializeField]
  float _zoomOutLarpSpeed
    = 0.01f;

  WorldController _worldController;

  void Awake() {
    _worldController = GetComponent<WorldController>();
  }

  // Update is called once per frame
  void Update() {
    bool zoomedIn = false;
    bool zoomedOut = false;
    if(!_worldController.MouseIsOverUI) {
      if(Input.GetAxis("Mouse ScrollWheel") < 0) {
        Camera.main.orthographicSize += _zoomSpeed;
        zoomedOut = true;
      }
      if(Input.GetAxis("Mouse ScrollWheel") > 0 && Camera.main.orthographicSize > 0) {
        Camera.main.orthographicSize -= _zoomSpeed;
        zoomedIn = true;
        if(Camera.main.orthographicSize < 0) {
          Camera.main.orthographicSize = 0.1f;
        }
      }

      if(_worldController.IsInEditMode && zoomedIn || zoomedOut) {
        Camera.main.transform.position = Vector3.Lerp(
          Camera.main.transform.position,
          new Vector3(
            _worldController.TileSelector.HoveredTileLocation.x,
            Camera.main.transform.position.y,
            _worldController.TileSelector.HoveredTileLocation.y
          ),
          zoomedIn ? _zoomInLarpSpeed : _zoomOutLarpSpeed
        );
      }
    }
  }
}
