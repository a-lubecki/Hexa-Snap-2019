/**
 * Hexa Snap
 * © Aurélien Lubecki 2019
 * All Rights Reserved
 */

import { GraphNodesMap } from "./GraphNodeMap"
import { Utils } from "../common/Utils"


export class Graph {


    readonly zones: number
    readonly unlockedNodes: GraphNodesMap
    readonly activeNodes: GraphNodesMap


    constructor(zones: number | undefined, unlockedNodes: GraphNodesMap | undefined, activeNodes: GraphNodesMap | undefined) {

        if (!Utils.isValueCorrect(zones, Number)) {
            throw new Error("Null Graph.zones")
        }

        if (!Utils.isValueCorrect(unlockedNodes, GraphNodesMap)) {
            throw new Error("Null Graph.unlockedNodes")
        }

        if (!Utils.isValueCorrect(activeNodes, GraphNodesMap)) {
            throw new Error("Null Graph.activeNodes")
        }

        this.zones = zones as number
        this.unlockedNodes = unlockedNodes as GraphNodesMap
        this.activeNodes = activeNodes as GraphNodesMap

        if (this.zones < 0) {
            throw new Error("Negative Graph.zones")
        }
    }

}
