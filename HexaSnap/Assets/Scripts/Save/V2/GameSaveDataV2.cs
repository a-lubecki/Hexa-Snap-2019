/**
 * Hexa Snap
 * © Aurélien Lubecki 2019
 * All Rights Reserved
 */

using System;

[Serializable]
public class GameSaveDataV2 {

    public GameSaveDataV2() {
    }

    public GameSaveDataV2(GameSaveDataV1 previousData) {

        if (previousData == null) {
            return;
        }

        var previousOptions = previousData.optionsSaveData;
        if (previousOptions != null) {
            optionsSaveData = new OptionsSaveDataV2(previousOptions);
        }

        hexacoinsWalletSaveData = previousData.hexacoinsWalletSaveData;
        upgradesSaveData = previousData.upgradesSaveData;
        bestScoresSaveData = previousData.bestScoresSaveData;
        characterSaveData = previousData.characterSaveData;
        propertiesSaveData = previousData.propertiesSaveData;
        shopItemsSaveData = previousData.shopItemsSaveData;
    }

	public OptionsSaveDataV2 optionsSaveData;
	public HexacoinsWalletSaveData hexacoinsWalletSaveData;
	public UpgradesSaveData upgradesSaveData;
	public BestScoresSaveData bestScoresSaveData;
    public CharacterSaveData characterSaveData;
    public PropertiesSaveData propertiesSaveData;
    public ShopItemsSaveData shopItemsSaveData;

}
