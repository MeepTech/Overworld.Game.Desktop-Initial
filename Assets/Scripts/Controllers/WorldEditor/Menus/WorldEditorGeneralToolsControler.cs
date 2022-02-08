using Overworld.Controllers.World;
using Overworld.Objects.Editor;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

namespace Overworld.Controllers.Editor {
  /// <summary>
  /// The tools pannel that shows for all world editor menus; tiles, entities etc
  /// </summary>
  public class WorldEditorGeneralToolsControler : MonoBehaviour, IWorldEditorToolContainerMenu {

    #region Unity Inspector Set

    [SerializeField]
    WorldEditorGeneralToolButtonController _buttonPrefab;

    [SerializeField]
    WorldEditorTool[] _tools;

    #endregion

    WorldEditorController _worldEditor
      => Demiurge.Self.WorldController.WorldEditor;
    Dictionary<string, (WorldEditorGeneralToolButtonController controller, WorldEditorTool data)> _buttons
    = new();
    RectTransform _rectTransform;
    ToggleGroup _toggleGroup;
    List<(WorldEditorGeneralToolButtonController controller, WorldEditorTool data)> _backgroundPreviewCallbacks
    = new();

    private void Awake() {
      _rectTransform ??= GetComponent<RectTransform>();
      _toggleGroup ??= GetComponent<ToggleGroup>();
    }

    // Start is called before the first frame update
    void Start() {
      /// Set up each provided tool as a button
      foreach(WorldEditorTool tool in _tools) {
        WorldEditorGeneralToolButtonController toolOption = Instantiate(_buttonPrefab, transform);
        tool.WorldEditor = _worldEditor;
        tool.FromMenu = this;
        _buttons.Add(tool.Name, (toolOption, tool));
        toolOption._initializeFor(tool, _worldEditor, _toggleGroup);
        if(tool.GetBackgroundPreview is not null) {
          _backgroundPreviewCallbacks.Add((toolOption, tool));
        }

        tool._initialize();
      }
    }

    // Update is called once per frame
    void Update() {
      if(_backgroundPreviewCallbacks.Any()) {
        _backgroundPreviewCallbacks.ForEach(
          tool => tool.controller._updateBackgroundImage(
            tool.data.GetBackgroundPreview.Invoke(_worldEditor)));
      }
    }

    public void OnToolDissabled(WorldEditorTool tool) {
      _buttons[tool.Name].controller._toggle.isOn = false;
    }
  }
}