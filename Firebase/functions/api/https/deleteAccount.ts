/**
 * Hexa Snap
 * © Aurélien Lubecki 2019
 * All Rights Reserved
 */

import * as functions from 'firebase-functions'
import { RequestParams } from "../common/RequestParams"
import { RequestCheck } from '../common/RequestCheck'
import { ResponseData } from '../common/ResponseData'
import { auth } from 'firebase-admin'


/*
 * Delete the user account
 */
export const deleteAccount = functions.https.onRequest(async (req, res) => {

    console.log(JSON.stringify(req.body.data))

    const params = await RequestCheck.checkFunctionOrFail(true, req, res, new Map<string, any>())
    const userId = params.get(RequestParams.paramUserId)

    try {
        await auth().deleteUser(userId)
    } catch {
        throw ResponseData.send500(res)
    }

    ResponseData.send200(res)
})
