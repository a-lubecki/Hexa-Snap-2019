/**
 * Hexa Snap
 * © Aurélien Lubecki 2019
 * All Rights Reserved
 */

using System;

public enum ItemType : int {
	
	Type1,
	Type5,
	Type20,
	Type100,
	Bonus

}

static class ItemTypeMethods {

	public static int getLength() {
		return 5;
	}

	public static int getScore(this ItemType itemType) {
		switch (itemType) {

		case ItemType.Type1:
			return 1;

		case ItemType.Type5:
			return 5;

		case ItemType.Type20:
			return 20;

		case ItemType.Type100:
			return 100;

		case ItemType.Bonus:
			return 0;
		}

		throw new NotSupportedException();
	}

}
