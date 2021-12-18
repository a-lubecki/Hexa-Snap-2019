/**
 * Hexa Snap
 * © Aurélien Lubecki 2019
 * All Rights Reserved
 */

import { BaseMode } from "./BaseMode"
import { GameMode } from "./GameMode"
import { Graph } from "./Graph"
import { Utils } from "../common/Utils"


export class ModeArcade extends BaseMode {


    readonly maxLevel: number


    constructor(maxScore: number | undefined, maxLevel: number | undefined, graph: Graph | undefined) {
        super(GameMode.ARCADE, maxScore, graph)

        if (!Utils.isValueCorrect(maxLevel, Number)) {
            throw new Error("Null ModeArcade.maxLevel")
        }

        this.maxLevel = maxLevel as number

        if (this.maxLevel < 1) {
            throw new Error("Negative or zero ModeArcade.maxLevel")
        }
    }

}
