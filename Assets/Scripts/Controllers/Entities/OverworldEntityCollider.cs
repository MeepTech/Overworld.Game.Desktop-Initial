using UnityEngine;

[RequireComponent(typeof(OverworldEntity))]
[RequireComponent(typeof(Rigidbody))]
public class OverworldEntityCollider : MonoBehaviour {

  /// <summary>
  /// Supported Colider shapes
  /// </summary>
  public enum Shape {
    Box,
    Sphere,
    Capsule,
    Custom // TODO: impliment
  }

  /// <summary>
  /// The collider shape to use.
  /// </summary>
  public Shape ColliderShape
    = Shape.Box;

  /// <summary>
  /// The offset applied to the collider
  /// </summary>
  public Vector3 BottomCenterPivotOffset {
    get => _bottomCenterPivotOffset;
  } [SerializeField, ReadOnly]
  Vector3 _bottomCenterPivotOffset;

  // Start is called before the first frame update
  void Start() {
    // TODO: move to world:
    Physics.gravity = new Vector3(0f, -9.81f, 0f);
    OverworldEntity entity = GetComponent<OverworldEntity>();

    // offset the center to the bottom of the object's sprite
    float x,y,z;
    x = 0;
    y = -((entity.SpriteData.sprite.texture.height - entity.RawSpriteDimensions.z) / 2f);
    z = -entity.RawSpriteDimensions.z / 2;
    _bottomCenterPivotOffset = new Vector3(x, y, z);

    switch(ColliderShape) {
      case Shape.Box:
        BoxCollider boxCollider = gameObject.AddComponent<BoxCollider>();
        boxCollider.size = entity.RawSpriteDimensions / entity.SpriteData.sprite.pixelsPerUnit;
        boxCollider.center += _bottomCenterPivotOffset / entity.SpriteData.sprite.pixelsPerUnit;
        break;
      case Shape.Sphere:
        SphereCollider sphereCollider = gameObject.AddComponent<SphereCollider>();
        sphereCollider.radius = entity.RawSpriteDimensions.x / entity.SpriteData.sprite.pixelsPerUnit;
        sphereCollider.center += _bottomCenterPivotOffset / entity.SpriteData.sprite.pixelsPerUnit;
        break;
      case Shape.Capsule:
        CapsuleCollider capsuleCollider = gameObject.AddComponent<CapsuleCollider>();
        capsuleCollider.direction = 2; // Z-axis
        capsuleCollider.radius = entity.RawSpriteDimensions.x / entity.SpriteData.sprite.pixelsPerUnit / 2;
        capsuleCollider.height = entity.RawSpriteDimensions.y / entity.SpriteData.sprite.pixelsPerUnit;
        _bottomCenterPivotOffset.z = 0;
        _bottomCenterPivotOffset.y /= 2;
        _bottomCenterPivotOffset.z -= entity.RawSpriteDimensions.y / 2;
        capsuleCollider.center += _bottomCenterPivotOffset / entity.SpriteData.sprite.pixelsPerUnit;
        break;
      case Shape.Custom:
      default:
        throw new System.NotSupportedException(ColliderShape.ToString());
    }

    //Move it above the ground a little.
    transform.position
      = new Vector3(transform.position.x, transform.position.y + 0.01f, transform.position.z);

    // For inspector display:
    _bottomCenterPivotOffset /= 32f ;
  }

  // Update is called once per frame
  void Update() {

  }
}