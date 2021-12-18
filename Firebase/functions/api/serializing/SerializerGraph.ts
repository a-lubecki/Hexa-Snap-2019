/**
 * Hexa Snap
 * © Aurélien Lubecki 2019
 * All Rights Reserved
 */

import { Graph } from "../model/Graph"
import { SerializerGraphNodesMap } from "./SerializerGraphNodesMap"
import { Utils } from "../common/Utils"
import { isNullOrUndefined } from "util"


export class SerializerGraph {


    public deserialize(data: any): Graph | undefined {

        if (!Utils.isValueCorrect(data, Object)) {
            return undefined
        }

        let zones = data["zones"]
        if (!Utils.isValueCorrect(zones, Number)) {
            zones = 0
        }

        const serializerNodesMap = new SerializerGraphNodesMap()

        try {
            return new Graph(
                zones,
                serializerNodesMap.deserialize(data["unlockedNodes"]),
                serializerNodesMap.deserialize(data["activeNodes"])
            )
        } catch (e) {
            console.error(e)
            return undefined
        }
    }

    public serialize(graph: Graph): object {

        const res: {[k: string]: any} = {}

        //add zone if value is non-zero
        if (graph.zones > 0) {
            res["zones"] = graph.zones
        }

        const serializerNodesMap = new SerializerGraphNodesMap()

        //add nodes elems if some nodes
        const newUnlockedNodes = serializerNodesMap.serialize(graph.unlockedNodes)

        if (!isNullOrUndefined(newUnlockedNodes) && newUnlockedNodes.length > 0) {
            res["unlockedNodes"] = newUnlockedNodes
        }

        //add nodes elems if some nodes
        const newActiveNodes = serializerNodesMap.serialize(graph.activeNodes)

        if (!isNullOrUndefined(newActiveNodes) && newActiveNodes.length > 0) {
            res["activeNodes"] = newActiveNodes
        }

        return res
    }

}
