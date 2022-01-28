using Overworld.Ux.Simple;
using System;
using UnityEngine;

namespace Overworld.Controllers.SimpleUx {

  public class SimpleUxColumnController : MonoBehaviour, ISimpleUxElementController {

    #region Unity Inspector Set Values

    [UnityEngine.Tooltip("The Prefab for Column Titles")]
    [SerializeField]
    SimpleUxTitleController _columnTitleController;

    [UnityEngine.Tooltip("The Prefab for a Header inside of a Column")]
    [SerializeField]
    SimpleUxTitleController _inColumnHeaderController;

    [SerializeField]
    [UnityEngine.Tooltip("The component for the background image")]
    internal UnityEngine.UI.Image _backgroundImage;

    #endregion

    public SimpleUxViewController View {
      get;
      internal set;
    }

    /// <summary>
    /// The (optional) column title
    /// </summary>
    public SimpleUxTitleController Title {
      get;
      private set;
    }

    /// <summary>
    /// The column this represents
    /// </summary>
    public UxColumn Column {
      get;
      private set;
    }

    public IUxViewElement Element 
      => Column;

    internal void _intializeFor(UxColumn column) {
      Column = column;
      if(column.Title is not null) {
        Title = Instantiate(_columnTitleController, transform);
        Title.IsTopTitleForColumn = true;
        Title._initializeFor(column.Title);
      }
    }

    internal SimpleUxFieldController _addField(UxDataField fieldData) {
      SimpleUxFieldController field = Instantiate(SimpleUxViewController.FieldControllerPrefabs[fieldData.Type], transform);
      field.View = View;
      View._fields.Add(field);
      field._intializeFor(fieldData);

      return field;
    }

    internal SimpleUxTitleController _addInColumnHeader(UxTitle titleData) {
      SimpleUxTitleController header = Instantiate(_inColumnHeaderController, transform);
      header.View = View;
      header.IsTopTitleForColumn = false;
      header._initializeFor(titleData);

      return header;
    }

    internal SimpleUxFieldController _addRow(UxRow rowData) {
      throw new NotImplementedException();
    }
  }
}