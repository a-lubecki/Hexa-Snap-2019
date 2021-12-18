/**
 * Hexa Snap
 * © Aurélien Lubecki 2019
 * All Rights Reserved
 */

using System;
using System.Collections.Generic;
using UnityEngine;


public class BonusCommandSimilarWipeout : BaseBonusCommand {

	public override string[] getAllMultipleTagSuffixes() {
		return new string[] { 
			ItemType.Type1.ToString(),
			ItemType.Type5.ToString(),
			ItemType.Type20.ToString(),
			ItemType.Type100.ToString()
		};
	}

	public override string getItemTagSuffix(ItemBonus item) {

        if (item == null) {
            return ItemType.Type1.ToString();
        }

        ItemType randomItemType = objectToItemType(item.specificBonusObject);
		return randomItemType.ToString();
	}

	public override void onItemBonusUsed(ItemBonus item) {

		if (item.specificBonusObject == null) {
			throw new InvalidOperationException();
		}

		Axis axis = item.activity.axis;

		ItemType randomItemType = objectToItemType(item.specificBonusObject);

		HashSet<Item> itemsToDestroy = axis.getSnappedItemsOfType(randomItemType);

        GameHelper.Instance.getAudioManager().playSound("Bonus.WIPEOUT");

        if (itemsToDestroy.Count <= 0) {
            playFX(item, Vector3.zero);
            return;
        }
        
        //play the fx on all the destroyed items
        foreach (Item destroyedItem in itemsToDestroy) {
            playFX(item, BaseModelBehavior.findTransform(destroyedItem).position);
        }
        
		//destroy items but don't unsnap free items because
		//it will be done at the end of the bonus execution
		//see Axis::selectBonusItem(ItemBonus), the current method is called through the "item.select();" call 
        axis.destroySnappedItems(itemsToDestroy, !item.isSnapped(), ItemDestroyCause.Select);

	}

    private void playFX(ItemBonus item, Vector3 pos) {

        playFX("FX.Bonus.SimilarWipeout", 0.7f, item, pos);
    }

    public override object newSpecificBonusObject() {
		//return a random ItemType between Type1 to Type100
        return itemTypeToObject((ItemType) Constants.newRandomInt((int)ItemType.Type1, (int)ItemType.Type100));
	}

	private object itemTypeToObject(ItemType itemType) {
		return itemType.ToString();
	}

	private ItemType objectToItemType(object itemType) {
		return (ItemType)Enum.Parse(typeof(ItemType), itemType as string);
	}

    public override bool mustMoveItemBeforeDestroying(ItemBonus item) {
        return true;
    }
    
}

