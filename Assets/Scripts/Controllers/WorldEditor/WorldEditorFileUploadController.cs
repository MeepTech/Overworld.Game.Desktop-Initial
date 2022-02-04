using B83.Win32;
using Overworld.Data;
using Overworld.Controllers.World;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEngine;

namespace Overworld.Controllers.Editor {
  class WorldEditorFileUploadController : MonoBehaviour {

    #region Unity Inspector Set

    [SerializeField]
    GameObject _defaultDroppedInEntityBase;

    [SerializeField]
    WorldEditorController _worldEditor;

    #endregion

    void OnUploadFiles(List<string> aPathNames, POINT aDropPoint) {
      Dictionary<string, object> options = new() {
        {
          $"{WorldController.CurrentUser.UniqueName} 's Custom Assets",
          true
        },
        {
          Tile.Porter.PixelsPerTileOption,
          _worldEditor.WorldController.World.Options.TileWidthInPixels
        }
      };

      // TOOD: validate file types:
      /// Tiles Tab
      if(_worldEditor.WorldEditorEditorMainMenu.TabbedMenuController.enabledTab.id == 0) {

        // import tile background in place:
        if(Input.GetKey(KeyCode.LeftShift) || Input.GetKey(KeyCode.RightShift) || _worldEditor.WorldEditorEditorMainMenu.ActiveOptions.ImportTilemapsAsBackgroundsInPlace) {
          options.Add(
            Overworld.Data.Tile.Porter.InPlaceTileCallbackOption,
            new Action<Vector2Int, Tile.Type>((locationInMap, tileType) =>
              _worldEditor.WorldController.TileBoards.CurrentDominantTileBoardForUser
                .UpdateTile(locationInMap + _worldEditor.WorldController.TileSelector.SelectedTileLocation, tileType))
          );
        }

        IEnumerable<Tile.Type> tileTypes
        = ImportTypes<Tile.Type>(_worldEditor, aPathNames, options);

        _worldEditor.WorldEditorEditorMainMenu.TilesMenu
          .AddTileBackgroundOptions(tileTypes);

      } // Entities Tab
      else if(_worldEditor.WorldEditorEditorMainMenu.TabbedMenuController.enabledTab.id == 1) {
        throw new NotImplementedException();
        /*foreach(string imageLocation in aPathNames) {
          // Get the sprite:
          Texture2D texture = new(2, 2);
          texture.LoadImage(File.ReadAllBytes(imageLocation));
          Sprite sprite = Sprite.Create(
            texture,
            new Rect(0,0,texture.width, texture.height),
            new Vector2(0.5f, 0.5f),
            _mainEditorMenu.WorldController.CurrentTileBoardController.TileWidthInPixels
          );

          Vector3 dropLocation = Camera.main.ScreenToWorldPoint(Input.mousePosition)
          // TODO: this should check the hight of the tile and put it there:
            .ReplaceY(0.1f);
            // TODO: is this needed?
            // add the tile center offset
            //+ new Vector3(0.5f,0,0.5f);

          GameObject newEntity = Instantiate(DefaultDragAndDropEntityBase, _mainEditorMenu.WorldController.EntitiesParent);
          newEntity.transform.position = dropLocation;
          newEntity.GetComponent<SpriteRenderer>().sprite = sprite;
          newEntity.GetComponent<OverworldEntity>().SetName(Path.GetFileNameWithoutExtension(imageLocation));
        }*/
      } // Equipment Tab
      else if(_worldEditor.WorldEditorEditorMainMenu.TabbedMenuController.enabledTab.id == 2) {
        // TODO: impliment equiptment tab drag and drop:
        throw new NotImplementedException($"Equipment does not yet exist.");
        //porter.ImportAndBuildNewArchetypeFromFolder(unzipedFolderLocation, options);
      } else
        throw new NotSupportedException();


    }

    public static IEnumerable<TArchetype> ImportTypes<TArchetype>(WorldEditorController worldEditor, List<string> fileNames, Dictionary<string, object> options)
      where TArchetype : Meep.Tech.Data.Archetype, IPortableArchetype {
      Overworld.Data.IO.ArchetypePorter<TArchetype> porter = (Overworld.Data.IO.ArchetypePorter<TArchetype>)worldEditor.Porters[typeof(TArchetype)];
      IEnumerable<TArchetype> @return = Enumerable.Empty<TArchetype>();
      if(fileNames.Count > 1) {
        @return = porter.ImportAndBuildNewArchetypeFromFiles(fileNames.ToArray(), options);
      } else if(fileNames.Count == 1) {
        if(Path.GetExtension(fileNames[0]).ToLower() == ".png") {
          @return = porter.ImportAndBuildNewArchetypeFromFile(fileNames[0], options);
        } else if(Path.GetExtension(fileNames[0]).ToLower() == ".zip") {
          throw new NotImplementedException($".Zip file support Not yet implimented.");
        } else if(Path.GetFileName(fileNames[0]).ToLower() == IArchetypePorter.ConfigFileName) {
          @return = porter.ImportAndBuildNewArchetypeFromFiles(fileNames.ToArray(), options);
        } else
          throw new NotSupportedException();
      } else
        throw new ArgumentException($"No files provided");

      @return.Select(porter.SerializeArchetypeToModFolder);
      return @return;
    }

    #region Drag and Drop

#if UNITY_STANDALONE_WIN && !UNITY_EDITOR_WIN

  void OnEnable() {
    UnityDragAndDropHook.InstallHook();
    UnityDragAndDropHook.OnDroppedFiles += OnDropFiles;
  }

  void OnDisable() {
    UnityDragAndDropHook.UninstallHook();
  }
#elif DEBUG

    [SerializeField]
    string[] _testDragAndDropFiles;

    void Update() {
      if(Input.GetKeyDown(KeyCode.F)) {
        OnUploadFiles(_testDragAndDropFiles.ToList(), new POINT(_worldEditor.WorldController.TileSelector.SelectedTileLocation.x, _worldEditor.WorldController.TileSelector.SelectedTileLocation.y));
      }
    }

#endif

    #endregion
  }
}