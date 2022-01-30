using Overworld.Ux.Simple;
using UnityEngine;

namespace Overworld.Controllers.SimpleUx {
  public class SimpleUxTitleController : MonoBehaviour, ISimpleUxColumnChildElementController {
    [SerializeField]
    TMPro.TextMeshProUGUI _titleText;

    public SimpleUxViewController View {
      get;
      internal set;
    }

    public SimpleUxColumnController Column {
      get;
      internal set;
    }

    public virtual float ItemHeight
      => _rectTransfrom.sizeDelta.y;

    RectTransform _rectTransfrom
      => __rectTransfrom ??= GetComponent<RectTransform>(); RectTransform __rectTransfrom;

    /// <summary>
    /// The title this represents
    /// </summary>
    public Title Title {
      get;
      private set;
    }

    /// <summary>
    /// If this is the Title for a Column Header
    /// </summary>
    public bool IsTopTitleForColumn {
      get;
      internal set;
    }

    public IUxViewElement Element 
      => Title;

    internal void _initializeFor(Title titleData) {
      Title = titleData;
      _titleText.text = (IsTopTitleForColumn ? "///" : "//") + titleData.Text;

      // add tootltip
      if(!string.IsNullOrWhiteSpace(titleData.Tooltip)) {
        Tooltip tooltip = _titleText.gameObject.AddComponent<Tooltip>();
        tooltip.TooltipStylePrefab = SimpleUxGlobalManager.DefaultTooltipPrefab;
        tooltip.Text = titleData.Tooltip;
      }
    }
  }
}