/**
 * Hexa Snap
 * © Aurélien Lubecki 2019
 * All Rights Reserved
 */

using System;
using System.Collections.Generic;

public enum LevelItemType {

    Type1,
    Type5,
    Type20,
    Type100,
    Bonus,
    Malus
}

static class LevelItemTypeMethods {

    public static int getLength() {
        return 6;
    }

    public static LevelItemType getType(Item item) {
        
        switch (item.itemType) {

            case ItemType.Type1:
                return LevelItemType.Type1;

            case ItemType.Type5:
                return LevelItemType.Type5;

            case ItemType.Type20:
                return LevelItemType.Type20;

            case ItemType.Type100:
                return LevelItemType.Type100;

            case ItemType.Bonus:

                if (!(item is ItemBonus)) {
                    throw new InvalidOperationException();
                }

                if ((item as ItemBonus).bonusType.isMalus) {
                    return LevelItemType.Malus;
                }

                return LevelItemType.Bonus;
        }

        throw new NotSupportedException();
    }

    public static List<LevelItemType> getSortedTypes() {

        List<LevelItemType> res = new List<LevelItemType>();

        res.Add(LevelItemType.Type1);
        res.Add(LevelItemType.Type5);
        res.Add(LevelItemType.Type20);
        res.Add(LevelItemType.Type100);
        res.Add(LevelItemType.Bonus);
        res.Add(LevelItemType.Malus);

        return res;
    }

}
