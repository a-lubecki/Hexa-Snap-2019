/**
 * Hexa Snap
 * © Aurélien Lubecki 2019
 * All Rights Reserved
 */

public class BonusCommandInversion : BaseBonusCommand {

	public override void onItemBonusUsed(ItemBonus item) {

        playFX("FX.Bonus.Inversion", 1f, item, item.activity.axisBehavior.transform.position);

        GameHelper.Instance.getAudioManager().playSound("Bonus.INVERSION");
	}

}

