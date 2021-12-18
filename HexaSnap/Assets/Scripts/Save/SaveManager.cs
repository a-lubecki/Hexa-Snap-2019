/**
 * Hexa Snap
 * © Aurélien Lubecki 2019
 * All Rights Reserved
 */


public class SaveManager {


    public static readonly SaveManager Instance = new SaveManager();

    private SaveManager() {
    }


    public void saveOptions() {

        GameSaverLocal.instance.saveOptions();
        GameSaverLocal.instance.saveAllToFile();
    }

    public void saveHexacoinsWallet() {

        GameSaverLocal.instance.saveHexacoinsWallet();
        GameSaverLocal.instance.saveAllToFile();

        FirebaseFunctionsManager.instance.updateWallet();
    }

    public void saveGraphs() {

        GameSaverLocal.instance.saveUpgrades();
        GameSaverLocal.instance.saveAllToFile();
    }

    public void saveGraphArcade() {

        saveGraphs();

        FirebaseFunctionsManager.instance.updateGraphArcade();
    }

    public void saveGraphTimeAttack() {

        saveGraphs();

        FirebaseFunctionsManager.instance.updateGraphTimeAttack();
    }

    public void saveBestScores() {

        GameSaverLocal.instance.saveBestScores();
        GameSaverLocal.instance.saveAllToFile();
    }

    public void saveResultArcade(int score, int level) {

        GameSaverLocal.instance.saveBestScores();
        GameSaverLocal.instance.saveAllToFile();

        FirebaseFunctionsManager.instance.updateResultArcade(score, level);
    }

    public void saveResultTimeAttack(int score, float timeSec) {

        GameSaverLocal.instance.saveBestScores();
        GameSaverLocal.instance.saveAllToFile();

        FirebaseFunctionsManager.instance.updateResultTimeAttack(score, timeSec);
    }

    public void saveCharacter() {

        GameSaverLocal.instance.saveCharacter();
        GameSaverLocal.instance.saveAllToFile();
    }

    public void saveShopItems() {

        GameSaverLocal.instance.saveShopItems();
        GameSaverLocal.instance.saveAllToFile();
    }

}

