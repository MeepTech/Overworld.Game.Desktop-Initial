using Overworld.Controllers.SimpleUx;

namespace Overworld.Objects.Editor {

  /// <summary>
  /// An editor object that can be right clicked to bring up an options menu.
  /// </summary>
  public interface IHasAnOpenableSettingsWindow {

    /// <summary>
    /// Get the view's settings window.
    /// </summary>
    ViewController GetSettingsWindow();
  }
}
