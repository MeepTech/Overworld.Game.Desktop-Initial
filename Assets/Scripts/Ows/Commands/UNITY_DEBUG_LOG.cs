using System;
using UnityEngine;
using static Overworld.Script.Ows;

namespace Overworld.Script.Unity {

  /// <summary>
  /// An helpful debug command
  /// </summary>
  public class UNITY_DEBUG_LOG : Ows.Command.Type {

    UNITY_DEBUG_LOG()
      : base(
          new("UNITY-DEBUG-LOG"),
          new[] {
              typeof(IParameter)
          }
        ) {
    }

    public override Func<Command.Context, Variable> Execute {
      get;
    } = context => {
      Debug.Log(context.GetUltimateParameterVariable(0));

      return null;
    };
  }
}
