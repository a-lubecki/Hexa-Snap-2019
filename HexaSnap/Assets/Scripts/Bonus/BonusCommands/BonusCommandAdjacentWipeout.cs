/**
 * Hexa Snap
 * © Aurélien Lubecki 2019
 * All Rights Reserved
 */

using System;
using System.Collections.Generic;
using UnityEngine;


public class BonusCommandAdjacentWipeout : BaseBonusCommand {

    public override void onItemBonusUsed(ItemBonus item) {

        Axis axis = item.activity.axis;

        ItemSnapPosition impactPos = item.impactPos;

        //play FX at pos
        Vector3 itemPos = Vector3.zero;
        if (impactPos != null) {
            itemPos = BaseModelBehavior.findTransform(item).position;
        }

        playFX("FX.Bonus.AdjacentWipeout", 1, item, itemPos);

        GameHelper.Instance.getAudioManager().playSound("Bonus.WIPEOUT");

        if (impactPos == null) {
            //no items to destroy
            return;
        }

        HashSet<Item> itemsToDestroy = new HashSet<Item>();
        bool mustUnsnapFreeItems = false;

	    if (!item.isSnapped()) {
            
            //destroy random chosen item at impact point
            var impactItem = axis.getSnappedItemAt(impactPos);

            itemsToDestroy.Add(impactItem);
            mustUnsnapFreeItems = true;
        }

        //get adjacent positions then destroy snapped items on these positions
        foreach (var newPos in impactPos.newSiblingItemPositions()) {
            
			var nearbyItem = axis.getSnappedItemAt(newPos);

			if (nearbyItem == null) {
				continue;
			}

			itemsToDestroy.Add(nearbyItem);
		}

        //destroy items but don't unsnap free items because
        //it will be done at the end of the bonus execution
        //see Axis::selectBonusItem(ItemBonus), the current method is called through the "item.select();" call 
        axis.destroySnappedItems(itemsToDestroy, mustUnsnapFreeItems, ItemDestroyCause.System);
	}
    
    public override bool mustMoveItemBeforeDestroying(ItemBonus item) {
        return true;
    }
    
    public override ItemSnapPosition getImpactPos(Axis axis, ItemBonus item) {//used to know where the item must move (when stacked or chosen bonus)

        ItemSnapPosition currentItemPos = base.getImpactPos(axis, item);

        if (currentItemPos != null) {
            return currentItemPos;
        }

        Item randomItem = axis.getRandomSnappedItem();
        if (randomItem == null) {
            //no items to destroy
            return null;
        }

        return randomItem.snapPosition;
    }

}

