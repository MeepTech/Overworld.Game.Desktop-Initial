using System;
using System.Linq;
using UnityEngine;

namespace Overworld.Utilities {

  [DefaultExecutionOrder(9999)]
  public class TabbedMenuController : MonoBehaviour {

    #region Constants

    /// <summary>
    /// the prefab used to make the menu tabs
    /// </summary>
    public MenuTabController TabPrefab;

    /// <summary>
    /// A map of models used for tile features organized like so
    /// SET IN UNITY
    /// </summary>
    public TabDataMap[] TabData;

    /// <summary>
    /// The index of the default tab
    /// </summary>
    public int DefaultTabIndex
      = 0;

    /// <summary>
    /// The canvas group
    /// </summary>
    public TabbedMenuSlideController MenuSlider
      => _menuSlider ?? (!_checked ? _menuSlider = GetComponent<TabbedMenuSlideController>() : null);
    TabbedMenuSlideController _menuSlider;
    bool _checked = false;

    #endregion

    /// <summary>
    /// The tabs
    /// </summary>
    public MenuTabController[] Tabs {
      get;
      private set;
    }

    /// <summary>
    /// Get the enabled tab
    /// </summary>
    public MenuTabController enabledTab {
      get {
        if(MenuSlider != null && MenuSlider.isSlidOpen) {
          return Tabs.First(tab => tab.isEnabled);
        } else if(MenuSlider is null) {
          return Tabs.First(tab => tab.isEnabled);
        }

        return null;
      }
    }


    /// <summary>
    /// Populate the tabs
    /// </summary>
    void Awake() {
      int tabIndex = 0;
      Tabs = new MenuTabController[TabData.Length];
      Transform tabArea = transform.GetChild(0).GetChild(0);
      foreach(TabDataMap tabData in TabData) {
        Tabs[tabIndex] = MenuTabController.Make(
          TabPrefab,
          tabArea,
          this,
          tabIndex,
          tabData.Contents,
          tabData.Name,
          tabIndex++ == DefaultTabIndex,
          tabData.Tooltip,
          tabData.Icon
        );
      }
    }

    /// <summary>
    /// Optional callback on tab changed.
    /// </summary>
    public Action<int> OnTabChanged;

    /// <summary>
    /// callback to tell all tabs except one to disable when another is clicked.
    /// </summary>
    /// <param name="id"></param>
    internal void TabCallback(int id) {
      // disable all tabs except the clicked one:
      foreach(MenuTabController tab in Tabs) {
        if (tab.id != id) {
          tab.disable();
        }
      }
      OnTabChanged?.Invoke(id);
    }


    /// <summary>
    /// Set the content into the correct place in the hireracthy
    /// </summary>
    /// <param name="childContent"></param>
    public void addContentChild(Transform childContent) {
      childContent.SetParent(transform.GetChild(0).GetChild(1), false);
    }

    /// <summary>
    /// A map representing the models for one type of terrain feature sorted by how many usages it has left
    /// </summary>
    [System.Serializable]
    public class TabDataMap {

      /// <summary>
      /// The sprite to use for this type of tool
      /// </summary>
      [SerializeField]
      public Sprite Icon;

      /// <summary>
      /// the id of the tile feature this is for
      /// </summary>
      [SerializeField]
      public string Name;

      /// <summary>
      /// the gameobject containing the contents of this tab
      /// </summary>
      [SerializeField]
      public GameObject Contents;

      /// <summary>
      /// Optional tooltip
      /// </summary>
      [SerializeField]
      public string Tooltip;
    }
  }
}