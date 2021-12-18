/**
 * Hexa Snap
 * © Aurélien Lubecki 2019
 * All Rights Reserved
 */

import { Player } from "../model/Player"
import { Utils } from "../common/Utils"


export class SerializerPlayer {


    public deserializeSnapshot(snapshot: FirebaseFirestore.DocumentSnapshot): Player | undefined {

        return this.deserialize(snapshot.data())
    }

    public deserialize(data: any): Player | undefined {

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

        let purchasesMap: Map<string, boolean> | undefined

        const purchases = data["purchases"]
        if (Utils.isValueCorrect(purchases, Object)) {
            purchasesMap = Utils.newMapFromObject<boolean>(purchases)
        }

        try {
            return new Player(
                false,
                hasRemovedAds,
                nbHexacoins,
                purchasesMap
            )
        } catch (e) {
            console.error(e)
            return undefined
        }
    }

    public serialize(player: Player, mustSerializePurchases: boolean): object {

        const res: {[k: string]: any} = {
            hasBeenCreated: player.hasBeenCreated,
            hasRemovedAds: player.hasRemovedAds,
            nbHexacoins: player.nbHexacoins,
        }

        if (mustSerializePurchases) {
            const purchases = player.purchases
            if (purchases.size > 0) {
                res["purchases"] = Utils.newObjectFromMap(purchases)
            }
        }

        return res
    }

}
