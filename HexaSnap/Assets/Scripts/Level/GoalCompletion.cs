/**
 * Hexa Snap
 * © Aurélien Lubecki 2019
 * All Rights Reserved
 */

using System;
using System.Collections.Generic;


public class GoalCompletion : BaseModel {

    private static GoalCompletionListener to(BaseModelListener listener) {
        return (GoalCompletionListener) listener;
    }

    public LevelGoal goal { get; private set; }
    private List<LevelItemType> availableTypes;

    private Dictionary<LevelItemType, int> nbItemsCounter = new Dictionary<LevelItemType, int>();
    private float completionPercentage;


    public GoalCompletion(int level) : base() {
        resetCompletion(level);
    }

    public void resetCompletion(int level) {

        goal = LevelGoalsManager.Instance.getLevelGoal(level);
        availableTypes = goal.getAvailableTypes();

        nbItemsCounter.Clear();
        completionPercentage = 0;

        notifyListeners(listener => {
            to(listener).onGoalChange(this);
        });
    }

    public float getCompletionPercentage() {
        return completionPercentage;
    }

    public void addSelectedItems(Item item, int nb) {

        if (item == null) {
            throw new ArgumentException();
        }
        if (nb <= 0) {
            throw new ArgumentException();
        }

        var levelType = getNullableAvailableType(item);
        if (!levelType.HasValue) {
            return;
        }

        var type = levelType.Value;
        if (nbItemsCounter.ContainsKey(type)) {
            nbItemsCounter[type] += nb;
        } else {
            nbItemsCounter.Add(type, nb);
        }

        updateCompletionPercentage();

        notifyListeners(listener => {
            to(listener).onGoalCompletionChange(this);
        });
    }

    private void updateCompletionPercentage() {

        float percentages = 0;

        int nbTypes = availableTypes.Count;

        foreach (LevelItemType type in availableTypes) {

            int maxNb = goal.getNbItemsToReach(type);
            int nb = 0;
            if (nbItemsCounter.ContainsKey(type)) {
                nb = nbItemsCounter[type];
            }

            if (nb > maxNb) {
                percentages += 1;//reached
            } else {
                percentages += (nb / (float) maxNb);
            }

        }

        completionPercentage = percentages / nbTypes;
    }

    public int getNbAvailableTypes() {
        return availableTypes.Count;
    }

    public List<LevelItemType> getAvailableTypes() {
        return new List<LevelItemType>(availableTypes);
    }

    public LevelItemType? getNullableAvailableType(Item item) {

        LevelItemType levelType = LevelItemTypeMethods.getType(item);
        if (availableTypes.Contains(levelType)) {
            return levelType;
        }

        return null;
    }

    public int getNbItems(LevelItemType type) {

        if (!nbItemsCounter.ContainsKey(type)) {
            return 0;
        }

        return nbItemsCounter[type];
    }

    public int getNbItemsToReach(LevelItemType type) {
        return goal.getNbItemsToReach(type);
    }

    public int getRemainingNbItemsToFinish(LevelItemType type) {

        var res = getNbItemsToReach(type) - getNbItems(type);
        if (res < 0) {
            return 0;
        }

        return res;
    }
}
