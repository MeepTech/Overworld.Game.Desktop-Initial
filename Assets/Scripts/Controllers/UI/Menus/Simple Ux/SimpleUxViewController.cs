using Meep.Tech.Data;
using Meep.Tech.Data.Utility;
using Overworld.Utilities;
using Overworld.Ux.Simple;
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

namespace Overworld.Controllers.SimpleUx {

  /// <summary>
  /// A controller for an Simple Ux View.
  /// This should be attached to the sizing contaner for the view.
  /// </summary>
  public class SimpleUxViewController : MonoBehaviour {

    /// <summary>
    /// The rect transform for the view window.
    /// </summary>
    public RectTransform RectTransform
      => _rectTransform;

    /// <summary>
    /// The controller prefabs for each type of Simple Ux Field.
    /// </summary>
    public static IReadOnlyDictionary<Ux.Simple.DataField.DisplayType, SimpleUxFieldController> FieldControllerPrefabs
      => _prefabs;
    static Dictionary<Ux.Simple.DataField.DisplayType, SimpleUxFieldController> _prefabs;

    #region Unity Inspector Set Fields

    [Header("Element Prefabs")]
    [SerializeField]
    [UnityEngine.Tooltip("The prefab for a pannel")]
    SimpleUxPannelController _pannelController;

    [SerializeField]
    [UnityEngine.Tooltip("The prefab for a pannel tab")]
    SimpleUxPannelTabController _pannelTabController;

    [SerializeField]
    [UnityEngine.Tooltip("A prefabs with a different Controller for each type of Simple UX Field Display Type")]
    SimpleUxFieldController[] _fieldControllers;

    [Header("View Parts")]
    [SerializeField] RectTransform _rectTransform;
    [SerializeField]
    FlexibleResizeHandler _resizeHandler;
    [SerializeField]
    TMPro.TextMeshProUGUI _mainViewTitleText;
    [SerializeField]
    RectTransform _pannelTabsArea;
    [SerializeField]
    RectTransform _pannelsArea;
    [SerializeField]
    UnityEngine.UI.Button _closeButton;

    #endregion

    /// <summary>
    /// The currently active pannel's index
    /// </summary>
    [SerializeField, ReadOnly] // for display in inspector for debugging
    internal string _activePannelKey
      = null;

    /// <summary>
    /// If this view should show it's close button.
    /// </summary>
    public bool ShouldShowCloseButton {
      get => _shouldShowCloseButton;
      set {
        // to true
        if(_shouldShowCloseButton = value) {
          _closeButton.gameObject.SetActive(true);
          _closeButton.onClick.AddListener(() => Close());
        } // to false
        else {
          _closeButton.gameObject.SetActive(false);
          _closeButton.onClick.RemoveAllListeners();
        }
      }
    } bool _shouldShowCloseButton;

  /// <summary>
  /// If this view's data should be kept after it's closed.
  /// If this is false, this view will reset it's data on close.
  /// </summary>
  public bool IsPersistent
      = true;

    /// <summary>
    /// If this view is currently open and displayed to the user
    /// </summary>
    public bool IsOpen
      => gameObject.activeSelf;

    /// <summary>
    /// The view this is controlling
    /// </summary>
    public View Data {
      get;
      private set;
    }

    /// <summary>
    /// Used to revert changes.
    /// </summary>
    public HashSet<string> _changedFields
      = new();

    /// <summary>
    /// The unique id of this view window
    /// </summary>
    public string Id {
      get;
    } = new Guid().ToString();

    internal OrderedDictionary<string, SimpleUxPannelController> _pannels;
    internal OrderedDictionary<string, SimpleUxPannelTabController> _pannelTabs;
    internal Dictionary<string, SimpleUxFieldController> _fields;
    float _tallestPannelHeight;
    int _mostColumnsInAPannel;
    bool _dimensionsAreDirty;
    bool _waitedWhileDirty;
    Vector2 _minDimensions;
    internal int _waitingOnDirtyChildren;
    bool _dirtyPannel;
    int _dirtyPannelWaited;
    Stack<HistoricalAction> _history
      = new();

    /// <summary>
    /// Setup
    /// </summary>
    void Awake() {
      _minDimensions = _resizeHandler.MinimumDimmensions;
      _prefabs ??= _fieldControllers.ToDictionary(controller => controller.DisplayType);
    }

    void Update() {
      /// if we have dirty dimensions from initialization, and have waited a frame for the recttransforms to init:
      if(_dimensionsAreDirty && _waitedWhileDirty && _waitingOnDirtyChildren <= 0) {
        // find the tallest pannel height we want (limited to 5 fields)
        // this also resizes the columns and pannels themselves
        _tallestPannelHeight = Math.Max(
            _calculateTallestPannelHeight(),
            _pannelTabs.Count() * _pannelTabController.GetComponent<RectTransform>().sizeDelta.y
          );

        // deactivate inactie pannels
        _pannels.Values.ForEach(pannel => {
          if(pannel.Pannel.Key.Key != _activePannelKey) {
            pannel.gameObject.SetActive(true);
            pannel.gameObject.SetActive(false);
          }
        });

        //and resize the view and set the min dimensions to the largest pannel value within reason we found before.
        _resizeHandler.MinimumDimmensions = new Vector2(
          // this uses the most columns in a pannel to determine how wide the window should be, and how narrow it can get
          _minDimensions.x + (_pannels.Count > 1 ? 103 : 53) + (180 * (_mostColumnsInAPannel - 1)),
          _minDimensions.y + _tallestPannelHeight
        );
        RectTransform.sizeDelta
          = _resizeHandler.MinimumDimmensions;

        _dimensionsAreDirty = false;
        _waitedWhileDirty = false;
        _dirtyPannel = true;
      } // we wait one frame while dimensions are dirty to make sure everything intis properly.
      // TODO: is this still needed using the values in the sizeDelta since they are preset?
      else if(_dimensionsAreDirty) {
        _waitedWhileDirty = true;
      }

      /// when pannel is changed, sometimes it needs to flash the items to update their position correctly.
      if(_dirtyPannel) {
        _dirtyPannelWaited++;
      }
      if(_dirtyPannel && _dirtyPannelWaited > 1) {
        _pannels[_activePannelKey].gameObject.SetActive(false);
      }

      if(_dirtyPannel && _dirtyPannelWaited > 2) {
        _pannels[_activePannelKey].gameObject.SetActive(true);
        _dirtyPannel = false;
        _dirtyPannelWaited = 0;
      }
    }

    /// <summary>
    /// Initialize this view for the view data.
    /// </summary>
    public void InitializeFor(View model) {
      /// init variables
      Data = model;
      _pannels = new();
      _pannelTabs = new();
      _fields = new();

      /// main title text and tootltip
      _mainViewTitleText.text = "+" + model.MainTitle.Text;
      if(!string.IsNullOrWhiteSpace(model.MainTitle.Tooltip)) {
        Tooltip tooltip = _mainViewTitleText.gameObject.AddComponent<Tooltip>();
        tooltip.TooltipStylePrefab = SimpleUxGlobalManager.DefaultTooltipPrefab;
        tooltip.Text = model.MainTitle.Tooltip;
      }

      /// build the sub controllers and set up pannels, columns, etc
      foreach((Pannel.Tab tabData, Pannel pannelData) in model) {
        SimpleUxPannelController pannel = _addPannel(
          tabData,
          pannelData
        );
        foreach(Column columnData in pannelData) {
          SimpleUxColumnController column = pannel._addColumn(columnData);
          foreach(IUxViewElement element in columnData) {
            if(element is Row rowData) {
              throw new NotImplementedException();
            } else if(element is DataField fieldData) {
              SimpleUxFieldController field = column._addField(fieldData);
            } else if(element is Title inColumnHeader) {
              column._addInColumnHeader(inColumnHeader);
            } else
              throw new NotSupportedException($"Unknown Simple Ux Element: {element.GetType().FullName}");
          }
        }
      }

      /// update all fields initally
      _updateAllFieldsForChangesIn(null);
      /// set to update dimensions and finish initialization on the next update
      _dimensionsAreDirty = true;
    }

    /// <summary>
    /// Revert the changes in the forum to their defaults.
    /// </summary>
    public void RevertAllChanges() {
      foreach(string changedfieldKey in _changedFields) {
        _fields[changedfieldKey].FieldData.ResetValueToDefault();
        _fields[changedfieldKey]._refreshCurrentValue();
      }
      _changedFields.Clear();
    }

    /// <summary>
    /// Close this view
    /// </summary>
    public void Close() {
      gameObject.SetActive(false);
    }

    /// <summary>
    /// Close this view
    /// </summary>
    public void Open() {
      gameObject.SetActive(true);
    }

    /// <summary>
    /// Move a window to the screen location, centering on the window's center
    /// </summary>
    /// <param name="centerOnScreenPixelLocation">pixel location on the game area screen where the window should be centered. Bottom Left is 0,0 Top right is Max,Max</param>
    public void MoveToScreenPixelLocation(Vector2 centerOnScreenPixelLocation) {
      RectTransform.localPosition =
        centerOnScreenPixelLocation
          - new Vector2(Screen.width, Screen.height) / 2
          - new Vector2(0, RectTransform.rect.height / 2);
    }

    /// <summary>
    /// Move a window to the screen location, centering on the window's center
    /// </summary>
    /// <param name="centerOnScreenPercentLocation">two floats between 0 and 1 representing the location on the game area screen where the window should be centered. Bottom Left is 0,0, Top Right is 1,1</param>
    public void MoveToScreenPercent(Vector2 centerOnScreenPercentLocation)
      => MoveToScreenPixelLocation(new Vector2(centerOnScreenPercentLocation.x * Screen.width, centerOnScreenPercentLocation.y * Screen.width));

    /// <summary>
    /// Move a window to the screen location, centering on it's title
    /// </summary>
    /// <param name="centerOnScreenPixelLocation">pixel location on the game area screen where the window should be centered. Bottom Left is 0,0</param>
    public void MoveTitleToScreenPercent(Vector2 titletoScreenPercentLocation)
      => MoveTitleToScreenPixelLocation(new Vector2(titletoScreenPercentLocation.x * Screen.width, titletoScreenPercentLocation.y * Screen.width));

    /// <summary>
    /// Move a window to the screen location, centering on it's title
    /// </summary>
    /// <param name="centerOnScreenPixelLocation">pixel location on the game area screen where the window should be centered. Bottom Left is 0,0</param>
    public void MoveTitleToScreenPixelLocation(Vector2 titletoScreenPercentLocation) {
      RectTransform.localPosition =
        titletoScreenPercentLocation
          - new Vector2(Screen.width, Screen.height) / 2
          - new Vector2(0, -15);
    }

    /// <summary>
    /// Called when an element of the view is updated.
    /// </summary>
    /// <param name="originalData">The element before it was edited.</param>
    internal void _onUpdated(IUxViewElement originalData) {
      if(originalData is DataField Field) {
        _history.Push(new HistoricalAction(Data.GetField(Field.DataKey), Field));
        _changedFields.Add(Field.DataKey);
        _updateAllFieldsForChangesIn(Field);
      }
    }

    /// <summary>
    /// Set the active tab
    /// </summary>
    internal void _setActiveTab(Pannel.Tab tab) {
      _pannels[_activePannelKey].gameObject.SetActive(false);
      _activePannelKey = tab.Key;
      _pannels[_activePannelKey].gameObject.SetActive(true);
      _dirtyPannel = true;
    }

    /// <summary>
    /// Add a pannel to this Ux
    /// </summary>
    SimpleUxPannelController _addPannel(
      Pannel.Tab tabData,
      Pannel pannelData
    ) {
      SimpleUxPannelTabController simpleUxPannelTabController = Instantiate(_pannelTabController, _pannelTabsArea);
      SimpleUxPannelController simpleUxPannelController = Instantiate(_pannelController, _pannelsArea);
      _pannels.Add(tabData.Key, simpleUxPannelController);
      _pannelTabs.Add(tabData.Key, simpleUxPannelTabController);

      simpleUxPannelTabController.View = this;
      simpleUxPannelTabController._intializeFor(tabData);
      simpleUxPannelController.View = this;
      simpleUxPannelController._intializeFor(pannelData);

      /// if we don't have one, this becomes the active pannel
      if(string.IsNullOrWhiteSpace(_activePannelKey)) {
        _activePannelKey = tabData.Key;
      }

      // activate the tab background if we have more than one tab
      if(_pannels.Count > 1) {
        _pannelTabsArea.gameObject.SetActive(true);
      }

      // Used for height restrictions
      _mostColumnsInAPannel = Math.Max(pannelData.Count(), _mostColumnsInAPannel);

      return simpleUxPannelController;
    }

    void _updateAllFieldsForChangesIn(DataField Field) {
      _fields.Values.ForEach(field => field._onOtherFieldUpdated(Data, Field));
    }

    /// <summary>
    /// Gets the tallest pannel height within a limit of 5 fields.
    /// Also sets pannel height to their own max, and column height to their own max.
    /// </summary>
    float _calculateTallestPannelHeight() {
      return _pannels.Values.Select(
        pannel => {
          float maxColumnHeight = 0;
          float maxPannelHeight = pannel._columns.Select(column => {
            return column._rows.Append(column.Title);
          }).Max(rows => {
            var column = (rows.First() as ISimpleUxColumnChildElementController).Column;
            //column?._elementsArea.ForceUpdateRectTransforms();
            float columnHeight = column?._elementsArea.rect.height ?? 0;

            maxColumnHeight = maxColumnHeight < columnHeight ? columnHeight : maxColumnHeight;
            return Math.Min(350, columnHeight);
          });

          pannel._columnArea.sizeDelta = pannel._columnArea.sizeDelta.ReplaceY(maxColumnHeight);
          return maxPannelHeight + 23;
        }).Max();
    }

    /// <summary>
    /// A historical action for recording changes to the view.
    /// </summary>
    public struct HistoricalAction {
      public readonly string FieldDataKey; 
      public readonly object UpdatedValue; 
      public readonly object OriginalValue; 

      internal HistoricalAction(DataField updatedField, DataField originalField) {
        FieldDataKey = updatedField.DataKey;
        UpdatedValue = updatedField.Value;
        OriginalValue = originalField.Value;
      }
    }
  }
}