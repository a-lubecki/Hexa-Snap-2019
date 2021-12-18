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
import { ResultArcade } from '../model/ResultArcade'
import { GameMode } from '../model/GameMode'
import { ResultType } from '../model/ResultType'
import { SerializerResultArcade } from '../serializing/SerializerResultArcade'
import { addNewResult } from './addNewResult'
import { ModeArcade } from '../model/ModeArcade'
import { SerializerModeArcade } from '../serializing/SerializerModeArcade'


/*
 * updateResultArcade(userId, score, level) : (maxScore, maxLevel)
 */
export const updateResultArcade = functions.https.onRequest(async (req, res) => {

    console.log(JSON.stringify(req.body.data))

    const extractors = new Map<string, any>()
    extractors.set("score", RequestParams.extractParamPositiveNumber)
    extractors.set("level", RequestParams.extractParamPositiveNumber)

    const params = await RequestCheck.checkFunctionOrFail(true, req, res, extractors)
    const userId = params.get(RequestParams.paramUserId)
    const score = params.get("score")
    const level = params.get("level")

    if (level < 1) {
        //the level can't be 0
        throw ResponseData.send400(res)
    }

    let maxScore = 0
    let maxLevel = 0

    const serializer = new SerializerModeArcade()

    //retrieve the current score + level from db
    const mode = await DatabasePlayersModes.getMode(userId, GameMode.ARCADE, serializer) as ModeArcade
    if (!isNullOrUndefined(mode)) {
        maxScore = mode.maxScore
        maxLevel = mode.maxLevel
    }

    //check if the new score + level are greater than the stored ones
    let mustUpdateScore = false
    let mustUpdateLevel = false

    if (score > maxScore) {
        maxScore = score
        mustUpdateScore = true
    }

    if (level > maxLevel) {
        maxLevel = level
        mustUpdateLevel = true
    }

    //if no changes : end
    if (!mustUpdateScore && !mustUpdateLevel) {
        endUpdateResult(res, maxScore, maxLevel)
        return
    }

    let newMode: ModeArcade
    try {
        newMode = new ModeArcade(
            maxScore,
            maxLevel,
            undefined
        )
    } catch (e) {
        throw ResponseData.send500(res)
    }

    //if the score or level are greater : write in db
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
    let newResult: ResultArcade
    try {
        newResult = new ResultArcade(new Date(), score, level)
    } catch (e) {
        throw ResponseData.send500(res)
    }

    const serializerResult = new SerializerResultArcade()

    if (mustUpdateScore) {

        const ok = await addNewResult(
            userId,
            GameMode.ARCADE,
            ResultType.BEST_SCORES,
            serializerResult,
            "score",
            newResult
        )

        if (!ok) {
            throw ResponseData.send500(res)
        }
    }

    if (mustUpdateLevel) {

        const ok = await addNewResult(
            userId,
            GameMode.ARCADE,
            ResultType.BEST_LEVELS,
            serializerResult,
            "level",
            newResult
        )

        if (!ok) {
            throw ResponseData.send500(res)
        }
    }

    //update best scores + best levels
    endUpdateResult(res, maxScore, maxLevel)
})

function endUpdateResult(res: Response, maxScore: number, maxLevel: number) {

    ResponseData.send200(res, {
        maxScore : maxScore,
        maxLevel : maxLevel
    })
}
