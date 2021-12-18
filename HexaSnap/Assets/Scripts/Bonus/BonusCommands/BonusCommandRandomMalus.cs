/**
 * Hexa Snap
 * © Aurélien Lubecki 2019
 * All Rights Reserved
 */

using System.Collections.Generic;
using System.Linq;

public class BonusCommandRandomMalus : BaseBonusCommandRandom {


    override protected List<ItemSnapPosition> orderFreePositions(List<ItemSnapPosition> freePositions) {

        return freePositions.OrderByDescending((pos) => pos.level).ToList();
    }

    protected override List<ItemType> getPossibleItemTypes() {

        return new List<ItemType> {
            ItemType.Type1,
            ItemType.Type5,
            ItemType.Type20,
            ItemType.Type100
        };
    }

    protected override int calculateTotalScore(int nbItems, int level) {
        
        return int.MaxValue - (nbItems * level * level);
    }

}

