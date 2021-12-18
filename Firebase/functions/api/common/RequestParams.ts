/**
 * Hexa Snap
 * © Aurélien Lubecki 2019
 * All Rights Reserved
 */

import * as functions from 'firebase-functions'
import Request = functions.Request
import { isNullOrUndefined } from 'util'
import { Utils } from './Utils'


export class RequestParams {

    static convertParamValue(value: any): any {

        if (isNullOrUndefined(value)) {
            return undefined
        }

        if (Array.isArray(value)) {

            const newArr: any[] = []

            for (const val of value) {

                //parse values recursively
                newArr.push(RequestParams.convertParamValue(val))
            }

            return newArr
        }

        if (typeof value === "number" && isNaN(value)) {
            return undefined
        }

        //parse int64 number from unity
        if (typeof value === "object" &&
            value["@type"] === "type.googleapis.com/google.protobuf.Int64Value") {

            //convert from string to number
            const convertedValue = parseInt(value["value"], 10)
            if (isNaN(convertedValue)) {
                return 0
            }

            return convertedValue
        }

        if (typeof value === "object") {

            const newObj: {[k: string]: any} = {}

            const keys = Object.keys(value)
            for (const k of keys) {

                //parse values recursively
                newObj[k] = RequestParams.convertParamValue(value[k])
            }

            return newObj
        }

        return value
    }

    static extractParam(req: Request, name: string): any {

        if (!Utils.isValueCorrect(name, String)) {
            return undefined
        }

        const body = req.body
        if (isNullOrUndefined(body)) {
            return undefined
        }

        const data = body.data
        if (isNullOrUndefined(data)) {
            return undefined
        }

        return RequestParams.convertParamValue(data[name])
    }

    static extractParamString(req: Request, name: string): string | undefined {

        const res = RequestParams.extractParam(req, name)

        if (isNullOrUndefined(res)) {
            return undefined
        }

        if (typeof res !== "string") {
            return undefined
        }

        if (res.length <= 0) {
            return undefined
        }

        return res
    }

    static extractParamBoolean(req: Request, name: string): boolean | undefined {

        const res = RequestParams.extractParam(req, name)

        if (isNullOrUndefined(res)) {
            return undefined
        }

        if (typeof res !== "boolean") {
            return undefined
        }

        return res
    }

    static extractParamNumber(req: Request, name: string): number | undefined {

        const res = RequestParams.extractParam(req, name)

        if (isNullOrUndefined(res)) {
            return undefined
        }

        if (typeof res !== "number") {
            return undefined
        }

        return res
    }

    static extractParamPositiveNumber(req: Request, name: string): number | undefined {

        const res = RequestParams.extractParamNumber(req, name)

        if (isNullOrUndefined(res)) {
            return undefined
        }

        if (typeof res !== "number") {
            return undefined
        }

        if (res < 0) {
            return undefined
        }

        return res
    }

    static extractParamNumberArray(req: Request, name: string): number[] | undefined {

        const arr = RequestParams.extractParam(req, name)

        if (isNullOrUndefined(arr)) {
            return undefined
        }

        if (!(arr instanceof Array)) {
            return undefined
        }

        const res = new Array<number>()

        for (const i in arr) {

            const val = arr[i]

            if (typeof val !== "number") {
                //val in array is not a number
                return undefined
            }

            res[i] = val
        }

        return res
    }

    static extractParamObject(req: Request, name: string): object | undefined {

        const res = RequestParams.extractParam(req, name)

        if (isNullOrUndefined(res)) {
            return undefined
        }

        if (typeof res !== "object") {
            return undefined
        }

        return res as object
    }


    //required params :

    static paramPlatform = "platform"
    static paramOrigin = "origin"
    static paramSignatures = "signatures"
    static paramUserId = "userId"

    static extractAppPlatform(req: Request): string | undefined {
        return RequestParams.extractParamString(req, RequestParams.paramPlatform)
    }

    static extractAppOrigin(req: Request): string | undefined {
        return RequestParams.extractParamString(req, RequestParams.paramOrigin)
    }

    static extractAppSignatures(req: Request): string | undefined {
        return RequestParams.extractParamString(req, RequestParams.paramSignatures)
    }

    static extractUserId(req: Request): string | undefined {
        return RequestParams.extractParamString(req, RequestParams.paramUserId)
    }

}
