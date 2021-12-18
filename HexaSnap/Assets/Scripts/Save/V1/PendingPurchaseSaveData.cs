
/**
 * Hexa Snap
 * © Aurélien Lubecki 2019
 * All Rights Reserved
 */

using System;


[Serializable]
public class PendingPurchaseSaveData {


    protected string transactionId;
    protected DateTime purchaseDate;
    protected string purchaseTag;
    protected string purchasePrice;
    protected int purchaseNbHexacoins;
    protected bool purchaseIsRemovingAds;
    protected string receipt;
    protected int nbHexacoinsToConsume;
    protected int nbHexacoins;
    protected bool hasRemovedAds;


    public PendingPurchaseSaveData(PendingPurchase p) {
        
        if (p == null) {
			throw new ArgumentException();
		}

        transactionId = p.transactionId;
        purchaseDate = p.purchaseDate;
	}

    public PendingPurchase toPendingPurchase() {
        
        return new PendingPurchase(
            transactionId,
            purchaseDate
        );
    }

}

