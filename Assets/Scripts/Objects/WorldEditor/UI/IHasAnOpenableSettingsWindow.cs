using Overworld.Ux.Simple;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Overworld.Objects.Editor {

  /// <summary>
  /// An editor object that can be right clicked to bring up an options menu.
  /// </summary>
  public interface IHasAnOpenableSettingsWindow {

    /// <summary>
    /// Get the view's settings window.
    /// </summary>
    View GetSettingsWindow();
  }
}
