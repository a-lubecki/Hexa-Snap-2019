/**
 * Hexa Snap
 * © Aurélien Lubecki 2019
 * All Rights Reserved
 */

import * as functions from 'firebase-functions'
import Response = functions.Response
import { RequestCheck } from '../common/RequestCheck'
import { RequestParams } from '../common/RequestParams'
import { SerializerPlayer } from '../serializing/SerializerPlayer'
import { ResponseData } from '../common/ResponseData'
import { DatabasePlayersModes } from '../db/DatabasePlayersModes'
import { GameMode } from '../model/GameMode'
import { SerializerModeArcade } from '../serializing/SerializerModeArcade'
import { BaseSerializerMode } from '../serializing/BaseSerializerMode'
import { SerializerModeTimeAttack } from '../serializing/SerializerModeTimeAttack'
import { BaseMode } from '../model/BaseMode'
import { ModeTimeAttack } from '../model/ModeTimeAttack'
import { ModeArcade } from '../model/ModeArcade'
import { Graph } from '../model/Graph'
import { GraphNodesMap } from '../model/GraphNodeMap'
import { Utils } from '../common/Utils'
import { updatePlayer } from './updatePlayer'
import { isNullOrUndefined } from 'util'
import { DatabasePurchases } from '../db/DatabasePurchases'
import { Purchase } from '../model/Purchase'
import { DatabasePlayers } from '../db/DatabasePlayers'


/*
 * when the player wants to reconnect from another device after playing some games => make a merge of device + db data
 * synchronize(userId, hasRemovedAds, arcade, timeAttack) : (nbHexacoins, hasRemovedAds, arcade, timeAttack)
    // body :
    {
        "data" : {
            "nbHexacoins" : 0,
            "hasRemovedAds" : false,
            "arcade" : {
                "maxScore" : 0,
                "maxLevel" : 0,
                "graph" : {
                    "zones" : 0,
                    "unlockedNodes" : [],
                    "activeNodes" : []
                }
            },
            "timeAttack" : {
                "maxScore" : 0,
                "maxTimeSec" : 0
                "graph" : {
                    "zones" : 0,
                    "unlockedNodes" : [],
                    "activeNodes" : []
                }
            },
            "pendingPurchases" : [
                "123456",
                "678910",
                "111213"
            ]
        }
    }
 */
export const synchronize = functions.https.onRequest(async (req, res) => {

    console.log(JSON.stringify(req.body.data))

    const extractors = new Map<string, any>()
    extractors.set("player", RequestParams.extractParamObject)

    const params = await RequestCheck.checkFunctionOrFail(true, req, res, extractors)
    const userId = params.get(RequestParams.paramUserId)
    const platform = params.get(RequestParams.paramPlatform)
    const playerData = params.get("player")

    const serializerPlayer = new SerializerPlayer()
    const sentPlayer = serializerPlayer.deserialize(playerData)

    //create an object to fill with new values and to return with 200
    const responsePlayerData = {
        player: {}
    }

    let sentHasRemovedAds = false
    let sentNbHexacoins = 0

    if (!isNullOrUndefined(sentPlayer)) {
        sentHasRemovedAds = sentPlayer.hasRemovedAds
        sentNbHexacoins = sentPlayer.nbHexacoins
    }

    let lastRemoteNbHexacoins = playerData["lastRemoteNbHexacoins"]
    if (!Utils.isValueCorrect(lastRemoteNbHexacoins, Number) || lastRemoteNbHexacoins < 0) {
        lastRemoteNbHexacoins = 0
    }

    const pendingPurchases = newPendingPurchasesMap(platform, playerData["pendingPurchases"])

    //merge sent player and db player
    const newPlayer = await updatePlayer(
        res,
        userId,
        sentHasRemovedAds,
        sentNbHexacoins,
        lastRemoteNbHexacoins,
        0,
        pendingPurchases
    )

    //fill response with player
    const serializedPlayer = serializerPlayer.serialize(newPlayer, false)
    if (!isNullOrUndefined(serializedPlayer)) {
        responsePlayerData["player"] = serializedPlayer
    }

    //create or update mode arcade
    let ok = await processMode(
        GameMode.ARCADE,
        new SerializerModeArcade(),
        (dbMode, dataMode) => mergeModeArcade(res, dbMode as ModeArcade, dataMode),
        userId,
        playerData,
        responsePlayerData
    )

    if (!ok) {
        throw ResponseData.send500(res, "Failed processMode Arcade")
    }

    //create or update mode time attack
    ok = await processMode(
        GameMode.TIME_ATTACK,
        new SerializerModeTimeAttack(),
        (dbMode, dataMode) => mergeModeTimeAttack(res, dbMode as ModeTimeAttack, dataMode),
        userId,
        playerData,
        responsePlayerData
    )

    if (!ok) {
        throw ResponseData.send500(res, "Failed processMode Time Attack")
    }

    //update the userId in the purchases collection then update the player again with the saved purchases
    await syncPlayerPurchases(
        userId,
        newPlayer.purchases,
        responsePlayerData
    )

    endSynchronize(res, responsePlayerData)
})

function newPendingPurchasesMap(platform: string, dataPendingPurchases: Array<any>): Map<string, boolean> {

    if (!Utils.isValueCorrect(dataPendingPurchases, Array)) {
        return new Map<string, boolean>()
    }

    const res = new Map<string, boolean>()

    dataPendingPurchases.forEach(transactionId => {

        if (!Utils.isValueCorrect(transactionId, String)) {
            return
        }

        let purchaseId: string
        try {
            purchaseId = Purchase.getPurchaseId(platform, transactionId)
        } catch (e) {
            console.warn(`newPendingPurchasesMap => ${platform}, ${transactionId}`)
            return
        }

        res.set(purchaseId, false)
    })

    return res
}

async function syncPlayerPurchases(userId: string, newPurchases: Map<string, boolean>, responsePlayerData: any) {

    //prepare the response
    const updatedPurchases = new Map(newPurchases)
    responsePlayerData["player"]["savedPurchases"] = []

    for (const [purchaseId, isUpdated] of newPurchases) {

        if (isUpdated) {
            //already updated
            continue
        }

        //update the userId in the purchases collection in db
        const ok = await DatabasePurchases.updatePlayer(purchaseId, userId)
        if (!ok) {
            continue
        }

        //mark as updated
        updatedPurchases.set(purchaseId, true)

        //if saved, send the transactionId of the purchase back so that the client can erase it from local save
        responsePlayerData["player"]["savedPurchases"].push(Purchase.getTransactionId(purchaseId))
    }

    //update the player purchases, ignore if operation fail
    await DatabasePlayers.updatePurchases(userId, updatedPurchases)
}

async function processMode(gameMode: GameMode, serializer: BaseSerializerMode,
    merger: (dbMode: BaseMode | undefined, dataMode: any) => BaseMode | undefined, userId: string,
    playerData: any, responsePlayerData: any): Promise<boolean> {

    //retrieve modes
    const dbMode = await DatabasePlayersModes.getMode(
        userId,
        gameMode,
        serializer
    )

    //merge sent mode and db mode
    const newMode = merger(dbMode, playerData[gameMode])

    if (isNullOrUndefined(newMode)) {
        return false
    }

    //create or update mode
    const ok = await DatabasePlayersModes.updateMode(
        userId,
        serializer,
        newMode
    )

    if (ok) {
        //fill the response object to return with 200
        responsePlayerData["player"][gameMode] = serializer.serialize(newMode)
    }

    return ok
}

function mergeModeArcade(res: Response, dbMode: ModeArcade | undefined, dataMode: any): ModeArcade | undefined {

    const sentMode = new SerializerModeArcade().deserialize(dataMode)

    let maxScore = 0
    let maxLevel = 0
    let dbGraph: Graph | undefined
    let sentGraph: Graph | undefined

    if (!isNullOrUndefined(dbMode)) {

        if (!isNullOrUndefined(sentMode)) {

            //take the most valuable values
            maxScore = Math.max(dbMode.maxScore, sentMode.maxScore)
            maxLevel = Math.max(dbMode.maxLevel, sentMode.maxLevel)
            dbGraph = dbMode.graph
            sentGraph = sentMode.graph

        } else {

            //take only the db values
            maxScore = dbMode.maxScore
            maxLevel = dbMode.maxLevel
            dbGraph = dbMode.graph
        }

    } else if (!isNullOrUndefined(sentMode)) {

        //take only the sent values
        maxScore = sentMode.maxScore
        maxLevel = sentMode.maxLevel
        sentGraph = sentMode.graph
    }

    try {
        return new ModeArcade(
            maxScore,
            maxLevel,
            mergeGraph(dbGraph, sentGraph)
        )
    } catch (e) {
        throw ResponseData.send500(res, "Failed creating ModeArcade")
    }
}

function mergeModeTimeAttack(res: Response, dbMode: ModeTimeAttack | undefined, dataMode: any): ModeTimeAttack | undefined {

    const sentMode = new SerializerModeTimeAttack().deserialize(dataMode)

    let maxScore = 0
    let maxTimeSec = 0
    let dbGraph: Graph | undefined
    let sentGraph: Graph | undefined

    if (!isNullOrUndefined(dbMode)) {

        if (!isNullOrUndefined(sentMode)) {

            //take the most valuable values
            maxScore = Math.max(dbMode.maxScore, sentMode.maxScore)
            maxTimeSec = Math.max(dbMode.maxTimeSec, sentMode.maxTimeSec)
            dbGraph = dbMode.graph
            sentGraph = sentMode.graph

        } else {

            //take only the db values
            maxScore = dbMode.maxScore
            maxTimeSec = dbMode.maxTimeSec
            dbGraph = dbMode.graph
        }

    } else if (!isNullOrUndefined(sentMode)) {

        //take only the sent values
        maxScore = sentMode.maxScore
        maxTimeSec = sentMode.maxTimeSec
        sentGraph = sentMode.graph
    }

    try {
        return new ModeTimeAttack(
            maxScore,
            maxTimeSec,
            mergeGraph(dbGraph, sentGraph)
        )
    } catch (e) {
        throw ResponseData.send500(res, "Failed creating ModeTimeAttack")
    }
}

function mergeGraph(dbGraph: Graph | undefined, sentGraph: Graph | undefined): Graph {

    let zones = 0
    let unlockedNodes: GraphNodesMap | undefined
    let activeNodes: GraphNodesMap | undefined

    if (!isNullOrUndefined(dbGraph)) {

        if (!isNullOrUndefined(sentGraph)) {

            //take the most valuable values
            zones = mergeNode(dbGraph.zones, sentGraph.zones)

            try {
                unlockedNodes = new GraphNodesMap(
                    mergeNodesMap(dbGraph.unlockedNodes.getNodes(), sentGraph.unlockedNodes.getNodes())
                )
            } catch (e) {
                console.error(e)
            }

            try {
                activeNodes = new GraphNodesMap(
                    mergeNodesMap(dbGraph.activeNodes.getNodes(), sentGraph.activeNodes.getNodes())
                )
            } catch (e) {
                console.error(e)
            }

        } else {

            //take only the db values
            zones = dbGraph.zones
            unlockedNodes = dbGraph.unlockedNodes
            activeNodes = dbGraph.activeNodes
        }

    } else if (!isNullOrUndefined(sentGraph)) {

        //take only the sent values
        zones = sentGraph.zones
        unlockedNodes = sentGraph.unlockedNodes
        activeNodes = sentGraph.activeNodes
    }

    return new Graph(
        zones,
        unlockedNodes,
        activeNodes
    )
}

function mergeNodesMap(dbMap: Map<string, number>, sentMap: Map<string, number>): Map<string, number> {

    const res = new Map<string, number>()

    //retrieve all keys to iterate on both maps together
    const keys = new Set<string>()

    for (const k of dbMap.keys()) {
        keys.add(k)
    }

    for (const k of sentMap.keys()) {
        keys.add(k)
    }

    for (const k of keys) {

        //merge the values of the same node
        res.set(k, mergeNode(dbMap.get(k), sentMap.get(k)))
    }

    return res
}

function mergeNode(dbNode: number | undefined, sentNode: number | undefined): number {

    if (isNullOrUndefined(sentNode)) {

        if (isNullOrUndefined(dbNode)) {
            return 0
        }

        return dbNode
    }

    if (isNullOrUndefined(dbNode)) {
        return sentNode
    }

    //merge bit to bit, take the best of both
    return sentNode | dbNode
}

function endSynchronize(res: Response, responseData: any) {

    ResponseData.send200(res, responseData)
}
