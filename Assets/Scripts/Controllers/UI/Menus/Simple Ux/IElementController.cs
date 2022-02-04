using Overworld.Ux.Simple;
using UnityEngine;

namespace Overworld.Controllers.SimpleUx {

  /// <summary>
  /// Controls a simple ux element
  /// </summary>
  public interface IColumnElementController : IElementController {

    /// <summary>
    /// The column this is part of
    /// </summary>
    ColumnController Column {
      get;
    }

    /// <summary>
    /// The rectransform.
    /// </summary>
    RectTransform RectTransform {
      get;
    }
  }

  public interface IElementController {

    /// <summary>
    /// The controller for the view this is a part of
    /// </summary>
    ViewController View {
      get;
    }

    /// <summary>
    /// The element the controller represents
    /// </summary>
    IUxViewElement Element {
      get;
    }
  }
}