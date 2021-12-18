/**
 * Hexa Snap
 * © Aurélien Lubecki 2019
 * All Rights Reserved
 */

using System;
using System.Collections.Generic;


public class BonusStack : InGameModel {

	private static BonusStackListener to(BaseModelListener listener) {
		return (BonusStackListener) listener;
	}


	public static int maxStackSize = 3;

    public bool isEnabled { get; private set; }
    public int stackSize { get; private set; }

	private List<ItemBonus> stack = new List<ItemBonus>();


	public BonusStack(Activity10 activity) : base(activity) {
		stackSize = 2;
	}


    public void setEnabled(bool enabled) {

        isEnabled = enabled;

        notifyListeners(listener => {
            to(listener).onBonusStackEnableChange(this);
        });
    }

    public int getStackCount() {
		return stack.Count;
	}

	public Item getItemAt(int pos) {

		if (pos < 0) {
			throw new ArgumentException(); 
		}
		if (pos >= stack.Count) {
			throw new ArgumentException();
		}

		return stack[pos];
	}

	public int getStackPos(ItemBonus itemBonus) {

		if (itemBonus == null) {
			throw new ArgumentException();
		}

		int pos = stack.IndexOf(itemBonus);

		if (pos < 0) {
			throw new InvalidOperationException("Item not found in stack : nb = " + stack.Count);
		}

		return pos;
	}

	public void incrementSize() {

		if (stackSize >= maxStackSize) {
			return;
		}

        int previousStackSize = stackSize;
		stackSize++;

		notifyListeners(listener => {
            to(listener).onBonusStackSizeChange(this, previousStackSize);
		});

	}

	public void decrementSize() {

		if (stackSize <= 0) {
			return;
		}

        int previousStackSize = stackSize;
		stackSize--;

		while (stack.Count > stackSize) {
			unstackItem();
		}

		notifyListeners(listener => {
            to(listener).onBonusStackSizeChange(this, previousStackSize);
		});

	}

	public void stackItem(ItemBonus itemBonus) {

        if (!isEnabled) {
            throw new InvalidOperationException();
        }

		stackItem(itemBonus, 0);
	}

	public void stackItem(ItemBonus itemBonus, int pos) {
        
		if (itemBonus == null) {
			throw new ArgumentException();
		}
		if (pos < 0) {
			throw new ArgumentException();
		}
		if (pos >= stackSize) {
			throw new ArgumentException();
		}

        if (!isEnabled) {
            throw new InvalidOperationException();
        }
        if (stackSize <= 0) {
			throw new InvalidOperationException();
		}

		while (stack.Count >= stackSize) {
			unstackItem();
		}

		stack.Insert(pos, itemBonus);
		itemBonus.addToStack();

		notifyListeners(listener => {
			to(listener).onBonusStackItemBonusAdd(this, itemBonus);
		});

	}

	private void unstackItem() {

		unstackItem(stack.Count - 1, true);
	}

	public void unstackItem(ItemBonus itemBonus, bool mustDestroyItem) {

		if (itemBonus == null) {
			throw new ArgumentException();
		}

        if (!isEnabled) {
            throw new InvalidOperationException();
        }

        int pos = stack.IndexOf(itemBonus);
		if (pos >= 0) {
			unstackItem(pos, mustDestroyItem);
		}

	}

	public void unstackItem(int pos, bool mustDestroyItem) {

        if (!isEnabled) {
            throw new InvalidOperationException();
        }

        if (pos < 0) {
			return;
		}
		if (pos >= stack.Count) {
			return;
		}

		ItemBonus itemBonus = stack[pos];

		stack.RemoveAt(pos);
		itemBonus.removeFromStack();

		notifyListeners(listener => {
			to(listener).onBonusStackItemBonusRemove(this, itemBonus);
		});

		if (mustDestroyItem) {
            itemBonus.destroy(ItemDestroyCause.System);
		}
	}

	public void unstackAllItems() {

        if (!isEnabled) {
            return;
        }

        while (stack.Count > 0) {

			int pos = stack.Count - 1;

			ItemBonus itemBonus = stack[pos];

			stack.RemoveAt(pos);
			itemBonus.removeFromStack();

            itemBonus.destroy(ItemDestroyCause.System);
		}

		notifyListeners(listener => {
			to(listener).onBonusStackClear(this);
		});
	}

	public void reset() {

        if (!isEnabled) {
            return;
        }

        List<ItemBonus> itemsToClear = new List<ItemBonus>(stack);
		stack.Clear();

		foreach (ItemBonus itemBonus in itemsToClear) {
			itemBonus.removeFromStack();
            itemBonus.destroy(ItemDestroyCause.System);
		}

        int previousStackSize = stackSize;
		stackSize = 1;

		notifyListeners(listener => {
			to(listener).onBonusStackClear(this);
            to(listener).onBonusStackSizeChange(this, previousStackSize);
		});
	}

}
