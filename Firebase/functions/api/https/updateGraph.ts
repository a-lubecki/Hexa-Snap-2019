/**
 * Hexa Snap
 * © Aurélien Lubecki 2019
 * All Rights Reserved
 */

import * as functions from 'firebase-functions'
import Request = functions.Request
import Response = functions.Response
import { RequestParams } from "../common/RequestParams"
import { SerializerGraphNodesMap } from "../serializing/SerializerGraphNodesMap"
import { GameMode } from "../model/GameMode"
import { Graph } from "../model/Graph"
import { DatabasePlayersModes } from "../db/DatabasePlayersModes"
import { SerializerGraph } from "../serializing/SerializerGraph"
import { ResponseData } from "../common/ResponseData"
import { RequestCheck } from '../common/RequestCheck'


export async function updateGraph(req: Request, res: Response, gameMode: GameMode): Promise<boolean> {

    const extractors = new Map<string, any>()
    extractors.set("zones", RequestParams.extractParamNumber)
    extractors.set("unlockedNodes", RequestParams.extractParamNumberArray)
    extractors.set("activeNodes", RequestParams.extractParamNumberArray)

    const params = await RequestCheck.checkFunctionOrFail(true, req, res, extractors)
    const userId = params.get(RequestParams.paramUserId)
    const zones = params.get("zones")
    const unlockedNodes = params.get("unlockedNodes")
    const activeNodes = params.get("activeNodes")

    const nodesSerializer = new SerializerGraphNodesMap()

    let graph: Graph
    try {
        graph = new Graph(
            zones,
            nodesSerializer.deserialize(unlockedNodes),
            nodesSerializer.deserialize(activeNodes)
        )
    } catch (e) {
        throw ResponseData.send400(res)
    }

    //add changes to db
    const ok = await DatabasePlayersModes.updateGraph(
        userId,
        gameMode,
        new SerializerGraph(),
        graph
    )

    if (!ok) {
        throw ResponseData.send500(res)
    }

    ResponseData.send200(res, {
        zones : zones,
        unlockedNodes : unlockedNodes,
        activeNodes : activeNodes
    })

    return true
}
