/**
 * Hexa Snap
 * © Aurélien Lubecki 2019
 * All Rights Reserved
 */

using System;
using System.Linq;
using System.Collections.Generic;


public class GraphPercentagesHolder {

    private Graph graph;

    private Dictionary<BonusType, float> percentages = new Dictionary<BonusType, float>();

    public float maxPercentage { get; private set; }


    public GraphPercentagesHolder(Graph graph) {

        if (graph == null) {
            throw new ArgumentException();
        }

        this.graph = graph;

    }


    public BonusType getRandomBonusType(float bonusProportion = 1, float malusProportion = 1) {
        return getRandomBonusTypeExcluding(null, bonusProportion, malusProportion);
    }

    public BonusType getRandomBonusTypeExcluding(HashSet<BonusType> typesToExclude, float bonusProportion = 1, float malusProportion = 1) {

        HashSet<BonusType> typesToInclude = new HashSet<BonusType>();

        if (typesToExclude == null) {

            foreach (BonusType type in percentages.Keys) {
                typesToInclude.Add(type);
            }

        } else {

            foreach (BonusType type in percentages.Keys) {

                if (typesToExclude.Contains(type)) {
                    continue;
                }

                typesToInclude.Add(type);
            }
        }

        return getRandomBonusTypeIncluding(typesToInclude, bonusProportion, malusProportion);
    }

    public BonusType getRandomBonusTypeIncluding(HashSet<BonusType> typesToInclude, float bonusProportion = 1, float malusProportion = 1) {

        if (typesToInclude == null) {
            return null;
        }
        if (typesToInclude.Count <= 0) {
            //no types to chose
            return null;
        }

        //keep a minimum value in case no bonus can be generated but a malus can
        if (bonusProportion <= 0) {
            bonusProportion = 0.00001f;
        }
        if (malusProportion <= 0) {
            malusProportion = 0.00001f;
        }

        float totalProportion = bonusProportion + malusProportion;
        float bonusPercentage = bonusProportion / totalProportion;
        float malusPercentage = malusProportion / totalProportion;

        //fill a dictionary of eligibles bonus and their percentage
        Dictionary<BonusType, int> eligibleBonusTypes = new Dictionary<BonusType, int>();

        foreach (BonusType type in percentages.Keys) {

            if (!typesToInclude.Contains(type)) {
                //eligibles don't include this type
                continue;
            }

            float additionalPercentage = type.isMalus ? malusPercentage : bonusPercentage;

            eligibleBonusTypes.Add(type, (int) (additionalPercentage * percentages[type] * 100));
        }

        if (eligibleBonusTypes.Count <= 0) {
            //no types to chose
            return null;
        }

        if (eligibleBonusTypes.Count == 1) {
            //choose the only one
            return eligibleBonusTypes.First().Key;
        }

        int maxValue = eligibleBonusTypes.Values.Sum();
        if (maxValue <= 1) {
            //choose the first
            return eligibleBonusTypes.First().Key;
        }

        int chosenPos = Constants.newRandomPosInArray(maxValue);
        int currentVal = 0;

        foreach (KeyValuePair<BonusType, int> e in eligibleBonusTypes) {

            currentVal += e.Value;

            if (chosenPos < currentVal) {
                //found
                return e.Key;
            }
        }
        
        throw new InvalidOperationException("Random item not found : " + chosenPos + " / " + maxValue + " (nb elems = " + eligibleBonusTypes.Count + ")");
    }

    public void updatePercentages() {

        //clear before update
        percentages.Clear();
        maxPercentage = 0;


        foreach (NodeZone nz in graph.getSortedNodesZone()) {

            if (nz.state != NodeZoneState.ACTIVATED) {
                continue;
            }

            foreach (NodeBonusType nbt in graph.getSortedNodesBonusType(nz.tag)) {

                float p = nbt.getActivatePercentage();
                if (p <= 0) {
                    continue;
                }

                percentages.Add(nbt.bonusType, p);
            }
        }

        //bake percentage to not recalculate it again and again
        float value = 0;
        foreach (float p in percentages.Values) {

            value += p;
        }

        maxPercentage = value / percentages.Count;
    }

    public int getNbGraphBonus() {
        return percentages.Count;
    }

    public bool hasAnyBonus() {
        
        foreach (BonusType type in percentages.Keys) {

            if (!type.isMalus) {
                return true;
            }
        }

        return false;
    }

    public bool hasAnyMalus() {

        foreach (BonusType type in percentages.Keys) {

            if (type.isMalus) {
                return true;
            }
        }

        return false;
    }

}
