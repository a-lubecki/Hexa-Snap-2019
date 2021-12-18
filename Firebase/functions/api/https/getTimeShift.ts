/**
 * Hexa Snap
 * © Aurélien Lubecki 2019
 * All Rights Reserved
 */

import * as functions from 'firebase-functions'
import { RequestParams } from "../common/RequestParams"
import { RequestCheck } from '../common/RequestCheck'
import { ResponseData } from '../common/ResponseData'


/*
 * Returns the difference between the sent datetime (device) and the server datetime
 * getTimeShift(dateTimeSec): timeShiftSec
 */
export const getTimeShift = functions.https.onRequest(async (req, res) => {

    console.log(JSON.stringify(req.body.data))

    const extractors = new Map<string, any>()
    extractors.set("dateTimeSec", RequestParams.extractParamNumber)

    const params = await RequestCheck.checkFunctionOrFail(false, req, res, extractors)
    const dateTimeSec = params.get("dateTimeSec")

    const nowSec = Math.round(new Date().getTime() / 1000)

    ResponseData.send200(res, {
        timeShiftSec : dateTimeSec - nowSec
    })
})
