using System.Collections.Generic;

namespace Overworld.Data {

  /// <summary>
  /// Overworld Script
  /// </summary>
  public static partial class Ows {

    /// <summary>
    /// True global variables
    /// </summary>
    static Dictionary<string, Variable> _globals
      = new Dictionary<string, Variable>();

    public static IEnumerable<string> BaseCommands = new [] {
      "GOTO",
      ""
    };

    public static IEnumerable<string> ReservedKeywords {
      get;
    }

    /// <summary>
    /// Used to indicate "All" in some cases
    /// </summary>
    public const char CollectAllSymbol = '*';

    /// <summary>
    /// Used to indicate "All" in some cases
    /// </summary>
    public const string CollectAllPhrase = "ALL";

    /// <summary>
    /// The initial Symbol indicating a label is beginning this line
    /// </summary>
    public const char LabelStartSymbol = '[';

    /// <summary>
    /// Symbol that indicates the end of a label
    /// </summary>
    public const char LabelEndSymbol = ']';

    /// <summary>
    /// Symbol that seperates a function name and it's parameters
    /// </summary>
    public const char FunctionSeperatorSymbol = ':';

    /// <summary>
    /// Symbol that is used to represent a string
    /// </summary>
    public const char StringQuotesSymbol = '"';

    /// <summary>
    /// Phrase used to set items to variables
    /// </summary>
    public const string SetToPhrase = "TO";

    /// <summary>
    /// Phrase used to set items to variables
    /// </summary>
    public const char SetToSymbol = '=';
  }
}
