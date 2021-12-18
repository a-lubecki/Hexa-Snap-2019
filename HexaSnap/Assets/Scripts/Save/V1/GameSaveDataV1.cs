/**
 * Hexa Snap
 * © Aurélien Lubecki 2019
 * All Rights Reserved
 */

using System;

[Serializable]
public class GameSaveDataV1 {

	public OptionsSaveData optionsSaveData;
	public HexacoinsWalletSaveData hexacoinsWalletSaveData;
	public UpgradesSaveData upgradesSaveData;
	public BestScoresSaveData bestScoresSaveData;
    public CharacterSaveData characterSaveData;
    public PropertiesSaveData propertiesSaveData;
    public ShopItemsSaveData shopItemsSaveData;

}
