/**
 * Hexa Snap
 * © Aurélien Lubecki 2019
 * All Rights Reserved
 */

using System.Collections.Generic;
using System.Linq;


public abstract class BaseBonusCommandRandom : BaseBonusCommand {
    

    private static readonly int MAX_LEVEL = 3;


    private ItemType nextItemType;


    protected abstract List<ItemSnapPosition> orderFreePositions(List<ItemSnapPosition> freePositions);

    protected abstract List<ItemType> getPossibleItemTypes();

    protected abstract int calculateTotalScore(int nbItems, int level);


    public override void onItemBonusUsed(ItemBonus item) {
        base.onItemBonusUsed(item);

        Activity10 activity = item.activity;
        Axis axis = activity.axis;

        //calculate the better new item type and position
        var nextItemPos = item.impactPos;
        if (nextItemPos == null) {
            //no position to add new item
            return;
        }

        Item newItem = new Item(activity, nextItemType);
        activity.registerItem(newItem);

        //create the item at the bonus position
        GameHelper.Instance.getPool().pickItemGameObject(
            newItem,
            null,
            false,
            BaseModelBehavior.findTransform(item).position
        );

        //attach the new item to the current item pos
        axis.snapItem(newItem, nextItemPos);
	}

    public override bool mustMoveItemBeforeDestroying(ItemBonus item) {
        return true;
    }

    public override ItemSnapPosition getImpactPos(Axis axis, ItemBonus item) {//used to know where the item must move (when stacked or chosen bonus)

        var freePositions = new List<ItemSnapPosition>(getFreeSiblingPositions(axis));
        if (freePositions.Count <= 0) {
            return null;
        }

        //order by level for progress
        freePositions = orderFreePositions(freePositions);

        var freeItemTypes = getPossibleItemTypes();

        //default score
        var possibleScore = new ItemPosScore(
            freeItemTypes.First(),
            freePositions.First(),
            0
        );

        var maxScore = -1;

        foreach (ItemType type in freeItemTypes) {
            
            foreach (ItemSnapPosition pos in freePositions) {

                var score = getScoreForPossibleItem(axis, type, pos);
                if (score <= maxScore) {
                    continue;
                }

                maxScore = score;

                possibleScore = new ItemPosScore(
                    type,
                    pos,
                    score
                );
            }
        }

        //save item type for next processing
        nextItemType = possibleScore.type;

        return possibleScore.position;
    }

    private HashSet<ItemSnapPosition> getFreeSiblingPositions(Axis axis) {

        HashSet<ItemSnapPosition> freePositions = new HashSet<ItemSnapPosition>();

        var itemPositions = axis.getSnappedItemPositions();
        if (itemPositions.Count <= 0) {
            //no items to snap
            freePositions.Add(new ItemSnapPosition(ItemSnapDirection.TOP, 0, 0));
            return freePositions;
        }

        foreach (ItemSnapPosition pos in itemPositions) {

            foreach (ItemSnapPosition possibleFreePos in pos.newSiblingItemPositions()) {

                if (possibleFreePos.level > MAX_LEVEL) {
                    continue;
                }

                if (!axis.hasSnappedItemAt(possibleFreePos)) {
                    freePositions.Add(possibleFreePos);
                }
            }
        }

        return freePositions;
    }

    private int getScoreForPossibleItem(Axis axis, ItemType type, ItemSnapPosition pos) {

        HashSet<Item> nearbyItemsGroup = new HashSet<Item>();

        foreach (Item nearbyItem in axis.getAdjacentItems(pos)) {

            if (nearbyItem.itemType != type) {
                continue;
            }

            if (nearbyItemsGroup.Contains(nearbyItem)) {
                //group already found
                continue;
            }

            nearbyItemsGroup.UnionWith(axis.getAttachedItemsGroup(nearbyItem, true));
        }

        int nbItems = nearbyItemsGroup.Count + 1;

        int level = 1 + MAX_LEVEL - pos.level;
        if (level < 0) {
            level = 0;
        }

        return calculateTotalScore(nbItems, level);
    }

}

class ItemPosScore {
    
    public readonly ItemType type;
    public readonly ItemSnapPosition position;
    public readonly int score;

    public ItemPosScore(ItemType type, ItemSnapPosition position, int score) {
            
        this.type = type;
        this.position = position;
        this.score = score;
    }
}