using Meep.Tech.Data;
using System.Collections.Generic;

namespace Overworld.Data {
  public partial class Entity {
    public partial class Animation {
      public class Tags
        : HashSet<Tag>,
          IModel.IComponent<Tags>,
          IComponent.IUseDefaultUniverse {
      }
    }
  }
}