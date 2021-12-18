/**
 * Hexa Snap
 * © Aurélien Lubecki 2019
 * All Rights Reserved
 */

import { Utils } from "../common/Utils"
import { Purchase } from "../model/Purchase"
import { isNullOrUndefined } from "util"


export class SerializerPurchase {


    readonly userId: string
    readonly platform: string


    constructor(userId: string, platform: string) {

        this.userId = userId
        this.platform = platform
    }


    public deserialize(data: any): Purchase | undefined {

        if (!Utils.isValueCorrect(data, Object)) {
            return undefined
        }

        let hasRemovedAds = data["hasRemovedAds"]
        if (!Utils.isValueCorrect(hasRemovedAds, Boolean)) {
            hasRemovedAds = false
        }

        let nbHexacoins = data["nbHexacoins"]
        if (!Utils.isValueCorrect(nbHexacoins, Number)) {
            nbHexacoins = 0
        }

        try {
            return new Purchase(
                this.userId,
                this.platform,
                data["purchaserId"],
                data["transactionId"],
                data["purchaseDate"],
                data["purchaseTag"],
                data["purchasePrice"],
                data["purchaseNbHexacoins"],
                data["purchaseIsRemovingAds"],
                data["receipt"],
                data["nbHexacoins"],
                hasRemovedAds
            )
        } catch (e) {
            console.error(e)
            return undefined
        }
    }

    public serialize(purchase: Purchase): object {

        const res: {[k: string]: any} = {
            platform: purchase.platform,
            purchaserId: purchase.purchaserId,
            transactionId: purchase.transactionId,
            purchaseDate: purchase.purchaseDate,
            purchaseTag: purchase.purchaseTag,
            purchasePrice: purchase.purchasePrice,
            purchaseNbHexacoins: purchase.purchaseNbHexacoins,
            purchaseIsRemovingAds: purchase.purchaseIsRemovingAds,
            receipt: purchase.receipt,
            hasRemovedAds: purchase.hasRemovedAds,
            nbHexacoins: purchase.nbHexacoins,
        }

        const userId = purchase.userId
        if (!isNullOrUndefined(userId)) {
            res["userId"] = userId
        }

        return res
    }

}
