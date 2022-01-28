using Overworld.Controllers.SimpleUx;
using Overworld.Ux.Simple;
using UnityEngine;

public class Test_SimpleUx : MonoBehaviour {

  [SerializeField]
  SimpleUxViewController _simpleUxViewController;

  // Start is called before the first frame update
  void Start() {
    var testView = Instantiate(_simpleUxViewController, transform);
    var testModel = new UxViewBuilder("Test")
      .AddField(new UxTextField("Test Field"))
    .Build();

    testView.InitializeFor(testModel);
  }
}
