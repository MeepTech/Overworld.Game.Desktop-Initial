using System;
using System.Collections.Generic;

namespace Overworld.Data {

  public partial class Ows {
    public partial class Command {
      /// <summary>
      /// Sets a value to a given key for the whole world
      /// </summary>
      public class SET_FOR_WORLD : SET {

        SET_FOR_WORLD()
          : base(
              new("SET-FOR-WORLD"),
              new[] {
                typeof(String),
                typeof(IParameter)
              }
            ) {
        }

        public override Func<Program, Data.Character, IList<Token>, Variable> Execute {
          get;
        } = (program, executor, @params) => {
          Ows._globals[((String)@params[0]).Value]
            = @params[1] is Command command
              ? command.ExecuteFor(executor)
              : (Variable)@params[1];

          return null;
        };
      }
    }
  }
}
