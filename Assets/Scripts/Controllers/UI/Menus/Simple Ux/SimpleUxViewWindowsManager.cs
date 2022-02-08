using Meep.Tech.Collections.Generic;
using Simple.Ux.Data;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Overworld.Controllers.SimpleUx {

  /// <summary>
  /// Controlls all the views for a player in a world.
  /// </summary>
  [DefaultExecutionOrder(-900)]
  public class SimpleUxViewWindowsManager : MonoBehaviour {

    [SerializeField]
    Canvas _inGameViewsCanvas;
    [SerializeField]
    Canvas _worldEditorViewsCanvas;

    /// <summary>
    /// All of the tracked windows in this manager
    /// </summary>
    public IEnumerable<ViewController> AllWindows
      => _windowsById.Values;

    /// <summary>
    /// The views with perminant data (maintained on close), indexed by view's key
    /// </summary>
    public IReadOnlyDictionary<string, ViewController> PersistentWindows
      => _persistentWindows; Dictionary<string, ViewController> _persistentWindows
        = new();

    /// <summary>
    /// tracked windows with temp data (cleared on close), indexed by their view keys 
    /// </summary>
    public IReadOnlyDictionary<string, IEnumerable<ViewController>> TemporatryWindows
      => _temporatryWindows.ToDictionary(
      windowType => windowType.Key,
      window => window.Value.Values.AsEnumerable()
    ); Dictionary<string, Dictionary<string, ViewController>> _temporatryWindows
          = new(); 

    Dictionary<string, ICollection<string>> _windowsByTitle
        = new();

    Dictionary<string, ViewController> _windowsById
        = new();

    HashSet<string> _editorModeWindows
      = new();

    /// <summary>
    /// The current set of views being controller and managed.
    /// </summary>
    public static SimpleUxViewWindowsManager Current {
      get;
      private set;
    }

    void Awake() {
      Current = this;
    }

    #region Data Access

    /// <summary>
    /// Get a window by it's id.
    /// </summary>
    public ViewController GetWindow(string viewWindowId)
      => _windowsById[viewWindowId];

    /// <summary>
    /// Get a window by it's id.
    /// </summary>
    public ViewController GetPersistentViewWindow(string viewId)
      => _persistentWindows[viewId];

    /// <summary>
    /// Get a window by it's id.
    /// </summary>
    public ViewController TryToGetWindow(string viewWindowId)
      => _windowsById.TryGetValue(viewWindowId, out var found)
        ? found
        : null;

    /// <summary>
    /// Get a window by it's id.
    /// </summary>
    public bool TryToGetWindow(string viewWindowId, out ViewController window)
      => _windowsById.TryGetValue(viewWindowId, out window);

    /// <summary>
    /// Get a window by it's id.
    /// </summary>
    public ViewController TryToGetPersistentViewWindow(string viewId)
      => _persistentWindows.TryGetValue(viewId, out var found)
        ? found
        : null;

    /// <summary>
    /// Get a window by it's id.
    /// </summary>
    public bool TryToGetPersistentViewWindow(string viewId, out ViewController window)
      => _persistentWindows.TryGetValue(viewId, out window);


    /// <summary>
    /// Try to find all the views with the given chunk of text in their main title.
    /// </summary>
    /// <param name="titleToFind">Not case sensitive, Can find by substring.</param>
    public IEnumerable<ViewController> FindViewsByMainTitle(string titleTextToFind)
      => _windowsByTitle.Keys.Where(title => title.ToUpper().Contains(titleTextToFind))
          .SelectMany(key => _windowsByTitle[key].Select(key => TryToGetWindow(key)).Where(view => view is not null));

    /// <summary>
    /// Check if the given view is open
    /// </summary>
    public bool ViewIsOpen(string viewId)
      => ViewIsOpen(viewId, out _);

    /// <summary>
    /// Check if the given view is open
    /// </summary>
    public bool ViewIsOpen(string viewId, out bool isPersistent) {
      isPersistent = false;
      if(PersistentWindows.TryGetValue(viewId, out var view)) {
        isPersistent = true;
        return view.IsOpen;
      }
      if(TemporatryWindows.TryGetValue(viewId, out var tempViews)) {
        return tempViews.Any(view => view.IsOpen);
      }

      return false;
    }

    /// <summary>
    /// Check if the given tracked view is open
    /// </summary>
    public bool WindowIsOpen(ViewController viewWindow)
      => _windowsById[viewWindow.Id].IsOpen;

    /// <summary>
    /// Check if the given tracked view is open
    /// </summary>
    public bool WindowIsOpen(string viewWindowId)
      => _windowsById[viewWindowId].IsOpen;

    /// <summary>
    /// Check if the given view is tracked by the view manager
    /// </summary>
    public bool WindowIsTracked(ViewController viewWindow)
      => _windowsById.ContainsKey(viewWindow.Id);

    /// <summary>
    /// Check if the given view is tracked by the view manager
    /// </summary>
    public bool WindowIsTracked(string viewWindowId, out ViewController window)
      => _windowsById.TryGetValue(viewWindowId, out window);

    /// <summary>
    /// Check if the given view is tracked by the view manager
    /// </summary>
    public bool WindowIsTracked(string viewWindowId)
      => WindowIsTracked(viewWindowId, out _);

    #endregion

    #region View and Window Manipulation

    /// <summary>
    /// Display a view to the user as a window
    /// </summary>
    public ViewController OpenView(View view, bool isPersistent = true)
      => _displayView(view, isPersistent);

    /// <summary>
    /// Display an already tracked persistent view to the user
    /// </summary>
    public void OpenPersistentView(string viewId)
      => _persistentWindows[viewId].Open();

    /// <summary>
    /// Display a view to the user
    /// </summary>
    public void OpenWindow(ViewController view)
      => _windowsById[view.Id].Open();

    /// <summary>
    /// Display a view to the user
    /// </summary>
    public void OpenWindow(string viewWindowId)
      => _windowsById[viewWindowId].Open();

    /// <summary>
    /// Display a view to the user
    /// </summary>
    public ViewController OpenViewInWorldEditorMode(View view, bool isPersistent = true)
      => _displayView(view, isPersistent, true);

    /// <summary>
    /// Move a window to the screen location, centering on the window's center
    /// </summary>
    /// <param name="viewWindowId"></param>
    /// <param name="centerOnScreenPixelLocation">pixel location on the game area screen where the window should be centered. Bottom Left is 0,0 Top right is Max,Max</param>
    public void MoveWindowToScreenPixelLocation(string viewWindowId, Vector2 centerOnScreenPixelLocation)
      => GetWindow(viewWindowId).MoveTitleToScreenPixelLocation(centerOnScreenPixelLocation);

    /// <summary>
    /// Move a window to the screen location, centering on the window's center
    /// </summary>
    /// <param name="viewWindowId"></param>
    /// <param name="centerOnScreenPercentLocation">two floats between 0 and 1 representing the location on the game area screen where the window should be centered. Bottom Left is 0,0, Top Right is 1,1</param>
    public void MoveWindowToScreenPercent(string viewWindowId, Vector2 centerOnScreenPercentLocation)
      => GetWindow(viewWindowId).MoveToScreenPercent(centerOnScreenPercentLocation);

    /// <summary>
    /// Move a window to the screen location, centering on it's title
    /// </summary>
    /// <param name="viewWindowId"></param>
    /// <param name="centerOnScreenPixelLocation">pixel location on the game area screen where the window should be centered. Bottom Left is 0,0</param>
    public void MoveWindowTitleToScreenPercent(string viewWindowId, Vector2 titletoScreenPercentLocation)
      => GetWindow(viewWindowId).MoveTitleToScreenPercent(titletoScreenPercentLocation);

    /// <summary>
    /// Move a window to the screen location, centering on it's title
    /// </summary>
    /// <param name="viewWindowId"></param>
    /// <param name="centerOnScreenPixelLocation">pixel location on the game area screen where the window should be centered. Bottom Left is 0,0</param>
    public void MoveWindowTitleToScreenPixelLocation(string viewWindowId, Vector2 titletoScreenPercentLocation)
      => GetWindow(viewWindowId).MoveTitleToScreenPixelLocation(titletoScreenPercentLocation);

    /// <summary>
    /// Hide a view from the user
    /// </summary>
    public void ClosePersistentView(string viewId)
      => PersistentWindows[viewId].Close();

    /// <summary>
    /// Try to Hide a view from the user
    /// </summary>
    public bool TryToClosePersistentView(string viewId) {
      if(PersistentWindows.TryGetValue(viewId, out var view)) {
        view.Close();
        return true;
      }

      return false;
    }

    /// <summary>
    /// Try to Hide a view from the user
    /// </summary>
    public bool TryToCloseWindow(string viewWindowId) {
      if(_windowsById.TryGetValue(viewWindowId, out var window)) {
        window.Close();
        return true;
      }

      return false;
    }

    /// <summary>
    /// Try to Hide a view from the user
    /// </summary>
    public bool TryToCloseWindow(ViewController view)
      => TryToCloseWindow(view.Id);

    /// <summary>
    /// Hide a view from the user
    /// </summary>
    public void CloseWindow(string viewWindowId)
      => _windowsById[viewWindowId].Close();
    
    /// <summary>
    /// Hide a view from the user
    /// </summary>
    public void CloseWindow(ViewController view)
      => _windowsById[view.Id].Close();

    /// <summary>
    /// Close all open views
    /// </summary>
    public void CloseAllWindows(bool editorViewsAsWell = true) {
      if(!editorViewsAsWell) {
        _windowsById.Keys.Except(_editorModeWindows).ForEach(key => _windowsById[key].Close());
      } else
        _windowsById.Values.ForEach(window => window.Close());
    }

    /// <summary>
    /// Close all open views
    /// </summary>
    public void CloseAllEditorModeWindows() {
      _editorModeWindows.ForEach(key => _windowsById[key].Close());
    }

    /// <summary>
    /// Display a view to the user
    /// </summary>
    public ViewController _displayView(View view, bool dataIsPersistent = true, bool inEditor = false) {
      // do we have a peristent window?
      if(dataIsPersistent && PersistentWindows.TryGetValue(view.Id, out ViewController found)) {
        found.Open();
        return found;
      } // what about a free temp window? 
      else if(!dataIsPersistent && _temporatryWindows.TryGetValue(view.Id, out var tempWindows)) {
        ViewController foundTemp = tempWindows.FirstOrDefault(
          temp => temp.Value.IsOpen
        ).Value;
        if(foundTemp is not null) {
          foundTemp.RevertAllChanges();
          foundTemp.transform.parent = inEditor
            ? _worldEditorViewsCanvas.transform
            : _inGameViewsCanvas.transform;
          foundTemp.Open();

          return foundTemp;
        }
      }

      // if we didn't find an existing window, make a new one:
      ViewController newView = Instantiate(
        SimpleUxGlobalManager.DefaultViewPrefab,
        inEditor
          ? _worldEditorViewsCanvas.transform
          : _inGameViewsCanvas.transform
      );
      Debug.Log($"Created New {(dataIsPersistent ? "" : "Temp ")}Window: {newView.Id}");
      newView.MoveToScreenPercent(new Vector2(0.5f, 0.5f));
      newView.IsPersistent = dataIsPersistent;
      newView.InitializeFor(view);
      _trackWindow(newView, dataIsPersistent, inEditor);

      newView.Open();
      return newView;
    }

    #endregion

    /// <summary>
    /// Remove a window from this manager's tracking.
    /// </summary>
    public void StopTrackingWindow(string viewWindowId)
      => _stopTrackingWindow(viewWindowId);

    /// <summary>
    /// Remove a window from this manager's tracking.
    /// </summary>
    public void StopTrackingWindow(ViewController window)
      => _stopTrackingWindow(window.Id);

    /// <summary>
    /// Remove a window from this manager's tracking.
    /// </summary>
    public void StopTrackingPersistentViewWindow(View view)
      => StopTrackingPersistentViewWindow(view.Id);
    
    /// <summary>
    /// Remove a window from this manager's tracking.
    /// </summary>
    public void StopTrackingPersistentViewWindow(string viewId)
      => StopTrackingWindow(_persistentWindows[viewId]);

    /// <summary>
    /// Close all open views
    /// </summary>
    public void StopTrackingAllWindows(bool editorModeWindowsAsWell = false) {
      if(!editorModeWindowsAsWell) {
        _windowsById.Keys.Except(_editorModeWindows).ForEach(key => _stopTrackingWindow(key));
      } else
        _windowsById.Keys.ForEach(key => _stopTrackingWindow(key));
    }

    /// <summary>
    /// Close all open views
    /// </summary>
    public void StopTrackingAllEditorModeWindows() {
      _editorModeWindows.ForEach(key => _stopTrackingWindow(key));
    }

    void _trackWindow(ViewController window, bool dataIsPersistent, bool isEditorMode) {
      Debug.Log($"Started Tracking {(dataIsPersistent ? "" : "Temp ")}Window: {window.Id}");
      if(dataIsPersistent) {
        _persistentWindows.Add(window.Data.Id, window);
      } else {
        if(_temporatryWindows.TryGetValue(window.Data.Id, out var exiting)) {
          exiting.Add(window.Id, window);
        } else
          _temporatryWindows.Add(window.Data.Id, new() {
            { window.Id, window }
          });
      }

      if(isEditorMode) {
        _editorModeWindows.Add(window.Id);
      }
      _windowsById.Add(window.Id, window);
      _windowsByTitle.AddToValueCollection(window.Data.MainTitle.Text, window.Id);
    }

    void _stopTrackingWindow(string windowId) {
      var toRemove = TryToGetWindow(windowId);
      if(toRemove is not null) {
        if(toRemove.IsPersistent) {
          _persistentWindows.Remove(windowId);
        } else
          _temporatryWindows.Values.ForEach(values =>values.Remove(windowId));

        if(_windowsByTitle.TryGetValue(toRemove.Data.MainTitle.Text, out var matches)) {
          matches.Remove(windowId);
        }
      } else {
        _persistentWindows.Remove(windowId);
        _temporatryWindows.Values.ForEach(values => values.Remove(windowId));
        _windowsByTitle.Values.ForEach(values => values.Remove(windowId));
      }
      Debug.Log($"No Longer Tracking {((toRemove?.IsPersistent ?? true) ? "" : "Temp ")}Window: {windowId}");

      _windowsById.Remove(windowId);
      _editorModeWindows.Remove(windowId);
    }
  }
}
