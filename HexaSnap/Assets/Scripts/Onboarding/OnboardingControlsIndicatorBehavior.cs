/**
 * Hexa Snap
 * © Aurélien Lubecki 2019
 * All Rights Reserved
 */

using UnityEngine;


public class OnboardingControlsIndicatorBehavior : MonoBehaviour {


    private RectTransform rectTransform;
    private CanvasGroup canvasGroup;


    private void Awake() {
        
        rectTransform = GetComponent<RectTransform>();
        canvasGroup = GetComponent<CanvasGroup>();

        deactivateIndicator();
    }

    public void activateIndicator(Vector2 startPoint) {

        if (gameObject.activeSelf) {
            //already activated
            return;
        }

        gameObject.SetActive(true);

        rectTransform.position = startPoint;
        setWidth(0);
    }

    public void updateIndicator(float xPoint) {

        if (!gameObject.activeSelf) {
            //not started
            return;
        }

        setWidth(xPoint - rectTransform.position.x);
    }

    public void deactivateIndicator() {

        if (!gameObject.activeSelf) {
            //already deactivated
            return;
        }

        rectTransform.position = Vector2.zero;
        setWidth(0);

        gameObject.SetActive(false);
    }

    private void setWidth(float width) {
        
        if (width < 0) {
            rectTransform.localScale = new Vector3(-1, 1, 1);
        } else {
            rectTransform.localScale = Vector3.one;
        }

        width = Mathf.Abs(width);

        rectTransform.sizeDelta = new Vector2(100 * width, 20);

        //don't show the indicator if indicator is too short
        var alpha = Mathf.Abs(width) - 0.5f;
        if (alpha < 0) {
            setAlpha(0);
        } else if (alpha > 1) {
            setAlpha(1);
        } else {
            setAlpha(alpha);
        }
    }

    private void setAlpha(float alpha) {
        canvasGroup.alpha = alpha;
    }
}
