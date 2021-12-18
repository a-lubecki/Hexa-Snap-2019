/**
 * Hexa Snap
 * © Aurélien Lubecki 2019
 * All Rights Reserved
 */

import * as functions from 'firebase-functions'
import Response = functions.Response
import { isNullOrUndefined } from 'util'
import { RequestParams } from '../common/RequestParams'
import { ResponseData } from '../common/ResponseData'
import { RequestCheck } from '../common/RequestCheck'
import { DatabasePlayersModes } from '../db/DatabasePlayersModes'
import { GameMode } from '../model/GameMode'
import { ResultType } from '../model/ResultType'
import { ResultTimeAttack } from '../model/ResultTimeAttack'
import { SerializerResultTimeAttack } from '../serializing/SerializerResultTimeAttack'
import { addNewResult } from './addNewResult'
import { SerializerModeTimeAttack } from '../serializing/SerializerModeTimeAttack'
import { ModeTimeAttack } from '../model/ModeTimeAttack'


/*
 * updateResultTimeAttack(userId, score, timeSec) : (maxScore, timeSec)
 */
export const updateResultTimeAttack = functions.https.onRequest(async (req, res) => {

    console.log(JSON.stringify(req.body.data))

    const extractors = new Map<string, any>()
    extractors.set("score", RequestParams.extractParamPositiveNumber)
    extractors.set("timeSec", RequestParams.extractParamPositiveNumber)

    const params = await RequestCheck.checkFunctionOrFail(true, req, res, extractors)
    const userId = params.get(RequestParams.paramUserId)
    const score = params.get("score")
    const timeSec = params.get("timeSec")

    let maxScore = 0
    let maxTimeSec = 0

    const serializer = new SerializerModeTimeAttack()

    //retrieve the current score + timeSec from db
    const mode = await DatabasePlayersModes.getMode(userId, GameMode.TIME_ATTACK, serializer) as ModeTimeAttack
    if (!isNullOrUndefined(mode)) {
        maxScore = mode.maxScore
        maxTimeSec = mode.maxTimeSec
    }

    //check if the new score + timeSec are greater than the stored ones
    let mustUpdateScore = false
    let mustUpdateTimeSec = false

    if (score > maxScore) {
        maxScore = score
        mustUpdateScore = true
    }

    if (timeSec > maxTimeSec) {
        maxTimeSec = timeSec
        mustUpdateTimeSec = true
    }

    //if no changes : end
    if (!mustUpdateScore && !mustUpdateTimeSec) {
        endUpdateResult(res, maxScore, maxTimeSec)
        return
    }

    let newMode: ModeTimeAttack
    try {
        newMode = new ModeTimeAttack(
            maxScore,
            maxTimeSec,
            undefined
        )
    } catch (e) {
        throw ResponseData.send500(res)
    }

    //if the score or timeSec are greater : write in db
    if (isNullOrUndefined(mode)) {

        //if nothing in db, create the mode
        const ok = await DatabasePlayersModes.updateMode(
            userId,
            serializer,
            newMode
        )

        if (!ok) {
            throw ResponseData.send500(res)
        }

    } else {

        //else update the mode
        const ok = await DatabasePlayersModes.updateMode(
            userId,
            serializer,
            newMode
        )

        if (!ok) {
            throw ResponseData.send500(res)
        }
    }

    //add the new result
    let newResult: ResultTimeAttack
    try {
        newResult = new ResultTimeAttack(new Date(), score, timeSec)
    } catch (e) {
        throw ResponseData.send500(res)
    }

    const serializerResult = new SerializerResultTimeAttack()

    if (mustUpdateScore) {

        const ok = await addNewResult(
            userId,
            GameMode.TIME_ATTACK,
            ResultType.BEST_SCORES,
            serializerResult,
            "score",
            newResult
        )

        if (!ok) {
            throw ResponseData.send500(res)
        }
    }

    if (mustUpdateTimeSec) {

        const ok = await addNewResult(
            userId,
            GameMode.TIME_ATTACK,
            ResultType.BEST_TIME_SEC,
            serializerResult,
            "timeSec",
            newResult
        )

        if (!ok) {
            throw ResponseData.send500(res)
        }
    }

    //update best scores + best times
    endUpdateResult(res, maxScore, maxTimeSec)
})

function endUpdateResult(res: Response, maxScore: number, maxTimeSec: number) {

    ResponseData.send200(res, {
        maxScore : maxScore,
        maxTimeSec : maxTimeSec
    })
}
