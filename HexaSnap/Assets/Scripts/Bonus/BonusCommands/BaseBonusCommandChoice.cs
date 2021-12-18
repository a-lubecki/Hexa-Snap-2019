/**
 * Hexa Snap
 * © Aurélien Lubecki 2019
 * All Rights Reserved
 */

using System.Collections.Generic;
using System.Linq;


public abstract class BaseBonusCommandChoice : BaseBonusCommand {


	protected abstract HashSet<BonusType> getGenerableBonusTypes();


    public override void onItemBonusUsed(ItemBonus item) {

		Activity10 activity = item.activity;

		//propose 3 items then trigger this item
		ItemBonus[] newItems = new ItemBonus[Activity10c.NB_ITEMS_TO_CHOOSE];

        //keep a set of available bonuses
        var bonusTypes = getGenerableBonusTypes();

		for (int i = 0 ; i < Activity10c.NB_ITEMS_TO_CHOOSE ; i++) {

            //pick a bonus type to not have 2 same types
            var newBonusType = bonusTypes.ElementAt(Constants.newRandomPosInArray(bonusTypes.Count));
            bonusTypes.Remove(newBonusType);

			if (newBonusType == null) {
				//no item can be generated : ignore, let the clicked item destroyed
				return;
			}

			ItemBonus newItemBonus = new ItemBonus(activity, newBonusType);
			newItems[i] = newItemBonus;
		}

		activity.showBonusChoiceDialog(newItems);
	}

}
