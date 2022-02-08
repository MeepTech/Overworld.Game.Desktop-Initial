namespace UnityEngine.InputSystem {
  public static class InputActionExtensions {
    public static InputAction AppendBinding(this InputAction action, InputBinding binding) {
      action.AddBinding(binding);
      return action;
    }

    public static InputActionMap AppendAction(this InputActionMap map, string name, InputActionType type = InputActionType.Value, string binding = null, string interactions = null, string processors = null, string groups = null, string expectedControlLayout = null) {
       map.AddAction(name, type, binding, interactions, processors, groups, expectedControlLayout);
      return map;
    }
  }
}
