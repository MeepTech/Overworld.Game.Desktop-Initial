using UnityEngine;

[RequireComponent(typeof(OverworldEntityCollider))]
public class CharacterMovementController : MonoBehaviour {

  [Header("Move")]
  [Tooltip("The base walk move speed")]
  public float BaseWalkSpeed 
    = 2.0f;

  [Tooltip("The value BaseWalkSpeed is multiplied by when this character runs")]
  public float RunSpeedMultiplier 
    = 1.5f;

  [Tooltip("The value BaseWalkSpeed is multiplied by when this character walks slowly")]
  public float SlowWalkSpeedMultiplier 
    = 0.5f;

  [Header("Jump")]
  [Tooltip("Jump Height in Tiles")]
  public float JumpHeight
    = 1.5f;

  public float CurrentSpeed {
    get => _currentSpeed ?? BaseWalkSpeed;
  } [SerializeField, ReadOnly]
  float? _currentSpeed;

  /// <summary>
  /// Virtual Z Value
  /// </summary>
  public float CurrentHeightAboveGround {
    get => _currentHeightAboveGround;
  } [SerializeField, ReadOnly]
  float _currentHeightAboveGround;

  Rigidbody _body;
  float _horizontalInput;
  float _verticalInput;
  float moveLimiter = 0.9f;

  void Start() {
    _body = GetComponent<Rigidbody>();
  }

  void Update() {
    _horizontalInput = Input.GetAxisRaw("Horizontal");
    _verticalInput = Input.GetAxisRaw("Vertical");
    // Run:
    if(Input.GetKeyDown(KeyCode.LeftShift)) {
      _currentSpeed = CurrentSpeed * RunSpeedMultiplier;
    }
    // Slow Walk:
    if(Input.GetKeyDown(KeyCode.LeftAlt)) {
      _currentSpeed = CurrentSpeed * SlowWalkSpeedMultiplier;
    }

    // Back to normal speed:
    if(Input.GetKeyUp(KeyCode.LeftAlt) || Input.GetKeyUp(KeyCode.LeftShift)) {
      _currentSpeed = BaseWalkSpeed;
    }
  }

  void FixedUpdate() {
    // Check for diagonal keyboard movement. If it is, limit it to 70% b/c math
    // TODO: ignore this for controllers, they take this into account:
    if(_horizontalInput != 0 && _verticalInput != 0) {
      _horizontalInput *= moveLimiter;
      _verticalInput *= moveLimiter;
    }

    _body.velocity = new Vector3(
      _horizontalInput * CurrentSpeed,
      0,
      _verticalInput * CurrentSpeed
    );
  }
}