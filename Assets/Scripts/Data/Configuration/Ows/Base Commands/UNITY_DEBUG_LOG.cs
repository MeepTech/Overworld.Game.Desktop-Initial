using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Overworld.Data {
  public partial class Ows {
    public partial class Command {

      /// <summary>
      /// An helpful debug command
      // TODO: move to unity library
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

        public override Func<Program, Data.Character, IList<Token>, Variable> Execute {
          get;
        } = (program, executor, @params) => {
          Debug.Log(@params.First() is Command conditional
            ? conditional.ExecuteFor(executor)
            : @params.First().Value);

          return null;
        };
      }
    }
  }
}
