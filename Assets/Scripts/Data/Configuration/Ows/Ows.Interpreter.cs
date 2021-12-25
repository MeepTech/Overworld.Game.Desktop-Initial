using System;
using System.Collections.Generic;
using System.Linq;

namespace Overworld.Data {

  public static partial class Ows {
    public static class Interpreter {

      /// <summary>
      /// Join together several .ows files.
      /// </summary>
      public static string JoinOwsFiles(IEnumerable<(string filename, string contents)> rawFiles)
        => (string)rawFiles.OrderBy(raw => raw.filename).SelectMany(raw => raw.contents);

      /// <summary>
      /// Join together several .ows files into a program
      /// </summary>
      public static string JoinOwsFiles(IEnumerable<string> rawFileNames)
        => JoinOwsFiles(rawFileNames.Select(rawFileName => 
          (rawFileName, System.IO.File.ReadAllText(rawFileName))
        ));

      /// <summary>
      /// Join together several .ows files.
      /// </summary>
      public static string JoinOwsFiles(params (string filename, string contents)[] raws)
        => JoinOwsFiles((IEnumerable<(string filename, string contents)>)raws);

      /*public static void Execute(string raw) {
        using(System.IO.StringReader reader = new System.IO.StringReader(raw)) {
          string line = reader.ReadLine();
        }
      }*/

      /// <summary>
      /// Build a new program from a bunch of files
      /// </summary>
      public static Program Build(Program.ContextData context, IEnumerable<(string filename, string contents)> rawFiles) {
        List<(string filename, string contents)> rawLines = new();
        List<(string filename, string contents)> rawPreInitLines = new();
        foreach(var (filename, contents) in rawFiles.OrderBy(file => file.filename)) {
          if(System.IO.Path.GetFileName(filename).StartsWith("_")) {
            rawPreInitLines.Add((filename, contents));
          } else
            rawLines.Add((filename, contents));
        }

        return new Program(context, JoinOwsFiles(rawFiles), JoinOwsFiles(rawPreInitLines));
      }

      /// <summary>
      /// Execute a line of Ows code, return the label if one was created.
      /// </summary>
      public static void ExecuteLine(string line, out string label) {
        throw new NotImplementedException();
      }
    }
  }
}
