/**
 * Hexa Snap
 * © Aurélien Lubecki 2019
 * All Rights Reserved
 */


public interface GoalCompletionListener : BaseModelListener {

    void onGoalChange(GoalCompletion g);

    void onGoalCompletionChange(GoalCompletion g);

}
