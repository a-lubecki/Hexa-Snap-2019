/**
 * Hexa Snap
 * © Aurélien Lubecki 2019
 * All Rights Reserved
 */

import { GameMode } from "../model/GameMode"
import { ResultType } from "../model/ResultType"
import { BaseResult } from "../model/BaseResult"
import { BaseSerializerResult } from "../serializing/BaseSerializerResult"
import { isNullOrUndefined } from "util"
import { DatabasePlayersModesResults } from "../db/DatabasePlayersModesResults"


export async function addNewResult(userId: string, gameMode: GameMode,
    resultType: ResultType, serializerResult: BaseSerializerResult,
    minBasedFieldName: string, newResult: BaseResult): Promise<boolean> {

    //get the best results by score, remove the worst, add the new one
    const bestResults = await DatabasePlayersModesResults.getResults(
        userId,
        gameMode,
        resultType,
        serializerResult
    )

    //if some results, keep only 5
    if (!isNullOrUndefined(bestResults) && bestResults.length >= 5) {

        //find the worst result to remove, can be ignored if db delete fails
        const resultToRemove = findWorstResult(bestResults, minBasedFieldName)
        if (!isNullOrUndefined(resultToRemove)) {

            const ok = await DatabasePlayersModesResults.deleteResult(
                userId,
                gameMode,
                resultType,
                resultToRemove
            )

            if (!ok) {
                //ignore but log anyway
                console.warn("Delete failed for results")
            }
        }
    }

    return DatabasePlayersModesResults.updateResult(
        userId,
        gameMode,
        resultType,
        serializerResult,
        newResult
    )
}

function findWorstResult(bestResults: BaseResult[], minBasedFieldName: string): BaseResult | undefined {

    let min = Number.MAX_VALUE
    let worstResult: BaseResult | undefined

    for (const r of bestResults) {

        const currentValue = (r as any)[minBasedFieldName]

        if (currentValue < min) {
            //worse value than current value
            min = currentValue
            worstResult = r
        }
    }

    return worstResult
}
