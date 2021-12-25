using Meep.Tech.Data;

namespace Overworld.Data {
  public partial class Entity {
    public partial class Animation {

      /// <summary>
      /// The layer of the entity an animation applies to
      /// </summary>
      public enum Layer {
        Underlay = -1,
        BaseBody,
        Overlay,
        OverallEffect = '*'
      }

      /// <summary>
      /// Tags used to find animations
      /// </summary>
      public partial class Tag : Enumeration<Tag> {

        /// <summary>
        /// Used for the entity's icon.
        /// </summary>
        public readonly static Tag Icon
          = new ("Icon");

        /// <summary>
        /// Indicates North Facing Sprite Animation
        /// </summary>
        public readonly static Tag North
          = new ("North");

        /// <summary>
        /// Indicates South Facing Sprite Animation
        /// </summary>
        public readonly static Tag South
          = new ("South");

        /// <summary>
        /// Indicates Eastward Facing Sprite Animation
        /// </summary>
        public readonly static Tag East
          = new ("East");

        /// <summary>
        /// Indicates Westward Facing Sprite Animation
        /// </summary>
        public readonly static Tag West
          = new ("West");

        /// <summary>
        /// Represents a sprite that is animated
        /// </summary>
        public readonly static Tag Animated
          = new ("Animated");

        /// <summary>
        /// Represents a single framed sprite or A non-moving one
        /// </summary>
        public readonly static Tag Still
          = new ("Still");

        /// <summary>
        /// Represents a sprite action used to move a number of tiles from your current position.
        /// For animation that stay in the same place but have motion, see "Animated"
        /// </summary>
        public readonly static Tag Move
          = new ("Still");

        /// <summary>
        /// Represents walking
        /// </summary>
        public readonly static Tag Walk
          = new ("Walk");

        /// <summary>
        /// Represents running
        /// </summary>
        public readonly static Tag Run
          = new ("Run");

        /// <summary>
        /// Represents jumping
        /// </summary>
        public readonly static Tag Jump
          = new ("Jump");

        public Tag(string name)
          : base(name.ToUpper()) { }
      }
    }
  }
}