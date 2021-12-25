using System;
using System.Collections.Generic;
using System.Linq;

namespace Overworld.Data {

  public partial class Ows {
    public partial class Command {

      /// <summary>
      /// Go To a Line of Code
      /// </summary>
      public class GO_TO : Ows.Command.Type {

        GO_TO()
          : base(
              new("GO-TO"),
              new[] {
                typeof(String),
                typeof(Number)
              }
            ) {
        }

        public override Func<Program, Data.Character, IList<Token>, Variable> Execute {
          get;
        } = (program, executor, @params)
             => program._executeAllStartingAtLine(program._labelsByLineNumber[(@params.First() as String).Value], executor, ((Number)@params[1]).IntValue);
      }
    }
  }
}
