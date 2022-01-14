using Meep.Tech.Data.Utility;
using Overworld.Data;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(RectTransform))]
[RequireComponent(typeof(ToggleGroup))]
[RequireComponent(typeof(GridLayoutGroup))]
public class TilesMenuSubMenuController : MonoBehaviour {

  /// <summary>
  /// The selected tile option
  /// </summary>
  public Tile.Type SelectedTileTypeOption {
    get => _selectedTileTypeOption;
    private set {
      _selectedTileTypeOption = value;
#if UNITY_EDITOR
      _tileType = _selectedTileTypeOption.Id.Key;
#endif
    }
  } Tile.Type _selectedTileTypeOption;

#if UNITY_EDITOR

  /// <summary>
  /// The selected tile option for display in unity editor
  /// </summary>
  [SerializeField, ReadOnly]
  string _tileType;

#endif

  internal OrderedDictionary<Tile.Type, TilesEditorMenuOptionController> _tileOptions
    = new();

  public RectTransform RectTransform => _rectTransform 
    ??= GetComponent<RectTransform>(); RectTransform _rectTransform;

  public ToggleGroup ToggleGroup => _toggleGroup 
    ??= GetComponent<ToggleGroup>(); ToggleGroup _toggleGroup;

  /// <summary>
  /// Add a tile option
  /// </summary>
  internal void AddTileOptions(
    IEnumerable<Tile.Type> backgroundTileArchetypes,
    TilesEditorMenuOptionController tileOptionPrefab
  ) {
    foreach(Overworld.Data.Tile.Type tileType in backgroundTileArchetypes) {
      TilesEditorMenuOptionController option =  Instantiate(tileOptionPrefab, transform);
      option.TileArchetype = tileType;
      option._parentMenu = this;

      // set up toggle
      option._toggle = option.GetComponent<Toggle>();
      option._toggle.group = ToggleGroup;
      option._toggle.onValueChanged.AddListener(
        toTrue => {
          if(toTrue) {
             SelectedTileTypeOption = tileType;
          }
        }
      );

      // set up tooltip if applicable
      if(!string.IsNullOrWhiteSpace(tileType.Description)) {
        var tooltip = option.GetComponent<Tooltip>();
        tooltip.Title = tileType.Id.Name;
        tooltip.Text = tileType.Description;
      }

      _tileOptions.Add(tileType, option);
    }

    int rows = _tileOptions.Count() / 4;
    RectTransform.offsetMin = new Vector2(RectTransform.offsetMin.x, -(rows * 35));
  }
}