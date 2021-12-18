/**
 * Hexa Snap
 * © Aurélien Lubecki 2019
 * All Rights Reserved
 */

import { BaseResult } from "./BaseResult"
import { Utils } from "../common/Utils"


export class ResultTimeAttack extends BaseResult {


    readonly timeSec: number


    constructor(date: Date | undefined, score: number | undefined, timeSec: number | undefined) {
        super(date, score)

        if (!Utils.isValueCorrect(timeSec, Number)) {
            throw new Error("Null ResultTimeAttack.timeSec")
        }

        this.timeSec = timeSec as number

        if (this.timeSec < 0) {
            throw new Error("Negative ResultTimeAttack.timeSec")
        }
    }

}
