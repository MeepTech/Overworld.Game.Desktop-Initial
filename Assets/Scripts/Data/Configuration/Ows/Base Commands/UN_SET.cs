using System;
using System.Collections.Generic;
using System.Linq;
using Meep.Tech.Data;

namespace Overworld.Data {

  public partial class Ows {
    public partial class Command {

      /// <summary>
      /// Un-sets any value for a given key for the local character
      /// </summary>
      public class UN_SET : Ows.Command.Type {

        UN_SET()
          : base(
              new("SET"),
              new[] {
                typeof(String)
              }
            ) {
        }

        /// <summary>
        /// For internal extension
        /// </summary>
        internal UN_SET(Identity id, IEnumerable<System.Type> paramTypes)
          : base(
              id,
              paramTypes
            ) {
        }

        public override Func<Program, Data.Character, IList<Token>, Variable> Execute {
          get;
        } = (program, executor, @params) => {
          if(program._variablesByCharacter.TryGetValue(executor.Id, out var characterVariables)) {
            characterVariables.Remove(((String)@params[0]).Value);
            if(!characterVariables.Any()) {
              program._variablesByCharacter.Remove(executor.Id);
            }
          }

          return null;
        };
      }
    }
  }
}
