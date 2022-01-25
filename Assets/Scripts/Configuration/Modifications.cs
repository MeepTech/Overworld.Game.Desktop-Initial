using Meep.Tech.Data;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

namespace Overworld.Data {
  public sealed partial class Modifications : Meep.Tech.Data.Configuration.Modifications {

    Modifications(Universe universe)
      : base(universe)  { }

    protected override void Initialize() {
      //IO.Entity.Animation.Porter.LoadAllAnimationDatas(UnityEngine.Application.persistentDataPath);
    }
  }
}
