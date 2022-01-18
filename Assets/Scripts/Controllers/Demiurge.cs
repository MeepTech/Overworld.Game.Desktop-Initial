using Meep.Tech.Data;
using Meep.Tech.Data.Configuration;
using Microsoft.EntityFrameworkCore;
using Overworld.Data;
using Overworld.Script;
using UnityEngine;

/// <summary>
/// the procreator script.
/// This is the first script executed in a loaded world.
/// </summary>
[DefaultExecutionOrder(-100)]
public class Demiurge : MonoBehaviour {

  /// <summary>
  /// The parent world editor controller.
  /// </summary>
  public WorldController WorldController
    => _worldController;
  [SerializeField] WorldController _worldController;

  /// <summary>
  /// The established X-Bam Universe.
  /// Default Universe can also be used.
  /// </summary>
  public Universe Universe {
    get;
    private set;
  }

  void Awake() {
    // Configure Settings
    Loader.Settings settings = new() {
#if DEBUG
      FatalOnCannotInitializeType = true,
#endif
      PreLoadAssemblies = new() {
        GetType().Assembly,
        typeof(Ows).Assembly
      }
    };

    // Add the converter for tags to both settings:
    settings.ModelSerializerOptions = new Model.Serializer.Settings {
      ConfigureModelJsonSerializerSettings = (defaultResolver, converters) => {
        var defaultSettings = settings.ModelSerializerOptions
            .ConfigureComponentJsonSerializerSettings(defaultResolver, converters);
        defaultSettings.Converters.Add(new Entity.Animation.Tag.JsonConverter());

        return defaultSettings;
      },
      ConfigureComponentJsonSerializerSettings = (defaultResolver, converters) => {
        var defaultSettings = settings.ModelSerializerOptions
            .ConfigureComponentJsonSerializerSettings(defaultResolver, converters);
        defaultSettings.Converters.Add(new Entity.Animation.Tag.JsonConverter());

        return defaultSettings;
      },
      GetDefaultDbContextForModelSerialization = (options, universe) =>
         new(options, universe, onConfiguring: optionsBuilder =>
         optionsBuilder.UseSqlite("Filename=Overworld.db")
        ),
      ModelsMustOptInToEfCoreUsingAttribute = true
    };

    /// Load Archetypes
    Loader loader = new(settings);
    loader.Initialize(
      Universe = new Universe(loader, "Overworld")
    );

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
  void Start() {/*
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
      );*/
  }

  // Update is called once per frame
  void Update() {

  }
}
