using System;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEditor;
using UnityEngine.UI;

namespace SpiritWorlds.Controllers {

  /// <summary>
  /// controls menu tabs
  /// </summary>
  public class MenuTabController : EventTrigger {

    #region Constants

    /// <summary>
    /// The background sprite for enabled tabs
    /// </summary>
    public Sprite EnabledTabBackground;

    /// <summary>
    /// the background sprite for disabled tabs
    /// </summary>
    public Sprite DisabledTabBackground;

    /// <summary>
    /// Container object for tabbed content
    /// </summary>
    public GameObject TabbedContentContainer;

    #endregion

    /// <summary>
    /// The id of this tab
    /// </summary>
    public int id {
      get;
      private set;
    }

    /// <summary>
    /// If this tab is enabled
    /// </summary>
    public bool isEnabled {
      get;
      private set;
    } = false;

    /// <summary>
    /// On click optional, takes current active state of the tab
    /// </summary>
    public Action<bool> onClick
      = null;

    /// <summary>
    /// The menu this is part of
    /// </summary>
    TabbedMenuController parentMenu;

    /// <summary>
    /// The contents of this tab
    /// </summary>
    GameObject contents;

    /// <summary>
    /// the place we put the title field.
    /// </summary>
    Text titleField;

    /// <summary>
    /// The place we put the icon
    /// </summary>
    Image iconSlot;

    /// <summary>
    /// The background of the tab
    /// </summary>
    Image tabBackground;

    #region Initialization

    /// <summary>
    /// Make a tabbed menu tab
    /// </summary>
    public static MenuTabController Make(
      MenuTabController prefab,
      Transform tabArea,
      TabbedMenuController parentMenu,
      int tabID,
      GameObject contents,
      string title,
      bool isSelected,
      Sprite icon = null
    ) {
      MenuTabController menuTab = Instantiate(prefab, tabArea) as MenuTabController;
      menuTab.initialize(tabID, parentMenu, contents, title, icon, isSelected);

      return menuTab;
    }

    /// <summary>
    /// Initialize this
    /// </summary>
    /// <param name="parentMenu"></param>
    void initialize(int tabID, TabbedMenuController parentMenu, GameObject contents, string title, Sprite icon, bool isSelected) {
      id = tabID;
      this.parentMenu = parentMenu;
      this.contents = contents;
      titleField = transform.GetChild(0).GetChild(1).GetChild(0).GetComponent<Text>();
      tabBackground = GetComponent<Image>();
      titleField.text = title;
      if (icon != null) {
        RectTransform textParent =  titleField.transform.parent.gameObject.GetComponent<RectTransform>();
        textParent.offsetMin = textParent.offsetMin.ReplaceX(30);
        GameObject iconContainer = transform.GetChild(0).GetChild(0).gameObject;
        iconContainer.SetActive(true);
        iconSlot = iconContainer.transform.GetChild(0).GetComponent<Image>();
        iconSlot.sprite = icon;
      }
      if (isSelected) {
        enable();
      } else disable();
    }

    #endregion

    #region Tab Manipulation

    /// <summary>
    /// Disable this tab
    /// </summary>
    public void disable() {
      isEnabled = false;
      // set the text and icon opacity back to full
      Color newColor = titleField.color;
      newColor.a = 0.5f;
      titleField.color = newColor;
      if (iconSlot) {
        newColor = iconSlot.color;
        newColor.a = 0.5f;
        iconSlot.color = newColor;
      }

      // set the correct background
      tabBackground.sprite = DisabledTabBackground;

      // disable the contents of this tab
      contents.SetActive(false);
    }

    /// <summary>
    /// enable this tab
    /// </summary>
    void enable() {
      isEnabled = true;
      // set the text and icon opacity back to full
      Color newColor = titleField.color;
      newColor.a = 1;
      titleField.color = newColor;
      if (iconSlot) {
        newColor = iconSlot.color;
        newColor.a = 1;
        iconSlot.color = newColor;
      }

      // set the correct background
      tabBackground.sprite = EnabledTabBackground;

      // enable the contents of this tab
      contents.SetActive(true);
    }

    #endregion

    #region Click Events

    /// <summary>
    /// when this tab is clicked.
    /// </summary>
    /// <param name="eventData"></param>
    public override void OnPointerClick(PointerEventData eventData) {
      onClick?.Invoke(contents.activeSelf);
      parentMenu.disableAllTabsExcept(id);
      enable();
    }

    #endregion

    #region Unity Editor
    #if UNITY_EDITOR
    
    [CustomEditor(typeof(MenuTabController))]
    public class MenuTabControllerEditor : Editor {
      public override void OnInspectorGUI() {
        base.OnInspectorGUI();
      }
    }
    #endif
    #endregion
  }
}