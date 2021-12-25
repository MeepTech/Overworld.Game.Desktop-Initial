using Meep.Tech.Data;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Overworld.Data {

  public static partial class Ows {

    public partial class Command {

      /// <summary>
      /// A type of command in Ows
      /// </summary>
      public abstract class Type : Archetype<Command, Command.Type> {

        protected override Dictionary<string, object> DefaultTestParams
          => new() {
            { nameof(Program), null},
            { nameof(Command.Parameters), ParameterTypes.Select(type => default(Token)).ToList()}
          };

        /// <summary>
        /// The types of params that this command requires
        /// </summary>
        public virtual IList<System.Type> ParameterTypes {
          get;
          internal set;
        }

        /// <summary>
        /// Execute logic for this command.
        /// Parameters: 
        ///   The Program,
        ///   The Character Executing the Command,
        ///   The Ordered Parameters provided to the Command
        /// </summary>
        public virtual Func<Program, Data.Character, IList<Token>, Variable> Execute {
          get;
        }

        #region Initialization and Configuration

        /// <summary>
        /// Make a new command
        /// </summary>
        protected Type(Identity id, IEnumerable<System.Type> paramTypes)
          : base(id) {
          ParameterTypes = paramTypes.Select(
            paramType => typeof(IParameter).IsAssignableFrom(paramType)
              ? paramType
              : throw new ArgumentException($"Param type must inherit from Ows.IParameter: {paramType.FullName}."))
            .ToList();
        }

        #endregion

        /// <summary>
        /// Make a command of the given type
        /// </summary>
        public TCommand Make<TCommand>(Program program, IEnumerable<Token> orderedParameters)
          where TCommand : Command 
            => Make<TCommand>(
              (nameof(Command.Parameters), orderedParameters),
              (nameof(Command.Program), program)
            );
        
        /// <summary>
        /// Make a command of the given type
        /// </summary>
        public TCommand Make<TCommand>(Program program, params Token[] orderedParameters)
          where TCommand : Command
            => Make<TCommand>(program, (IEnumerable<Token>)orderedParameters);

        /// <summary>
        /// Make a command of the given type
        /// </summary>
        public Command Make(Program program, IEnumerable<Token> orderedParameters)
          => Make(
            (nameof(Command.Parameters), orderedParameters),
            (nameof(Command.Program), program)
          );
        
        /// <summary>
        /// Make a command of the given type
        /// </summary>
        public Command Make(Program program, params Token[] orderedParameters)
          => Make(program, (IEnumerable<Token>)orderedParameters);
      }
    }
  }
}