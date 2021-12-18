
/**
 * Hexa Snap
 * © Aurélien Lubecki 2019
 * All Rights Reserved
 */

import { Utils } from "../common/Utils"


export class Purchase {


    readonly purchaseId: string
    readonly userId: string | undefined
    readonly platform: string
    readonly purchaserId: string
    readonly transactionId: string
    readonly purchaseDate: Date
    readonly purchaseTag: string
    readonly purchasePrice: string
    readonly purchaseNbHexacoins: number
    readonly purchaseIsRemovingAds: boolean
    readonly receipt: string
    readonly nbHexacoins: number
    readonly hasRemovedAds: boolean


    constructor(
        userId: string | undefined,
        platform: string | undefined,
        purchaserId: string | undefined,
        transactionId: string | undefined,
        purchaseDate: Date | number | undefined,
        purchaseTag: string | undefined,
        purchasePrice: string | undefined,
        purchaseNbHexacoins: number | undefined,
        purchaseIsRemovingAds: boolean | undefined,
        receipt: string | undefined,
        nbHexacoins: number | undefined,
        hasRemovedAds: boolean | undefined) {

        if (!Utils.isValueCorrect(platform, String)) {
            throw new Error("Null BasePurchase.platform")
        }

        if (!Utils.isValueCorrect(purchaserId, String)) {
            throw new Error("Null BasePurchase.platform")
        }

        if (!Utils.isValueCorrect(transactionId, String)) {
            throw new Error("Null BasePurchase.transactionId")
        }

        let date: Date
        if (Utils.isValueCorrect(purchaseDate, Date)) {
            date = purchaseDate as Date
        } else if (Utils.isValueCorrect(purchaseDate, Number)) {
            date = new Date(purchaseDate as number * 1000)
        } else {
            throw new Error("Null Purchase.purchaseDate")
        }

        if (!Utils.isValueCorrect(purchaseTag, String)) {
            throw new Error("Null Purchase.purchaseTag")
        }

        if (!Utils.isValueCorrect(purchasePrice, String)) {
            throw new Error("Null Purchase.purchasePrice")
        }

        if (!Utils.isValueCorrect(purchaseNbHexacoins, Number)) {
            throw new Error("Null Purchase.purchaseNbHexacoins")
        }

        if (!Utils.isValueCorrect(purchaseIsRemovingAds, Boolean)) {
            throw new Error("Null Purchase.purchaseIsRemovingAds")
        }

        if (!Utils.isValueCorrect(receipt, String)) {
            throw new Error("Null Purchase.receipt")
        }

        if (!Utils.isValueCorrect(nbHexacoins, Number)) {
            throw new Error("Null BasePurchase.nbHexacoins")
        }

        if (!Utils.isValueCorrect(hasRemovedAds, Boolean)) {
            throw new Error("Null BasePurchase.hasRemovedAds")
        }

        this.userId = userId
        this.platform = platform as string
        this.purchaserId = purchaserId as string
        this.transactionId = transactionId as string
        this.purchaseDate = date
        this.purchaseTag = purchaseTag as string
        this.purchasePrice = purchasePrice as string
        this.purchaseNbHexacoins = purchaseNbHexacoins as number
        this.purchaseIsRemovingAds = purchaseIsRemovingAds as boolean
        this.receipt = receipt as string
        this.nbHexacoins = nbHexacoins as number
        this.hasRemovedAds = hasRemovedAds as boolean

        this.purchaseId = Purchase.getPurchaseId(this.platform, this.transactionId)
    }

    static getPurchaseId(platform: string, transactionId: string) {

        if (platform.length <= 0) {
            throw new Error("Empty BasePurchase.platform")
        }

        if (transactionId.length <= 0) {
            throw new Error("Empty BasePurchase.transactionId")
        }

        return platform + "-" + transactionId
    }

    static getTransactionId(purchaseId: string): string {

        const pos = purchaseId.indexOf("-")
        if (pos < 0) {
            return ""
        }

        return purchaseId.substr(pos + 1)
    }

}
