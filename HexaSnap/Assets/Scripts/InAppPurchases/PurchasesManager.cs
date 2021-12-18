/**
 * Hexa Snap
 * © Aurélien Lubecki 2019
 * All Rights Reserved
 */

using System;
using System.Linq;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Purchasing;
using UnityEngine.Purchasing.Security;


public class PurchasesManager : IStoreListener {


    public int nbPurchasesDone { get; private set; }
    private HashSet<PendingPurchase> pendingPurchases = new HashSet<PendingPurchase>();

    public bool isInitializingIAP { get; private set; }

    private IStoreController storeController;
    private IExtensionProvider storeExtensionProvider;

    public bool isProcessingPurchase { get; private set; }

    private WeakReference listener;


    private IPurchasesManagerListener getListener() {

        if (listener == null) {
            return null;
        }

        return listener.Target as IPurchasesManagerListener;
    }

    public void init(int nbPurchasesDone, HashSet<PendingPurchase> pendingPurchases) {

        this.nbPurchasesDone = nbPurchasesDone;

        if (pendingPurchases == null) {
            this.pendingPurchases = new HashSet<PendingPurchase>();
        } else {
            this.pendingPurchases = new HashSet<PendingPurchase>(pendingPurchases);
        }
    }

    public bool isIAPInitialized() {
        return storeController != null && storeExtensionProvider != null;
    }

    public void initIAP(IPurchasesManagerListener listener) {
        
        if (isInitializingIAP) {
            return;
        }

        if (listener != null) {
            this.listener = new WeakReference(listener);
        }

        isInitializingIAP = true;

        //retrieve catalog from unity dashboard
        var builder = ConfigurationBuilder.Instance(StandardPurchasingModule.Instance());
        foreach (var item in ShopItem.PURCHASES) {

            if (item.type != ShopItemType.IAP) {
                continue;
            }

            builder.AddProduct(item.tag, ProductType.Consumable);
        }

        //initialize then wait for data fetching
        UnityPurchasing.Initialize(this, builder);
    }

    #region IStoreListener init
    void IStoreListener.OnInitialized(IStoreController controller, IExtensionProvider extensions) {

        isInitializingIAP = false;

        storeController = controller;
        storeExtensionProvider = extensions;

        //init shop items with IAP products
        foreach (ShopItem item in ShopItem.PURCHASES) {

            var product = controller?.products?.WithID(item.tag);
            if (product != null && product.availableToPurchase) {
                item.setIAPProduct(product);
            }
        }

        getListener()?.onPurchaseInitDone();
    }

    void IStoreListener.OnInitializeFailed(InitializationFailureReason error) {
        
        isInitializingIAP = false;

        getListener()?.onPurchaseInitFailed();
    }
    #endregion

    public void processPurchase(ShopItem item) {

        if (!isIAPInitialized()) {
            return;
        }

        if (item == null || item.iapProduct == null) {
            return;
        }

        if (isProcessingPurchase) {
            //already purchasing a product
            return;
        }

        isProcessingPurchase = true;

        getListener()?.onProcessPurchaseStart(item);

        storeController.InitiatePurchase(item.iapProduct);
    }

    #region IStoreListener process purchase
    PurchaseProcessingResult IStoreListener.ProcessPurchase(PurchaseEventArgs args) {

        var product = args.purchasedProduct;
        var item = findShopItem(args.purchasedProduct);

        //validate the transaction
        var itemTag = product.definition.id;
        var receipt = product.receipt;
        IPurchaseReceipt validatedReceipt = null;

        if (receipt != null) {

            var validator = new CrossPlatformValidator(
                GooglePlayTangle.Data(),
                AppleTangle.Data(),
                Application.identifier
            );

            try {
                var result = validator.Validate(receipt);

                //find the receipt
                foreach (var r in result) {
                    
                    if (itemTag.Equals(r.productID)) {
                        //found
                        validatedReceipt = r;
                        break;
                    }
                }

            } catch (IAPSecurityException) {

                //not a valid transaction
                isProcessingPurchase = false;

                getListener()?.onProcessPurchaseFailed(item, false);

                trackIAPFail(item, T.Value.IAP_REASON_INVALID_RECEIPT);

                return PurchaseProcessingResult.Complete;
            }
        }


        if (validatedReceipt == null) {

            //couldn't find receipt
            isProcessingPurchase = false;

            getListener()?.onProcessPurchaseFailed(item, false);

            trackIAPFail(item, T.Value.IAP_REASON_NO_RECEIPT);

            return PurchaseProcessingResult.Complete;
        }

        sendPurchaseToFirebase(args, validatedReceipt);

        return PurchaseProcessingResult.Pending;
    }

    void IStoreListener.OnPurchaseFailed(Product product, PurchaseFailureReason reason) {

        isProcessingPurchase = false;

        getListener()?.onProcessPurchaseFailed(
            findShopItem(product),
            reason == PurchaseFailureReason.UserCancelled
        );

        trackIAPFail(findShopItem(product), "e_" + reason);
    }
    #endregion

    public ShopItem findShopItem(Product purchasedProduct) {
        return ShopItem.PURCHASES.First(p => p.tag == purchasedProduct.definition?.id);
    }

    public void sendPurchaseToFirebase(PurchaseEventArgs args, IPurchaseReceipt receipt) {
        
        var product = args.purchasedProduct;
        var item = findShopItem(args.purchasedProduct);

        getListener()?.onProcessPurchaseStart(item);

        var purchase = new Purchase(
            UserIdManager.Instance.getUserId(),
            receipt.transactionID ?? "",
            receipt.purchaseDate.ToUniversalTime(),
            item.tag,
            product.metadata?.localizedPriceString ?? "",
            item.nbEarnedHexacoins,
            item.isRemovingAds,
            receipt.ToString() ?? "",
            GameHelper.Instance.getHexacoinsWallet().nbHexacoins,
            GameHelper.Instance.getGameManager().hasRemovedAds
        );

        FirebaseFunctionsManager.instance.addInAppPurchase(
            purchase,
            () => {

                isProcessingPurchase = false;

                //confirm transaction
                storeController.ConfirmPendingPurchase(product);

                //increase nb purchases for tracking
                nbPurchasesDone++;

                //manage pending purchase local save
                if (!LoginManager.Instance.isLoggedInFacebook()) {
                    //can't register purchase if not logged, save it before logging in
                    addPendingPurchase(new PendingPurchase(purchase));
                }

                SaveManager.Instance.saveShopItems();

                getListener()?.onPurchaseSendingDone(item, purchase);

                trackIAPSuccess(purchase);
            },
            _ => {

                isProcessingPurchase = false;

                getListener()?.onPurchaseSendingFailed(findShopItem(product), args, receipt);

                trackIAPFail(item, T.Value.IAP_REASON_SENDING_FAILED);
            }
        );
    }

    public HashSet<PendingPurchase> getPendingPurchases() {
        //defensive copy
        return new HashSet<PendingPurchase>(pendingPurchases);
    }

    public void addPendingPurchase(PendingPurchase purchase) {
        
        if (purchase == null) {
            throw new ArgumentException();
        }

        var transactionId = purchase.transactionId;

        if (pendingPurchases.Any(p => transactionId.Equals(p.transactionId))) {
            //already existing
            return;
        }

        pendingPurchases.Add(purchase);
    }

    public void removePendingPurchase(string transactionId) {

        if (transactionId == null) {
            throw new ArgumentException();
        }

        pendingPurchases.RemoveWhere((p) => transactionId.Equals(p.transactionId));
    }

    public void clearPendingPurchases() {

        pendingPurchases.Clear();
    }

    private void trackIAPSuccess(Purchase purchase) {

        //track IAP
        TrackingManager.instance.setUserProperty(
            T.Property.IAP_NB_PURCHASED,
            Prop.nbPurchasedIAP.increment()
        );

        TrackingManager.instance.setUserProperty(
            T.Property.IAP_MAX_HEXACOINS,
            Prop.maxHexacoinsEarnedWithIAP.putIfGreater(purchase.purchaseNbHexacoins)
        );

        TrackingManager.instance.setUserProperty(
            T.Property.IAP_TOTAL_HEXACOINS,
            Prop.totalHexacoinsEarnedWithIAP.add(purchase.purchaseNbHexacoins)
        );

        TrackingManager.instance.prepareEvent(T.Event.IAP_PAID)
                       .add(T.Param.ID, purchase.purchaseTag)
                       .add(T.Param.NB_HEXACOINS, purchase.purchaseNbHexacoins)
                       .add(T.Param.REMOVING_ADS, purchase.purchaseIsRemovingAds ? T.Value.TRUE : T.Value.FALSE)
                       .track();
    }

    private void trackIAPFail(ShopItem item, string reason) {

        TrackingManager.instance.prepareEvent(T.Event.IAP_FAILED)
                       .add(T.Param.ID, item.tag)
                       .add(T.Param.NB_HEXACOINS, item.nbEarnedHexacoins)
                       .add(T.Param.REMOVING_ADS, item.isRemovingAds ? T.Value.TRUE : T.Value.FALSE)
                       .add(T.Param.REASON, reason)
                       .track();
    }

}
