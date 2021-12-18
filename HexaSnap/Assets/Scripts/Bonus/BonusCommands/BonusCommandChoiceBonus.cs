/**
 * Hexa Snap
 * © Aurélien Lubecki 2019
 * All Rights Reserved
 */

using System.Collections.Generic;

public class BonusCommandChoiceBonus : BaseBonusCommandChoice {

	protected override HashSet<BonusType> getGenerableBonusTypes() {

		BonusManager bonusManager = GameHelper.Instance.getBonusManager();

		HashSet<BonusType> res = new HashSet<BonusType>();

		res.Add(bonusManager.bonusTypeAdjacentWipeout);
		res.Add(bonusManager.bonusTypeExtension);
		res.Add(bonusManager.bonusTypeMultiplier);
		res.Add(bonusManager.bonusTypeProgression);
		res.Add(bonusManager.bonusTypeRowWipeout);
		res.Add(bonusManager.bonusTypeShortage);
        res.Add(bonusManager.bonusTypeSimilarWipeout);
        res.Add(bonusManager.bonusTypeSlowDown);
        res.Add(bonusManager.bonusTypeRandomBonus);

		return res;
	}

}

