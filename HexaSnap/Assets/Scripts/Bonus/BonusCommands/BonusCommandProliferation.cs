/**
 * Hexa Snap
 * © Aurélien Lubecki 2019
 * All Rights Reserved
 */

using System;
using System.Collections.Generic;

public class BonusCommandProliferation : BaseBonusCommand {

	public override void onItemBonusUsed(ItemBonus item) {

        Activity10 activity = item.activity;
        AxisBehavior axisBehavior = activity.axisBehavior;
        Axis axis = axisBehavior.axis;

        ItemSnapPosition impactPos = item.impactPos;

		HashSet<ItemSnapPosition> itemsPosToAdd = new HashSet<ItemSnapPosition>();
		Dictionary<ItemSnapPosition, Item> itemsToAdd = new Dictionary<ItemSnapPosition, Item>(itemsPosToAdd.Count);

		//get adjacent positions then get free items pos on these positions
        foreach (ItemSnapDirection d in Constants.getAllDirections()) {

			ItemSnapPosition newPos;

            if (impactPos == null) {
				
				//get axis sibling pos
				newPos = new ItemSnapPosition(d, 0, 0);

			} else {
				
				//get item sibling pos
                newPos = impactPos.newSiblingItemPosition(d);

				if (newPos == null) {
					continue;
				}

                if (axis.hasSnappedItemAt(newPos)) {
					//not a free pos
					continue;
				}
			}

			itemsPosToAdd.Add(newPos);

			//alternate type
			ItemType type = ((int)d % 2 == 0) ? ItemType.Type1 : ItemType.Type5;

			//create item and register it
			Item newItem = new Item(activity, type);
			activity.registerItem(newItem);

			//let pop the item close to the pos it must be, it will then snap naturally
			GameHelper.Instance.getPool().pickItemGameObject(
				newItem, 
				null, 
				false, 
				axisBehavior.calculateGameObjectPositionInGrid(newPos, false)
			);

			itemsToAdd.Add(newPos, newItem);
		}


		//order the list of items before adding them, they must have an adjacent item before snapping or the item won't be snapped at its exact pos
		while(itemsPosToAdd.Count > 0) {

			ItemSnapPosition chosenPos = null;

			foreach (ItemSnapPosition pos in itemsPosToAdd) {

				if (pos.level <= 0) {
					//the item is attached to the axis
					chosenPos = pos;
					break;
				}

                HashSet<Item> adjacentItems = axis.getAdjacentItems(pos);
				int nbAdjacentItems = adjacentItems.Count;

				if (nbAdjacentItems > 1 || (nbAdjacentItems == 1 && !adjacentItems.Contains(item))) {
					//the item is attached to another item but not the current bonus item
					chosenPos = pos;
					break;
				}
			}

			if (chosenPos == null) {
				throw new InvalidOperationException("The item can't be attached");
			}

			itemsPosToAdd.Remove(chosenPos);

			//finally snap
			axis.snapItem(itemsToAdd[chosenPos], chosenPos);
		}

	}

    public override bool mustMoveItemBeforeDestroying(ItemBonus item) {
        return true;
    }

    public override ItemSnapPosition getImpactPos(Axis axis, ItemBonus item) {//used to know where the item must move (when stacked or chosen bonus)

        ItemSnapPosition currentItemPos = base.getImpactPos(axis, item);

        if (currentItemPos != null) {
            return currentItemPos;
        }

        //get random position with free siblings
        return getRandomSnapPositionWithFreeSiblings(axis);
    }

    private ItemSnapPosition getRandomSnapPositionWithFreeSiblings(Axis axis) {

        HashSet<Item> snappedItems = axis.getSnappedItems();
        if (snappedItems.Count <= 0) {
            return null;
        }

        List<ItemSnapPosition> possiblePositions = new List<ItemSnapPosition>();

        foreach (Item item in snappedItems) {

            ItemSnapPosition pos = item.snapPosition;

            foreach (ItemSnapPosition siblingPos in pos.newSiblingItemPositions()) {

                if (!axis.hasSnappedItemAt(siblingPos)) {
                    //new items can pop around the current pos, add to the possible pos
                    possiblePositions.Add(pos);
                    break;
                }
            }

        }

        int nbItems = possiblePositions.Count;
        if (nbItems <= 0) {
            return null;
        }

        if (nbItems == 1) {
            return possiblePositions[0];
        }

        return possiblePositions[Constants.newRandomPosInArray(nbItems)];
    }

}

