using Overworld.Ux.Simple;

namespace Overworld.Controllers.SimpleUx {

  /// <summary>
  /// Controls a simple ux element
  /// </summary>
  public interface ISimpleUxElementController {

    /// <summary>
    /// The controller for the view this is a part of
    /// </summary>
    SimpleUxViewController View {
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