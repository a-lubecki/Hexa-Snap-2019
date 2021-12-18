/**
 * Hexa Snap
 * © Aurélien Lubecki 2019
 * All Rights Reserved
 */

using System;
using UnityEngine;

public class TimerRunningBonusBehavior : InGameModelBehavior, GameTimerListener {
	
	private static float timerWidth = 1.2f;
	private static float sticksGap = timerWidth / (TimerRunningBonus.nbSticks - 1);


	public TimerRunningBonus timer {
		get {
			return (TimerRunningBonus) model;
		}
	}


	private static Sprite spriteTimerBackgroundBonus;
	private static Sprite spriteTimerBackgroundMalus;
	private static Sprite spriteTimerStickBonus;
	private static Sprite spriteTimerStickMalus;


	private Transform transformTimerBackground;

	private SpriteRenderer spriteRendererBackground;
	private SpriteRenderer spriteRendererIcon;

	private PositionInterpolator positionInterpolator;


	protected override void onAwake() {

		if (spriteTimerBackgroundBonus == null) {

			spriteTimerBackgroundBonus = GameHelper.Instance.loadSpriteAsset(Constants.PATH_DESIGNS_OBJECTS + "TimerRunningBonus.Background.Bonus");
			spriteTimerBackgroundMalus = GameHelper.Instance.loadSpriteAsset(Constants.PATH_DESIGNS_OBJECTS + "TimerRunningBonus.Background.Malus");
			spriteTimerStickBonus = GameHelper.Instance.loadSpriteAsset(Constants.PATH_DESIGNS_OBJECTS + "TimerRunningBonus.Stick.Bonus");
			spriteTimerStickMalus = GameHelper.Instance.loadSpriteAsset(Constants.PATH_DESIGNS_OBJECTS + "TimerRunningBonus.Stick.Malus");
		}

		transformTimerBackground = transform.Find("SpriteTimerBackground").transform;

		spriteRendererBackground = transform.Find("SpriteBonusBackground").GetComponent<SpriteRenderer>();
		spriteRendererIcon = transform.Find("SpriteBonusIcon").GetComponent<SpriteRenderer>();

		positionInterpolator = new PositionInterpolator(transform);
	}

	protected override void onInit() {
		base.onInit();

        spriteRendererBackground.sprite = timer.itemBonus.bonusType.isMalus ? ItemBonusBehavior.spriteBgHexagonMalus : ItemBonusBehavior.spriteBgHexagonBonus;
		spriteRendererIcon.sprite = GameHelper.Instance.getBonusManager().getSpriteItemBonus(timer.itemBonus.getTag());

		transformTimerBackground.GetComponent<SpriteRenderer>().sprite = getTimerBackgroundSprite(timer.itemBonus.bonusType);
	}

	protected override void onUpdate() {
		base.onUpdate();

		positionInterpolator.update();
	}

	public Vector3 getLastAnimatedPos() {

		if (!isInit()) {
			return transform.position;
		}

		return positionInterpolator.getLastInterpolatedPos();
	}

	public void animatePosition(PositionInterpolatorBundle bundle, Action<bool> completion = null) {

		if (!isInit()) {
			return;
		}

		positionInterpolator.addNextPosition(bundle, completion);
	
	}


	BaseModelBehavior BaseModelListener.getModelBehavior() {
		return this;
	}

	void GameTimerListener.onTimerRunningBonusStart(GameTimer timer) {
		//do nothing
	}

	void GameTimerListener.onTimerRunningBonusProgress(GameTimer timer) {

		//add sticks if required

		int nbSticks = (int)((TimerRunningBonus.nbSticks + 1) * timer.getProgressPercentage());
		int currentNbSticks = transformTimerBackground.childCount;

		GameObjectPoolBehavior pool = GameHelper.Instance.getPool();

		if (currentNbSticks < nbSticks) {
			//add sprites

			for (int pos = currentNbSticks ; pos < nbSticks ; pos++) {

				GameObject gameObjectStick = pool.pickTimerProgressStickGameObject(transformTimerBackground, true, new Vector3(-0.5f * timerWidth + pos * sticksGap, 0, -1));
				gameObjectStick.transform.SetParent(transformTimerBackground);
                
				gameObjectStick.GetComponent<SpriteRenderer>().sprite = getTimerStickSprite(((TimerRunningBonus) timer).itemBonus.bonusType);

			}
		}

	}

	void GameTimerListener.onTimerRunningBonusFinish(GameTimer timer) {

		clearSticks();
	}

	void GameTimerListener.onTimerRunningBonusCancel(GameTimer timer) {

		clearSticks();
	}

    void GameTimerListener.onTimerRunningBonusDurationChange(GameTimer timer) {
        //do nothing
    }

    private void clearSticks() {

		GameObjectPoolBehavior pool = GameHelper.Instance.getPool();

		//remove all children transform
		while (transformTimerBackground.childCount > 0) {
			pool.storeTimerProgressStickGameObject(transformTimerBackground.GetChild(0).gameObject);
		}

	}


	private static Sprite getTimerBackgroundSprite(BonusType type) {

		if (type.isMalus) {
			return spriteTimerBackgroundMalus;
		}

		return spriteTimerBackgroundBonus;
	}

	private static Sprite getTimerStickSprite(BonusType type) {

		if (type.isMalus) {
			return spriteTimerStickMalus;
		}

		return spriteTimerStickBonus;
	}

}

