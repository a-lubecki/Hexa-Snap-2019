/**
 * Hexa Snap
 * © Aurélien Lubecki 2019
 * All Rights Reserved
 */


public class BonusCommandSlowDown : BaseBonusCommand {
    
    public override void onItemBonusUsed(ItemBonus item) {

        GameHelper.Instance.getAudioManager().playSound("Bonus.SLOW_DOWN");
    }

}

