using Meep.Tech.Data;
using Newtonsoft.Json;
using System;
using System.Diagnostics.CodeAnalysis;

namespace Overworld.Data {
  public partial class Entity {
    public partial class Animation {
      public partial class Tag {

        /// <summary>
        /// Used to convert tags to strings and back by default
        /// </summary>
        public new class JsonConverter : JsonConverter<Tag> {
          public override Tag ReadJson(JsonReader reader, System.Type objectType, [AllowNull] Tag existingValue, bool hasExistingValue, JsonSerializer serializer) {
            if(reader.TokenType == JsonToken.String) {
              return Archetypes.DefaultUniverse.Enumerations.Get<Tag>(reader.Value);
            } else
              throw new ArgumentException($"Tag could not be deserialized. Just use the string text of the tag: {reader.Value?.ToString() ?? "null"}");
          }

          public override void WriteJson(JsonWriter writer, [AllowNull] Tag value, JsonSerializer serializer) {
            writer.WriteValue(value.ExternalId.ToString().ToUpper());
          }
        }
      }
    }
  }
}