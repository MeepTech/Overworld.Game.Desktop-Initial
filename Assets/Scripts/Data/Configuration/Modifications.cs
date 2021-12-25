using Meep.Tech.Data;
using UnityEngine;

namespace Overworld.Data {
  public sealed partial class Modifications : Meep.Tech.Data.Configuration.Modifications {

    Modifications(Universe universe)
      : base(universe)  { }

    protected override void Initialize() {
      Entity.Animation.Porter.LoadAllAnimationDatas(Application.persistentDataPath);
    }
  }
}
