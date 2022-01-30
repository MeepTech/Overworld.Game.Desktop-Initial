using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Overworld.Controllers.SimpleUx {

  /// <summary>
  /// Controlls all the views for a player in a world.
  /// </summary>
  [DefaultExecutionOrder(-900)]
  public class SimpleUxViewsController : MonoBehaviour {

    /// <summary>
    /// The views, indexed by unique key
    /// </summary>
    public IReadOnlyDictionary<string, SimpleUxViewController> Views
      => _views; static Dictionary<string, SimpleUxViewController> _views
        = new(); 
    Dictionary<string, ICollection<SimpleUxViewController>> _viewsByTitle
        = new();

    /// <summary>
    /// The current set of views being controller and managed.
    /// </summary>
    public static SimpleUxViewsController Current {
      get;
      private set;
    }

    void Awake() {
      Current = this;
    }

    /// <summary>
    /// Check if the given view is open
    /// </summary>
    public bool ViewIsOpen(string viewId)
      => Views[viewId].IsOpen;

    /// <summary>
    /// Try to find all the views with the given chunk of text in their main title.
    /// </summary>
    /// <param name="titleToFind">Not case sensitive, Can find by substring.</param>
    public IEnumerable<SimpleUxViewController> FindViewsByMainTitle(string titleTextToFind)
      => _viewsByTitle.Keys.Where(title => title.ToUpper().Contains(titleTextToFind))
          .SelectMany(key => _viewsByTitle[key]);

    /// <summary>
    /// Check if the given view is open
    /// </summary>
    public bool ViewIsOpen(SimpleUxViewController view)
      => Views[view.Id].IsOpen;

    internal void _addView(SimpleUxViewController view) {
      _views[view.Id] = view;
      _viewsByTitle.AddToValueCollection(view.Data.MainTitle.Text, view);
    }
  }
}
