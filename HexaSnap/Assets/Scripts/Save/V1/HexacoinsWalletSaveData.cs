/**
 * Hexa Snap
 * © Aurélien Lubecki 2019
 * All Rights Reserved
 */

using System;


[Serializable]
public class HexacoinsWalletSaveData {


    protected int nbHexacoins;
	protected int lastRemoteNbHexacoins;


	public HexacoinsWalletSaveData(HexacoinsWallet hexacoinsWallet) {

		if(hexacoinsWallet == null) {
			throw new ArgumentException();
		}

        nbHexacoins = hexacoinsWallet.nbHexacoins;
        lastRemoteNbHexacoins = hexacoinsWallet.lastRemoteNbHexacoins;
	}

    public int getNbHexacoins() {
        return nbHexacoins;
    }

    public int getLastRemoteNbHexacoins() {
        return lastRemoteNbHexacoins;
    }

}

