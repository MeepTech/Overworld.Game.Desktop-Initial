using Meep.Tech.Data;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace Overworld.Data {

  public static partial class Ows {

    /// <summary>
    /// An executable ows program
    /// </summary>
    public partial class Program {

      /// <summary>
      /// The context of this program relating to the world and controller that executed it
      /// </summary>
      public ContextData Context {
        get;
      }

      /// <summary>
      /// The variables universal to all parts of this program
      /// </summary>
      internal Dictionary<string, Variable> _topLevelVariables
        = new();

      /// <summary>
      /// The variables unique to each character, by variable name
      /// </summary>
      internal Dictionary<string, Dictionary<string, Variable>> _variablesByCharacter
        = new();

      /// <summary>
      /// The names of local variables
      /// </summary>
      static HashSet<string> _localVariableNames
        = new();

      /// <summary>
      /// The raw text of the entire program
      /// </summary>
      public string RawText {
        get;
      }

      /// <summary>
      /// Label name keys and what line they relate to
      /// </summary>
      internal Dictionary<string, int> _labelsByLineNumber
        = new();

      /// <summary>
      /// The commands by line
      /// </summary>
      List<Command> _commands
        = new();

      /// <summary>
      /// The line the program starts at
      /// </summary>
      public int InitialLine {
        get;
      } = 0;

      /// <summary>
      /// Make and compile a new Ows program from a collection of lines
      /// </summary>
      public Program(ContextData context, IEnumerable<string> rawLines, IEnumerable<string> preInitialLines) {
        Context = context;
        RawText ??= string.Join(Environment.NewLine, rawLines)
          .Trim()
          .ToUpper();

        int lineNumber = 0;
        InitialLine = preInitialLines?.Count() ?? InitialLine;
        foreach(string line in preInitialLines.Concat(rawLines)) {
          string currentCommandText = string.Copy(line);

          // if there's a label, store it
          if(line.First() == LabelStartSymbol) {
            _labelsByLineNumber.Add(
              (string)line.Skip(1).Until(LabelEndSymbol)
                /*skip the :*/.Skip(1),
              lineNumber
            );
            currentCommandText = currentCommandText.After(LabelEndSymbol);
          }

          /// Process the command
          _commands[lineNumber] 
            = _parseCommandBlock(ref currentCommandText);

          lineNumber++;
        }
      }
      
      /// <summary>
      /// Make a new ows program from a full doc
      /// </summary>
      public Program(ContextData context, string rawText, string preInitialText = null)
        : this(context, rawText.Split(Environment.NewLine), preInitialText?.Split(Environment.NewLine)) 
      {
        RawText = rawText
          .Trim()
          .ToUpper();
      }

      /// <summary>
      /// Execute this program as a specific character
      /// </summary>
      public void ExecuteAs(Data.Character character)
        => _executeAllStartingAtLine(InitialLine, character, null);

      /// <summary>
      /// Get a matching character by id or unique name
      /// TODO: these should be turned into modular object fetcher/builder plugins
      /// </summary>
      public Data.Character GetCharacter(string characterNameOrId) {
        if (Context.Characters.TryGetValue(characterNameOrId, out Data.Character found)) {
          return found;
        }

        return Context.Characters
          .FirstOrDefault(character => character.Value.UniqueName.Equals(characterNameOrId)).Value;
      }

      /// <summary>
      /// Get a matching entity by id or  name
      /// TODO: these should be turned into modular object fetcher/builder plugins
      /// </summary>
      public Data.Entity GetEntity(string entityNameOrId) {
        if (Context.Entities.TryGetValue(entityNameOrId, out Data.Entity found)) {
          return found;
        }

        return Context.Entities
          .FirstOrDefault(character => character.Value.Name.Equals(entityNameOrId)).Value;
      }


      /// <summary>
      /// Execute the whole program, starting at the given line
      /// </summary>
      internal Variable _executeAllStartingAtLine(int line, Data.Character character, int? fromLine = null) {
        while(line != _commands.Count()) {
          if(_commands[line].Archetype is Command.GO_TO) {
            _commands[line].executeWithExtraParams(character, new List<Token> { new Number(this, line) });
            // if it was a goto command, it is expected to have finished the rest itself.
            break;
          } // GO-BACK is implimented here:
          else if(_commands[line].Archetype is Command.GO_BACK) {
            if(fromLine.HasValue) {
              line = fromLine.Value + 1;
              fromLine = null;
            } else
              throw new ArgumentNullException($"No available From Line to go back to. GO-BACK should only work once.");
          }

          _commands[line].ExecuteFor(character);

          line++;
        }

        return null;
      }

      /// <summary>
      /// Process the given command
      Command _parseCommandBlock(ref string remainingCommandText) {
        string currentFunctionName = remainingCommandText.Until(FunctionSeperatorSymbol).Trim();
        remainingCommandText = remainingCommandText.After(FunctionSeperatorSymbol).Trim();

        // make sure it's a recognized command
        if(Context.Commands.TryGetValue(currentFunctionName, out Command.Type commandType)) {
          /// Set command has some specialish syntax:
          if(commandType is Command.SET) {
            // set-for syntax:
            if(currentFunctionName.Equals(Archetypes<Command.IF_NOT>._.Id.Name)) {
              Collection collection = _parseCollection(ref remainingCommandText, typeof(Character));
              if(remainingCommandText.Contains(SetToSymbol)) {
                string[] parts = remainingCommandText.Split(SetToSymbol);
                remainingCommandText = ((string)remainingCommandText.Skip(parts.First().Length)).Trim();
                return commandType.Make(
                  this,
                  collection,
                  new String(this, parts[0].Trim()),
                  _parseCommandParam(ref remainingCommandText, typeof(Token))
                );
              } else if(remainingCommandText.Contains($" {SetToPhrase} ")) {
                string[] parts = remainingCommandText.Split($" {SetToPhrase} ");
                remainingCommandText = ((string)remainingCommandText.Skip(parts.First().Length)).Trim();
                return commandType.Make(
                  this,
                  collection,
                  new String(this, parts[0].Trim()),
                  _parseCommandParam(ref remainingCommandText, typeof(Token))
                );
              }
            }

            // basic set
            if(remainingCommandText.Contains(SetToSymbol)) {
              string[] parts = remainingCommandText.Split(SetToSymbol);
              remainingCommandText = (string)remainingCommandText.Skip(parts.First().Length);
              return commandType.Make(
                this,
                new String(this, parts[0].Trim()),
                _parseCommandParam(ref remainingCommandText, typeof(Token))
              );
            } else if(remainingCommandText.Contains($" {SetToPhrase} ")) {
              string[] parts = remainingCommandText.Split($" {SetToPhrase} ");
              remainingCommandText = (string)remainingCommandText.Skip(parts.First().Length);
              return commandType.Make(
                this,
                new String(this, parts[0].Trim()),
                _parseCommandParam(ref remainingCommandText, typeof(Token))
              );
            }
          }

          // normal commands:
          int commandParams = commandType.ParameterTypes.Count();
          List<Token> parameters = new();
          for(int paramIndex = 0; paramIndex < commandParams; paramIndex++) {
            parameters.Add(
              _parseCommandParam(
                ref remainingCommandText,
                commandType.ParameterTypes[paramIndex]
              )
            );
          }
          return commandType.Make(this, parameters);
        } else
          throw new System.MissingMethodException(nameof(Ows), currentFunctionName);
      }


      /// <summary>
      /// Create a token as a param for a command
      /// </summary>
      Token _parseCommandParam(ref string fullRemaininglineText, Type expectedType) {
        string firstParamStub = fullRemaininglineText
          .Until(FunctionSeperatorSymbol)
          .Trim();

        /// if it's a bool/conditional
        if(firstParamStub.StartsWith("TRUE", true, null) && firstParamStub.Length == 4) {
          if(!expectedType.Equals(typeof(Boolean))) {
            throw new ArgumentException($"{expectedType} Expected but Boolean provided at Point: \n {fullRemaininglineText}");
          }
          firstParamStub = firstParamStub.After('E').Trim();
          fullRemaininglineText = ((string)fullRemaininglineText.Skip(firstParamStub.Length + 1)).Trim();
          return new Boolean(this, true);
        }
        if(firstParamStub.StartsWith("FALSE", true, null) && firstParamStub.Length == 4) {
          if(!expectedType.Equals(typeof(Boolean))) {
            throw new ArgumentException($"{expectedType} Expected but Boolean provided at Point: \n {fullRemaininglineText}");
          }
          firstParamStub = firstParamStub.After('E').Trim();
          fullRemaininglineText = ((string)fullRemaininglineText.Skip(firstParamStub.Length + 1)).Trim();
          return new Boolean(this, false);
        }
        if(typeof(Condition).IsAssignableFrom(expectedType)) {
          return _parseCondition(ref fullRemaininglineText);
        }

        /// if it's a string
        if(firstParamStub.First() == StringQuotesSymbol) {
          if(expectedType.IsAssignableToGeneric(typeof(Collection<>))) {
            // if we expect a collection of somekind, it could be a collection of strings, or entities.
            return _parseCollection(ref fullRemaininglineText, expectedType);
          }

          if(!expectedType.Equals(typeof(String))) {
            throw new ArgumentException($"{expectedType} Expected but String provided at Point: \n {fullRemaininglineText}");
          }
          return _parseString(ref fullRemaininglineText, firstParamStub);
        }

        /// if it's a plain number
        if(char.IsNumber(firstParamStub.First())) {
          // get all characters until we have a non number/decimal point, or a space.
          int decimalCount = 0;
          string value = firstParamStub.Until(chararcter => {
            // allow one decimal
            if (chararcter.Equals('.') && decimalCount == 0) {
              decimalCount++;
              return true;
            } else if (decimalCount == 1) {
              decimalCount++;
              return false;
            }

            // if it's not a number, return
            return !char.IsNumber(chararcter);
          });
          if(decimalCount == 2) {
            throw new ArgumentException($"Unexpected second decimal character in float value starting: {fullRemaininglineText}");
          }
          fullRemaininglineText = ((string)fullRemaininglineText.Skip(firstParamStub.Length + 1)).Trim();
          return new Number(this, double.Parse(value));
        }

        // check if it's a command
        if(firstParamStub.Contains(FunctionSeperatorSymbol)) {
          if(Context.Commands.TryGetValue(firstParamStub.Until(FunctionSeperatorSymbol), out Command.Type found)) {
            return _parseCommandBlock(ref fullRemaininglineText);
          } else
            throw new MissingMethodException(nameof(Ows), firstParamStub.Until(FunctionSeperatorSymbol));
        }

        /// lastly, assume it's a variable
        return _parseExistingVariable(ref fullRemaininglineText, firstParamStub);
      }

      /*static IEnumerable<char> _conditionSplitChars {
        get;
      } = new[] {
        (char)Comparitors.And,
        (char)Comparitors.Or,
        (char)Comparitors.Equals,
        (char)Comparitors.GreaterThan,
        (char)Comparitors.LessThan
      };*/

      /// <summary>
      /// Parse a condition recursively.
      /// Left side takes precendence.
      /// </summary>
      Condition _parseCondition(ref string fullRemaininglineText) {
        string conditionText = fullRemaininglineText.Until(FunctionSeperatorSymbol).Trim();
        fullRemaininglineText = fullRemaininglineText.After(FunctionSeperatorSymbol).Trim();

        return _getCondition(conditionText);
      }

      Condition _getCondition(string conditionText) {
        // first or
        Condition condition = _tryToMakeSplitCondition(conditionText, Comparitors.Or);
        if(condition is null) {
          condition = _tryToMakeSplitCondition(conditionText, Comparitors.And);
        }
        if(condition is null) {
          condition = _tryToMakeSplitCondition(conditionText, Comparitors.GreaterThan);
        }
        if(condition is null) {
          condition = _tryToMakeSplitCondition(conditionText, Comparitors.LessThan);
        }
        if(condition is null) {
          condition = _tryToMakeSplitCondition(conditionText, Comparitors.Equals);
        }

        if(condition == null) {
          // Not is last
          if(conditionText.Contains((char)Comparitors.Not) || conditionText.Contains(Comparitors.Not.ToString().ToUpper() + '-')) {
            if(conditionText.First().Equals((char)Comparitors.Not)) {
              condition = Archetypes<Condition.Type>._.Make(this, new Token[] {
              _getExistingVariable(conditionText.TrimStart((char)Comparitors.Not))
            }, Comparitors.Not);
            } else
              condition = Archetypes<Condition.Type>._.Make(this, new Token[] {
              _getExistingVariable(conditionText.After('-'))
            }, Comparitors.Not);
          }
        }

        /// identity is the last resort
        return condition ?? Archetypes<Condition.Type>._.Make(this, new Token[] {
          _getExistingVariable(conditionText)
        }, Comparitors.Identity);
      }

      Condition _tryToMakeSplitCondition(string conditionText, Comparitors splitComparitor) {
        // or is first
        int symbolLocation;
        int phraseLocation = -1;
        if((symbolLocation = conditionText.IndexOf((char)splitComparitor)) > 0
          || (phraseLocation = conditionText.IndexOf(splitComparitor.ToString().ToUpper())) > 0
        ) {
          string[] parts = conditionText.Split(symbolLocation > phraseLocation
            ? splitComparitor.ToString().ToUpper()
            : ((char)splitComparitor).ToString());
          return Archetypes<Condition.Type>._.Make(this, new Token[] {
            _getCondition(parts[0]),
            _getCondition(parts[1])
          }, splitComparitor);
        }

        return null;
      }

      Variable _parseExistingVariable(ref string fullRemaininglineText, string variableName) {
        fullRemaininglineText = ((string)fullRemaininglineText.Skip(variableName.Length + 1)).Trim();
        return _getExistingVariable(variableName);
      }

      Variable _getExistingVariable(string variableName) {
        // check if it's a global, then program, then local character variable
        if(_globals.TryGetValue(variableName, out var foundGlobal)) {
          return foundGlobal;
        } else if(_topLevelVariables.TryGetValue(variableName, out var foundProgramLevel)) {
          return foundProgramLevel;
        } // as a last resort, assume it's a char specific variable:
        else {
          return new CharacterSpecificVariable(this, variableName);
        }
      }

      /// <summary>
      /// Parse the next bit of the remaining line as a string
      /// </summary>
      String _parseString(ref string fullRemaininglineText, string firstParamStub) {
        string value = firstParamStub.Skip(1).Until(StringQuotesSymbol).Trim();
        fullRemaininglineText = ((string)fullRemaininglineText.Skip(firstParamStub.Length + 2)).Trim();
        return new String(this, value);
      }

      Collection _parseCollection(ref string fullRemaininglineText, Type expectedType) {
        IList list;
        Collection collection;
        Type collectionItemType;
        // TODO: Impliment these as modular collection builders.
        if(expectedType.GenericTypeArguments.First().Equals(typeof(String))) {
          collectionItemType = typeof(String);
          list = new List<String>();
          collection = new Collection<String>(this, list as IList<String>);
        } else if(expectedType.GenericTypeArguments.First().Equals(typeof(Character))) {
          collectionItemType = typeof(Character);
          list = new List<Character>();
          collection = new Collection<Character>(this, list as IList<Character>);
        } else if(expectedType.GenericTypeArguments.First().Equals(typeof(Entity))) {
          collectionItemType = typeof(Entity);
          list = new List<Entity>();
          collection = new Collection<Entity>(this, list as IList<Entity>);
        } else
          throw new NotSupportedException($"Collections of type {expectedType} are not yet supported");

        /// Get the relevant text
        string collectionText = fullRemaininglineText
          .Until(FunctionSeperatorSymbol)
          .TrimEnd();

        // trim off the text we're parsing
        fullRemaininglineText = fullRemaininglineText
          .After(FunctionSeperatorSymbol)
          .Trim();

        /// check if all is used
        bool usingAllSymbol;
        if((usingAllSymbol = collectionText.First().Equals(CollectAllSymbol)) || collectionText.StartsWith(CollectAllPhrase)) {
          if(!typeof(Object).IsAssignableFrom(collectionItemType.GetType())) {
            throw new NotSupportedException($"Collections of type {collectionItemType.FullName} do not support the ALL/* Syntax");
          }

          // Trimm off the * syntax
          string remainingCollectionText =
            usingAllSymbol
              ? ((string)collectionText.Skip(1)).Trim()
              : ((string)collectionText.Skip(3)).Trim();

          // Check for ! syntax
          if(remainingCollectionText.Length > 0) {
            if((remainingCollectionText[1].Equals((char)Comparitors.And)
              || remainingCollectionText.StartsWith(Comparitors.And.ToString().ToUpper()))
              && (collectionText.Contains("!")
                || collectionText.Contains("NOT-"))
            ) {
              IEnumerable<string> parts = remainingCollectionText
                .Split(CollectAllSymbol)
                // needs a space:
                .SelectMany(splitPart => splitPart.Split($" {CollectAllPhrase} "))
                .Select(part => part.Trim());

              // loop though each part
              foreach(string collectionPart in parts) {
                // Not syntax only:
                bool isUsingPhrase;
                if(!((isUsingPhrase = collectionPart[0].Equals((char)Comparitors.Not))
                  || collectionPart.StartsWith(Comparitors.Not.ToString().ToUpper()))) {
                  throw new NotSupportedException($"The ALL Syntax (*), can only be used Alone or with AND + NOT Syntax. EX: (*&!VAR). Error Location: {collectionPart} in: \n {fullRemaininglineText}");
                }

                string trimmedPartText
                  = isUsingPhrase
                    ? ((string)collectionPart.Skip(3)).Trim()
                    : ((string)collectionPart.Skip(1)).Trim();

                // parse and add the tokens
                list.Add(_getCollectionItemTokenForType(trimmedPartText, expectedType));
              }
            } else
              throw new NotSupportedException($"The ALL Syntax (*), can only be used Alone or with AND + NOT Syntax. EX: (*&!VAR)");
          } else
            return _getAllOfObjectType(collectionItemType);
        }

        return collection;
      }

      Token _getCollectionItemTokenForType(string tokenText, Type expectedType) {
        // TODO: make this modular as well with the other 2 places. Probably replace them all with a settings virtual obj
        if(tokenText[0].Equals(StringQuotesSymbol)) {
          if(expectedType.Equals(typeof(String))) {
            return new String(this, tokenText.Trim(StringQuotesSymbol));
          } else if(expectedType.Equals(typeof(Character))) {
            return new Character(this, GetCharacter(tokenText.Trim(StringQuotesSymbol)));
          } else if(expectedType.Equals(typeof(Character))) {
            return new Entity(this, GetEntity(tokenText.Trim(StringQuotesSymbol)));
          } else
            throw new NotSupportedException($"Collections of type {expectedType} are not yet supported.");
        } else
          return _getExistingVariable(tokenText);
      }

      /// <summary>
      /// Get all the variables of a given object type
      /// </summary>
      Collection _getAllOfObjectType(Type collectionItemType) {
        // TODO: Cache these and make these more modular
        if(collectionItemType.Equals(typeof(Character))) {
          return new Collection<Character>(this, (IList<Character>)Context.Characters.Values.ToList());
        } else if(collectionItemType.Equals(typeof(Entity))) {
          return new Collection<Character>(this, (IList<Character>)Context.Characters.Values.ToList());
        } else
          throw new NotSupportedException($"The ALL Syntax (*), can only be used with entities and characters atm");
      }

      /// <summary>
      /// Set the variable for the given characters
      /// </summary>
      void _setVariableFor(IEnumerable<string> characterIds, string name, object value) {
        _onSetNammedVariable(name);
        characterIds.ForEach(characterId => {
          if(_variablesByCharacter.TryGetValue(characterId, out var characterVariables)) {
            characterVariables[name] = Variable.Make(this, name, value);
          } else
            characterVariables = new Dictionary<string, Variable> {
              { name, Variable.Make(this, name, value) }
            };
        });
      }

      /// <summary>
      /// Add a variable to the global "program" context
      /// </summary>
      void _setProgramVariable(string name, object value) {
        _onSetNammedVariable(name, isProgramLevel: true);
        _topLevelVariables.Add(name, Variable.Make(this, name, value));
      }

      /// <summary>
      /// add a variable to the global "world" context
      /// </summary>
      void _setGlobalVariable(string name, object value) {
        _onSetNammedVariable(name, isGlobal: true);
        _globals.Add(name, Variable.Make(this, name, value));
      }

      /// <summary>
      /// logic executed on adding a variable to the runtime.
      /// </summary>
      void _onSetNammedVariable(string name, bool isGlobal = false, bool isProgramLevel = false) {
        if(Context.Commands.ContainsKey(name)) {
          throw new ArgumentException(name, $"Variable name conflict, variable shares a name with the Command: {name}");
        }
        if(isGlobal || isProgramLevel) {
          if(_localVariableNames.Contains(name)) {
            throw new ArgumentException(name, $"Variable name conflict, local and global or program level variable share a name: {name}");
          }
        }
        else {
          _localVariableNames.Add(name);
        }

        if(!isGlobal && _globals.ContainsKey(name)) {
          throw new ArgumentException(name, $"Cannot have another variable with a name matching existing global variable: {name}");
        }

        if(!isProgramLevel && _topLevelVariables.ContainsKey(name)) {
          throw new ArgumentException(name, $"Cannot have another variable with a name matching existing top level Program variable: {name}");
        }
      }
    }
  }
}
