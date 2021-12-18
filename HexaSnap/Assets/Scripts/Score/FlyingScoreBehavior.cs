/**
 * Hexa Snap
 * © Aurélien Lubecki 2019
 * All Rights Reserved
 */

using System;
using UnityEngine;
using UnityEngine.UI;


public class FlyingScoreBehavior : MonoBehaviour {

    private float alpha = 0;
    public float textAlpha {
        get {
            return alpha;
        }
        set {
            alpha = value;
        }
    }

    private bool isFlying = false;

    private Text textScore;
    private Animation animFly;


    private void Awake() {

        textScore = GetComponentInChildren<Text>();
        animFly = GetComponent<Animation>();

    }

    void OnDisable() {

        animFly.Stop();

        isFlying = false;

    }


    public void startFlying(string score, Action completion) {

        if (isFlying) {
            return;
        }

        isFlying = true;

        textScore.text = score;

        Constants.playAnimation(animFly, null, false);
        
        Async.call(animFly.clip.length, () => {

            isFlying = false;
            
            if (completion != null) {
                completion();
            }
        });
        
    }

}

