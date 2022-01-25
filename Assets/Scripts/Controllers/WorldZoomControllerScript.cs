using UnityEngine;

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
  float _zoomInLerpSpeed
    = 0.05f;

  /// <summary>
  /// How fast the scrollwheel should move the camera when zooming in.
  /// </summary>
  [SerializeField]
  float _zoomOutLerpSpeed
    = 0.01f;

  WorldController _worldController;

  void Awake() {
    _worldController = GetComponent<WorldController>();
  }

  // Update is called once per frame
  void Update() {
    /// scroll can be prevented in a tool by overriding the middle mouse button.
    if(_worldController.IsInEditMode
      && (_worldController.WorldEditor.ToolController.CurrentlyEnabledTool?.HotKeys?.Contains(KeyCode.Mouse2) ?? false)
    ) {
      return;
    }

    ZoomUpdateLogic();
  }

  /// <summary>
  /// the update logic needed to zoom in and out.
  /// </summary>
  public void ZoomUpdateLogic() {
    bool zoomedIn = false;
    bool zoomedOut = false;
    if(!_worldController.MouseIsOverUI) {
      /// scroll out
      if(Camera.main.orthographicSize < _worldController.World.Options.ZoomLimit.min || _worldController.IsInEditMode) {
        if(Input.GetAxis("Mouse ScrollWheel") < 0) {
          Camera.main.orthographicSize += _zoomSpeed;
          zoomedOut = true;
        }
      }

      /// scroll in
      if(Camera.main.orthographicSize > 0 && Camera.main.orthographicSize > _worldController.World.Options.ZoomLimit.min) {
        if(Input.GetAxis("Mouse ScrollWheel") > 0) {
          Camera.main.orthographicSize -= _zoomSpeed;
          zoomedIn = true;
          if(Camera.main.orthographicSize < _worldController.World.Options.ZoomLimit.min) {
            Camera.main.orthographicSize = _worldController.World.Options.ZoomLimit.min;
          }
          if(Camera.main.orthographicSize <= 0) {
            Camera.main.orthographicSize = 0.1f;
          }
        }
      }

      /// edit mode lerps toward the mouse on scroll!
      if(_worldController.IsInEditMode && zoomedIn || zoomedOut) {
        Camera.main.transform.position = Vector3.Lerp(
          Camera.main.transform.position,
          new Vector3(
            _worldController.TileSelector.HoveredTileLocation.x,
            Camera.main.transform.position.y,
            _worldController.TileSelector.HoveredTileLocation.y
          ),
          zoomedIn ? _zoomInLerpSpeed : _zoomOutLerpSpeed
        );
      }
    }
  }
}
