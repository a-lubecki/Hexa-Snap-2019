/**
 * Hexa Snap
 * © Aurélien Lubecki 2019
 * All Rights Reserved
 */

import * as functions from 'firebase-functions'
import Response = functions.Response
import { RequestParams } from "../common/RequestParams"
import { RequestCheck } from '../common/RequestCheck'
import { ResponseData } from '../common/ResponseData'
import { SerializerPlayer } from '../serializing/SerializerPlayer'
import { DatabasePlayers } from '../db/DatabasePlayers'
import { transformNbHexacoins } from './updatePlayer'


/*
 * updateWallet(userId, nbHexacoins): nbHexacoins
 */
export const updateWallet = functions.https.onRequest(async (req, res) => {

    console.log(JSON.stringify(req.body.data))

    const extractors = new Map<string, any>()
    extractors.set("lastRemoteNbHexacoins", RequestParams.extractParamPositiveNumber)
    extractors.set("nbHexacoins", RequestParams.extractParamPositiveNumber)

    const params = await RequestCheck.checkFunctionOrFail(true, req, res, extractors)
    const userId = params.get(RequestParams.paramUserId)
    const lastRemoteNbHexacoins = params.get("lastRemoteNbHexacoins")
    const nbHexacoins = params.get("nbHexacoins")

    //update the nbHexacoins in db
    const updatedNbHexacoins = await updateNbHexacoins(
        res,
        userId,
        nbHexacoins,
        lastRemoteNbHexacoins
    )

    ResponseData.send200(res, {
        nbHexacoins : updatedNbHexacoins
    })
})

export async function updateNbHexacoins(res: Response, userId: string,
    sentNbHexacoins: number | undefined, lastRemoteNbHexacoins: number | undefined): Promise<number> {

    const serializerPlayer = new SerializerPlayer()

    //retrieve player
    const player = await DatabasePlayers.getPlayer(
        userId,
        serializerPlayer
    )

    //take the most valuable values
    const nbHexacoins = transformNbHexacoins(player, sentNbHexacoins, lastRemoteNbHexacoins)

    //update nb in db
    const ok = await DatabasePlayers.updateNbHexacoins(
        userId,
        nbHexacoins
    )

    if (!ok) {
        throw ResponseData.send500(res)
    }

    return nbHexacoins
}
