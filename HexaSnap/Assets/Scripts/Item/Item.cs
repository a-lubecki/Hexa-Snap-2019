/**
 * Hexa Snap
 * © Aurélien Lubecki 2019
 * All Rights Reserved
 */

using System;
using UnityEngine;


public class Item : InGameModel {

	private static ItemListener to(BaseModelListener listener) {
		return (ItemListener) listener;
	}


	public ItemType itemType { get; private set; }

    public bool isEnqueued { get; private set; }
    public bool wasEnqueued { get; private set; }
    public bool isStacked { get; private set; }
    public bool wasStacked { get; private set; }

    public ItemSnapPosition snapPosition { get; private set; }
    public bool wasSnapped { get; private set; }
    public bool hasBeenSnappedBefore { get; private set; }

    public bool isSelectable { get; private set; }
	public bool isSelected { get; private set; }
    

    public Item(Activity10 activity, ItemType itemType) : base(activity) {

		this.itemType = itemType;
	}

    public void onFallingItemCollide(Vector3 currentPos) {

		notifyListeners(listener => {
            to(listener).onFallingItemCollide(this, currentPos);
		});
	}

    public virtual void onItemClick() {

        if (!isClickable()) {
            return;
        }

        if (isSnapped()) {
			
			notifyListeners(listener => {
				to(listener).onSnappedItemClick(this);
			});
		}
	}

    public bool isSnapped() {
        return (snapPosition != null);
    }
    
    public void snap(ItemSnapPosition snapPosition) {

        if (isSelected) {
			return;
		}
		if (isSnapped()) {
			return;
		}

        wasEnqueued = false;
        wasStacked = false;

        this.snapPosition = snapPosition;
        hasBeenSnappedBefore = true;
        wasSnapped = true;

        snapBeforeNotifyingListeners();

		notifyListeners(listener => {
			to(listener).onItemChangeZone(this);
        });
	}

	protected virtual void snapBeforeNotifyingListeners() {
		//override this
	}

	public void unsnap() {

		if (!isSnapped()) {
			return;
        }
        
        snapPosition = null;

		unsnapBeforeNotifyingListeners();

		notifyListeners(listener => {
			to(listener).onItemChangeZone(this);
		});
	}

	protected virtual void unsnapBeforeNotifyingListeners() {
		//override this
	}

	public void incrementItemType() {

		if (isSelected) {
			return;
		}
		if (!isSnapped()) {
			return;
		}
		if (itemType >= (ItemType)(ItemTypeMethods.getLength() - 1)) {
			return;
		}

		itemType++;

		notifyListeners(listener => {
			to(listener).onItemTypeChange(this);
		});
	}

	public void enqueue() {

		if (isStacked) {
			throw new ArgumentException();
		}
		if (isSnapped()) {
			throw new ArgumentException();
		}
		if (isSelected) {
			throw new ArgumentException();
		}
		if (isEnqueued) {
			return;
		}

        wasStacked = false;
        wasSnapped = false;

        isEnqueued = true;
        wasEnqueued = true;

        enqueueBeforeNotifyingListeners();

		notifyListeners(listener => {
			to(listener).onItemChangeZone(this);
        });
	}

	protected virtual void enqueueBeforeNotifyingListeners() {
		//override this
	}

	public void dequeue() {

		if (isStacked) {
			throw new ArgumentException();
		}
		if (isSnapped()) {
			throw new ArgumentException();
		}
		if (isSelected) {
			throw new ArgumentException();
		}
		if (!isEnqueued) {
			return;
		}
        
        isEnqueued = false;

        dequeueBeforeNotifyingListeners();

		notifyListeners(listener => {
			to(listener).onItemChangeZone(this);
		});
	}

	protected virtual void dequeueBeforeNotifyingListeners() {
		//override this
	}

	public void addToStack() {

		if (isEnqueued) {
			throw new ArgumentException();
		}
		if (isSnapped()) {
			throw new ArgumentException();
		}
		if (isSelected) {
			throw new ArgumentException();
		}
		if (isStacked) {
			return;
		}

        wasEnqueued = false;
        wasSnapped = false;

        isStacked = true;
        wasStacked = true;

        addToStackBeforeNotifyingListeners();

		notifyListeners(listener => {
			to(listener).onItemChangeZone(this);
		});
	}

	protected virtual void addToStackBeforeNotifyingListeners() {
		//override this
	}

	public void removeFromStack() {

		if (isEnqueued) {
			throw new ArgumentException();
		}
		if (isSnapped()) {
			throw new ArgumentException();
		}
		if (isSelected) {
			throw new ArgumentException();
		}
		if (!isStacked) {
			return;
		}

		isStacked = false;

		removeFromStackBeforeNotifyingListeners();

		notifyListeners(listener => {
			to(listener).onItemChangeZone(this);
		});
	}

	protected virtual void removeFromStackBeforeNotifyingListeners() {
		//override this
	}

	public void setSelectable(bool selectable) {
		
        this.isSelectable = selectable;

        notifyListeners(listener => {
            to(listener).onItemSelectableChange(this);
        });
	}

	public virtual bool isClickable() {

		if (activity.timeManager.getTotalTimeScalePlay() <= 0) {
			//can't select if time is paused
			return false;
		}

		return isSelectable;
	}

	public bool select() {

		if (isEnqueued) {
			throw new ArgumentException();
		}
		if (isSelected) {
			return false;
		}

		isSelectable = false;
		isSelected = true;

		selectBeforeNotifyingListener();

		notifyListeners(listener => {
			to(listener).onItemSelect(this);
		});

		return true;
	}

	public virtual void selectBeforeNotifyingListener() {
		//override this
	}

    public void destroy(ItemDestroyCause cause) {
        
        destroyBeforeNotifyingListener(cause);

		notifyListeners(listener => {
            to(listener).onItemDestroyRequest(this, hasBeenSnappedBefore, cause);
		});
	}

    public virtual void destroyBeforeNotifyingListener(ItemDestroyCause cause) {
		//override this
	}

}

