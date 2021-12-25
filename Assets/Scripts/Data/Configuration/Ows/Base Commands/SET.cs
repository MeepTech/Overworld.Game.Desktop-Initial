using System;
using System.Collections.Generic;

namespace Overworld.Data {

  public partial class Ows {
    public partial class Command {

      /// <summary>
      /// Sets a value to a given key for the executing character
      /// </summary>
      public class SET : Ows.Command.Type {

        SET()
          : base(
              new("SET"),
              new[] {
                typeof(String),
                typeof(IParameter)
              }
            ) {
        }

        /// <summary>
        /// For internal extension
        /// </summary>
        internal SET(Identity id, IEnumerable<System.Type> paramTypes)
          : base(
              id,
              paramTypes
            ) {
        }

        public override Func<Program, Data.Character, IList<Token>, Variable> Execute {
          get;
        } = (program, executor, @params) => {
          GetVariablesForCharacter(program, executor)[((String)@params[0]).Value]
            = @params[1] is Command command
              ? command.ExecuteFor(executor)
              : (Variable)@params[1];

          return null;
        };

        /// <summary>
        /// Helper function to get the variables for a character safely
        /// </summary>
        protected static Dictionary<string, Variable> GetVariablesForCharacter(Program program, Data.Character character)
          => program._variablesByCharacter.TryGetValue(character.Id, out var found)
            ? found
            : (program._variablesByCharacter[character.Id] = new Dictionary<string, Variable>());
      }
    }
  }
}
