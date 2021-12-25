using System;
using System.Collections.Generic;
using System.Linq;

namespace Overworld.Data {

  public partial class Ows {
    public partial class Command {
      /// <summary>
      /// Sets a value to a given key for the executing character
      /// </summary>
      public class SET_FOR : SET {

        SET_FOR()
          : base(
              new("SET-FOR"),
              new[] {
                typeof(Collection<Character>),
                typeof(String),
                typeof(IParameter)
              }
            ) {
        }

        public override Func<Program, Data.Character, IList<Token>, Variable> Execute {
          get;
        } = (program, executor, @params) => {
          foreach(string characterId in (@params[0] as Collection<Character>).Value.Select(
            character => character.Value.Id
          )) {
            Data.Character character = program.GetCharacter(characterId);
            GetVariablesForCharacter(program, character)[((String)@params[1]).Value]
              = @params[2] is Command command
                ? command.ExecuteFor(executor)
                : (Variable)@params[2];
          }

          return null;
        };
      }
    }
  }
}
