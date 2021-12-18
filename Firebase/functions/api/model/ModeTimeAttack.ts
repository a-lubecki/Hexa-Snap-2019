/**
 * Hexa Snap
 * © Aurélien Lubecki 2019
 * All Rights Reserved
 */

import { BaseMode } from "./BaseMode"
import { GameMode } from "./GameMode"
import { Graph } from "./Graph"
import { Utils } from "../common/Utils"


export class ModeTimeAttack extends BaseMode {


    readonly maxTimeSec: number


    constructor(maxScore: number | undefined, maxTimeSec: number | undefined, graph: Graph | undefined) {
        super(GameMode.TIME_ATTACK, maxScore, graph)

        if (!Utils.isValueCorrect(maxTimeSec, Number)) {
            throw new Error("Null ModeModeTimeAttack.maxTimeSec")
        }

        this.maxTimeSec = maxTimeSec as number

        if (this.maxTimeSec < 0) {
            throw new Error("BaseMode ModeModeTimeAttack.maxTimeSec")
        }
    }

}
