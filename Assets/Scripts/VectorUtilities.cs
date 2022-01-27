using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Overworld.Utilities {

  /// <summary>
  /// Extensions for vector3s in unity
  /// </summary>
  public static class VectorUtilities {

    static readonly Func<int, int, bool> _reverseCompare
        = (curr, end) => curr >= end;

    /// <summary>
    /// Quick inline for turning vec2 into vec3 with Y = 0;
    /// </summary>
    public static Vector3 X_0_Y(this Vector2 vector2)
      => new(vector2.x, 0, vector2.y);

    /// <summary>
    /// Quick inline for turning vec2 into vec3 with z = 0;
    /// </summary>
    public static Vector3 X_Y_0(this Vector2 vector2)
      => new(vector2.x, vector2.y, 0);

    /// <summary>
    /// Quick inline for turning vec2 into vec3 with Y = 0;
    /// </summary>
    public static Vector3Int X_0_Y(this Vector2Int vector2)
      => new(vector2.x, 0, vector2.y);

    /// <summary>
    /// Quick inline for turning vec2 into vec3 with z = 0;
    /// </summary>
    public static Vector3Int X_Y_0(this Vector2Int vector2)
      => new(vector2.x, vector2.y, 0);

    /// <summary>
    /// Quick inline for turning vec3 into vec2 using x,z;
    /// </summary>
    public static Vector2Int X_Z(this Vector3Int vector3)
      => new(vector3.x, vector3.z);

    /// <summary>
    /// Quick inline for turning vec3 into vec2 using x,y
    /// </summary>
    public static Vector2Int X_Y(this Vector3Int vector3)
      => new(vector3.x, vector3.y);

    /// <summary>
    /// Quick inline for turning vec3 into vec2 using x,z;
    /// </summary>
    public static Vector2 X_Z(this Vector3 vector3)
      => new(vector3.x, vector3.z);

    /// <summary>
    /// Quick inline for turning vec3 into vec2 using x,y
    /// </summary>
    public static Vector2 X_Y(this Vector3 vector3)
      => new(vector3.x, vector3.y);

    /// <summary>
    /// replace the x value and return for chaining
    /// </summary>
    public static Vector3 ReplaceX(this Vector3 vector3, float x) {
      vector3.x = x;
      return vector3;
    }

    /// <summary>
    /// snap to int using floor
    /// </summary>
    public static Vector3Int Snap(this Vector3 vector3)
      => new(Mathf.FloorToInt(vector3.x), Mathf.FloorToInt(vector3.y), Mathf.FloorToInt(vector3.z));

    /// <summary>
    /// convert int to persise float based vector
    /// </summary>
    public static Vector3 AsPersise(this Vector3Int vector3)
      => new(vector3.x, vector3.y, vector3.z);

    /// <summary>
    /// snap to int using floor
    /// </summary>
    public static Vector2Int Snap(this Vector2 vector3)
      => new(Mathf.FloorToInt(vector3.x), Mathf.FloorToInt(vector3.y));

    /// <summary>
    /// convert int to persise float based vector
    /// </summary>
    public static Vector2 AsPersise(this Vector2Int vector3)
      => new(vector3.x, vector3.y);

    /// <summary>
    /// replace the y value and return for chaining
    /// </summary>
    public static Vector3Int ReplaceY(this Vector3Int vector3, int y) {
      vector3.y = y;
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

    /// <summary>
    /// Get a vector3 with all values set to the provided one
    /// </summary>
    public static Vector3 CubeVector(this float value)
      => new(value, value, value);

    /// <summary>
    /// Get a vector3 with all values set to the provided one
    /// </summary>
    public static Vector3Int CubeVector(this int value)
      => new(value, value, value);

    /// <summary>
    /// Get a vector2 with all values set to the provided one
    /// </summary>
    public static Vector2 SquareVector(this float value)
      => new(value, value);

    /// <summary>
    /// Get a vector2 with all values set to the provided one
    /// </summary>
    public static Vector2Int SquareVector(this int value)
      => new(value, value);

    /// <summary>
    /// add each element of a vector2
    /// </summary>
    public static Vector2Int Plus(this Vector2Int vector2, Vector2Int other)
      => new(vector2.x + other.x, vector2.y + other.y);

    /// <summary>
    /// add each element of a vector3
    /// </summary>
    public static Vector3Int Plus(this Vector3Int vector3, Vector3Int other)
      => new(vector3.x + other.x, vector3.y + other.y, vector3.z + other.z);

    /// <summary>
    /// Multiply each element of a vector2
    /// </summary>
    public static Vector2Int Times(this Vector2Int vector2, Vector2Int other)
      => new(vector2.x * other.x, vector2.y * other.y);

    /// <summary>
    /// Multiply each element of a vector2
    /// </summary>
    public static Vector3 Times(this Vector3 vector3, Vector3 other)
      => new(vector3.x * other.x, vector3.y * other.y, vector3.z * other.z);

    /// <summary>
    /// Divide each element of a vector2
    /// </summary>
    public static Vector3 DividedBy(this Vector3 vector3, Vector3 other)
      => new(vector3.x / other.x, vector3.y / other.y, vector3.z / other.z);

    /// <summary>
    /// Divide each element of a vector2
    /// </summary>
    public static Vector2Int DividedBy(this Vector2Int vector2, Vector2Int other)
      => new(vector2.x / other.x, vector2.y / other.y);

    /// <summary>
    /// Divide each element of a vector2
    /// </summary>
    public static Vector2 DividedBy(this Vector2 vector2, Vector2 other)
      => new(vector2.x / other.x, vector2.y / other.y);

    /// <summary>
    /// Do something with every coordinate from start until the end. (last is inclusive)
    /// </summary>
    public static void Until(this Vector3Int start, Vector3Int last, Action<Vector3Int> @do) {
      start.Until(last, new Vector3Int(
        start.x > last.x
          ? -1
          : 1,
        start.y > last.y
          ? -1
          : 1,
        start.z > last.z
          ? -1
          : 1
      ),
      @do,
      new Func<int, int, bool>[] {
        start.x > last.x
          ? _reverseCompare
          : null,
        start.y > last.y
          ? _reverseCompare
          : null,
        start.z > last.z
          ? _reverseCompare
          : null
      });
    }

    /// <summary>
    /// Do something with every coordinate from start until the end. (last is inclusive)
    /// </summary>
    public static void Until(this Vector3Int start, Vector3Int last, Vector3Int step, Action<Vector3Int> @do, Func<int, int, bool>[] comparitors = null) {
      if(step.x == 0 || step.y == 0 || step.z == 0)
        throw new ArgumentException($"step vector3 cannot contain any fields with a value of 0");

      Func<int, int, bool> xCompare = comparitors?.ElementAtOrDefault(0) ?? ((currX, endX) => currX <= endX);
      Func<int, int, bool> yCompare = comparitors?.ElementAtOrDefault(1) ?? ((currY, endY) => currY <= endY);
      Func<int, int, bool> zCompare = comparitors?.ElementAtOrDefault(2) ?? ((currZ, endZ) => currZ <= endZ);

      for(int x = start.x; xCompare(x, last.x); x += step.x) {
        for(int y = start.y; yCompare(y, last.y); y += step.y) {
          for(int z = start.z; zCompare(z, last.z); z += step.z) {
            @do(new Vector3Int(x, y, z));
          }
        }
      }
    }

    /// <summary>
    /// Do something with every coordinate from start until the end. (last is inclusive)
    /// </summary>
    public static void Until(this Vector2Int start, Vector2Int last, Vector2Int step, Action<Vector2Int> @do, Func<int, int, bool>[] comparitors = null) {
      if(step.x == 0 || step.y == 0)
        throw new ArgumentException($"step vector3 cannot contain any fields with a value of 0");

      Func<int, int, bool> xCompare = comparitors?.ElementAtOrDefault(0) ?? ((currX, endX) => currX <= endX);
      Func<int, int, bool> yCompare = comparitors?.ElementAtOrDefault(1) ?? ((currY, endY) => currY <= endY);

      for(int x = start.x; xCompare(x, last.x); x += step.x) {
        for(int y = start.y; yCompare(y, last.y); y += step.y) {
          @do(new Vector2Int(x, y));
        }
      }
    }

    /// <summary>
    /// Do something with every coordinate from start until the end. (last is inclusive)
    /// </summary>
    public static void Until(this Vector2Int start, Vector2Int last, Action<Vector2Int> @do) {
      start.Until(last, new Vector2Int(
        start.x > last.x
        ? -1
        : 1,
        start.y > last.y
        ? -1
        : 1
      ), @do,
      new Func<int, int, bool>[] {
        start.x > last.x
          ? _reverseCompare
          : null,
        start.y > last.y
          ? _reverseCompare
          : null
      });
    }

    /// <summary>
    /// Do something with every coordinate from start until the end. (last is inclusive)
    /// </summary>
    public static IEnumerable<TResult> SelectUntil<TResult>(this Vector2Int start, Vector2Int last, Vector2Int step, Func<Vector2Int, TResult> select, Func<int, int, bool>[] comparitors = null) {
      if(step.x == 0 || step.y == 0)
        throw new ArgumentException($"step vector3 cannot contain any fields with a value of 0");

      Func<int, int, bool> xCompare = comparitors?.ElementAtOrDefault(0) ?? ((currX, endX) => currX <= endX);
      Func<int, int, bool> yCompare = comparitors?.ElementAtOrDefault(1) ?? ((currY, endY) => currY <= endY);

      for(int x = start.x; xCompare(x, last.x); x += step.x) {
        for(int y = start.y; yCompare(y, last.y); y += step.y) {
          yield return select(new Vector2Int(x, y));
        }
      }
    }

    /// <summary>
    /// Do something with every coordinate from start until the end. (last is inclusive)
    /// </summary>
    public static IEnumerable<Vector2Int> SelectUntil(this Vector2Int start, Vector2Int last) 
      => start.SelectUntil(last, new Vector2Int(
          start.x > last.x
          ? -1
          : 1,
          start.y > last.y
          ? -1
          : 1
        ),
        new Func<int, int, bool>[] {
          start.x > last.x
            ? _reverseCompare
            : null,
          start.y > last.y
            ? _reverseCompare
            : null
      });

    /// <summary>
    /// Do something with every coordinate from start until the end. (last is inclusive)
    /// </summary>
    public static IEnumerable<Vector2Int> SelectUntil(this Vector2Int start, Vector2Int last, Vector2Int step, Func<int, int, bool>[] comparitors = null) {
      if(step.x == 0 || step.y == 0)
        throw new ArgumentException($"step vector3 cannot contain any fields with a value of 0");

      Func<int, int, bool> xCompare = comparitors?.ElementAtOrDefault(0) ?? ((currX, endX) => currX <= endX);
      Func<int, int, bool> yCompare = comparitors?.ElementAtOrDefault(1) ?? ((currY, endY) => currY <= endY);

      for(int x = start.x; xCompare(x, last.x); x += step.x) {
        for(int y = start.y; yCompare(y, last.y); y += step.y) {
          yield return new Vector2Int(x, y);
        }
      }
    }

    /// <summary>
    /// Do something with every coordinate from start until the end. (last is inclusive)
    /// </summary>
    public static IEnumerable<TResult> SelectUntil<TResult>(this Vector2Int start, Vector2Int last, Func<Vector2Int, TResult> select) 
      => start.SelectUntil(last, new Vector2Int(
          start.x > last.x
          ? -1
          : 1,
          start.y > last.y
          ? -1
          : 1
        ),
        select,
        new Func<int, int, bool>[] {
          start.x > last.x
            ? _reverseCompare
            : null,
          start.y > last.y
            ? _reverseCompare
            : null
      });
  }
}