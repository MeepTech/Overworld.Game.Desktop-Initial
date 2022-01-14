using Overworld.Data;
using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(Toggle))]
public class TilesEditorMenuOptionController : MonoBehaviour {

  public Overworld.Data.Tile.Type TileArchetype {
    get => _tileArchetype;
    set {
      _tileArchetype = value;
      _tileNameTitle.text = _tileArchetype.Id.Name;
      _tileImage.sprite = _tileArchetype.DefaultBackground?.sprite;
    }
  } Tile.Type _tileArchetype;

  #region Unity Inspector Set

  public Image Image {
    get => _tileImage;
  } [SerializeField] Image _tileImage;

  [SerializeField]
  TMPro.TextMeshProUGUI _tileNameTitle;

  #endregion

  internal TilesMenuSubMenuController _parentMenu;

  internal Toggle _toggle;

  private void OnDestroy() {
    _toggle.onValueChanged.RemoveAllListeners();
  }
}
