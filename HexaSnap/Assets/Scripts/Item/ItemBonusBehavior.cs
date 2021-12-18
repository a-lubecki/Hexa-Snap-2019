/**
 * Hexa Snap
 * © Aurélien Lubecki 2019
 * All Rights Reserved
 */

using UnityEngine;


public class ItemBonusBehavior : ItemBehavior, ItemBonusListener {


    public static Sprite spriteBgHexagonBonus { get; private set; }
    public static Sprite spriteBgHexagonMalus { get; private set; }
    public static Sprite spriteBgCircleBonus { get; private set; }
    public static Sprite spriteBgCircleMalus { get; private set; }


    public ItemBonus itemBonus {
		get {
			return (ItemBonus) item;
		}
	}

    private SpriteRenderer spriteRendererIcon;
	private SpriteRenderer spriteRendererTimerMalus;

    private Transform trMaskParent;
    private Transform trMaskTimerMalus;


	protected override void onAwake() {
		base.onAwake();

		if (spriteBgHexagonBonus == null) {
			
			spriteBgHexagonBonus = GameHelper.Instance.loadSpriteAsset(Constants.PATH_DESIGNS_OBJECTS + "Item.Hexagon.Bonus");
			spriteBgHexagonMalus = GameHelper.Instance.loadSpriteAsset(Constants.PATH_DESIGNS_OBJECTS + "Item.Hexagon.Malus");
			spriteBgCircleBonus = GameHelper.Instance.loadSpriteAsset(Constants.PATH_DESIGNS_OBJECTS + "Item.Circle.Bonus");
			spriteBgCircleMalus = GameHelper.Instance.loadSpriteAsset(Constants.PATH_DESIGNS_OBJECTS + "Item.Circle.Malus");
		}

		spriteRendererBackground = transform.Find("SpriteItemBackground").GetComponent<SpriteRenderer>();
		spriteRendererIcon = transform.Find("SpriteItemIcon").GetComponent<SpriteRenderer>();
		spriteRendererTimerMalus = transform.Find("SpriteTimerMalus").GetComponent<SpriteRenderer>();
        
        trMaskParent = transform.Find("MaskPivot");
        trMaskTimerMalus = trMaskParent.Find("MaskTimerMalus");
	}
    
    protected override void onInit() {
        base.onInit();
    
        updateForegroundIcon();

        //reset the rotation of the icon in case the bonus type is direction based
        spriteRendererIcon.transform.localRotation = Quaternion.identity;

        //reset the rotation of the mask => horizontal
        trMaskParent.rotation = Quaternion.identity;
        
        updateBackground();
		updateTimerBackground();
	}

	protected override void onUpdate() {
		base.onUpdate();

		if (!itemBonus.bonusType.isDirectionBased) {
			//the bonus image is always horizontal then the axis rotates
			spriteRendererIcon.transform.rotation = Quaternion.identity;
        }

        //the mask must always be horizontal
        trMaskParent.rotation = Quaternion.identity;
    }

    private void updateForegroundIcon() {

        if (!itemBonus.bonusType.hasIcon) {
            spriteRendererIcon.sprite = null;
        } else {
            spriteRendererIcon.sprite = GameHelper.Instance.getBonusManager().getSpriteItemBonus(itemBonus.getTag());
        }
    }

    protected override void updateBackground() {
		//no base call, fully override

		if (!positionInterpolator.isInterpolating && (itemBonus.isStacked || itemBonus.isSnapped())) {
            spriteRendererBackground.sprite = getCurrentBackgroundSpriteHexagon();
		} else {
            spriteRendererBackground.sprite = getCurrentBackgroundSpriteCircle();
		}
	}

    protected override float updateBackgroundAlpha() {

        float aBg = base.updateBackgroundAlpha();

        Color cIcon = spriteRendererIcon.color;

        if (aBg != cIcon.a) {
            Color c = new Color(cIcon.r, cIcon.g, cIcon.b, aBg);
            spriteRendererIcon.color = c;
        }

        Color cTimerMalus = spriteRendererTimerMalus.color;


        float aTimer = 1;

        //blink at the last second
        float remainingTimeSec = itemBonus.getTimerMalusRemainingTimeSec();
        if (remainingTimeSec < 1) {

            //square function
            aTimer = Mathf.FloorToInt(remainingTimeSec * 100) % 2;
        }

        if (aTimer > aBg) {
            //timer alpha must be lower than item alpha
            aTimer = aBg;
        }

        if (aTimer != cTimerMalus.a) {
            Color c = new Color(cTimerMalus.r, cTimerMalus.g, cTimerMalus.b, aTimer);
            spriteRendererTimerMalus.color = c;
        }

        return aBg;
	}

    void ItemBonusListener.onBonusTypeChange(ItemBonus itemBonus) {

        updateForegroundIcon();
    }

    void ItemBonusListener.onEnqueuedItemBonusClick(ItemBonus itemBonus) {
		//do nothing
	}

	void ItemBonusListener.onStackedItemBonusClick(ItemBonus itemBonus) {
		//do nothing
	}

	void ItemBonusListener.onItemBonusInstantSelect(ItemBonus itemBonus) {
		//do nothing
	}

	void ItemBonusListener.onItemBonusPersistentSelect(ItemBonus itemBonus) {
		//do nothing
	}

	void ItemBonusListener.onTimerMalusTriggerStart(ItemBonus itemBonus) {
		updateTimerBackground();
	}

	void ItemBonusListener.onTimerMalusTriggerProgress(ItemBonus itemBonus) {
		updateTimerBackground();
	}

	void ItemBonusListener.onTimerMalusTriggerFinish(ItemBonus itemBonus) {
		updateTimerBackground();
	}

	void ItemBonusListener.onTimerMalusTriggerCancel(ItemBonus itemBonus) {
		updateTimerBackground();
	}

	private void updateTimerBackground() {

		if (itemBonus.isTimerMalusRunning()) {

			spriteRendererTimerMalus.enabled = true;

			float percentage = itemBonus.getTimerMalusProgressPercentage();
            trMaskTimerMalus.localScale = new Vector3(1, percentage * 2, 1);
            
        } else {

			spriteRendererTimerMalus.enabled = false;
		}
	}

    public Sprite getSpriteIcon() {
        return spriteRendererIcon.sprite;
    }

    public float getIconAngleDegrees() {
        return spriteRendererIcon.transform.rotation.eulerAngles.z;
    }

    public override Sprite getCurrentBackgroundSpriteHexagon() {

        if (itemBonus.bonusType.isMalus) {
            return spriteBgHexagonMalus;
        }

        return spriteBgHexagonBonus;
    }

    public override Sprite getCurrentBackgroundSpriteCircle() {
        
        if (itemBonus.bonusType.isMalus) {
            return spriteBgCircleMalus;
        }

        return spriteBgCircleBonus;
    }

}

