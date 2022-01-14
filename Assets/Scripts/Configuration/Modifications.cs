using Meep.Tech.Data;

namespace Overworld.Data {
  public sealed partial class Modifications : Meep.Tech.Data.Configuration.Modifications {

    Modifications(Universe universe)
      : base(universe)  { }

    protected override void Initialize() {
      //IO.Entity.Animation.Porter.LoadAllAnimationDatas(UnityEngine.Application.persistentDataPath);
    }
  }
}
