using Simple.Ux.Data;
using UnityEngine;
using UnityEngine.UI;

namespace Overworld.Controllers.SimpleUx {
  public class PannelTabController : MonoBehaviour, IElementController {

    [SerializeField]
    TMPro.TextMeshProUGUI _titleText;

    [SerializeField]
    Button _tabButton;

    public ViewController View {
      get;
      internal set;
    }

    public Pannel.Tab Tab
      => View.Data.GetTab(_key);

    internal string _key {
      get;
      private set;
    }

    public IUxViewElement Element 
      => Tab;

    internal void _intializeFor(Pannel.Tab tabData) {
      _key = tabData.Key;
      _titleText.text = tabData.Name;

      // listener for active tab changing
      _tabButton.onClick.AddListener(() 
        => View._setActiveTab(Tab));

      // add tootltip
      if(!string.IsNullOrWhiteSpace(tabData.Tooltip)) {
        Tooltip tooltip = _titleText.gameObject.AddComponent<Tooltip>();
        tooltip.TooltipStylePrefab = SimpleUxGlobalManager.DefaultTooltipPrefab;
        tooltip.Text = tabData.Tooltip;
      }
    }
  }
}