/**
 * Hexa Snap
 * © Aurélien Lubecki 2019
 * All Rights Reserved
 */

public abstract class Activity12 : Activity12_13 {


    protected override string getTextTitle() {
        return Tr.get("Activity12.Title");
    }

    protected override bool isGameOverScreen() {
        return true;
    }

    protected override bool canContinuePlaying() {
        return false;
    }

    protected abstract int getLastScoreValue();
    protected abstract int getLastLevelValue();
    protected abstract float getLastTimeSecValue();

    protected override int getNbHexacoinsToEarn() {
        return 0;
    }

    protected override CharacterSituation getEndGameCharacterSituation() {
        
        int lastScore = getLastScoreValue();
        int currentScore = getScoreValue();

        int lastLevel = getLastLevelValue();
        int currentLevel = getLevelValue();

        float lastTimeSec = getLastTimeSecValue();
        float timeSec = getTimeSecValue();

        if (currentLevel <= Constants.MAX_LEVEL_HARDCORE &&
            currentScore <= lastScore && 
            (currentLevel <= lastLevel || timeSec <= lastTimeSec)) {

            //show a random disapointed speech
            return new CharacterSituation()
                .enqueueTrRandom("12.Bad", 10)
                .enqueueExpression(CharacterRes.EXPR_SAD, 4);
        }

        if (currentLevel > lastLevel) {

            return new CharacterSituation()
                .enqueueTrRandom("12a.Level", 4)
                .enqueueExpression(CharacterRes.EXPR_AMAZED, 4)
                .enqueueMove(CharacterRes.MOVE_BOUNCE)
                .enqueueMove(CharacterRes.MOVE_BOUNCE)
                .enqueueMove(CharacterRes.MOVE_BOUNCE);
        }

        if (timeSec > lastTimeSec) {

            return new CharacterSituation()
                .enqueueTrRandom("12b.Time", 4)
                .enqueueExpression(CharacterRes.EXPR_AMAZED, 4)
                .enqueueMove(CharacterRes.MOVE_SPIRAL);
        }

        if (currentScore > lastScore) {

            return new CharacterSituation()
                .enqueueTrRandom("12.Score", 5)
                .enqueueExpression(CharacterRes.EXPR_AMAZED, 4)
                .enqueueMove(CharacterRes.MOVE_SHIVER);
        }

        return null;
    }

}