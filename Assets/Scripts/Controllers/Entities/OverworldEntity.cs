using Meep.Tech.Data;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public partial class OverworldEntity : MonoBehaviour {

  public static IEnumerable<OverworldEntity.Event.Hook> ValidHookTypes
    => OverworldEntity.Event.Hook.Type.All.Where(type
      => type.GetType().BaseType == typeof(Enumeration<OverworldEntity.Event.Hook.Type>))
        .Select(type => new Event.Hook(type));

  /// <summary>
  /// If this entity can spin
  /// </summary>
  public bool canSpin
    = false;

  [Header("Optional Override Dimensions (0 Is Ignored)")]
  /// <summary>
  /// (Optional; 0 is ignored) Override for Entity Witdth/Radius in Tiles.
  /// This uses the pixels compared to world tile size to determine height by default.
  /// </summary>
  [Tooltip(@"
    (Optional; 0 is ignored) Override for Entity Witdth/Radius in Tiles.
    This uses the pixels compared to world tile size to determine width by default.
  ")]
  [SerializeField]
  float _widthOverride;
  public float? WidthOverride
    => _widthOverride == 0 ? null : _widthOverride;

  /// <summary>
  /// (Optional; 0 is ignored) Override for Entity Height in Tiles.
  /// This uses the pixels compared to world tile size to determine height by default.
  /// </summary>
  [Tooltip(@"
    (Optional; 0 is ignored) Override for Entity Height in Tiles.
    This uses the pixels compared to world tile size to determine height by default.
  ")]
  [SerializeField]
  float _heightOverride;
  public float? HeightOverride
    => _heightOverride == 0 ? null : _heightOverride;

  /// <summary>
  /// (Optional; 0 is ignored) Override for Entity Depth in Tiles.
  /// This uses the pixels compared to world tile size to determine height by default.
  /// </summary>
  [Tooltip(@"
    (Optional; 0 is ignored) Override for Entity Depth in Tiles.
    This uses the pixels compared to world tile size to determine width by default.
  ")]
  [SerializeField]
  float _depthOverride;
  public float? DepthOverride
    => _depthOverride == 0 ? null : _depthOverride;

  /// <summary>
  /// The entity this represents
  /// </summary>
  public Overworld.Data.Entity Entity {
    get;
    private set;
  }

  /// <summary>
  /// The event hooks
  /// </summary>
  Dictionary <Event.Hook, Event> _hooks
    = new();

  /// <summary>
  /// The compiled dimensions
  /// </summary>
  public Vector3 Dimensions {
    get => _dimensions;
  }[SerializeField, ReadOnly]
  Vector3 _dimensions;

  /// <summary>
  /// The sprite
  /// </summary>
  public SpriteRenderer SpriteData {
    get;
    private set;
  }

  /// <summary>
  /// The raw dimensions for the sprite itself
  /// </summary>
  public Vector3 RawSpriteDimensions {
    get;
    private set;
  }

  /// <summary>
  /// Update the entity's name.
  /// </summary>
  public void SetName(string newNameValue) {
    Entity.Name = newNameValue;
  }

  void Start() {
    Entity = new Overworld.Data.Entity("New Object");
    Rigidbody rigidbody = GetComponent<Rigidbody>();
    if(rigidbody != null) {
      rigidbody.freezeRotation = !canSpin;
    }

    SpriteData = GetComponent<SpriteRenderer>();
    float x = _widthOverride == 0f
        ? SpriteData.sprite.texture.width
        : _widthOverride * 32f;
    float y = _heightOverride == 0f
        ? SpriteData.sprite.texture.height
        : _heightOverride * 32f;
    float z = _depthOverride == 0f
        ? SpriteData.sprite.texture.width
        : _depthOverride * 32f;
    RawSpriteDimensions = new Vector3(x, y, z);
    _dimensions = RawSpriteDimensions / SpriteData.sprite.pixelsPerUnit;
  }

  public void ExecuteHook(Event.Hook hook, Overworld.Data.Character executor, Dictionary<string, object> extraContextParams) {
    if(_hooks.TryGetValue(hook, out Event @event)) {
      @event.ExecuteFor(new Event.Context(Entity, executor, hook));
    }
  }
}
