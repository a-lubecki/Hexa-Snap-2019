/**
 * Hexa Snap
 * © Aurélien Lubecki 2019
 * All Rights Reserved
 */

import * as functions from 'firebase-functions'
import Request = functions.Request
import Response = functions.Response
import * as admin from 'firebase-admin'
import { isNullOrUndefined } from "util"
import { RequestParams } from './RequestParams'
import { ResponseData } from './ResponseData'
import { Utils } from './Utils'
import { UserRecord } from 'firebase-functions/lib/providers/auth'



export type ParamExtractor = ((req: Request, paramName: string) => any)

export class RequestCheck {


    static checkFunctionOrFail(mustCheckUserAuth: boolean, req: Request, res: Response,
        requiredParamExtractors: Map<string, ParamExtractor>): Promise<Map<string, any>> {

        //log("Checking function : " + functionName)

        const extractedParams = new Map<string, any>()

        //anti-piracy protection
        RequestCheck.checkSignatureOrFail(req, res, extractedParams)

        if (!mustCheckUserAuth) {
            //verify params directly
            RequestCheck.checkRequiredParamsOrFail(req, res, requiredParamExtractors, extractedParams)
            return Promise.resolve(extractedParams)
        }

        //verify the caller then params
        return RequestCheck.checkUserAuthOrFail(req, res, extractedParams)
            .then(_ => {

                //check params
                RequestCheck.checkRequiredParamsOrFail(req, res, requiredParamExtractors, extractedParams)
                return extractedParams
            })
    }

    /**
     * Check if the signature and the origin of the app (google play / app store) are black listed.
     * If so send an error and return true as the sending has been processed.
     */
    private static checkSignatureOrFail(req: Request, res: Response, extractedParams: Map<string, any>) {
        //code obfuscated for github
    }

    private static isRunningLocally(req: Request): boolean {
        return req.ip === "127.0.0.1"
    }

    private static checkUserAuthOrFail(req: Request, res: Response, extractedParamsToFill: Map<string, any>): Promise<void> {

        return RequestCheck.checkUserAuth(req, res, extractedParamsToFill)
            .then(done => {

                if (!done && extractedParamsToFill.get(RequestParams.paramOrigin) !== "editor") {
                    //if auth signature failed and not testing in the editor
                    throw ResponseData.send401(res)
                }
            })
    }

    private static async checkUserAuth(req: Request, res: Response, extractedParamsToFill: Map<string, any>): Promise<boolean> {

        const userId = RequestParams.extractUserId(req) as string
        if (!Utils.isValueCorrect(userId, String)) {
            return Promise.resolve(false)
        }

        //check if user still exists (can be deleted or deactivated)
        let user: UserRecord
        try {
            user = await admin.auth().getUser(userId)
        } catch {
            //not found or deleted user, force to sign out
            throw ResponseData.send401(res)
        }

        if (isNullOrUndefined(user) || user.disabled) {
            //disabled by the admin, force to sign out
            throw ResponseData.send401(res)
        }

        extractedParamsToFill.set(RequestParams.paramUserId, userId)

        if (RequestCheck.isRunningLocally(req)) {
            //bypass the header verification if running locally
            return Promise.resolve(true)
        }

        //check token in the headers
        const authStr = req.headers.authorization as string
        if (Utils.isValueCorrect(authStr, String) && authStr.startsWith("Bearer ")) {
            return RequestCheck.checkAuthToken(authStr.split("Bearer ")[1], userId)
        }

        //console.log("No auth token provided in header")

        //check token in the cookies
        if (req.cookies) {
            return RequestCheck.checkAuthToken(req.cookies.__session, userId)
        }

        //console.log("No auth token provided in cookies")

        return Promise.resolve(false)
    }

    private static checkAuthToken(authToken: string, userId: string): Promise<boolean> {

        if (!Utils.isValueCorrect(authToken, String)) {
            return Promise.resolve(false)
        }

        if (!Utils.isValueCorrect(userId, String)) {
            return Promise.resolve(false)
        }

        return admin.auth()
            .verifyIdToken(authToken)
            .then(decodedIdToken => {

                return userId === decodedIdToken.uid

            }).catch(e => {
                console.warn(`RequestCheck.checkAuthToken(${authToken}) : ` + e)
                return false
            })
    }

    static checkRequiredParamsOrFail(req: Request, res: Response, paramExtractors: Map<string, ParamExtractor>, extractedParamsToFill: Map<string, any>) {

        for (const e of paramExtractors.entries()) {

            const paramName = e[0]
            const extractor = e[1]

            if (!Utils.isValueCorrect(paramName, String)) {
                //ignore
                continue
            }

            const extractedValue = extractor(req, paramName)
            if (isNullOrUndefined(extractedValue)) {
                console.warn("Missing or invalid param for " + extractor.name + " => " + extractedValue)
                throw ResponseData.send400(res)
            }

            extractedParamsToFill.set(paramName, RequestParams.convertParamValue(extractedValue))
        }
    }

}
