using Meep.Tech.Data;
using System.Collections.Generic;

namespace Overworld.Data {
  public partial class Entity {

    /// <summary>
    /// A sprite animation.
    /// </summary>
    public partial class Animation
      : Model<Animation, Animation.Type>.WithComponents {

      /// <summary>
      /// If this animation should scale to fit the entity it's applied to
      /// </summary>
      public bool ScaleToFitEntity {
        get;
        protected set;
      }

      /// <summary>
      /// Custom Entity-Setable tags for this animation. 
      /// This allows you to customize the tags of an animation on an entity.
      /// </summary>
      public IEnumerable<Tag> CustomTags {
        get;
        protected set;
      }

      /// <summary>
      /// Custom Entity-Setable weights for each tag applied to this animation.
      /// This allows you to customize the weight of different tags in animations for different entities.
      /// </summary>
      public IReadOnlyDictionary<Tag, byte> CustomTagWeights {
        get;
        private set;
      }

      public Animation() : base() {
        AddNewComponent<Tags>();
      }
    }
  }
}