/**
 * Hexa Snap
 * © Aurélien Lubecki 2019
 * All Rights Reserved
 */

using System;
using System.Collections.Generic;
using UnityEngine;


public class BonusCommandRowWipeout : BaseBonusCommand {

	public override string[] getAllMultipleTagSuffixes() {
		return new string[] { 
			ItemSnapDirection.TOP.ToString(), 
			ItemSnapDirection.RIGHT_TOP.ToString(), 
			ItemSnapDirection.RIGHT_BOTTOM.ToString()
		};
	}

	public override string getItemTagSuffix(ItemBonus item) {

        if (item == null) {
            return ItemSnapDirection.TOP.ToString();
        }

		ItemSnapDirection randomDirection = objectToDirection(item.specificBonusObject);
		return randomDirection.ToString();
	}

	public override void onItemBonusSnapped(ItemBonus item) {

		if (item.specificBonusObject == null) {
			throw new InvalidOperationException();
		}

		if (item.isStacked) {
			//the item has the right direction
			return;
		}

		//change the row direction to have a coherent direction with the axis rotation
		ItemSnapDirection randomDirection = objectToDirection(item.specificBonusObject);

		float angleDegrees = item.activity.axisBehavior.getAngleDegrees();
		ItemSnapDirection newDirection = Constants.findDirection(angleDegrees + Constants.getAngle(randomDirection));

		//change the direction to have the right tag when getting the sprite name
		if (newDirection == ItemSnapDirection.BOTTOM) {
			newDirection = ItemSnapDirection.TOP;
		} else if (newDirection == ItemSnapDirection.LEFT_BOTTOM) {
			newDirection = ItemSnapDirection.RIGHT_TOP;
		} else if (newDirection == ItemSnapDirection.LEFT_TOP) {
			newDirection = ItemSnapDirection.RIGHT_BOTTOM;
		}

		item.updateSpecificBonusObject(directionToObject(newDirection));
	}

	public override void onItemBonusUsed(ItemBonus item) {

		if (item.specificBonusObject == null) {
			throw new InvalidOperationException();
		}

        AxisBehavior axisBehavior = item.activity.axisBehavior;
        Axis axis = axisBehavior.axis;

		ItemSnapDirection randomDirection = objectToDirection(item.specificBonusObject);

        float axisAngleDegrees = axisBehavior.getAngleDegrees();

        if (!item.isSnapped()) {

            //adapt direction with the axis orientation to have a coherent row destruction
            randomDirection = Constants.findDirection(axisAngleDegrees + Constants.getAngle(randomDirection));
        }

        ItemSnapPosition impactPos = item.impactPos;
        
        //play FX at pos
        Vector3 itemPos = Vector3.zero;
        if (impactPos != null) {
            itemPos = BaseModelBehavior.findTransform(item).position;
        }

        //rotate FX sprite => item icon angle - row bonus angle (0°, 60°, 120°) - sprite horizontal to vertical (90°)
        float fxAngleDegrees = axisBehavior.transform.eulerAngles.z - Constants.getAngle(randomDirection) - 90;
        playFX("FX.Bonus.RowWipeout", 1, item, itemPos, fxAngleDegrees);

        GameHelper.Instance.getAudioManager().playSound("Bonus.WIPEOUT");

        if (impactPos == null) {
            //no items to destroy
            return;
        }

        HashSet<Item> itemsToDestroy = new HashSet<Item>();
		bool mustUnsnapFreeItems = false;

        if (!item.isSnapped()) {

            //destroy random chosen item at impact point
            Item impactItem = axis.getSnappedItemAt(impactPos);

            itemsToDestroy.Add(impactItem);
            mustUnsnapFreeItems = true;
        }

		fillItemsToDestroy(axis, itemsToDestroy, impactPos, randomDirection);
		fillItemsToDestroy(axis, itemsToDestroy, impactPos, getOppositeDirection(randomDirection));

		//destroy items but don't unsnap free items because
		//it will be done at the end of the bonus execution
		//see Axis::selectBonusItem(ItemBonus), the current method is called through the "item.select();" call 
        axis.destroySnappedItems(itemsToDestroy, mustUnsnapFreeItems, ItemDestroyCause.System);

	}
    
    public override object newSpecificBonusObject() {
        return directionToObject((ItemSnapDirection) Constants.newRandomInt((int)ItemSnapDirection.TOP, (int)ItemSnapDirection.RIGHT_BOTTOM));
	}

	private object directionToObject(ItemSnapDirection direction) {
		return direction.ToString();
	}

	private ItemSnapDirection objectToDirection(object direction) {
		return (ItemSnapDirection) Enum.Parse(typeof(ItemSnapDirection), direction as string);
	}

	private void fillItemsToDestroy(Axis axis, HashSet<Item> itemsToDestroy, ItemSnapPosition newPos, ItemSnapDirection direction) {

		do {

			ItemSnapDirection nextDirection = (ItemSnapDirection) ((6 + (int)direction - (int)newPos.direction) % 6);

			if (newPos.level <= 0 && newPos.pos <= 0 && direction == getOppositeDirection(newPos.direction)) {
				//the next pos will be the axis, create a position on the opposite direction
				newPos = new ItemSnapPosition(direction, 0, 0);

			} else {

				newPos = newPos.newSiblingItemPosition(nextDirection);
			}

			Item newItem = axis.getSnappedItemAt(newPos);

			if (newItem != null) {
				itemsToDestroy.Add(newItem);
			}

		} while (newPos.level <= 5);//has reach max level ?

	}

	private ItemSnapDirection getYAxisOppositeDirection(ItemSnapDirection direction) {

		if (direction == ItemSnapDirection.TOP) {
			return ItemSnapDirection.TOP;
		}
		if (direction == ItemSnapDirection.BOTTOM) {
			return ItemSnapDirection.BOTTOM;
		}
		if (direction == ItemSnapDirection.LEFT_TOP) {
			return ItemSnapDirection.RIGHT_TOP;
		}
		if (direction == ItemSnapDirection.RIGHT_TOP) {
			return ItemSnapDirection.LEFT_TOP;
		}
		if (direction == ItemSnapDirection.LEFT_BOTTOM) {
			return ItemSnapDirection.RIGHT_BOTTOM;
		}
		if (direction == ItemSnapDirection.RIGHT_BOTTOM) {
			return ItemSnapDirection.LEFT_BOTTOM;
		}

		throw new NotSupportedException();
	}

	private ItemSnapDirection getOppositeDirection(ItemSnapDirection direction) {

		if (direction == ItemSnapDirection.BOTTOM) {
			return ItemSnapDirection.TOP;
		}
		if (direction == ItemSnapDirection.TOP) {
			return ItemSnapDirection.BOTTOM;
		}
		if (direction == ItemSnapDirection.LEFT_TOP) {
			return ItemSnapDirection.RIGHT_BOTTOM;
		}
		if (direction == ItemSnapDirection.RIGHT_BOTTOM) {
			return ItemSnapDirection.LEFT_TOP;
		}
		if (direction == ItemSnapDirection.LEFT_BOTTOM) {
			return ItemSnapDirection.RIGHT_TOP;
		}
		if (direction == ItemSnapDirection.RIGHT_TOP) {
			return ItemSnapDirection.LEFT_BOTTOM;
		}

		throw new NotSupportedException();
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

