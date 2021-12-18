/**
 * Hexa Snap
 * © Aurélien Lubecki 2019
 * All Rights Reserved
 */

using System.Collections.Generic;
using System.Linq;


public class BonusCommandRandomBonus : BaseBonusCommandRandom {


    override protected List<ItemSnapPosition> orderFreePositions(List<ItemSnapPosition> freePositions) {

        return freePositions.OrderBy((pos) => pos.level).ToList();
    }

    protected override List<ItemType> getPossibleItemTypes() {

        return new List<ItemType> {
            ItemType.Type100,
            ItemType.Type20,
            ItemType.Type5,
            ItemType.Type1
        };
    }

    protected override int calculateTotalScore(int nbItems, int level) {
        
        return nbItems * nbItems * level;
    }

}

