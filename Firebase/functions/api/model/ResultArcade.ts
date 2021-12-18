/**
 * Hexa Snap
 * © Aurélien Lubecki 2019
 * All Rights Reserved
 */

import { BaseResult } from "./BaseResult"
import { Utils } from "../common/Utils"


export class ResultArcade extends BaseResult {


    readonly level: number


    constructor(date: Date | undefined, score: number | undefined, level: number | undefined) {
        super(date, score)

        if (!Utils.isValueCorrect(level, Number)) {
            throw new Error("Null ResultArcade.level")
        }

        this.level = level as number

        if (this.level < 0) {
            throw new Error("Negative ResultArcade.level")
        }
    }

}
