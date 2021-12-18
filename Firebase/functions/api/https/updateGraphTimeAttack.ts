/**
 * Hexa Snap
 * © Aurélien Lubecki 2019
 * All Rights Reserved
 */

import * as functions from 'firebase-functions'
import { GameMode } from '../model/GameMode'
import { updateGraph } from './updateGraph'


/*
 * updateGraphTimeAttack(userId, zones, unlockedNodes, activeNodes): (zones, unlockedNodes, activeNodes)
     // body :
    {
        "zones" : 0,
        "unlockedNodes" : [],
        "activeNodes" : []
    }
 */
export const updateGraphTimeAttack = functions.https.onRequest(async (req, res) => {

    console.log(JSON.stringify(req.body.data))

    await updateGraph(
        req,
        res,
        GameMode.TIME_ATTACK
    )
})
