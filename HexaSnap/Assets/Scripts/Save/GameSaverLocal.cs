/**
 * Hexa Snap
 * © Aurélien Lubecki 2019
 * All Rights Reserved
 */

using UnityEngine;
using System;


public class GameSaverLocal {


    public static readonly GameSaverLocal instance = new GameSaverLocal();


    private readonly FileSaver fileSaver = new FileSaver();
    private readonly GameSaverVersionsHandler versionsHandler;

    private GameSaveDataV2 gameSaveData = new GameSaveDataV2();

    private bool hasLoadedDataFirst;
    private bool hasEnqueuedSaving;


    private GameSaverLocal() {
        versionsHandler = new GameSaverVersionsHandler(fileSaver);
    }

    public void loadAllFromFile() {

        hasLoadedDataFirst = true;

        var data = versionsHandler.loadVersionsUntilCurrent();
        if (data != null) {
            gameSaveData = (GameSaveDataV2)data;
        }
	}

    public void saveAllToFile() {

        if (!hasLoadedDataFirst) {
            //wait to retrieve data before writing data
            return;
        }

        Debug.Log("SAVE asked");

        //avoid calling saving too much on the same frame
        if (hasEnqueuedSaving) {
            return;
        }

        hasEnqueuedSaving = true;

        //call asynchronously to avoid blocking the read/write operation of another thread
        DispatchQueue.Instance.Invoke(saveAllToFileFromMainThread);

        Debug.Log("SAVE pending");
    }

    public void saveAllToFileFromMainThread() {

        if (!hasLoadedDataFirst) {
            //wait to retrieve data before writing data
            return;
        }

        hasEnqueuedSaving = false;

        fileSaver.saveAllToFile(GameSaverVersionsHandler.CURRENT_VERSION, gameSaveData);
	}

	public void deleteSave() {

        fileSaver.deleteSave(GameSaverVersionsHandler.CURRENT_VERSION);
	}

	public OptionsSaveDataV2 getOptionsSaveData() {

		return gameSaveData.optionsSaveData;
	}

	public void saveOptions() {

        gameSaveData.optionsSaveData = new OptionsSaveDataV2(GameHelper.Instance.getGameManager());
	}

	public HexacoinsWalletSaveData getHexacoinsWalletSaveData() {

		return gameSaveData.hexacoinsWalletSaveData;
	}

	public void saveHexacoinsWallet() {

		gameSaveData.hexacoinsWalletSaveData = new HexacoinsWalletSaveData(GameHelper.Instance.getHexacoinsWallet());
	}

	public UpgradesSaveData getUpgradesSaveData() {

		return gameSaveData.upgradesSaveData;
	}

	public void saveUpgrades() {

		gameSaveData.upgradesSaveData = new UpgradesSaveData(GameHelper.Instance.getUpgradesManager());
	}

    public BestScoresSaveData getBestScoresSaveData() {

        return gameSaveData.bestScoresSaveData;
    }

    public void saveBestScores() {

        gameSaveData.bestScoresSaveData = new BestScoresSaveData(GameHelper.Instance.getGameManager());
    }

    public CharacterSaveData getCharacterSaveData() {

        return gameSaveData.characterSaveData;
    }

    public void saveCharacter() {

        gameSaveData.characterSaveData = new CharacterSaveData(GameHelper.Instance.getGameManager());
    }

    public PropertiesSaveData getPropertiesSaveData() {

        return gameSaveData.propertiesSaveData;
    }

    public void saveProperties() {

        gameSaveData.propertiesSaveData = new PropertiesSaveData(PropertyManager.Instance);
    }

    public ShopItemsSaveData getShopItemsSaveData() {

        return gameSaveData.shopItemsSaveData;
    }

    public void saveShopItems() {

        gameSaveData.shopItemsSaveData = new ShopItemsSaveData(GameHelper.Instance.getGameManager());
    }

}

