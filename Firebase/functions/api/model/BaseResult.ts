/**
 * Hexa Snap
 * © Aurélien Lubecki 2019
 * All Rights Reserved
 */

import { Utils } from "../common/Utils"


export abstract class BaseResult {


    readonly date: Date
    readonly score: number


    constructor(date: Date | undefined, score: number | undefined) {

        if (!Utils.isValueCorrect(date, Date)) {
            throw new Error("Null BaseResult.date")
        }

        if (!Utils.isValueCorrect(score, Number)) {
            throw new Error("Null BaseResult.score")
        }

        this.date = date as Date
        this.score = score as number

        if (this.score < 0) {
            throw new Error("Negative BaseResult.score")
        }
    }

}
