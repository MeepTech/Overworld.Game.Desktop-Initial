using Meep.Tech.Data;
using Meep.Tech.Data.Configuration;
using Overworld.Data;
using Overworld.Data.Entities.Components;
using Overworld.Script;
using UnityEngine;

namespace Assets.Scripts.Configuration {

  /// <summary>
  /// Initialize globals and config inside the game, but outside the loaded game world.
  /// The very first loaded script in the game.
  /// </summary>
  [DefaultExecutionOrder(-1000)]
  public class Barbelo : MonoBehaviour {

    /// <summary>
    /// The background self
    /// </summary>
    public static Barbelo Self {
      get;
      private set;
    }

    /// <summary>
    /// The established X-Bam Universe, and all loaded archetypes
    /// Default Universe can also be used.
    /// This just grows while the game is open while people travel to worlds and caches archetypes.
    /// </summary>
    public Universe Omniverse {
      get;
      private set;
    }

    void Awake() {
      Self = this;
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
        ConfigureJsonSerializerSettings = (defaultResolver, converters) => {
          var defaultSettings = settings.ModelSerializerOptions
            .ConfigureJsonSerializerSettings(defaultResolver, converters);
          defaultSettings.Converters.Add(new Entity.Animation.Tag.JsonConverter());

          return defaultSettings;
        }
      };

      /// Load Archetypes
      Loader loader = new(settings);
      loader.Initialize(
        Omniverse = new Universe(loader, "Overworld")
      );

      /*Character character = Archetypes<Character.Type>._
        .Make<Character>((nameof(BasicPhysicalStats.Height), 10));*/
    }
  }
}
