/**
 * Hexa Snap
 * © Aurélien Lubecki 2019
 * All Rights Reserved
 */

using System;
using System.Collections.Generic;


public class LevelGoal {

    private Dictionary<LevelItemType, int> nbItemsToReach = new Dictionary<LevelItemType, int>();

    public LevelGoal(int nb1, int nb5 = 0, int nb20 = 0, int nb100 = 0, int nbBonus = 0, int nbMalus = 0) {

        addNewNbToReach(LevelItemType.Type1, nb1);
        addNewNbToReach(LevelItemType.Type5, nb5);
        addNewNbToReach(LevelItemType.Type20, nb20);
        addNewNbToReach(LevelItemType.Type100, nb100);
        addNewNbToReach(LevelItemType.Bonus, nbBonus);
        addNewNbToReach(LevelItemType.Malus, nbMalus);

        if (nbItemsToReach.Count <= 0) {
            throw new ArgumentException("The total nb of items is 0 => no goal");
        }
    }

    public LevelGoal(Dictionary<LevelItemType, int> items) {

        foreach (KeyValuePair<LevelItemType, int> e in items) {
            addNewNbToReach(e.Key, e.Value);
        }

        if (nbItemsToReach.Count <= 0) {
            throw new ArgumentException("The total nb of items is 0 => no goal");
        }
    }

    private void addNewNbToReach(LevelItemType type, int nb) {

        if (nb < 0) {
            throw new ArgumentException();
        }

        if (nb == 0) {
            return;
        }

        nbItemsToReach.Add(type, nb);
    }
    
    public List<LevelItemType> getAvailableTypes() {

        List<LevelItemType> res = new List<LevelItemType>();

        //return the sorted types
        foreach (LevelItemType type in LevelItemTypeMethods.getSortedTypes()) {

            if (nbItemsToReach.ContainsKey(type)) {
                res.Add(type);
            }
        }

        return res;
    }

    public int getNbItemsToReach(LevelItemType type) {

        if (nbItemsToReach.ContainsKey(type)) {
            return nbItemsToReach[type];
        }

        return 0;
    }


}
