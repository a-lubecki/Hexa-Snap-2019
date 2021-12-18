/**
 * Hexa Snap
 * © Aurélien Lubecki 2019
 * All Rights Reserved
 */

import * as admin from 'firebase-admin'
import { BaseMode } from '../model/BaseMode'
import { GameMode } from '../model/GameMode'
import { BaseSerializerMode } from '../serializing/BaseSerializerMode'
import { SerializerGraph } from '../serializing/SerializerGraph'
import { Graph } from '../model/Graph'
import { isNullOrUndefined } from 'util'


const db = admin.firestore()

function document(userId: string, gameMode: GameMode): FirebaseFirestore.DocumentReference {

    return db.collection("players")
        .doc(userId)
        .collection("modes")
        .doc(gameMode)
}


export class DatabasePlayersModes {


    static getMode(userId: string, gameMode: GameMode, serializer: BaseSerializerMode): Promise<BaseMode | undefined> {

        return document(userId, gameMode)
            .get()
            .then(doc => {

                if (!doc.exists) {
                    return undefined
                }

                return serializer.deserializeSnapshot(doc)
            })
            .catch(e => {
                console.warn(`DatabasePlayersModes.getMode(${userId}, ${gameMode}) : ` + e)
                return undefined
            })

    }

    static updateMode(userId: string, serializer: BaseSerializerMode, mode: BaseMode): Promise<boolean> {

        const newMode = serializer.serialize(mode)

        if (isNullOrUndefined(newMode)) {
            return Promise.resolve(false)
        }

        return document(userId, mode.gameMode)
            .set(
                newMode,
                { merge: true }
            )
            .then(_ => true)
            .catch(e => {
                console.warn(`DatabasePlayersModes.updateMode(${userId}, ${mode.gameMode}, ${mode.maxScore}) : ` + e)
                return false
            })

    }

    static updateGraph(userId: string, gameMode: GameMode, serializer: SerializerGraph, graph: Graph): Promise<boolean> {

        const newGraph = serializer.serialize(graph)

        if (isNullOrUndefined(newGraph)) {
            return Promise.resolve(false)
        }

        return document(userId, gameMode)
            .set(
                {
                    graph : newGraph
                },
                { merge: true }
            )
            .then(_ => true)
            .catch(e => {
                console.warn(`DatabasePlayersModes.updateGraph(${userId}, ${gameMode}) : ` + e)
                return false
            })

    }

}
