/**
 * Hexa Snap
 * © Aurélien Lubecki 2019
 * All Rights Reserved
 */

import { GraphNodesMap } from "../model/GraphNodeMap"
import { isNullOrUndefined } from "util"
import { Utils } from "../common/Utils"


export class SerializerGraphNodesMap {


    public deserialize(data: any): GraphNodesMap | undefined {

        let nodes: Map<string, number> | undefined

        if (Utils.isValueCorrect(data, Array)) {

            nodes = new Map()
            for (const i in data as Array<any>) {
                nodes.set(i.toString(), data[i])
            }

        } else if (Utils.isValueCorrect(data, Object)) {

            nodes = Utils.newMapFromObject<number>(data)

        } else {

            return undefined
        }

        try {
            return new GraphNodesMap(nodes)

        } catch (e) {
            console.error(e)
            return undefined
        }
    }

    public serialize(nodesMap: GraphNodesMap): number[] {

        const newNodesArray = new Array<number>()
        const map = nodesMap.getNodes()

        //add nodes only if non-zero
        for (const e of map.entries()) {

            const key = e[0]
            const val = e[1]

            if (isNullOrUndefined(val)) {
                continue
            }

            newNodesArray[Number(key)] = val
        }

        //set all undefined values to 0
        for (const i in newNodesArray) {

            const node = newNodesArray[i]

            if (isNullOrUndefined(node) || node < 0) {
                newNodesArray[i] = 0
            }
        }

        return newNodesArray
    }

}
