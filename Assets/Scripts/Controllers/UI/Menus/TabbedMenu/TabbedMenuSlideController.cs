using UnityEngine;

namespace SpiritWorlds.Controllers {

  /// <summary>
  /// Adds up/down slide functionality to a tabbed menu controller when the open tab is clicked
  /// </summary>
  [DefaultExecutionOrder(10000)]
  [RequireComponent(typeof(TabbedMenuController))]
  public class TabbedMenuSlideController : MonoBehaviour {

    /// <summary>
    /// where it's set when open
    /// </summary>
    public float OpenPosY = -240F;

    /// <summary>
    /// where it's set when closed
    /// </summary>
    public float ClosedPosY = -740F;

    /// <summary>
    /// Speed of le slide
    /// </summary>
    public float SlideTime = 1.0f;

    /// <summary>
    /// if this is open now (also at start)
    /// </summary>
    public bool isSlidOpen = false;

    /// <summary>
    /// The rectransform
    /// </summary>
    RectTransform rectTransform;

    /// <summary>
    /// if it's sliding
    /// </summary>
    bool isSliding = false;

    /// <summary>
    /// the slide timer
    /// </summary>
    float timer = 0;

    /// <summary>
    /// Apply the slide capabilities to the tabs of the menu controller
    /// </summary>
    void Awake() {
      rectTransform = GetComponent<RectTransform>();
      TabbedMenuController tabbedMenuController = GetComponent<TabbedMenuController>();
      foreach(MenuTabController menuTab in tabbedMenuController.tabs) {
        menuTab.onClick = tabIsActive => {
          if(!isSliding) {
            if(isSlidOpen) {
              if(tabIsActive) {
                slideClosed();
              }
            } else {
              slideOpen();
            }
          }
        };
      }
    }

    /// <summary>
    /// Slide
    /// </summary>
    void Update() {
      if(isSliding) {
        timer += Time.deltaTime;
        rectTransform.anchoredPosition
          = rectTransform.anchoredPosition.ReplaceY(Mathf.Lerp(
            isSlidOpen ? ClosedPosY : OpenPosY,
            isSlidOpen ? OpenPosY : ClosedPosY,
            timer.scale(1, 0, SlideTime, 0)
        ));
        if(timer >= SlideTime) {
          isSliding = false;
        }
      }
    }

    void slideOpen() {
      isSlidOpen = true;
      isSliding = true;
      timer = 0;
    }

    void slideClosed() {
      isSlidOpen = false;
      isSliding = true;
      timer = 0;
    }
  }

  public static class RangeExtensions {

    /// <summary>
    /// Scale a float value to a new set of maxes and mins.
    /// </summary>
    /// <param name="value"></param>
    /// <param name="newMax"></param>
    /// <param name="newMin"></param>
    /// <param name="oldMax"></param>
    /// <param name="oldMin"></param>
    /// <returns></returns>
    public static float scale(this float value, float newMax, float newMin, float oldMax = 1.0f, float oldMin = -1.0f) {
      float scaled = newMin + (value - oldMin) / (oldMax - oldMin) * (newMax - newMin);
      return scaled;
    }
  }
}