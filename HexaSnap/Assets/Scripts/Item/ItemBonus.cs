/**
 * Hexa Snap
 * © Aurélien Lubecki 2019
 * All Rights Reserved
 */

using System;

public class ItemBonus : Item, GameTimerListener {
	
	private static ItemBonusListener to(BaseModelListener listener) {
		return (ItemBonusListener) listener;
	}

	public BonusType bonusType { get; private set; }
	public object specificBonusObject { get; private set; }

    public ItemSnapPosition impactPos { get; set; } // the pos in grid where the bonus must act (used for some bonus types) 

    public bool snappedMalusTimerDisabled { get; private set; }
    private GameTimer timerMalusTrigger;


	public ItemBonus(Activity10 activity, BonusType bonusType) : base(activity, ItemType.Bonus) {

		if (bonusType == null) {
			throw new ArgumentException();
		}

		this.bonusType = bonusType;
		specificBonusObject = bonusType.newSpecificBonusObject();
	}

	public void updateSpecificBonusObject(object newObject) {

        if (newObject == specificBonusObject) {
            //no change
            return;
        }

        specificBonusObject = newObject;

        notifyListeners(listener => {
            to(listener).onBonusTypeChange(this);
        });
    }

	public string getTag() {
		return bonusType.getTag(this);
	}

    public override void onItemClick() {

        if (!isClickable()) {
            //void malus items can't be clicked
            return;
        }

        base.onItemClick();

        if (isEnqueued) {

            notifyListeners(listener => {
                to(listener).onEnqueuedItemBonusClick(this);
            });

        } else if (isStacked) {

			notifyListeners(listener => {
				to(listener).onStackedItemBonusClick(this);
			});
		}
			
	}

	protected override void snapBeforeNotifyingListeners() {
        base.snapBeforeNotifyingListeners();

		setSelectable(true);

		startSnappedMalusTimer();

		bonusType.onItemBonusSnapped(this);
	}

	protected override void unsnapBeforeNotifyingListeners() {
        base.unsnapBeforeNotifyingListeners();

		cancelSnappedMalusTimer();
	}

    protected override void enqueueBeforeNotifyingListeners() {
        base.enqueueBeforeNotifyingListeners();

		setSelectable(true);
	}

    protected override void dequeueBeforeNotifyingListeners() {
        base.dequeueBeforeNotifyingListeners();

		setSelectable(false);
	}

    protected override void addToStackBeforeNotifyingListeners() {
        base.addToStackBeforeNotifyingListeners();

		setSelectable(true);

		startSnappedMalusTimer();
	}

    protected override void removeFromStackBeforeNotifyingListeners() {
        base.removeFromStackBeforeNotifyingListeners();

		setSelectable(false);

		cancelSnappedMalusTimer();
	}

    public void setSnappedMalusTimerDisabled(bool disabled) {
        snappedMalusTimerDisabled = disabled;
    }

	public void startSnappedMalusTimer() {

		if (!bonusType.isMalus) {
			return;
		}
		if (!isSnapped() && !isStacked) {
			return;
		}
		if (timerMalusTrigger != null) {
            //already running
			return;
		}
        if (snappedMalusTimerDisabled) {
            return;
        }

		timerMalusTrigger = new GameTimer(activity, false, Constants.DELAY_MALUS_TRIGGER_S);
		timerMalusTrigger.addListener(this);

		timerMalusTrigger.start();
	}

    public void cancelSnappedMalusTimer() {

		if (timerMalusTrigger == null) {
            //not running
			return;
		}

		timerMalusTrigger.cancel();
		timerMalusTrigger = null;
	}

	public bool isTimerMalusRunning() {

		if (timerMalusTrigger == null) {
			return false;
		}

		return timerMalusTrigger.isRunning;
	}

	public float getTimerMalusProgressPercentage() {

		if (timerMalusTrigger == null) {
			return 0;
		}

		return timerMalusTrigger.getProgressPercentage();
	}

    public float getTimerMalusRemainingTimeSec() {

        if (timerMalusTrigger == null) {
            return 0;
        }

        return timerMalusTrigger.remainingTimeSec;
    }

    public override void selectBeforeNotifyingListener() {

        bonusType.onItemBonusUsed(this);

		//trigger the bonus/malus
		if (bonusType.isInstant) {

			notifyListeners(listener => {
				to(listener).onItemBonusInstantSelect(this);
			});

		} else {

			notifyListeners(listener => {
				to(listener).onItemBonusPersistentSelect(this);
			});

		}

	}

    public override void destroyBeforeNotifyingListener(ItemDestroyCause cause) {

		cancelSnappedMalusTimer();
	}


	BaseModelBehavior BaseModelListener.getModelBehavior() {
		return null;
	}

	void GameTimerListener.onTimerRunningBonusStart(GameTimer timer) {

		notifyListeners(listener => {
			to(listener).onTimerMalusTriggerStart(this);
		});
	}

	void GameTimerListener.onTimerRunningBonusProgress(GameTimer timer) {

		notifyListeners(listener => {
			to(listener).onTimerMalusTriggerProgress(this);
		});
	}

	void GameTimerListener.onTimerRunningBonusFinish(GameTimer timer) {

		notifyListeners(listener => {
			to(listener).onTimerMalusTriggerFinish(this);
		});

	}

	void GameTimerListener.onTimerRunningBonusCancel(GameTimer timer) {

		notifyListeners(listener => {
			to(listener).onTimerMalusTriggerCancel(this);
		});

    }

    void GameTimerListener.onTimerRunningBonusDurationChange(GameTimer timer) {
        //do nothing
    }

}

