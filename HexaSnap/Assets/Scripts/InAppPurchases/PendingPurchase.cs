/**
 * Hexa Snap
 * © Aurélien Lubecki 2019
 * All Rights Reserved
 */

using System;


public class PendingPurchase {


    public string transactionId { get; private set; }
    public DateTime purchaseDate { get; private set; }


    public PendingPurchase(string transactionId, DateTime purchaseDate) {

        if (string.IsNullOrEmpty(transactionId)) {
            throw new ArgumentException();
        }

        this.transactionId = transactionId;
        this.purchaseDate = purchaseDate;
    }

    public PendingPurchase(Purchase purchase) {

        if (purchase == null) {
            throw new ArgumentException();
        }

        this.transactionId = purchase.transactionId;
        this.purchaseDate = purchase.purchaseDate;
    }

}
