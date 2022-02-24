using Overworld.Objects.Editor;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace Overworld.Controllers.Editor {

  [RequireComponent(typeof(Tooltip))]
  public class WorldEditorGeneralToolButtonController : MonoBehaviour, IPointerClickHandler {
    [SerializeField]
    Image _icon;
    [SerializeField]

    Image _backgroundImage;
    internal Toggle _toggle;
    Tooltip _tooltip;
    WorldEditorTool _tool;

    internal void _initializeFor(WorldEditorTool tool, WorldEditorController worldEditor, ToggleGroup toggleGroup) {
      _tool = tool;
      _toggle = GetComponent<Toggle>();
      _toggle.group = toggleGroup;
      _toggle.onValueChanged.AddListener(
        toTrue => {
          if(toTrue) {
            worldEditor.ToolController.EnableTool(tool);
          } else {
            worldEditor.ToolController.DissableEnabledTool(tool);
          }
        }
      );

      _tooltip = GetComponent<Tooltip>();
      _tooltip.Title = tool.Name;
      _tooltip.Text = tool.Description;
      _icon.sprite = tool.Icon;
    }

    internal void _updateBackgroundImage(Sprite sprite) {
      if(sprite == null) {
        if(_backgroundImage.sprite != null) {
          _backgroundImage.sprite = null;
          _backgroundImage.enabled = false;
        }

        return;
      }

      if(!_backgroundImage.enabled) {
        _backgroundImage.enabled = true;
      }

      if(_backgroundImage.sprite == null || _backgroundImage.sprite != sprite) {
        _backgroundImage.sprite = sprite;
      }
    }

    public void OnPointerClick(PointerEventData eventData) {
      if(eventData.button == PointerEventData.InputButton.Right && _tool is IHasAnOpenableSettingsWindow toolWithWindow) {
        SimpleUx.SimpleUxViewWindowsManager.Current.OpenWindow(toolWithWindow.GetSettingsWindow());
      }
    }
  }
}