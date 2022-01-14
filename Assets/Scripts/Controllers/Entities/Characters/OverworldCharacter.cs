using Overworld.Data;
using System.Collections.Generic;
using System.Linq;

public partial class OverworldCharacter : OverworldEntity {

  public new static IEnumerable<OverworldEntity.Event.Hook> ValidHookTypes
    => OverworldEntity.ValidHookTypes.Concat(OverworldEntity.Event.Hook.Type.All.Where(type
      => typeof(Event.Hook.Type).IsAssignableFrom(type.GetType()))
        .Select(type => new OverworldEntity.Event.Hook(type)));

  public Character Character
    => Entity as Character;
}
