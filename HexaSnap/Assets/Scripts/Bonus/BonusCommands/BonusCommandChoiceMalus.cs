/**
 * Hexa Snap
 * © Aurélien Lubecki 2019
 * All Rights Reserved
 */

using System.Collections.Generic;

public class BonusCommandChoiceMalus : BaseBonusCommandChoice {

	protected override HashSet<BonusType> getGenerableBonusTypes() {

		BonusManager bonusManager = GameHelper.Instance.getBonusManager();

		HashSet<BonusType> res = new HashSet<BonusType>();

		res.Add(bonusManager.bonusTypeDivider);
		res.Add(bonusManager.bonusTypeErosion);
		res.Add(bonusManager.bonusTypeInversion);
		res.Add(bonusManager.bonusTypeLimitation);
		res.Add(bonusManager.bonusTypeProfusion);
		res.Add(bonusManager.bonusTypeProliferation);
		res.Add(bonusManager.bonusTypeRegression);
		res.Add(bonusManager.bonusTypeSpeedUp);
        res.Add(bonusManager.bonusTypeRandomMalus);

		return res;
	}

}

