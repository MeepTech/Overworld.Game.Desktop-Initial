using Overworld.Ux.Simple;
using UnityEngine;

namespace Overworld.Controllers.SimpleUx {
  public class SimpleUxTitleController : MonoBehaviour,  ISimpleUxElementController {
    [SerializeField]
    TMPro.TextMeshProUGUI _titleText;

    public SimpleUxViewController View {
      get;
      internal set;
    }

    /// <summary>
    /// The title this represents
    /// </summary>
    public UxTitle Title {
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

    internal void _initializeFor(UxTitle titleData) {
      Title = titleData;
      _titleText.text = (IsTopTitleForColumn ? "///" : "//") + titleData.Text;
    }
  }
}