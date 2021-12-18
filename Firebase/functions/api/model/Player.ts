import { isNullOrUndefined } from "util"

/**
 * Hexa Snap
 * © Aurélien Lubecki 2019
 * All Rights Reserved
 */


export class Player {


    readonly hasBeenCreated: boolean
    readonly hasRemovedAds: boolean
    readonly nbHexacoins: number
    readonly purchases: Map<string, boolean>

    constructor(hasBeenCreated: boolean, hasRemovedAds: boolean, nbHexacoins: number,
        purchases: Map<string, boolean> | undefined) {

        this.hasBeenCreated = hasBeenCreated
        this.hasRemovedAds = hasRemovedAds
        this.nbHexacoins = nbHexacoins

        if (!isNullOrUndefined(purchases)) {
            this.purchases = purchases
        } else {
            this.purchases = new Map()
        }

        if (this.nbHexacoins < 0) {
            throw new Error("Negative Player.nbHexacoins")
        }
    }

}
