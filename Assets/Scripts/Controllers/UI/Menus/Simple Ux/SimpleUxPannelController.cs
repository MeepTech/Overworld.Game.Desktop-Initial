using Overworld.Ux.Simple;
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Overworld.Controllers.SimpleUx {
  public class SimpleUxPannelController : MonoBehaviour, ISimpleUxElementController {

    [SerializeField]
    [UnityEngine.Tooltip("The prefab for a column")]
    SimpleUxColumnController _columnPrefab;

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

    List<SimpleUxColumnController> _columns;

    internal void _intializeFor(UxPannel pannelData) {
      Pannel = pannelData;
      _columns = new();
    }

    internal SimpleUxColumnController _addColumn(UxColumn columnData) {
      SimpleUxColumnController column = Instantiate(_columnPrefab, transform);
      _columns.Add(column);
      column.View = View;
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