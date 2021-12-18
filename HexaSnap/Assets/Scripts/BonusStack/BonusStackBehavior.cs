/**
 * Hexa Snap
 * © Aurélien Lubecki 2019
 * All Rights Reserved
 */

using UnityEngine;


public class BonusStackBehavior : InGameModelBehavior, BonusStackListener {

	public BonusStack bonusStack {
		get {
			return (BonusStack) model;
		}
	}


	private GameObject[] slots;

	protected override void onAwake() {
		base.onAwake();

		slots = new GameObject[BonusStack.maxStackSize];

		for (int i = 0 ; i < slots.Length ; i++) {
			slots[i] = transform.GetChild(i).gameObject;
		}
	}
    

    protected override void onInit() {
        base.onInit();

        updateBonusStackSlots();
    }

    private void updateBonusStackSlots() {

        int stackSize = bonusStack.stackSize;

        for (int i = 0 ; i < slots.Length ; i++) {

            SpriteRenderer srSlot = slots[i].GetComponent<SpriteRenderer>();

            Color color = srSlot.color;
            color.a = (i < stackSize) ? 1 : 0.15f;
            srSlot.color = color;
        }

    }

    BaseModelBehavior BaseModelListener.getModelBehavior() {
		return this;
	}

    void BonusStackListener.onBonusStackEnableChange(BonusStack bonusStack) {

        bool isEnabled = bonusStack.isEnabled;

        for (int i = 0 ; i < slots.Length ; i++) {
            slots[i].SetActive(bonusStack.isEnabled);
        }

    }

    void BonusStackListener.onBonusStackSizeChange(BonusStack bonusStack, int previousStackSize) {

        updateBonusStackSlots();
	}

	void BonusStackListener.onBonusStackItemBonusAdd(BonusStack bonusStack, ItemBonus itemBonus) {
		
		foreach (ItemBonusBehavior ibb in transform.GetComponentsInChildren<ItemBonusBehavior>()) {
			
			moveItemBonus(ibb);
		}

		addItemBonus(itemBonus);
	}

	void BonusStackListener.onBonusStackItemBonusRemove(BonusStack bonusStack, ItemBonus itemBonus) {

		foreach (ItemBonusBehavior ibb in transform.GetComponentsInChildren<ItemBonusBehavior>()) {

			if (itemBonus == ibb.itemBonus) {
				
				removeItemBonus(ibb);

			} else {
				
				moveItemBonus(ibb);
			}
		}

	}

	void BonusStackListener.onBonusStackClear(BonusStack bonusStack) {

		foreach (ItemBonusBehavior ibb in transform.GetComponentsInChildren<ItemBonusBehavior>()) {
			removeItemBonus(ibb);
		}
	}

	private void addItemBonus(ItemBonus itemBonus) {

		ItemBonusBehavior ib = BaseModelBehavior.findModelBehavior<ItemBonusBehavior>(itemBonus);

		if (!ib.isInit()) {
			return;
		}

		ib.transform.SetParent(transform);

		Vector3 foreseenCurrentPos = ib.getLastAnimatedPos();
		Vector3 endPos = getItemEndPosition(0);

		//separate anims in two parts
		float firstAnimTime = 0.2f * Constants.INTERPOLATION_STACK_IN;
		float secondAnimTime = Constants.INTERPOLATION_STACK_IN - firstAnimTime;

		//translate on the x axis only
		ib.animatePositions(
			new PositionInterpolatorBundle[] {
				new PositionInterpolatorBundle(
					new Vector3(foreseenCurrentPos.x, endPos.y, foreseenCurrentPos.z),
					firstAnimTime, 
					InterpolatorCurve.EASE_IN
				),
				new PositionInterpolatorBundle(
					endPos,
					secondAnimTime,
					InterpolatorCurve.EASE_OUT
				)
			},
			false
		);

        Async.call(Constants.INTERPOLATION_STACK_IN + 0.25f, playSnapSound);
	}

	private void removeItemBonus(ItemBonusBehavior ibb) {

		ibb.transform.SetParent(null);

		float remaininDelay = ibb.getAnimationRemainingTimeSec();
		ibb.setDelayBeforeUnregister(remaininDelay + Constants.INTERPOLATION_STACK_OUT);

		Vector3 foreseenCurrentPos = ibb.getLastAnimatedPos();
		foreseenCurrentPos.x += 10;

		ibb.animatePosition(
			new PositionInterpolatorBundle(
				foreseenCurrentPos,
				Constants.INTERPOLATION_ITEMS_GENERATOR_OUT,
				InterpolatorCurve.EASE_IN
			),
			false
		);

	}

	private void moveItemBonus(ItemBonusBehavior ibb) {

		int index = bonusStack.getStackPos(ibb.itemBonus);
		ibb.animatePosition(
			new PositionInterpolatorBundle(
				getItemEndPosition(index),
				Constants.INTERPOLATION_ITEMS_GENERATOR_MOVE,
				InterpolatorCurve.EASE_IN_OUT
			),
			false
        );

        Async.call(Constants.INTERPOLATION_ITEMS_GENERATOR_MOVE, playSnapSound);
	}
    
    public Vector3 getItemEndPosition(int index) {
        return Constants.newVector3(slots[index].transform.position, 0, 0, -1);
    }

    private void playSnapSound() {
        GameHelper.Instance.getAudioManager().playSound("Item.Snap." + Constants.newRandomInt(1, 4));
    }
}

