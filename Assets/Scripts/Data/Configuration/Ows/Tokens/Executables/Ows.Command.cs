using Meep.Tech.Data;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Overworld.Data {

  public static partial class Ows {

    /// <summary>
    /// An executable Ows command
    /// </summary>
    public partial class Command 
      : Token,
        IParameter,
        IModel<Command, Command.Type>,
        IModel.IUseDefaultUniverse
    {

      /// <summary>
      /// The type of archetype
      /// </summary>
      public virtual Type Archetype {
        get;
      }

      /// <summary>
      /// The parameters provided to this command
      /// </summary>
      public virtual IEnumerable<Token> Parameters
        => _parameters;
      List<Token> _parameters {
        get;
      }

      /// <summary>
      /// The default value, providing null as the character.
      /// Use ExecuteFor if you need a character passed in.
      /// </summary>
      public override object Value
        => ExecuteFor(null);

      /// <summary>
      /// Make a command with the given params
      /// </summary>
      protected Command(IBuilder<Command> builder)
        : base(builder.GetAndValidateParamAs<Program>(nameof(Program)), builder.Archetype.Id.Name) {
        Archetype ??= (Type)builder.Archetype;
        var @params = builder.GetParam(nameof(Command.Parameters), new List<Token>());
        if(@params.Count() != Archetype.ParameterTypes.Count) {
          throw new ArgumentException($"Provided only {@params.Count()} parameters to command: {this}, which requires {Archetype.ParameterTypes.Count}");
        }
        _parameters
          ??= @params;
      }

      /// <summary>
      /// Execute this command for the given character
      /// </summary>
      public Variable ExecuteFor(Data.Character executor)
        => Archetype.Execute(Program, executor, _parameters.Select(param => param is CharacterSpecificVariable characterSpecific
          // get the character specific variable if there's one
          ? characterSpecific.GetFor(executor)
          : param).ToList());

      /// <summary>
      /// Execute this command for the given character with som eextra provided commands
      /// </summary>
      internal Variable executeWithExtraParams(Data.Character executor, IList<Token> extraParams)
        => Archetype.Execute(Program, executor, _parameters.Concat(extraParams).Select(param => param is CharacterSpecificVariable characterSpecific
          // get the character specific variable if there's one
          ? characterSpecific.GetFor(executor)
          : param).ToList());
    }
  }
}
