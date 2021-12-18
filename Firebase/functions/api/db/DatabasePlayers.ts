/**
 * Hexa Snap
 * © Aurélien Lubecki 2019
 * All Rights Reserved
 */

import * as admin from 'firebase-admin'
import { Player } from '../model/Player'
import { SerializerPlayer } from '../serializing/SerializerPlayer'
import { isNullOrUndefined } from 'util'
import { Utils } from '../common/Utils'


const db = admin.firestore()

function document(userId: string): FirebaseFirestore.DocumentReference {

    return db.collection("players")
        .doc(userId)
}


export class DatabasePlayers {


    static getPlayer(userId: string, serializer: SerializerPlayer): Promise<Player | undefined> {

        return document(userId)
            .get()
            .then(doc => {

                if (!doc.exists) {
                    return undefined
                }

                return serializer.deserializeSnapshot(doc)
            })
            .catch(e => {
                console.warn(`DatabasePlayers.getPlayer(${userId}) : ` + e)
                return undefined
            })
    }

    static updatePlayer(userId: string, serializer: SerializerPlayer, player: Player): Promise<boolean> {

        const newPlayer = serializer.serialize(player, true)

        if (isNullOrUndefined(newPlayer)) {
            return Promise.resolve(false)
        }

        return document(userId)
            .set(
                newPlayer,
                { merge: true }
            )
            .then(_ => true)
            .catch(e => {
                console.warn(`DatabasePlayers.updatePlayer(${userId}) : ` + e)
                return false
            })
    }

    static updateNbHexacoins(userId: string, nbHexacoins: number): Promise<boolean> {

        return document(userId)
            .set({
                    nbHexacoins : nbHexacoins
                },
                { merge: true }
            )
            .then(_ => true)
            .catch(e => {
                console.warn(`DatabasePlayers.updateNbHexacoins(${userId}, ${nbHexacoins}) : ` + e)
                return false
            })
    }

    static updateHasRemovedAds(userId: string, hasRemovedAds: boolean): Promise<boolean> {

        return document(userId)
            .set({
                    hasRemovedAds : hasRemovedAds
                },
                { merge: true }
            )
            .then(_ => true)
            .catch(e => {
                console.warn(`DatabasePlayers.updateHasRemovedAds(${userId}, ${hasRemovedAds}) : ` + e)
                return false
            })
    }

    static updatePurchases(userId: string, purchases: Map<string, boolean>): Promise<boolean> {

        if (purchases.size <= 0) {
            return Promise.resolve(true)
        }

        return document(userId)
            .set({
                    purchases : Utils.newObjectFromMap(purchases)
                },
                { merge: true }
            )
            .then(_ => true)
            .catch(e => {
                console.warn(`DatabasePlayers.updatePurchases(${userId}, ${purchases}) : ` + e)
                return false
            })
    }

}
