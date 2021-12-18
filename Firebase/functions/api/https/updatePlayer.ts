/**
 * Hexa Snap
 * © Aurélien Lubecki 2019
 * All Rights Reserved
 */

import * as functions from 'firebase-functions'
import Response = functions.Response
import { ResponseData } from "../common/ResponseData"
import { Player } from "../model/Player"
import { DatabasePlayers } from "../db/DatabasePlayers"
import { SerializerPlayer } from "../serializing/SerializerPlayer"
import { isNullOrUndefined } from "util"


function transformHasRemovedAds(dbHasRemovedAds: boolean | undefined, sentHasRemovedAds: boolean | undefined): boolean {

    if (isNullOrUndefined(dbHasRemovedAds)) {

        if (isNullOrUndefined(sentHasRemovedAds)) {
            return false
        }

        return sentHasRemovedAds

    } else if (isNullOrUndefined(sentHasRemovedAds)) {

        return dbHasRemovedAds
    }

    return dbHasRemovedAds || sentHasRemovedAds
}

export function transformNbHexacoins(dbPlayer: Player | undefined,
    sentNbHexacoins: number | undefined, lastRemoteNbHexacoins: number | undefined): number {

    let dbNbHexacoins = 0
    if (!isNullOrUndefined(dbPlayer)) {
        dbNbHexacoins = dbPlayer.nbHexacoins
    }

    let sent = 0
    if (!isNullOrUndefined(sentNbHexacoins)) {
        sent = sentNbHexacoins
    }

    let lastRemote: number
    if (!isNullOrUndefined(lastRemoteNbHexacoins)) {
        lastRemote = lastRemoteNbHexacoins
    } else {
        //result will only be the sent hexacoins
        lastRemote = dbNbHexacoins
    }

    //take the most valuable values
    let result = dbNbHexacoins + sent - lastRemote
    if (result < 0) {
        result = 0
    }

    return result
}

export async function updatePlayer(res: Response, userId: string, sentHasRemovedAds: boolean,
    sentNbHexacoins: number, lastRemoteNbHexacoins: number | undefined, minNbHexacoins: number,
    additionalPurchasesSyncState: Map<string, boolean>): Promise<Player> {

    const serializerPlayer = new SerializerPlayer()

    let hasBeenCreated = false
    let dbHasRemovedAds = false

    //retrieve player
    const player = await DatabasePlayers.getPlayer(
        userId,
        serializerPlayer
    )

    let purchases: Map<string, boolean>

    if (isNullOrUndefined(player)) {
        hasBeenCreated = true
        purchases = additionalPurchasesSyncState
    } else {
        dbHasRemovedAds = player.hasRemovedAds
        purchases = mergePurchasesSyncStates(player.purchases, additionalPurchasesSyncState)
    }

    //take the most valuable values
    const hasRemovedAds = transformHasRemovedAds(dbHasRemovedAds, sentHasRemovedAds)
    let nbHexacoins = transformNbHexacoins(player, sentNbHexacoins, lastRemoteNbHexacoins)

    if (nbHexacoins < minNbHexacoins) {
        nbHexacoins = minNbHexacoins
    }

    let newPlayer: Player
    try {
        newPlayer = new Player(
            hasBeenCreated,
            hasRemovedAds,
            nbHexacoins,
            purchases
        )
    } catch (e) {
        throw ResponseData.send500(res)
    }

    //create or update player if doesn't exist yet
    const ok = await DatabasePlayers.updatePlayer(
        userId,
        serializerPlayer,
        newPlayer
    )

    if (!ok) {
        throw ResponseData.send500(res)
    }

    return newPlayer
}

function mergePurchasesSyncStates(dbPurchases: Map<string, boolean>, sentPurchases: Map<string, boolean>) {

    if (isNullOrUndefined(dbPurchases)) {
        return sentPurchases
    }

    if (isNullOrUndefined(sentPurchases)) {
        return dbPurchases
    }

    return new Map([...sentPurchases, ...dbPurchases])
}
