using Overworld.Ux.Simple;
using UnityEngine;
using static Overworld.Ux.Simple.UxDataField;

namespace Overworld.Controllers.SimpleUx {

  /// <summary>
  /// An in game controller for a simple ux field.
  /// </summary>
  public interface ISimpleUxFieldController {

    /// <summary>
    /// The display type of this field
    /// </summary>
    public DisplayType DisplayType {
      get;
    }

    /// <summary>
    /// The parent view controller
    /// </summary>
    SimpleUxViewController View {
      get;
    }

    /// <summary>
    /// The field data this is controlling.
    /// </summary>
    UxDataField FieldData {
      get;
    }

    /// <summary>
    /// The gamobject that represents the field's title.
    /// Used for tooltip hover.
    /// </summary>
    GameObject FieldTitle {
      get;
    }

    /// <summary>
    /// Used to initialize this contorller for a view and set of field data
    /// </summary>
    void IntializeFor(UxDataField dataField);

    /// <summary>
    /// Get the current in game value of this simple Ux Field
    /// </summary>
    object GetCurrentValue();
  }
}
