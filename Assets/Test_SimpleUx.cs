using Overworld.Controllers.SimpleUx;
using Overworld.Ux.Simple;
using UnityEngine;

public class Test_SimpleUx : MonoBehaviour {

  // Start is called before the first frame update
  void Start() {
    var testView = Instantiate(SimpleUxGlobalManager.DefaultViewPrefab, transform);
    var testModel = new UxViewBuilder("Test")
      .AddField(new UxTextField("Test Text"))
      .AddField(new UxToggleField("Test Toggle"))
      .AddField(new UxToggleField("Test Toggle", dataKey: "toggle2"))
    .Build();

    testView.InitializeFor(testModel);

    testView = Instantiate(SimpleUxGlobalManager.DefaultViewPrefab, transform);
    testModel = new UxViewBuilder("Test2")
      .AddField(new UxTextField("Test Text"))
      .AddField(new UxToggleField("Test Toggle"))
      .StartNewColumn(new UxTitle("Test Col", "Tooltip for test col title"))
      .AddField(new UxToggleField("Test Toggle", dataKey: "toggle2"))
    .Build();

    testView.InitializeFor(testModel);

    testView = Instantiate(SimpleUxGlobalManager.DefaultViewPrefab, transform);
    testModel = new UxViewBuilder("Test3")
      .AddField(new UxTextField("Test Text"))
      .AddField(new UxToggleField("Test Toggle"))
      .StartNewColumn(new UxTitle("Test Col", "Tooltip for test col title"))
      .AddField(new UxToggleField("Test Toggle", dataKey: "toggle2"))
      .StartNewPannel(new UxPannel.Tab("pannel 2", tooltip: "pannel 2 tooltip"))
      .AddField(new UxTextField("Other Pannel Field"))
    .Build();

    testView.InitializeFor(testModel);

    testView = Instantiate(SimpleUxGlobalManager.DefaultViewPrefab, transform);
    testModel = new UxViewBuilder("Test4")
      .AddField(new UxTextField("Test Text"))
      .AddField(new UxToggleField("Test Toggle"))
      .StartNewColumn(new UxTitle("Test Col", "Tooltip for test col title"))
      .AddField(new UxToggleField("Test Toggle", dataKey: "toggle2"))
      .StartNewPannel(new UxPannel.Tab("pannel 3", tooltip: "pannel 2 tooltip"))
      .AddField(new UxTextField("Other Pannel Field"))
      .StartNewPannel(new UxPannel.Tab("pannel 4", tooltip: "pannel 2 tooltip"))
      .AddField(new UxTextField("Other Pannel Field2"))
      .StartNewPannel(new UxPannel.Tab("pannel 5", tooltip: "pannel 2 tooltip"))
      .AddField(new UxTextField("Other Pannel Field3"))
      .StartNewPannel(new UxPannel.Tab("pannel 6", tooltip: "pannel 2 tooltip"))
      .AddField(new UxTextField("Other Pannel Field4"))
      .StartNewPannel(new UxPannel.Tab("pannel 7", tooltip: "pannel 2 tooltip"))
      .AddField(new UxTextField("Other Field5"))
      .AddField(new UxTextField("Other Field6"))
      .AddField(new UxTextField("Other Field7"))
      .AddField(new UxTextField("Other Field8"))
      .AddField(new UxTextField("Other Field9"))
      .AddField(new UxTextField("Other Field10"))
      .AddField(new UxTextField("Other Pannel Field11"))
      .AddField(new UxTextField("Other Pannel Field543"))
      .AddField(new UxTextField("Other Pannel Field556"))
      .AddField(new UxTextField("Other Pannel Field5t"))
      .AddField(new UxTextField("Other Pannel Field5gfdh"))
    .Build();

    testView.InitializeFor(testModel);
  }
}
