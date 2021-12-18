/**
 * Hexa Snap
 * © Aurélien Lubecki 2019
 * All Rights Reserved
 */

using UnityEngine;


public class ItemsGeneratorBehavior : InGameModelBehavior, ItemsGeneratorListener {

	public ItemsGenerator itemsGenerator {
		get {
			return (ItemsGenerator) model;
		}
	}

    
    BaseModelBehavior BaseModelListener.getModelBehavior() {
		return this;
	}

	void ItemsGeneratorListener.onItemsGeneratorStart(ItemsGenerator itemsGenerator) {
		//TODO animate background
	}

	void ItemsGeneratorListener.onItemsGeneratorStop(ItemsGenerator itemsGenerator) {
		//TODO stop background anim
	}

	void ItemsGeneratorListener.onItemsGeneratorAddItem(ItemsGenerator itemsGenerator, Item item) {

		Vector3 endPos = getItemEndPosition(itemsGenerator.getItemPos(item));
		GameObject gameObjectItem = GameHelper.Instance.getPool().pickItemGameObject(
			item, 
			transform, 
			false, 
			Constants.newVector3(endPos, 0, 1.5f, 0)
		);

		//animate from top to the current position
		gameObjectItem.GetComponent<ItemBehavior>().animatePosition(
			new PositionInterpolatorBundle(
				endPos, 
				Constants.INTERPOLATION_ITEMS_GENERATOR_IN, 
				InterpolatorCurve.EASE_IN
			),
			false
		);
	}

	void ItemsGeneratorListener.onItemsGeneratorRemoveItem(ItemsGenerator itemsGenerator, Item item) {

		foreach (ItemBehavior ib in transform.GetComponentsInChildren<ItemBehavior>()) {

			if (item == ib.item) {
				
				removeItem(ib);
			
			} else {

				moveItem(ib);
			}
		}

        GameHelper.Instance.getAudioManager().playSound("Item.Generation");
	}

	void ItemsGeneratorListener.onItemsGeneratorDequeueItem(ItemsGenerator itemsGenerator, Item item) {
		
		foreach (ItemBehavior ib in transform.GetComponentsInChildren<ItemBehavior>()) {

			if (item == ib.item) {

				//move the dequeued item to the dropping point
				ib.transform.SetParent(null);
				ib.animatePosition(
					new PositionInterpolatorBundle(
						transform.position, 
						Constants.INTERPOLATION_ITEMS_GENERATOR_MOVE, 
						InterpolatorCurve.EASE_IN
					),
					false
				);

			} else {

				//move the rest of the items
				moveItem(ib);
			}
		}

        GameHelper.Instance.getAudioManager().playSound("Item.Generation");
	}

	void ItemsGeneratorListener.onItemsGeneratorClearItems(ItemsGenerator itemsGenerator) {

		foreach (ItemBehavior ib in transform.GetComponentsInChildren<ItemBehavior>()) {
			
			ib.setDelayBeforeUnregister(Constants.INTERPOLATION_ITEMS_GENERATOR_OUT);

			removeItem(ib);
		}
	}

	private Vector3 getItemEndPosition(int index) {
		return Constants.newVector3(transform.position, -1.1f * (1 + index), 0, 0);
	}


	private void removeItem(ItemBehavior ib) {

		ib.transform.SetParent(null);

		Vector3 foreseenCurrentPos = ib.getLastAnimatedPos();
		foreseenCurrentPos.y -= 1.5f;

		ib.animatePosition(
			new PositionInterpolatorBundle(
				foreseenCurrentPos,
				Constants.INTERPOLATION_ITEMS_GENERATOR_OUT, 
				InterpolatorCurve.EASE_OUT
			),
			true
		);
	}

	private void moveItem(ItemBehavior ib) {

		int index = itemsGenerator.getItemPos(ib.item);
		ib.animatePosition(
			new PositionInterpolatorBundle(
				getItemEndPosition(index), 
				Constants.INTERPOLATION_ITEMS_GENERATOR_MOVE, 
				InterpolatorCurve.EASE_IN
			),
			false
		);
	}

}
