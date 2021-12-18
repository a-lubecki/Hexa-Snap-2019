/**
 * Hexa Snap
 * © Aurélien Lubecki 2019
 * All Rights Reserved
 */

using System;


public class Purchase {


    public string purchaserId { get; private set; }
    public string transactionId { get; private set; }
    public DateTime purchaseDate { get; private set; }
    public string purchaseTag { get; private set; }
    public string purchasePrice { get; private set; }
    public int purchaseNbHexacoins { get; private set; }
    public bool purchaseIsRemovingAds { get; private set; }
    public string receipt { get; private set; }
    public int nbHexacoins { get; private set; }
    public bool hasRemovedAds { get; private set; }


    public Purchase(string purchaserId, string transactionId, DateTime purchaseDate, string purchaseTag, 
                    string purchasePrice, int purchaseNbHexacoins, bool purchaseIsRemovingAds, string receipt,
                    int nbHexacoins, bool hasRemovedAds) {

        if (string.IsNullOrEmpty(transactionId)) {
            throw new ArgumentException();
        }

        if (string.IsNullOrEmpty(purchaseTag)) {
            throw new ArgumentException();
        }

        this.purchaserId = purchaserId;
        this.transactionId = transactionId;
        this.purchaseDate = purchaseDate;
        this.purchaseTag = purchaseTag;
        this.purchasePrice = purchasePrice;
        this.purchaseNbHexacoins = purchaseNbHexacoins;
        this.purchaseIsRemovingAds = purchaseIsRemovingAds;
        this.receipt = receipt;
        this.nbHexacoins = nbHexacoins;
        this.hasRemovedAds = hasRemovedAds;
    }

}
