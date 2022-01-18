using UnityEngine;

namespace Overworld.Data {

  public partial class World {

    /// <summary>
    /// creator/owner specified settings for a world
    /// </summary>
    public class Settings {

      /// <summary>
      /// The witdth of a tile in pixesls in this world
      /// </summary>
      public int TileWidthInPixels {
        get;
      } = 32;

      /// <summary>
      /// Allow players to drag the screen in this world.
      /// Always enabled in editor mode.
      /// </summary>
      public bool AllowDragging {
        get;
      } = true;

      /// <summary>
      /// The dimensions of the world, in tiles.
      /// </summary>
      public Vector2Int Dimensions {
        get;
      } = new Vector2Int(500, 500);

      /// <summary>
      /// The zoom in and out limits for the in-game camera.
      /// </summary>
      public (float min, float max) ZoomLimit {
        get;
      } = new (0.1f, 50);
    }
  }
}