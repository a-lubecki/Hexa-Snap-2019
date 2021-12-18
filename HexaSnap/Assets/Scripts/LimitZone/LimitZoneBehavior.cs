/**
 * Hexa Snap
 * © Aurélien Lubecki 2019
 * All Rights Reserved
 */

using UnityEngine;


public class LimitZoneBehavior : MonoBehaviour {


    void Update() {

        var gameManager = GameHelper.Instance.getGameManager();
        var activity = gameManager.getNullableInGameActivity();

        var alpha = 1f;

        if (activity != null && gameManager.isGamePlaying && activity.timeManager.getTotalTimeScalePlay() <= 0) {
            alpha = 0.3f;
        }

        GetComponent<SpriteRenderer>().color = new Color(1, 1, 1, alpha);
    }

}
