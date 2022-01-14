using UnityEngine;

namespace SpiritWorlds.Controllers {
  /// <summary>
  /// Extensions for vector3s in unity
  /// </summary>
  public static class VectorUtilities {

    /// <summary>
    /// replace the x value and return for chaining
    /// </summary>
    public static Vector3 ReplaceX(this Vector3 vector3, float x) {
      vector3.x = x;
      return vector3;
    }

    /// <summary>
    /// replace the y value and return for chaining
    /// </summary>
    public static Vector3 ReplaceY(this Vector3 vector3, float y) {
      vector3.y = y;
      return vector3;
    }
    /// <summary>
    /// replace the x value and return for chaining
    /// </summary>
    public static Vector2 ReplaceX(this Vector2 vector2, float x) {
      vector2.x = x;
      return vector2;
    }

    /// <summary>
    /// replace the y value and return for chaining
    /// </summary>
    public static Vector2 ReplaceY(this Vector2 vector2, float y) {
      vector2.y = y;
      return vector2;
    }

    /// <summary>
    /// replace the z value and return for chaining
    /// </summary>
    public static Vector3 ReplaceZ(this Vector3 vector3, float z) {
      vector3.z = z;
      return vector3;
    }
  }
  }