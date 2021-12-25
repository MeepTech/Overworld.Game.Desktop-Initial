using Overworld.Data;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Overworld.Data {

  public static partial class Ows {
    public partial class Program {

      /// <summary>
      /// Context for an Ows program
      /// </summary>
      public class ContextData {

        /// <summary>
        /// The world the program is executed in
        /// </summary>
        public World World {
          get;
        }/*

        /// <summary>
        /// The controller this program is attached to
        /// </summary>
        public IEntityController Controller {
          get;
        }*/

        /// <summary>
        /// The commands within this context
        /// </summary>
        public virtual IReadOnlyDictionary<string, Command.Type> Commands
          => World.Commands;

        /// <summary>
        /// The characters in the world of thdis script
        /// </summary>
        public IReadOnlyDictionary<string, Data.Character> Characters 
          => _characters
            ??= Entities.Where(entity => entity.Value is Data.Character)
              .ToDictionary(e => e.Key, e => e.Value as Data.Character);
        IReadOnlyDictionary<string, Data.Character> _characters;
        
        /// <summary>
        /// The entities in the world of this script
        public IReadOnlyDictionary<string, Data.Entity> Entities
          => _entities 
            ??= World.Entities;
        IReadOnlyDictionary<string, Data.Entity> _entities;

        /// <summary>
        /// Create a context for a new program
        /// </summary>
        public ContextData(/*IEntityController executingController, */World world) {
          //Controller = executingController;
          World = world;
        }
      }
    }
  }
}
