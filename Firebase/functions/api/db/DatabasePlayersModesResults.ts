/**
 * Hexa Snap
 * © Aurélien Lubecki 2019
 * All Rights Reserved
 */

import * as admin from 'firebase-admin'
import { GameMode } from '../model/GameMode'
import { ResultType } from '../model/ResultType'
import { BaseResult } from '../model/BaseResult'
import { BaseSerializerResult } from '../serializing/BaseSerializerResult'
import { isNullOrUndefined } from 'util'


const db = admin.firestore()

function collection(userId: string, gameMode: GameMode, resultType: ResultType): FirebaseFirestore.CollectionReference {

    return db.collection("players")
        .doc(userId)
        .collection("modes")
        .doc(gameMode)
        .collection(resultType)
}

function document(userId: string, gameMode: GameMode, resultType: ResultType, date: Date): FirebaseFirestore.DocumentReference {

    return collection(userId, gameMode, resultType)
        .doc(Math.round(date.getTime() / 1000).toString())
}


export class DatabasePlayersModesResults {

    static getResults(userId: string, gameMode: GameMode, resultType: ResultType, serializer: BaseSerializerResult): Promise<Array<BaseResult> | undefined> {

        return collection(userId, gameMode, resultType)
            .get()
            .then(snapshot => {
                return snapshot.docs
                    .map(doc => serializer.deserializeSnapshot(doc))
                    .filter(res => isNullOrUndefined(res)) as Array<BaseResult>
            })
            .catch(e => {
                console.warn(`DatabasePlayersModeResults.getBestScores(${userId}, ${gameMode}, ${resultType}) : ` + e)
                return undefined
            })

    }

    static updateResult(userId: string, gameMode: GameMode, resultType: ResultType,
        serializer: BaseSerializerResult, result: BaseResult): Promise<boolean> {

        const newResult = serializer.serialize(result)

        if (isNullOrUndefined(newResult)) {
            return Promise.resolve(false)
        }

        return document(userId, gameMode, resultType, result.date)
            .set(
                newResult,
                { merge: true }
            )
            .then(_ => true)
            .catch(e => {
                console.warn(`DatabasePlayersModeResults.updateResult(${userId}, ${gameMode}, ${resultType}, ${result.date}) : ` + e)
                return false
            })

    }

    static deleteResult(userId: string, gameMode: GameMode, resultType: ResultType, result: BaseResult): Promise<boolean> {

        return document(userId, gameMode, resultType, result.date)
            .delete()
            .then(_ => true)
            .catch(e => {
                console.warn(`DatabasePlayersModeResults.deleteResult(${userId}, ${gameMode}, ${resultType}, ${result.date}) : ` + e)
                return false
            })

    }

}
