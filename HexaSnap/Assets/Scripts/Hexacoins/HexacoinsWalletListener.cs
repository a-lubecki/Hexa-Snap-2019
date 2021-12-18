/**
 * Hexa Snap
 * © Aurélien Lubecki 2019
 * All Rights Reserved
 */

public interface HexacoinsWalletListener : BaseModelListener {
		
    void onNbHexacoinsChanged(HexacoinsWallet hexacoinsWallet, int difference);
}

