using Meep.Tech.Data;
using Meep.Tech.Data.Configuration;
using Microsoft.EntityFrameworkCore;
using Overworld.Data;
using UnityEngine;

public class Demiurge : MonoBehaviour {

  public Universe Universe {
    get;
    private set;
  }

  void Awake() {
    Loader.Settings settings = new() {
#if DEBUG
      FatalOnCannotInitializeType = true,
#endif
      PreLoadAssemblies = new() {
        typeof(Animation).Assembly
      },
      GetDefaultDbContextForModelSerialization = (options, universe) => 
         new (options, universe, onConfiguring: optionsBuilder => 
          optionsBuilder.UseSqlite("Filename=Overworld.db")
        )
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
      }
    };
    Loader loader = new(settings);
    loader.Initialize(
      Universe = new Universe(loader, "Overworld")
    );
  }

  // Start is called before the first frame update
  void Start() {
    World world = new World();

    Ows.Interpreter interpreter 
      = new(new(world));

    interpreter.Build(@"
      IF-NOT : TRUE: GO-TO : SKIP_SET_ON_FALSE
      SET: TEST TO TRUE
      [SKIP_SET_ON_FALSE] : UNITY_DEBUG_LOG:TEST
    ");

    Character person 
      = Entity.Types.Get<Character.Type>()
        .Make<Character>((nameof(Character.UniqueName), "Test"));

    interpreter.Program
      .ExecuteAs(person);

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