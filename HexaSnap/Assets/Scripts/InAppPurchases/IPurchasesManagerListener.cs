/**
 * Hexa Snap
 * © Aurélien Lubecki 2019
 * All Rights Reserved
 */

using UnityEngine.Purchasing;
using UnityEngine.Purchasing.Security;


public interface IPurchasesManagerListener {

    void onPurchaseInitDone();

    void onPurchaseInitFailed();

    void onProcessPurchaseStart(ShopItem item);

    void onProcessPurchaseFailed(ShopItem item, bool wasCanceled);

    void onPurchaseSendingStart(ShopItem item);

    void onPurchaseSendingDone(ShopItem item, Purchase purchase);

    void onPurchaseSendingFailed(ShopItem item, PurchaseEventArgs pendingArgs, IPurchaseReceipt pendingReceipt);

}
