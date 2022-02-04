using Overworld.Ux.Simple;
using System;
using System.Collections.Generic;
using UnityEngine;

namespace Overworld.Controllers.SimpleUx {

  /// <summary>
  /// An in game controller for a simple ux field.
  /// </summary>
  public interface IFieldController : IColumnElementController {

    /// <summary>
    /// The display type of this field
    /// </summary>
    public DataField.DisplayType DisplayType {
      get;
    }

    /// <summary>
    /// This can be overriden to force validation of the DataField to a handful of types, or one type ,etc.
    /// </summary>
    HashSet<Type> ValidFieldDataTypes {
      get;
    }

    /// <summary>
    /// The field data this is controlling.
    /// </summary>
    DataField FieldData {
      get;
    }

    /// <summary>
    /// The gamobject that represents the field's title.
    /// Used for tooltip hover.
    /// </summary>
    TitleController Title {
      get;
    }

    /// <summary>
    /// Get the current in game value of this simple Ux Field
    /// </summary>
    object GetCurrentValue();
  }
}
