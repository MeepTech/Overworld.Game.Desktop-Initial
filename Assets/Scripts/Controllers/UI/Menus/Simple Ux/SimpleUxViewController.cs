using Meep.Tech.Data;
using Meep.Tech.Data.Utility;
using Overworld.Utilities;
using Overworld.Ux.Simple;
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Overworld.Controllers.SimpleUx {

  /// <summary>
  /// A controller for an Simple Ux View.
  /// This should be attached to the sizing contaner for the view.
  /// </summary>
  public class SimpleUxViewController : MonoBehaviour {

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
    [SerializeField]
    RectTransform _rectTransform;
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
    /// The controller prefabs for each type of Simple Ux Field.
    /// </summary>
    public static IReadOnlyDictionary<Ux.Simple.UxDataField.DisplayType, SimpleUxFieldController> FieldControllerPrefabs
      => _prefabs;
    static Dictionary<Ux.Simple.UxDataField.DisplayType, SimpleUxFieldController> _prefabs;

    /// <summary>
    /// If this view should show it's close button.
    /// </summary>
    public bool ShouldShowCloseButton 
      = true;

    /// <summary>
    /// The view this is controlling
    /// </summary>
    public UxView View {
      get;
      private set;
    }

    /// <summary>
    /// Pannels
    /// </summary>
    internal OrderedDictionary<string, SimpleUxPannelController> _pannels;

    /// <summary>
    /// Pannel controllers
    /// </summary>
    internal OrderedDictionary<string, SimpleUxPannelTabController> _pannelTabs;

    /// <summary>
    /// Fields
    /// </summary>
    internal List<SimpleUxFieldController> _fields;

    /// <summary>
    /// The currently active pannel's index
    /// </summary>
    [SerializeField, ReadOnly] // for display in inspector for debugging
    internal string _activePannelKey
      = null;

    float _tallestPannelHeight;
    int _mostColumnsInAPannel;
    bool _dimensionsAreDirty;
    bool _waitedWhileDirty;
    Vector2 _minDimensions;

    /// <summary>
    /// Setup
    /// </summary>
    void Awake() {
      _minDimensions = _resizeHandler.MinimumDimmensions;
      _prefabs ??= _fieldControllers.ToDictionary(controller => controller.DisplayType);
    }

    void Update() {
      if(_dimensionsAreDirty && _waitedWhileDirty) {
        _tallestPannelHeight = Math.Max(
            _getTallestPannelHeight(),
            _pannelTabs.Count() * _pannelTabController.GetComponent<RectTransform>().sizeDelta.y
          );

        _pannels.Values.ForEach(pannel => {
          //pannel._horizontalLayoutGroup.enabled = false;
          if(pannel.Pannel.Key.Key != _activePannelKey) {
            pannel.gameObject.SetActive(false);
          }
        });
        _resizeHandler.MinimumDimmensions = new Vector2(
          _minDimensions.x + (_pannels.Count > 1 ? 66 : 0) + (170 * (_mostColumnsInAPannel - 1)),
          _minDimensions.y + _tallestPannelHeight
        //Math.Max(_minDimensions.x, 150 + (_pannels.Count > 1 ? 66 : 0) + (150 * (_mostColumnsInAPannel - 1))),
        //Math.Max(_minDimensions.y, _tallestPannelHeight)
        );
        _rectTransform.sizeDelta
          = _resizeHandler.MinimumDimmensions;

        _dimensionsAreDirty = false;
        _waitedWhileDirty = false;
      } else if(_dimensionsAreDirty) {
        _pannels.Values.ForEach(pannel => { 
          //pannel._horizontalLayoutGroup.childControlHeight = true; 
          //pannel._horizontalLayoutGroup.childControlWidth = true; 
        });
        _waitedWhileDirty = true;
      }
    }

    private float _getTallestPannelHeight() {
      return _pannels.Values.Select(
        pannel => {
          float maxColumnHeight = 0;
          float maxPannelHeight = pannel._columns.Select(column => {
            return column._rows.Append(column.Title);
          }).Max(rows => {
          var column = (rows.First() as ISimpleUxColumnChildElementController).Column;
            float _maxPannelHeight = 0;
            float _maxColumnHeight = 0;
            int rowNumber = 0;
            foreach(ISimpleUxColumnChildElementController row in rows) {
              float rowHeight = ((row as MonoBehaviour)
                ?.GetComponent<RectTransform>()
                ?.sizeDelta.y
                  ?? 0) + 5;
              if (rowNumber < 7) {
                _maxPannelHeight += rowHeight;
              }
              _maxColumnHeight+= rowHeight;
              rowNumber++;
            }

            maxColumnHeight = maxColumnHeight < _maxColumnHeight ? _maxColumnHeight : maxColumnHeight;
            return _maxPannelHeight;
          });

          pannel._columnArea.sizeDelta = pannel._columnArea.sizeDelta.ReplaceY(maxColumnHeight);
          return maxPannelHeight;
        }).Max();
    }

    /// <summary>
    /// Initialize this view for the view data.
    /// </summary>
    public void InitializeFor(UxView model) {
      /// TODO: a view manager should register it for it's Ux view uuid if the view has a cache on close setting.
      View = model;
      _pannels = new();
      _pannelTabs = new();
      _fields = new();
      _mainViewTitleText.text = "+" + model.MainTitle;
      if(ShouldShowCloseButton) {
        _closeButton.gameObject.SetActive(true);
        _closeButton.onClick.AddListener(() => {
          gameObject.SetActive(false);
        });
      }

      foreach((UxPannel.Tab tabData, UxPannel pannelData) in model) {
        SimpleUxPannelController pannel = _addPannel(
          tabData,
          pannelData
        );
        foreach(UxColumn columnData in pannelData) {
          SimpleUxColumnController column = pannel._addColumn(columnData);
          foreach(IUxViewElement element in columnData) {
            if(element is UxRow rowData) {
              throw new NotImplementedException();
            } else if(element is UxDataField fieldData) {
              SimpleUxFieldController field = column._addField(fieldData);
            } else if(element is UxTitle inColumnHeader) {
              column._addInColumnHeader(inColumnHeader);
            } else
              throw new NotSupportedException($"Unknown Simple Ux Element: {element.GetType().FullName}");
          }
        }
      }

      _updateAllFieldsForChangesIn(null);
      _dimensionsAreDirty = true;
    }

    /// <summary>
    /// Called when an element of the view is updated.
    /// </summary>
    /// <param name="originalData">The element before it was edited.</param>
    internal void _onUpdated(IUxViewElement originalData) {
      if(originalData is UxDataField uxField) {
        _updateAllFieldsForChangesIn(uxField);
      }
    }

    void _updateAllFieldsForChangesIn(UxDataField uxField) {
      _fields.ForEach(field => field._onOtherFieldUpdated(View, uxField));
    }

    /// <summary>
    /// Set the active tab
    /// </summary>
    internal void _setActiveTab(UxPannel.Tab tab) {
      _pannels[_activePannelKey].gameObject.SetActive(false);
      _activePannelKey = tab.Key;
      _pannels[_activePannelKey].gameObject.SetActive(true);
    }

    /// <summary>
    /// Add a pannel to this Ux
    /// </summary>
    SimpleUxPannelController _addPannel(
      UxPannel.Tab tabData, 
      UxPannel pannelData
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
  }
}