/**
 * Hexa Snap
 * © Aurélien Lubecki 2019
 * All Rights Reserved
 */

using System;
using System.Collections.Generic;

public class ItemSnapPosition {

	public ItemSnapDirection direction { get; private set; }
    public int level { get; private set; }
    public int pos { get; private set; }

	public ItemSnapPosition(ItemSnapDirection direction, int level, int pos) {

		if ((int)direction < 0) {
			throw new ArgumentException();
		}
		if ((int)direction >= 6) {
			throw new ArgumentException();
		}
		if (level < 0) {
			throw new ArgumentException();
		}
		if (pos < 0) {
			throw new ArgumentException();
		}
		if (pos > level) {
			throw new ArgumentException();
		}

		this.direction = direction;
		this.level = level;
		this.pos = pos;
	}

	public int getMaxPos() {
		return level;
	}

	public ItemSnapDirection getNextDirection() {

		ItemSnapDirection newDirection = direction + 1;

		if (newDirection >= (ItemSnapDirection)Enum.GetNames(typeof(ItemSnapDirection)).Length) {
			newDirection = (ItemSnapDirection)0;
		}

		return newDirection;
	}

	public ItemSnapDirection getPreviousDirection() {

		ItemSnapDirection newDirection = direction - 1;

		if (newDirection < (ItemSnapDirection)0) {
			newDirection = (ItemSnapDirection)(Enum.GetNames(typeof(ItemSnapDirection)).Length - 1);
		}

		return newDirection;
	}

	public List<ItemSnapPosition> newSiblingItemPositions() {
		
		List<ItemSnapPosition> res = new List<ItemSnapPosition>();

        foreach (ItemSnapDirection d in Constants.getAllDirections()) {

            ItemSnapPosition p = newSiblingItemPosition(d);
            if (p == null) {
				continue;
			}

            res.Add(p);
		}

		return res;
	}

	public ItemSnapPosition newSiblingItemPosition(ItemSnapDirection localDirection) {

		//turn clockwise
		if (localDirection == ItemSnapDirection.TOP) {
			return new ItemSnapPosition(direction, level + 1, pos);
		}

		if (localDirection == ItemSnapDirection.RIGHT_TOP) {
			return new ItemSnapPosition(direction, level + 1, pos + 1);
		}

		if (localDirection == ItemSnapDirection.RIGHT_BOTTOM) {
			if (pos < getMaxPos()) {
				return new ItemSnapPosition(direction, level, pos + 1);
			}
			return new ItemSnapPosition(getNextDirection(), level, 0);
		}

		if (localDirection == ItemSnapDirection.BOTTOM) {
			if (level <= 0) {
				return null;
			}
			if (pos < getMaxPos()) {
				return new ItemSnapPosition(direction, level - 1, pos);
			}
			return new ItemSnapPosition(getNextDirection(), level - 1, 0);
		}

		if (localDirection == ItemSnapDirection.LEFT_BOTTOM) {
			if (pos <= 0) {
				return new ItemSnapPosition(getPreviousDirection(), level, getMaxPos());
			}
			return new ItemSnapPosition(direction, level - 1, pos - 1);
		}

		if (localDirection == ItemSnapDirection.LEFT_TOP) {
			if (pos <= 0) {
				return new ItemSnapPosition(getPreviousDirection(), level + 1, getMaxPos() + 1);
			}
			return new ItemSnapPosition(direction, level, pos - 1);
		}

		throw new InvalidOperationException();
	}

	public override string ToString() {
		return "ItemSnapPosition ( direction:" + direction + ", level:" + level + ", pos:" + pos + " )";
	}

	public override bool Equals (object obj) {
		if (obj == null)
			return false;
		if (ReferenceEquals (this, obj))
			return true;
		if (obj.GetType () != typeof(ItemSnapPosition))
			return false;
		ItemSnapPosition other = (ItemSnapPosition)obj;
		return level == other.level && pos == other.pos && direction == other.direction;
	}

	public override int GetHashCode() {
		unchecked {
			return level.GetHashCode () ^ pos.GetHashCode () ^ direction.GetHashCode ();
		}
	}
	

}

public enum ItemSnapDirection : int {

	TOP,
	RIGHT_TOP,
	RIGHT_BOTTOM,
	BOTTOM,
	LEFT_BOTTOM,
	LEFT_TOP
}