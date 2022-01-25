using Meep.Tech.Data;
using Overworld.Script;
using UnityEngine;

/// <summary>
/// the procreator script.
/// This is the first script executed in a loaded world.
/// </summary>
[DefaultExecutionOrder(-100)]
public class Demiurge : MonoBehaviour {

  /// <summary>
  /// The one and only demiurge itself
  /// </summary>
  public static Demiurge Self {
    get;
    private set;
  }

  /// <summary>
  /// The parent world editor controller.
  /// </summary>
  public WorldController WorldController
    => _worldController;
  [SerializeField] WorldController _worldController;

  /// <summary>
  /// Initial world level configuration:
  /// </summary>
  void Awake() {
    Self = this;

    /// Set up World
    WorldController.World = new() {
      OwsContext = new Ows.Program.ContextData(
        commands: new() {
          {
            Archetypes<Overworld.Script.Unity.UNITY_DEBUG_LOG>.Archetype.Id.Name,
            Ows.Command.Types.Get<Overworld.Script.Unity.UNITY_DEBUG_LOG>()
          }
        }
      )
    };
  }

  // Start is called before the first frame update
  /*void Start() {
   Ows.Program test = world.OwsInterpreter.Build(@"
      IF-NOT : TRUE: GO-TO : SKIP_SET_ON_FALSE
      SET: TEST TO TRUE
      [SKIP_SET_ON_FALSE] : UNITY_DEBUG_LOG:TEST
    ");

    Character person 
      = Entity.Types.Get<Character.Type>()
        .Make<Character>((nameof(Character.UniqueName), "Test"));

    test.ExecuteAs(person);*/

    /*person.Animations
      .SelectMatches(
        Entity.Animation.Tag.Walk,
        Entity.Animation.Tag.Animated,
        Entity.Animation.Tag.North
      ).FirstWithTagsOrDefault(
        Entity.Animation.Tag.North
      );
  }*/
}
