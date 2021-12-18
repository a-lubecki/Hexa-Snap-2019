/**
 * Hexa Snap
 * © Aurélien Lubecki 2019
 * All Rights Reserved
 */

using System;
using System.Linq;
using System.Collections.Generic;
using UnityEngine;


public class Axis : InGameModel {

	private static AxisListener to(BaseModelListener listener) {
		return (AxisListener) listener;
	}


    public bool isRotationLocked = false;

	private Dictionary<ItemSnapPosition, Item> itemsPositions = new Dictionary<ItemSnapPosition, Item>();


	public Axis(Activity10 activity) : base(activity) {

	}

	public void setRotationAngle(float angle) {

        if (isRotationLocked) {
            return;
        }

        notifyListeners(listener => {
			to(listener).onRotationAngleChange(this, angle);
		});
    }

    public void rotateClockwise(float force) {

        if (isRotationLocked) {
            return;
        }

        notifyListeners(listener => {
            to(listener).onRotationForceClockwiseChange(this, force * activity.bonusQueue.getEnqueuedRotationMultiplier());
        });
    }

    public void rotateCounterClockwise(float force) {

        if (isRotationLocked) {
            return;
        }

        notifyListeners(listener => {
            to(listener).onRotationForceCounterClockwiseChange(this, force * activity.bonusQueue.getEnqueuedRotationMultiplier());
        });
    }

    public int getNbSnappedItems() {
        return itemsPositions.Count;
    }

    public HashSet<Item> getSnappedItems() {
        return new HashSet<Item>(itemsPositions.Values);
    }

    public HashSet<ItemSnapPosition> getSnappedItemPositions() {
        return new HashSet<ItemSnapPosition>(itemsPositions.Keys);
    }

	public Item getRandomSnappedItem() {

        int nbItems = itemsPositions.Count;
        if (nbItems <= 0) {
            return null;
        }

        if (nbItems == 1) {
            return itemsPositions.First().Value;
        }

        int randomPos = Constants.newRandomPosInArray(nbItems);
		return itemsPositions.Values.ToList()[randomPos];
	}

	public HashSet<Item> getSnappedItemsOfType(ItemType type) {

		HashSet<Item> items = new HashSet<Item>();

		foreach (Item item in itemsPositions.Values) {

			if (item.itemType == type) {
				items.Add(item);
			}
		}
		
		return items;
	}


	public bool hasSnappedItemAt(ItemSnapPosition pos) {

		if (pos == null) {
			throw new ArgumentException();
		}

		return itemsPositions.ContainsKey(pos);
	}

	public Item getSnappedItemAt(ItemSnapPosition pos) {

        if (pos == null) {
            throw new ArgumentException();
        }

        Item item = null;
        itemsPositions.TryGetValue(pos, out item);

		return item;
	}

	public void snapItem(Item item, ItemSnapPosition pos) {

		if (item == null) {
			throw new ArgumentException();
		}
		if (pos == null) {
			throw new ArgumentException();
		}

		if (item.snapPosition != null) {
			throw new InvalidOperationException("Item already snapped");
		}
		if (itemsPositions.ContainsKey(pos)) {
			throw new InvalidOperationException("Can't add, already contains items for pos : " + pos);
		}

		itemsPositions.Add(pos, item);
		item.snap(pos);

		updateSelectableItems();
	}

	private void updateSelectableItems() {

		HashSet<Item> selectableItems = new HashSet<Item>();
        HashSet<Item> checkedItems = new HashSet<Item>();

		//find the selectable items
		foreach (Item item in itemsPositions.Values) {

			if (checkedItems.Contains(item)) {
				//already checked
				continue;
			}

			if (item.itemType == ItemType.Bonus) {

				selectableItems.Add(item);
                checkedItems.Add(item);

			} else {
				//try to find if the non-bonus item can be selected
				HashSet<Item> group = getAttachedItemsGroup(item, true);

                checkedItems.UnionWith(group);

				if (group.Count < Constants.NB_ITEMS_TO_SELECT_GROUP) {
					continue;
				}

				//retain all items of the group
				foreach (Item grouppedItem in group) {
					selectableItems.Add(grouppedItem);
				}
			}

		}

		//set as selectable or not
		foreach (Item item in itemsPositions.Values) {
			item.setSelectable(selectableItems.Contains(item));
		}
	}

	public void unsnapItem(Item item) {

		if (item == null) {
			throw new ArgumentException();
		}

		ItemSnapPosition pos = item.snapPosition;
		if (pos == null) {
			throw new InvalidOperationException("Item not snapped any more");
		}
		if (!itemsPositions.ContainsKey(pos)) {
			throw new InvalidOperationException("Can't remove, doesn't contains pos : " + pos);
		}

		itemsPositions.Remove(pos);
		item.unsnap();

		updateSelectableItems();
	}

	public void unsnapAllItems() {

		List<Item> items = new List<Item>(itemsPositions.Values);

		//clear before unsnapping to avoid bugs
		itemsPositions.Clear();

		foreach (Item item in items) {
			item.unsnap();
		}

		updateSelectableItems();
	}

	public HashSet<Item> getAdjacentItems(ItemSnapPosition originPos) {

		if (originPos == null) {
			throw new ArgumentException();
		}

		HashSet<Item> res = new HashSet<Item>();

		foreach (ItemSnapPosition pos in originPos.newSiblingItemPositions()) {

			Item item = getSnappedItemAt(pos);
			if (item == null) {
				continue;
			}

			res.Add(item);
		}

		return res;
	}

	public HashSet<Item> getNearbyItemsGroup(Item originItem) {

		if (originItem == null) {
			throw new ArgumentException();
		}
		if (originItem.snapPosition == null) {
			throw new ArgumentException();
		}

		HashSet<Item> res = new HashSet<Item>();

		res.Add(originItem);

		foreach (ItemSnapPosition pos in originItem.snapPosition.newSiblingItemPositions()) {

			Item item = getSnappedItemAt(pos);
			if (item == null) {
				continue;
			}

			if (originItem.itemType != item.itemType) {
				continue;
			}

			res.Add(item);
		}

		return res;
	}

    public HashSet<Item> getAttachedItemsGroup(Item originItem, bool selectSameItemType) {

		if (originItem == null) {
			throw new ArgumentException();
		}
		if (originItem.snapPosition == null) {
			throw new ArgumentException();
		}

		var res = new HashSet<Item>();
        var checkedPositions = new HashSet<ItemSnapPosition>();

        res.Add(originItem);
        checkedPositions.Add(originItem.snapPosition);

        fillAttachedItemsGroup(originItem, originItem.snapPosition, selectSameItemType, res, checkedPositions);

		return res;
	}

    private void fillAttachedItemsGroup(Item originItem, ItemSnapPosition currentPos, bool selectSameItemType, HashSet<Item> attachedItems, HashSet<ItemSnapPosition> checkedPositions) {
        
        foreach (ItemSnapPosition pos in currentPos.newSiblingItemPositions()) {

            if (checkedPositions.Contains(pos)) {
                continue;
            }

            checkedPositions.Add(pos);

			Item item = getSnappedItemAt(pos);
			if (item == null) {
				continue;
			}

            if (selectSameItemType && item.itemType != originItem.itemType) {
                continue;
            }

            attachedItems.Add(item);

            fillAttachedItemsGroup(originItem, item.snapPosition, selectSameItemType, attachedItems, checkedPositions);
		}
	}

	public void unsnapFreeItems() {
        
        var closestItems = new HashSet<Item>();
        var nonFreeItems = new HashSet<Item>();

        //check if there are some items snapped to the axis
        foreach (var d in Constants.getAllDirections()) {
            
            var item = getSnappedItemAt(new ItemSnapPosition(d, 0, 0));
            if (item == null) {
                continue;
            }

            closestItems.Add(item);
        }

        //for the 6 possible items, get the other non-free items
        foreach (var item in closestItems) {

            if (nonFreeItems.Contains(item)) {
                //the magic : if an item from the closest 6 has already been checked, don't check it again
                continue;
            }

            //add the sibling and the current items to the non-free items
            var itemsGroup = getAttachedItemsGroup(item, false);

            nonFreeItems.UnionWith(itemsGroup);
        }

        //all items that are not in the non-free hashset are free
        var freeItems = new HashSet<Item>(itemsPositions.Values);
        freeItems.ExceptWith(nonFreeItems);

		foreach (Item item in freeItems) {
			unsnapItem(item);
		}
	}

    public void destroySnappedItem(Item itemToDestroy, bool mustUnsnapFreeItems, ItemDestroyCause cause) {

		if (itemToDestroy == null) {
			throw new ArgumentException();
		}

		HashSet<Item> items = new HashSet<Item>();
		items.Add(itemToDestroy);

		destroySnappedItems(items, mustUnsnapFreeItems, cause);
	}

    public void destroySnappedItems(HashSet<Item> itemsToDestroy, bool mustUnsnapFreeItems, ItemDestroyCause cause) {

		if (itemsToDestroy == null) {
			throw new ArgumentException();
		}

		foreach (Item item in itemsToDestroy) {

			unsnapItem(item);
            item.destroy(cause);
		}

		if (mustUnsnapFreeItems) {
			unsnapFreeItems();
		}
	}

	public override string ToString () {

		//avoid GC work with strings concat for release build
		if (Debug.isDebugBuild) {

			String res = "";
			foreach (ItemSnapPosition pos in itemsPositions.Keys) {

				Item item = itemsPositions[pos];
				res += item.itemType + " - " + item.isSnapped() + " - " + item.isSelected + " => " + pos + " (" + item.snapPosition + ")\n";
			}

			return res;
		}

		return base.ToString();
	}

    public int getFreeLayerLevel() {

        int level = -1;

        foreach (ItemSnapPosition pos in itemsPositions.Keys) {

            if (pos.level > level) {
                level = pos.level;
            }
        }

        return level + 1;
    }

    public HashSet<Item> getItemsOnLayerLevel(int level) {

        HashSet<Item> res = new HashSet<Item>();

        foreach (KeyValuePair<ItemSnapPosition, Item> elem in itemsPositions) {

            if (level == elem.Key.level) {
                res.Add(elem.Value);
            }
        }

        return res;
    }
    
    public bool isItemOutsideLimit(ItemSnapPosition snapPosition) {

        int level = snapPosition.level;
        int pos = snapPosition.pos;

        if (level <= 3) {
            return false;
        }

        if (level <= 4) {

            if (pos >= 1) {
                return false;
            }
        }

        return true;
    }

}
