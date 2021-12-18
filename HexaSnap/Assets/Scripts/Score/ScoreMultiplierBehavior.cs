/**
 * Hexa Snap
 * © Aurélien Lubecki 2019
 * All Rights Reserved
 */
 
using UnityEngine;
using UnityEngine.UI;


public class ScoreMultiplierBehavior : InGameModelBehavior, ScoreMultiplierListener {

    public ScoreMultiplier scoreMultiplier {
        get {
            return (ScoreMultiplier) model;
        }
    }


    private Text textMultiplier;
    private Image imageTimerEnd;

    private float lastTimerAlpha;

    protected override void onAwake() {
        base.onAwake();

        textMultiplier = GetComponent<Text>();
        imageTimerEnd = transform.parent.Find("ImageTimerEnd").GetComponent<Image>();

    }

    protected override void onInit() {
        base.onInit();

        updateMultiplier();
        updateTimerEnd();
    }

    private void updateMultiplier() {

        textMultiplier.text = "x" + ((int) scoreMultiplier.itemsMultiplier).ToString();
    }

    private void updateTimerEnd() {
        
        if (scoreMultiplier.isTimerEndRunning()) {

            imageTimerEnd.enabled = true;
            imageTimerEnd.fillAmount = 1 - scoreMultiplier.getTimerEndProgressPercentage();

            textMultiplier.color = Constants.COLOR_BONUS_MULTIPLIER;

        } else {

            imageTimerEnd.enabled = false;

            textMultiplier.color = Constants.COLOR_TITLE;
        }
    }

    BaseModelBehavior BaseModelListener.getModelBehavior() {
        return this;
    }

    void ScoreMultiplierListener.onMultiplierChanged(ScoreMultiplier scoreMultiplier) {

        updateMultiplier();
        updateTimerAlpha(1);
    }
    
    void ScoreMultiplierListener.onTimerUpdate(ScoreMultiplier scoreMultiplier) {

        updateTimerEnd();
        blinkTimerBeforeEnd();
    }

    private void blinkTimerBeforeEnd() {

        float alpha = 1;

        //blink every 3 seconds
        float remainingTimeSec = scoreMultiplier.getTimerEndRemainingTimeSec();
        if (remainingTimeSec < 3) {

            // dt : 0 => 1
            float deltaTime = 1 - remainingTimeSec % 1;
             
            if (deltaTime > 0.75f) {
                //dt : 0.75 => 1 / sdt : 0 => 1 / a : 0.3 to 0.1
                float sectionDeltaTime = (deltaTime - 0.75f) * 4;
                alpha = 1 - ((1 - sectionDeltaTime) * 0.6f);
            } else if (deltaTime > 0.5f) {
                //sdt : 0.5 => 0.75 / sdt : 0 => 1 / a : 1 to 0.3
                float sectionDeltaTime = (deltaTime - 0.5f) * 4;
                alpha = 1 - (sectionDeltaTime * 0.6f);
            } 
        }

        updateTimerAlpha(alpha);
    }

    private void updateTimerAlpha(float alpha) {
        
        if (alpha == lastTimerAlpha) {
            return;
        }

        lastTimerAlpha = alpha;
        
        Color c = textMultiplier.color;
        textMultiplier.color = new Color(c.r, c.g, c.b, alpha);
    }

}

