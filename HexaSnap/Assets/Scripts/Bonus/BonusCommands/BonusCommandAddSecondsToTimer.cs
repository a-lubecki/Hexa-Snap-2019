/**
 * Hexa Snap
 * © Aurélien Lubecki 2019
 * All Rights Reserved
 */

using System;
using UnityEngine;


public class BonusCommandAddSecondsToTimer : BaseBonusCommand {

    private int nbSecondsToAdd;

    public BonusCommandAddSecondsToTimer(int nbSecondsToAdd) {

        if (nbSecondsToAdd == 0) {
            throw new ArgumentException();
        }

        this.nbSecondsToAdd = nbSecondsToAdd;
    }

    public override void onItemBonusUsed(ItemBonus item) {
        
        Activity10b activity = (item.activity as Activity10b);
        Transform trTimer = activity.getHUD().transform.Find("TextTimer");

        string animName = (nbSecondsToAdd < 0) ? "FX.Bonus.TimeMinus" : "FX.Bonus.TimePlus";
        playFX(animName, 0.6f, item, trTimer.position);

        activity.addSeconds(nbSecondsToAdd);

        Constants.playAnimation(trTimer.GetComponent<Animation>(), null, false);

        if (nbSecondsToAdd > 0) {
            GameHelper.Instance.getAudioManager().playSound("Bonus.MORE_SEC");
        } else {
            GameHelper.Instance.getAudioManager().playSound("Bonus.LESS_SEC");
        }
    }

}

