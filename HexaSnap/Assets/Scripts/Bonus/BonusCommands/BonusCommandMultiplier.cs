/**
 * Hexa Snap
 * © Aurélien Lubecki 2019
 * All Rights Reserved
 */

using UnityEngine;


public class BonusCommandMultiplier : BaseBonusCommand {
    
    public override void onItemBonusUsed(ItemBonus item) {
        
        Transform trMultiplier = item.activity.getHUD().transform.Find("ImageMultiplier").Find("ScoreMultiplier");

        playFX("FX.Bonus.Multiplier", 0.6f, item, trMultiplier.position);
    }

}

