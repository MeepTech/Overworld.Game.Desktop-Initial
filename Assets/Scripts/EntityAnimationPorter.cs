using Meep.Tech.Data;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using System.IO;
using UnityEditor;
using System.Diagnostics.CodeAnalysis;

namespace Overworld.Data {
  public partial class Entity {
    public partial class Animation {

      /// <summary>
      /// For in-porting and ex-porting and cross-porting entity animations.
      /// Animation metadata is stored in data/entities/animations
      /// </summary>
      public static class Porter {

        /// <summary>
        /// The file path used for this type of metadata by default
        /// </summary>
        public static string MetadataFilePath {
          get;
        } = Path.Combine(
          "data",
          "entities",
          "animations"
        );

        /// <summary>
        /// The format the images can be in for simple animation imports
        /// </summary>
        public enum ImageFormat {
          DividedSheet,
          IndivudualFrames
        }

        /// <summary>
        /// Load all the animation datas from a root folder containing many nested folders
        /// </summary>
        internal static IEnumerable<Type> LoadAllAnimationDatas(string rootFolder) {
          string entityAnimationsRootFolder = Path.Combine(
            rootFolder,
            MetadataFilePath
          );

          IEnumerable<Type> entityAnimationsInRootFolder =
              // get each subdirectory under /data/entities/animations/.../
            Directory.GetDirectories(
              entityAnimationsRootFolder,
              "*",
              SearchOption.AllDirectories
            // ignore folder names that start with . or _
            ).Where(folderName => !folderName.StartsWith("_") && !folderName.StartsWith("."))
            // foreach Animation folder, compile the data
            .Select(folderName 
              => LoadAnimationTypeFromFolder(folderName, entityAnimationsRootFolder));

          // Refresh all assets
          AssetDatabase.SaveAssets();
          AssetDatabase.Refresh();

          return entityAnimationsInRootFolder;
        }

        /// <summary>
        /// load one animation from the given folder.
        /// </summary>
        /// <param name="entityAnimationsRootFolder">The folder root parts to trim away for default namespace generation</param>
        /// <returns></returns>
        internal static Type LoadAnimationTypeFromFolder([NotNull] string folderName, string entityAnimationsRootFolder = "") {
          /// Fetch config, if one doesn't exist we'll build one:
          string configFileName = Path.Combine(folderName, "config.json");
          JObject config = JObject.Parse(
            File.ReadAllText(configFileName) ?? "{}"
          );

          /// Step one, check the config version.
          string versionKey = null;
          string name = null;
          string @namespace = null;
          string getAnimationFileName() 
            => $"{@namespace}.{name}.{versionKey}.entity.anim";
          AnimationClip clip;
          if(config.TryGetValue("version", out JToken versionToken)) {
            versionKey = versionToken.Value<string>();
            if(config.ContainsKey("name") && config.ContainsKey("namespace")) {
              // if there's an animation clip with the same current version:
              // .. we will NOT compile a new one or change anything.
              if((clip = Resources.Load<AnimationClip>(getAnimationFileName())) != null) {
                return new Type(config, clip, null);
              }
            }
          } else {
            versionKey = (config["version"] = "a0.0.0.0").Value<string>();
          }

          /// Step two, Get the images
          List<string> imageFiles = new();
          // Get all PNG image files from the folder:
          foreach(string imageFileName in Directory.GetFiles(folderName, "*.png").OrderBy(folderName => folderName)) {
            imageFiles.Add(imageFileName);
          }
          if(!imageFiles.Any()) {
            throw new MissingReferenceException($"No PNG images found in the entity animation folder: {folderName}.");
          }

          /// Step three, get and check other basic data in the config
          name ??= config.TryGetValue("name", out JToken nameToken)
            ? nameToken.Value<string>()
            : (config["name"] = Path.GetDirectoryName(folderName)).Value<string>();
          @namespace ??= config.TryGetValue("namespace", out JToken namespaceToken)
            ? namespaceToken.Value<string>()
            : (config["namespace"] = GetDefaultNamespace(folderName, entityAnimationsRootFolder
              ?? throw new ArgumentNullException(entityAnimationsRootFolder))).Value<string>();
          int frameRate = config.TryGetValue("frameRate", out JToken frameRateToken)
            ? frameRateToken.Value<int>()
            : (config["frameRate"] = 12).Value<int>();
          ImageFormat format = config.TryGetValue("format", out JToken formatToken)
            ? formatToken.Value<ImageFormat>()
            : (config["format"] = JToken.FromObject(ImageFormat.IndivudualFrames))
              .Value<ImageFormat>();
          bool loop = config.TryGetValue("loop", out JToken loopToken)
            ? loopToken.Value<bool>()
            : (config["loop"] = false).Value<bool>();
          bool scaleToFitEntityByDefault = config.TryGetValue("scaleToFitEntity", out JToken scaleToken)
            ? scaleToken.Value<bool>()
            : (config["scaleToFitEntity"] = true).Value<bool>();
          Layer layer = config.TryGetValue("layer", out JToken layerToken)
            ? layerToken.Value<Layer>()
            : (config["layer"] = JToken.FromObject(Layer.BaseBody)).Value<Layer>();

          /// Step four, get tags
          IEnumerable<Tag> tags = config.TryGetValue("tags", out JToken tagArray)
            ? tagArray.ToObject<IEnumerable<Tag>>(Models.DefaultUniverse.ModelSerializer.ModelJsonSerializer)
            : Enumerable.Empty<Tag>();
          if(!tags.Any()) {
            config.Add("tags", JArray.FromObject(
              Enumerable.Empty<object>(),
              Models.DefaultUniverse.ModelSerializer.ModelJsonSerializer
            ));
          }

          ///Step five: Try to get the sprite animation dimensions
          (int width, int height)? dimensions = null;
          if(config.TryGetValue("dimensions", out JToken dimensionsJson)) {
            dimensions = dimensionsJson.ToObject<(int, int)>();
          }

          // try to get dimensions automatically for images that need to be sliced up
          if(imageFiles.Count == 1 || format == ImageFormat.DividedSheet) {
            // try to get dimensions from the image name. a captial or lowercase X can be used
            if(dimensions is null) {
              IEnumerable<string> dimensionParts = Path.GetFileName(imageFiles.First())
                .Split('x')
                .Select(x => x.Trim());
              if(dimensionParts.Count() != 2) {
                dimensionParts = Path.GetFileName(imageFiles.First())
                .Split('X')
                .Select(x => x.Trim());
              }

              if(dimensionParts.Count() == 2) {
                // if we have one image with this name format it is a split sheet
                config["format"] = JToken.FromObject(ImageFormat.DividedSheet);
                format = ImageFormat.DividedSheet;

                dimensions = (
                  int.Parse(dimensionParts.First()),
                  int.Parse(dimensionParts.Last())
                );
              } // if it's not in the name, it MUST be in the config
              else
                throw new NotImplementedException($"Dimensions for the Sprite Animation that needs to be sliced up were not provided. Please either name the image file with the format: '[heightInPx] x [heightInPx]', or provide an 'entity.animation.json' config file with a provided 'dimensions' setting with the 'height' and 'width' in pixels.");
            }
          }

          // make sure the dimensions are set on the config
          config[nameof(dimensions)]
            = JToken.FromObject(dimensions);

          /// Step six, compile the keyframes
          List<Sprite> keyFrames = new();
          // validate all image file dimensions while loading keyframes:
          if(format == ImageFormat.IndivudualFrames) {
            Sprite firstFrame = null;

            // use the first image's dimensions as a default if they haven't been set yet:
            if(dimensions is null) {
              // TODO: resources.load won't work here, need to read from bytes from file
              Sprite firstSprite = Resources.Load<Sprite>(imageFiles.First());
              dimensions = (firstSprite.texture.width, firstSprite.texture.height);
            }

            foreach(string imageFile in imageFiles) {
              // if we already grabbed the first frame to check dimensions:
              if(!keyFrames.Any() && firstFrame is not null) {
                keyFrames.Add(firstFrame);
                continue;
              }

              Sprite frame = Resources.Load<Sprite>(imageFile);
              if(frame.texture.width != dimensions?.width
                && frame.texture.height != dimensions?.height) {
                throw new ArgumentException($"Animation Frame Image at: {imageFile}, has the incorrect dimensions. Should be, [{dimensions?.width} x {dimensions?.height}]. Found: [{frame.texture.width} x {frame.texture.height}]");
              }

              keyFrames.Add(frame);
            }
          } // Split the single image into multiple keyframes
          else {
            // load the spritesheet image
            (int width, int height) spriteSheetPxDimensions;
            Texture2D spriteSheet = new(2, 2);
            spriteSheet.LoadImage(File.ReadAllBytes(imageFiles.First()));
            spriteSheetPxDimensions = (spriteSheet.width, spriteSheet.height);
            if(
              spriteSheetPxDimensions.width % dimensions.Value.width != 0
              || spriteSheetPxDimensions.height / dimensions.Value.height != 0
            )
              throw new ArgumentException(
              $"Dimensions for Sprite Sheet images are off. "
              + $"Sprite sheet is [{spriteSheetPxDimensions.width} x {spriteSheetPxDimensions.height}],"
              + $" sprites are [{dimensions.Value.width} x {dimensions.Value.height}]. "
              + $"Sprites must be evenly divisible into the sprite sheet."
            );

            // get how many sprites are in the sheet in the height and width dimensions
            (int width, int height) spriteSheetSpriteCountDimensions = (
              spriteSheetPxDimensions.width / dimensions.Value.width,
              spriteSheetPxDimensions.height / dimensions.Value.height
            );

            // chop out all the sprites
            for(int x = 0; x < spriteSheetSpriteCountDimensions.width; x++) {
              for(int y = 0; y < spriteSheetSpriteCountDimensions.height; y++) {
                Sprite choppedSprite = Sprite.Create(spriteSheet, new Rect(
                  x * dimensions.Value.width,
                  y * dimensions.Value.height,
                  dimensions.Value.width,
                  dimensions.Value.height
                ), new Vector2(0.5f, 0.5f));

                keyFrames.Add(choppedSprite);
              }
            }
          }

          /// set up the animation clip
          clip = new() {
            frameRate = frameRate
          };

          // make an animation curve for the sprite renderer
          EditorCurveBinding spriteBinding = new EditorCurveBinding {
            type = typeof(SpriteRenderer),
            path = "",
            propertyName = "m_Sprite"
          };

          // Create the KeyFrames
          ObjectReferenceKeyframe[] spriteKeyFrames = new ObjectReferenceKeyframe[keyFrames.Count];
          for(int frameIndex = 0; frameIndex < spriteKeyFrames.Length; frameIndex++) {
            spriteKeyFrames[frameIndex] = new() {
              time = frameIndex / clip.frameRate,
              value = keyFrames[frameIndex]
            };
          }

          // connect the curve bindings to the animation
          AnimationUtility.SetObjectReferenceCurve(
            clip,
            spriteBinding,
            spriteKeyFrames
          );

          //Some other clip settings
          AnimationClipSettings settings =
            AnimationUtility.GetAnimationClipSettings(clip);
          settings.loopTime = loop;
          AnimationUtility.SetAnimationClipSettings(clip, settings);

          ///Save the clip
          AssetDatabase.CreateAsset(clip, getAnimationFileName());

          /// Save the config
          File.WriteAllText(configFileName, config.ToString());

          /// Return
          return new Type(config, null, tags);
        }

        static JToken GetDefaultNamespace(string folderPath, string contentRoot) {
          string trimmedFolderPath = (string)folderPath.Skip(contentRoot.Length);
          string folderPathWithoutName = trimmedFolderPath.Remove(
          trimmedFolderPath.Length - Path.GetDirectoryName(folderPath).Length
        );

          return folderPathWithoutName
            .Replace('/', '.')
            .Replace('\\', '.')
            .Trim('\\')
            .Trim('/')
            .Trim('.')
            .Trim();
        }
      }
    }
  }
}