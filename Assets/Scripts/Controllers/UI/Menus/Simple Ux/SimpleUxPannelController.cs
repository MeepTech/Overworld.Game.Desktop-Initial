using Overworld.Ux.Simple;
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

namespace Overworld.Controllers.SimpleUx {
  public class SimpleUxPannelController : MonoBehaviour, ISimpleUxElementController {

    [SerializeField]
    [UnityEngine.Tooltip("The prefab for a column")]
    SimpleUxColumnController _columnPrefab;

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

    public SimpleUxViewController View {
      get;
      internal set;
    }

    /// <summary>
    /// The pannel this represents
    /// </summary>
    public UxPannel Pannel {
      get;
      private set;
    }

    public IUxViewElement Element
      => Pannel;

    internal List<SimpleUxColumnController> _columns;

    internal void _intializeFor(UxPannel pannelData) {
      Pannel = pannelData;
      _columns = new();
    }

    internal SimpleUxColumnController _addColumn(UxColumn columnData) {
      SimpleUxColumnController column = Instantiate(_columnPrefab, _columnArea);
      _columns.Add(column);
      column.View = View;
      column.Pannel = this;
      column._intializeFor(columnData);

      if(_columns.Count > 1) {
        column._backgroundImage.enabled = true;
        // the first column background needs to be activated too if there was more than one column
        _columns.First()._backgroundImage.enabled = true;
      }

      return column;
    }
  }
}