using System.Collections.Generic;
using System.Linq;
/// <summary>
/// An equipment item.
/// Equipment can change what buttons do while you hold them, or just look neat and provide effects.
/// You cannot override the action of the emergency de-equip button: the 'DEL' key.
/// </summary>
public partial class OverworldEquipment : OverworldEntity {

  public new static IEnumerable<OverworldEntity.Event.Hook> ValidHookTypes
    => OverworldEntity.ValidHookTypes.Concat(OverworldEntity.Event.Hook.Type.All.Where(type
      => typeof(Event.Hook.Type).IsAssignableFrom(type.GetType()))
        .Select(type => new OverworldEntity.Event.Hook(type)));
}