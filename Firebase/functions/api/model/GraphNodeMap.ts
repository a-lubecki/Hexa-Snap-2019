/**
 * Hexa Snap
 * © Aurélien Lubecki 2019
 * All Rights Reserved
 */

import { isNullOrUndefined } from "util"
import { Utils } from "../common/Utils"


export class GraphNodesMap {


    private readonly nodes: Map<string, number>


    constructor(nodes: Map<string, number> | undefined) {

        if (isNullOrUndefined(nodes)) {

            this.nodes = new Map()

        } else {

            if (!Utils.isTypeCorrect(nodes, Map)) {
                throw new Error("Bad type GraphNodesMap.constructor")
            }

            //defensive copy
            this.nodes = new Map()

            for (const e of nodes.entries()) {

                const key = e[0]
                const value = e[1]

                if (!Utils.isValueCorrect(value, Number)) {
                    throw new Error("Bad node type in GraphNodesMap.constructor : " + key + " => " + value)
                }

                if (value < 0) {
                    throw new Error("Negative node in GraphNodesMap.constructor : " + key + " => " + value)
                }

                this.nodes.set(key, value)
            }
        }
    }

    public getNodes(): Map<string, number> {
        //defensive copy
        return new Map(this.nodes)
    }

    public getNodeNames(): string[] {
        return Array.from(this.nodes.keys())
    }

    public getNode(name: string): number {

        const res = this.nodes.get(name)
        if (isNullOrUndefined(res)) {
            return 0
        }

        return res
    }

}
