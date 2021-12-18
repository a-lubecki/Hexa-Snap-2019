/**
 * Hexa Snap
 * © Aurélien Lubecki 2019
 * All Rights Reserved
 */

using System;
using System.Collections.Generic;


public class LevelGoalsManager {

    private static readonly LevelGoal[] GOALS = {
        new LevelGoal(10, 0, 0, 0, 0, 0), //level 1
        new LevelGoal(0, 10, 0, 0, 0, 0), //level 2
        new LevelGoal(10, 0, 10, 0, 0, 0), //level 3
        new LevelGoal(0, 15, 0, 5, 0, 0), //level 4
        new LevelGoal(5, 5, 5, 5, 0, 0), //level 5
        new LevelGoal(0, 20, 0, 0, 0, 0), //level 6
        new LevelGoal(20, 0, 15, 0, 0, 0), //level 7
        new LevelGoal(0, 20, 10, 0, 0, 0), //level 8
        new LevelGoal(0, 0, 0, 15, 0, 0), //level 9
        new LevelGoal(10, 10, 10, 10, 0, 0), //level 10
        new LevelGoal(30, 0, 20, 0, 0, 0), //level 11
        new LevelGoal(0, 40, 0, 0, 0, 0), //level 12
        new LevelGoal(30, 20, 10, 5, 0, 0), //level 13
        new LevelGoal(20, 0, 20, 10, 0, 0), //level 14
        new LevelGoal(15, 15, 15, 15, 0, 0), //level 15
        new LevelGoal(50, 0, 0, 0, 0, 0), //level 16
        new LevelGoal(30, 30, 0, 10, 0, 0), //level 17
        new LevelGoal(0, 30, 20, 10, 0, 0), //level 18
        new LevelGoal(30, 0, 30, 0, 0, 0), //level 19
        new LevelGoal(20, 20, 20, 20, 0, 0), //level 20
        new LevelGoal(40, 30, 0, 0, 3, 0), //level 21
        new LevelGoal(30, 40, 0, 0, 0, 3), //level 22
    };

    private static readonly LevelGoal GOAL_LEVEL_100 = new LevelGoal(1);//joke for hardcore gamers


    //singleton
    public static LevelGoalsManager Instance = new LevelGoalsManager();

    private LevelGoalsManager() {
    }


    //keep a list of generated goals in case the player start the previous level (malus decrement level)
    private Dictionary<int, LevelGoal> generatedGoals = new Dictionary<int, LevelGoal>();


    public LevelGoal getLevelGoal(int level) {

        if (level == 100) {
            //special case for level 100
            return GOAL_LEVEL_100;
        }

        int pos = level - 1;

        if (pos < 0) {
            throw new ArgumentException();
        }

        if (pos < GOALS.Length) {
            //pick the predefined goal
            return GOALS[pos];
        }
        
        if (generatedGoals.ContainsKey(level)) {
            //pick the existing generated goal
            return generatedGoals[level];
        }

        //add a new goal in the generated goals
        LevelGoal newGoal = newRandomGoal(level);
        generatedGoals[level] = newGoal;

        return newGoal;
    }

    public void resetGeneratedGoals() {
        generatedGoals.Clear();
    }


    private LevelGoal newRandomGoal(int level) { 

        //nb items to generate for this goal
        int maxNbItems = Constants.newRandomInt(level, level + 20);

        //randomize priority of items goal generation (the last items have less chances to be in the goals)
        List<LevelItemType> types = LevelItemTypeMethods.getSortedTypes();

        LevelItemType[] randomizedTypes = new LevelItemType[Constants.MAX_GOAL_ITEMS_FOR_LEVEL];
        for (int i = 0 ; i < Constants.MAX_GOAL_ITEMS_FOR_LEVEL ; i++) {

            //pick a random type
            int nextPos = Constants.newRandomPosInArray(types.Count);

            randomizedTypes[i] = types[nextPos];
            types.RemoveAt(nextPos);
        }

        //fill the goals
        Dictionary<LevelItemType, int> itemsToReach = new Dictionary<LevelItemType, int>();

        int totalNbItems = 0;

        foreach (LevelItemType type in randomizedTypes) {

            int maxRandom;

            if (type == LevelItemType.Type100) {
                maxRandom = 1 + (int)(level / 5f);
            } else if (type == LevelItemType.Bonus || type == LevelItemType.Malus) {
                maxRandom = 1 + (int)(level / 20f);
            } else {
                maxRandom = level;
            }

            int nb = Constants.newRandomInt(0, maxRandom);
            if (totalNbItems + nb > maxNbItems) {
                nb = maxNbItems - totalNbItems;
            }

            itemsToReach.Add(type, nb);

            totalNbItems += nb;

            if (totalNbItems >= maxNbItems) {
                //end the filling
                break;
            }
        }

        if (totalNbItems <= 0) {

            //special case, nothing was generated => put the max on Type1
            itemsToReach.Add(LevelItemType.Type1, maxNbItems);

        } else if (totalNbItems < maxNbItems) {

            //add diff if less items => distribute equally between the generated items
            int diff = maxNbItems - totalNbItems;
            int nbToGive = (int)(diff / (float) itemsToReach.Count);

            if (nbToGive > 0) {
                
                //copy keys to avoid out of sync exception
                foreach (LevelItemType type in new List<LevelItemType>(itemsToReach.Keys)) {
                    itemsToReach[type] += nbToGive;
                }
            }
        }

        return new LevelGoal(itemsToReach);
    }


}
