using Simple.Ux.Data;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

namespace Overworld.Controllers.SimpleUx {
  public class PannelController : MonoBehaviour, IElementController {

    [SerializeField]
    [UnityEngine.Tooltip("The prefab for a column")]
    ColumnController _columnPrefab;

    [SerializeField]
    [UnityEngine.Tooltip("The rect transform of the pannel")]
    internal RectTransform _rectTransform;

    [SerializeField]
    [UnityEngine.Tooltip("The rect transform of the column area")]
    internal RectTransform _columnArea;

    [SerializeField]
    [UnityEngine.Tooltip("The horizontal layout group of the root panel object")]
    internal HorizontalLayoutGroup _horizontalLayoutGroup;

    internal LayoutElement _columnAreaLayout => __columnAreaLayout
      ??= _columnArea.GetComponent<LayoutElement>(); LayoutElement __columnAreaLayout;

    public ViewController View {
      get;
      internal set;
    }

    /// <summary>
    /// The pannel this represents
    /// </summary>
    public Pannel Pannel {
      get;
      private set;
    }

    public IUxViewElement Element
      => Pannel;

    internal List<ColumnController> _columns;

    internal void _intializeFor(Pannel pannelData) {
      Pannel = pannelData;
      _columns = new();
    }

    internal ColumnController _addColumn(Column columnData) {
      ColumnController column = Instantiate(_columnPrefab, _columnArea);
      column.View = View;
      column.Pannel = this;
      column._intializeFor(columnData);
      _columns.Add(column);

      if(_columns.Count > 1) {
        column._backgroundImage.enabled = true;
        // the first column background needs to be activated too if there was more than one column
        _columns.First()._backgroundImage.enabled = true;
      }

      return column;
    }
  }
}