/**
 * Hexa Snap
 * © Aurélien Lubecki 2019
 * All Rights Reserved
 */

using System;
using System.Linq;
using System.Collections.Generic;


[Serializable]
public class ShopItemsSaveData {


    protected int nbPurchasesDone;

    protected Dictionary<string, DateTime> shopItemsUnblockDateTime;

    private HashSet<PendingPurchaseSaveData> pendingPurchases;


    public ShopItemsSaveData(GameManager gameManager) {
        
        if (gameManager == null) {
			throw new ArgumentException();
		}

        nbPurchasesDone = gameManager.purchasesManager.nbPurchasesDone;

        pendingPurchases = new HashSet<PendingPurchaseSaveData>();

        var purchases = gameManager.purchasesManager.getPendingPurchases();
        if (purchases != null) {
            
            foreach (var p in purchases) {
                pendingPurchases.Add(new PendingPurchaseSaveData(p));
            }
        }

        shopItemsUnblockDateTime = new Dictionary<string, DateTime>();

        var itemsUnblockDateTime = gameManager.rewardsManager.getShopItemsUnblockDateTime();
        if (itemsUnblockDateTime != null) {

            foreach (var elem in itemsUnblockDateTime) {
                shopItemsUnblockDateTime[elem.Key.tag] = elem.Value;
            }
        }
	}

    public int getNbPurchasesDone() {
        return nbPurchasesDone;
    }

    public HashSet<PendingPurchase> getPendingPurchases() {

        var res = new HashSet<PendingPurchase>();

        if (pendingPurchases == null) {
            return res;
        }

        foreach (var p in pendingPurchases) {
            res.Add(p.toPendingPurchase());
        }

        return res;
    }

    public Dictionary<ShopItem, DateTime> getShopItemsUnblockDateTime() {

        var res = new Dictionary<ShopItem, DateTime>();

        if (shopItemsUnblockDateTime == null) {
            return res;
        }

        foreach (var elem in shopItemsUnblockDateTime) {

            //find the item by id
            var foundItem = ShopItem.PURCHASES.FirstOrDefault((item) => item.tag.Equals(elem.Key));
            if (foundItem == null) {
                continue;
            }

            res[foundItem] = elem.Value;
        }

        return res;
    }

}

