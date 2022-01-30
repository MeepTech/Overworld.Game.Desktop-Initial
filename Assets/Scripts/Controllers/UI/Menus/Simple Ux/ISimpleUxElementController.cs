using Overworld.Ux.Simple;

namespace Overworld.Controllers.SimpleUx {

  /// <summary>
  /// Controls a simple ux element
  /// </summary>
  public interface ISimpleUxColumnChildElementController : ISimpleUxElementController {

    /// <summary>
    /// The column this is part of
    /// </summary>
    SimpleUxColumnController Column {
      get;
    }

    /// <summary>
    /// The height this takes up in the column
    /// </summary>
    float ItemHeight {
      get;
    }
  }

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