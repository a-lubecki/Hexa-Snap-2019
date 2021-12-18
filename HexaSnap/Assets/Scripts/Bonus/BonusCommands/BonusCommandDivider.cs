/**
 * Hexa Snap
 * © Aurélien Lubecki 2019
 * All Rights Reserved
 */

using UnityEngine;


public class BonusCommandDivider : BaseBonusCommand {
    
    public override void onItemBonusUsed(ItemBonus item) {
        
        Transform trMultiplier = item.activity.getHUD().transform.Find("ImageMultiplier").Find("ScoreMultiplier");

        playFX("FX.Bonus.Divider", 0.6f, item, trMultiplier.position);
    }

}

