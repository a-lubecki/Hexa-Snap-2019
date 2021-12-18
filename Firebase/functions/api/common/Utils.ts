/**
 * Hexa Snap
 * © Aurélien Lubecki 2019
 * All Rights Reserved
 */

import { isNullOrUndefined } from "util"


export class Utils {


    static isTypeCorrect(value: any, type: any): boolean {

        if (isNullOrUndefined(type)) {
            console.error("Utils.isValueCorrect : type is undefined")
            return false
        }

        const typePrototype = type.prototype
        if (isNullOrUndefined(typePrototype)) {
            //check if type as enum contains value
            const enumNames = Object.keys(type)
            return (enumNames.map(k => type[k]).indexOf(value) >= 0)
        }

        return Object.getPrototypeOf(value) === typePrototype
    }

    static isValueCorrect(value: any, type: any): boolean {

        if (isNullOrUndefined(value)) {
            return false
        }

        return Utils.isTypeCorrect(value, type)
    }

    static newObjectFromMap<T>(map: Map<string, T> | undefined): object | undefined {

        if (isNullOrUndefined(map)) {
            return {}
        }

        if (!Utils.isTypeCorrect(map, Map)) {
            console.error("Utils.newObjectFromMap bad map type")
            return undefined
        }

        const res: {[k: string]: any} = {}

        for (const e of map.entries()) {

            const key = e[0]
            const value = e[1]

            if (isNullOrUndefined(value)) {
                continue
            }

            res[key] = value
        }

        return res
    }

    static newMapFromObject<T>(obj: object | undefined): Map<string, T> | undefined {

        if (isNullOrUndefined(obj)) {
            return new Map<string, T>()
        }

        if (!Utils.isTypeCorrect(obj, Object)) {
            console.error("Utils.newMapFromObject bad obj type")
            return undefined
        }

        const objRef: {[k: string]: any} = obj

        const res = new Map<string, T>()

        for (const key of Object.keys(obj)) {

            const value = objRef[key] as T
            if (isNullOrUndefined(value)) {
                continue
            }

            res.set(key, value)
        }

        return res
    }

    static areEquals<T>(map1: Map<string, T> | undefined, map2: Map<string, T> | undefined): boolean {

        if (!Utils.isValueCorrect(map1, Map)) {
            return false
        }

        if (!Utils.isValueCorrect(map2, Map)) {
            return false
        }

        const map1Ref = map1 as Map<string, T>
        const map2Ref = map2 as Map<string, T>

        if (map1Ref.size !== map2Ref.size) {
            return false
        }

        for (const key of map1Ref.keys()) {

            if (map1Ref.get(key) !== map2Ref.get(key)) {
                return false
            }
        }

        return true
    }

    static sleep(delaySec: number): Promise<NodeJS.Timer> {

        let delay = delaySec
        if (!Utils.isValueCorrect(delaySec, Number) || delaySec <= 0) {
            delay = 0
        }

        return new Promise(r => setTimeout(r, delay * 1000))
    }

    static sleepUntil(nextDate: Date): Promise<NodeJS.Timer> {

        return this.sleep(nextDate.getTime() - Date.now())
    }

}
