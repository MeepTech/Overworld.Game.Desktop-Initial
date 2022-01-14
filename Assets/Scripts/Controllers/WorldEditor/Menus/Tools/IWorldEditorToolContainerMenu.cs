public interface IWorldEditorToolContainerMenu {

  /// <summary>
  /// Called when a tool is disabled, usually by another tool being enabled.
  /// </summary>
  void OnToolDissabled(WorldEditorTool tool);
}
