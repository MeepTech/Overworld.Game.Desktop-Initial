using Meep.Tech.Data.Utility;
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
    TMPro.TextMeshProUGUI _mainViewTitleText;
    [SerializeField]
    Transform _pannelTabsArea;
    [SerializeField]
    Transform _pannelsArea;

    #endregion

    /// <summary>
    /// The controller prefabs for each type of Simple Ux Field.
    /// </summary>
    public IReadOnlyDictionary<Ux.Simple.UxDataField.DisplayType, SimpleUxFieldController> FieldControllerPrefabs
      => _prefabs ??= _fieldControllers.ToDictionary(controller => controller.DisplayType);
    Dictionary<Ux.Simple.UxDataField.DisplayType, SimpleUxFieldController> _prefabs;

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

    /// <summary>
    /// Initialize this view for the view data.
    /// </summary>
    public void InitializeFor(UxView model) {
      View = model;
      _pannels = new();
      _pannelTabs = new();
      _fields = new();
      _mainViewTitleText.text = "+" + model.MainTitle;

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
      SimpleUxPannelTabController simpleUxPannelTabController = Instantiate(_pannelTabController);
      SimpleUxPannelController simpleUxPannelController = Instantiate(_pannelController);
      _pannels.Add(tabData.Key, simpleUxPannelController);
      _pannelTabs.Add(tabData.Key, simpleUxPannelTabController);

      simpleUxPannelTabController.transform.parent = _pannelTabsArea;
      simpleUxPannelController.transform.parent = _pannelsArea;

      simpleUxPannelTabController.View = this;
      simpleUxPannelTabController._intializeFor(tabData);
      simpleUxPannelController.View = this;
      simpleUxPannelController._intializeFor(pannelData);

      /// if we don't have one, this becomes the active pannel
      if(_activePannelKey is null) {
        _activePannelKey = tabData.Key;
      }

      if(tabData.Key != _activePannelKey) {
        simpleUxPannelController.gameObject.SetActive(false);
      }

      // activate the tab background if we have more than one tab
      if(_pannels.Count > 1) {
        _pannelTabsArea.gameObject.SetActive(true);
      }

      return simpleUxPannelController;
    }
  }
}