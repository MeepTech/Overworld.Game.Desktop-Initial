using Overworld.Ux.Simple;
using UnityEngine;

namespace Overworld.Controllers.SimpleUx {
  public class TitleController : MonoBehaviour, IColumnElementController {
    [SerializeField]
    TMPro.TextMeshProUGUI _titleText;

    public ViewController View {
      get;
      internal set;
    }

    public ColumnController Column {
      get;
      internal set;
    }

    public virtual float ItemHeight
      => RectTransform.sizeDelta.y;

    public RectTransform RectTransform
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
      _titleText.text = Column is not null 
        ? (IsTopTitleForColumn ? "///" : "//") + titleData.Text 
        : titleData.Text;

      // add tootltip
      if(!string.IsNullOrWhiteSpace(titleData.Tooltip)) {
        Tooltip tooltip = _titleText.gameObject.AddComponent<Tooltip>();
        tooltip.TooltipStylePrefab = SimpleUxGlobalManager.DefaultTooltipPrefab;
        tooltip.Text = titleData.Tooltip;
      }
    }
  }
}