using Simple.Ux.Data;
using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace Overworld.Controllers.SimpleUx {

  public class ColumnController : MonoBehaviour, IElementController {

    #region Unity Inspector Set Values

    [UnityEngine.Tooltip("The Prefab for Column Titles")]
    [SerializeField]
    TitleController _columnTitleController;

    [UnityEngine.Tooltip("The Prefab for a Header inside of a Column")]
    [SerializeField]
    TitleController _inColumnHeaderController;

    [SerializeField]
    [UnityEngine.Tooltip("The component for the background image")]
    internal UnityEngine.UI.Image _backgroundImage;

    [SerializeField]
    [UnityEngine.Tooltip("where elements in this column are added")]
    internal RectTransform _elementsArea;

    [SerializeField]
    [UnityEngine.Tooltip("the column's scroll rect")]
    ScrollRect _scrollRect;

    #endregion

    public PannelController Pannel {
      get;
      internal set;
    }

    public ViewController View {
      get;
      internal set;
    }

    /// <summary>
    /// The (optional) column title
    /// </summary>
    public TitleController Title {
      get;
      private set;
    }

    /// <summary>
    /// The column this represents
    /// </summary>
    public Column Column {
      get;
      private set;
    }

    /// <summary>
    /// The rows of items in this column
    /// </summary>
    internal List<IElementController> _rows
      = new();

    public IUxViewElement Element 
      => Column;

    internal void _intializeFor(Column column) {
      Column = column;
      _scrollRect.viewport = Pannel._columnArea;
      if(column.Title is not null) {
        Title = Instantiate(_columnTitleController, _elementsArea);
        Title.Column = this;
        Title.IsTopTitleForColumn = true;
        Title._initializeFor(column.Title);
      }
    }

    internal FieldController _addField(DataField fieldData) {
      FieldController field = Instantiate(ViewController.FieldControllerPrefabs[fieldData.Type], _elementsArea);
      field.View = View;
      field.Column = this;
      field._intializeFor(fieldData);
      _rows.Add(field);
      if(!fieldData.IsReadOnly) {
        View._fields.Add(field.FieldData.DataKey.ToLower(), field);
      }

      return field;
    }

    internal TitleController _addInColumnHeader(Title titleData) {
      TitleController header = Instantiate(_inColumnHeaderController, _elementsArea);
      header.View = View;
      header.Column = this;
      header.IsTopTitleForColumn = false;
      header._initializeFor(titleData);
      _rows.Add(header);

      return header;
    }

    internal FieldController _addRow(Row rowData) {
      throw new NotImplementedException();
    }
  }
}