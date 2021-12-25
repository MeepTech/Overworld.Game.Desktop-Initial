using Meep.Tech.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using static Meep.Tech.Data.Configuration.Loader.Settings;

namespace Overworld.Data {
  public static partial class Ows {
    public partial class Condition {

      /// <summary>
      /// Base archetype for conditions
      /// </summary>
      [Branch]
      public new class Type : Command.Type {

        protected Type(Identity id)
          : base(
              id ?? new Command.Type.Identity("Condition"),
              new[] { typeof(Token), typeof(Token) }
            ) {
        }

        /// <summary>
        /// Make function to make a new condition
        /// </summary>
        public Condition Make(Program program, IEnumerable<Token> @params, Comparitors? comparitor = null)
          => Make<Condition>(
            (nameof(Command.Parameters), @params.Count() == 1
              && (comparitor == Comparitors.Identity
                || comparitor == Comparitors.Not)
                // by default, it adds null for identities and nots
                ? @params.Append(null)
                : @params),
            (nameof(Command.Program), program),
            (nameof(Condition.Comparitor), comparitor)
          );

        public override Func<Program, Data.Character, IList<Token>, Variable> Execute {
          get;
        } = (program, executor, @params) => {
          Condition condition = (Condition)@params.Last();
          switch(condition?.Comparitor) {
            case Comparitors.Identity:
              return ((IConditional)@params.First()).ComputeFor(executor);
            case Comparitors.Not:
              return ((IConditional)@params.First()).ComputeFor(executor).Not;
            case Comparitors.And:
              return ((IConditional)@params.First()).ComputeFor(executor)
                .And(((IConditional)@params[1]).ComputeFor(executor));
            case Comparitors.Or:
              return ((IConditional)@params.First()).ComputeFor(executor)
                .Or(((IConditional)@params[1]).ComputeFor(executor));
            case Comparitors.Equals:
              return new Boolean(program, @params[0].Value.Equals(@params[1].Value));
            case Comparitors.LessThan:
              if(@params[0] is Number && @params[1] is Number) {
                return new Boolean(
                  program,
                  ((Number)@params[0]).RawValue < ((Number)@params[1]).RawValue
                );
              } else
                throw new ArgumentException($"Condition of type {condition.Comparitor} requires two Number parameters");
            case Comparitors.GreaterThan:
              if(@params[0] is Number && @params[1] is Number) {
                return new Boolean(
                  program,
                  ((Number)@params[0]).RawValue > ((Number)@params[1]).RawValue
                );
              } else
                throw new ArgumentException($"Condition of type {condition.Comparitor} requires two Number parameters");
            default:
              throw new ArgumentException($"No Conditional Type Provided.");
          };
        };
      }
    }
  }
}
