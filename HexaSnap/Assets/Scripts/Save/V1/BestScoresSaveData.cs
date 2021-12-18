/**
 * Hexa Snap
 * © Aurélien Lubecki 2019
 * All Rights Reserved
 */

using System;

[Serializable]
public class BestScoresSaveData {

    //TODO sauvegarder une liste des N meilleurs scores avec la date plutôt que de sauvegarder les scores en vrac
    //et utiliser un BestScoresManager à utiliser dans l'activity50 plutôt que de stocker les scores dans le GameManager

    protected int maxArcadeLevel;
    protected int maxArcadeScore;
    protected int maxTimeAttackScore;
    protected float timeAttackTimeSec;

    public BestScoresSaveData(GameManager gameManager) {

		if(gameManager == null) {
			throw new ArgumentException();
		}

        maxArcadeLevel = gameManager.maxArcadeLevel;
        maxArcadeScore = gameManager.maxArcadeScore;
        maxTimeAttackScore = gameManager.maxTimeAttackScore;
        timeAttackTimeSec = gameManager.maxTimeAttackTimeSec;
    }

    public int getMaxArcadeLevel() {
        return maxArcadeLevel;
    }

    public int getMaxArcadeScore() {
        return maxArcadeScore;
    }

    public int getMaxTimeAttackScore() {
        return maxTimeAttackScore;
    }

    public float getTimeAttackTimeSec() {
        return timeAttackTimeSec;
    }

}

