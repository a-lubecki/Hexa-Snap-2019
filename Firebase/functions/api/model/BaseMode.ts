/**
 * Hexa Snap
 * © Aurélien Lubecki 2019
 * All Rights Reserved
 */

import { GameMode } from "./GameMode"
import { Graph } from "./Graph"
import { Utils } from "../common/Utils"
import { isNullOrUndefined } from "util"


export abstract class BaseMode {


    readonly gameMode: GameMode
    readonly maxScore: number
    readonly graph: Graph | undefined


    constructor(gameMode: GameMode | undefined, maxScore: number | undefined, graph: Graph | undefined) {

        if (!Utils.isValueCorrect(gameMode, GameMode)) {
            throw new Error("Null BaseMode.gameMode")
        }

        if (!Utils.isValueCorrect(maxScore, Number)) {
            throw new Error("Null BaseMode.maxScore")
        }

        if (!isNullOrUndefined(graph) && !Utils.isTypeCorrect(graph, Graph)) {
            throw new Error("Bad type BaseMode.graph")
        }

        this.gameMode = gameMode as GameMode
        this.maxScore = maxScore as number
        this.graph = graph

        if (this.maxScore < 0) {
            throw new Error("Negatve BaseMode.maxScore")
        }
    }

}
