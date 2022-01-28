using Overworld.Ux.Simple;
using UnityEngine;
using UnityEngine.UI;

namespace Overworld.Controllers.SimpleUx {
  public class SimpleUxPannelTabController : MonoBehaviour, ISimpleUxElementController {

    [SerializeField]
    TMPro.TextMeshProUGUI _titleText;

    [SerializeField]
    Button _tabButton;

    public SimpleUxViewController View {
      get;
      internal set;
    }

    public UxPannel.Tab Tab
      => View.View.GetTab(_key);

    internal string _key {
      get;
      private set;
    }

    public IUxViewElement Element 
      => Tab;

    public void _intializeFor(UxPannel.Tab tabData) {
      _key = tabData.Key;
      _titleText.text = tabData.Name;
      _tabButton.onClick.AddListener(() 
        => View._setActiveTab(Tab));
    }
  }
}