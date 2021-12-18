/**
 * Hexa Snap
 * © Aurélien Lubecki 2019
 * All Rights Reserved
 */


public class BonusCommandSpeedUp : BaseBonusCommand {
    
    public override void onItemBonusUsed(ItemBonus item) {

        GameHelper.Instance.getAudioManager().playSound("Bonus.SPEED_UP");
    }

}

