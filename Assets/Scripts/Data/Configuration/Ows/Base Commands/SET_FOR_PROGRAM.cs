using System;
using System.Collections.Generic;

namespace Overworld.Data {

  public partial class Ows {
    public partial class Command {
      /// <summary>
      /// Sets a value to a given key for the current program
      /// </summary>
      public class SET_FOR_PROGRAM : SET {

        SET_FOR_PROGRAM()
          : base(
              new("SET-FOR-PROGRAM"),
              new[] {
                typeof(String),
                typeof(IParameter)
              }
            ) {
        }

        public override Func<Program, Data.Character, IList<Token>, Variable> Execute {
          get;
        } = (program, executor, @params) => {
          program._topLevelVariables[((String)@params[0]).Value]
            = @params[1] is Command command
              ? command.ExecuteFor(executor)
              : (Variable)@params[1];

          return null;
        };
      }
    }
  }
}
