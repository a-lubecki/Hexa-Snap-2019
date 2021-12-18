/**
 * Hexa Snap
 * © Aurélien Lubecki 2019
 * All Rights Reserved
 */

import * as functions from 'firebase-functions'
import Response = functions.Response
import { isNullOrUndefined } from 'util'


export class ResponseData {

    private static sendData(res: Response, data: any) {

        const json = isNullOrUndefined(data) ? "{}" : JSON.stringify(data)

        res.status(200).send(`{"data":${json}}`)

        console.log("RESPONSE => " + json)
    }

    private static sendError(res: Response, errorCode: number) {

        const json = `{"errorCode":${errorCode}}`

        res.status(200).send(`{"data":${json}}`)

        console.log("ERROR => " + json)
    }

    static send200(res: Response, data?: any) {

        ResponseData.sendData(res, data)
    }

    static send400(res: Response, message?: string): functions.https.HttpsError {

        ResponseData.sendError(res, 400)
        return new functions.https.HttpsError('invalid-argument', message ? message : '')
    }

    static send401(res: Response): functions.https.HttpsError {

        ResponseData.sendError(res, 401)
        return new functions.https.HttpsError('unauthenticated', 'Unauthorized')
    }

    static send403(res: Response): functions.https.HttpsError {

        ResponseData.sendError(res, 403)
        return new functions.https.HttpsError('permission-denied', 'Forbidden')
    }

    static send404(res: Response): functions.https.HttpsError {

        ResponseData.sendError(res, 404)
        return new functions.https.HttpsError('not-found', 'Not Found')
    }

    static send409(res: Response, message?: string): functions.https.HttpsError {

        ResponseData.sendError(res, 409)
        return new functions.https.HttpsError('already-exists', message ? message : '')
    }

    static send451(res: Response): functions.https.HttpsError {

        ResponseData.sendError(res, 451)
        return new functions.https.HttpsError('resource-exhausted', 'Unavailable For Legal Reasons')
    }

    static send500(res: Response, message?: string): functions.https.HttpsError {

        ResponseData.sendError(res, 500)
        return new functions.https.HttpsError('internal', message ? message : '')
    }

}
