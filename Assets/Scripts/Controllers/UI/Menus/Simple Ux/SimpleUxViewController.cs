using Overworld.Ux.Simple;
using System;
using System.Collections.Generic;

namespace Overworld.Controllers.SimpleUx {
  public class SimpleUxViewController {

    /// <summary>
    /// The view this is controlling
    /// </summary>
    public UxView View {
      get;
      private set;
    }

    /// <summary>
    /// Fields
    /// </summary>
    List<SimpleUxFieldController> _fields
      = new();

    public void InitializeFor(UxView view) {
      View = view;
      foreach((UxPannel.Tab tab, UxPannel pannel) in view) {
        foreach(UxColumn column in pannel) {
          foreach(IUxViewElement element in column) {
            if(element is UxRow row) {

            } else {
              
            }
          }
        }
      }

      _fields.ForEach(field => field._updateFieldEnabledState());
    }

    /// <summary>
    /// Called when an element of the view is updated.
    /// </summary>
    /// <param name="originalData">The element before it was edited.</param>
    public void OnUpdated(IUxViewElement originalData) {
      if(originalData is UxDataField uxField) {
        _fields.ForEach(field => field.OnViewUpdated(View, uxField));
      }
    }
  }
}