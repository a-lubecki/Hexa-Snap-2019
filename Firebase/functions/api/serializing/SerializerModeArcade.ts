/**
 * Hexa Snap
 * © Aurélien Lubecki 2019
 * All Rights Reserved
 */

import { isNullOrUndefined } from "util"
import { ModeArcade } from "../model/ModeArcade"
import { SerializerGraph } from "./SerializerGraph"
import { Utils } from "../common/Utils"
import { BaseSerializerMode } from "./BaseSerializerMode"


export class SerializerModeArcade extends BaseSerializerMode {


    public deserializeSnapshot(snapshot: FirebaseFirestore.DocumentSnapshot): ModeArcade | undefined {

        return this.deserialize(snapshot.data())
    }

    public deserialize(data: any): ModeArcade | undefined {

        if (!Utils.isValueCorrect(data, Object)) {
            return undefined
        }

        let maxScore = data["maxScore"]
        if (!Utils.isValueCorrect(maxScore, Number)) {
            maxScore = 0
        }

        let maxLevel = data["maxLevel"]
        if (!Utils.isValueCorrect(maxLevel, Number)) {
            maxLevel = 0
        }

        try {
            return new ModeArcade(
                maxScore,
                maxLevel,
                new SerializerGraph().deserialize(data["graph"])
            )
        } catch (e) {
            console.error(e)
            return undefined
        }
    }

    public serialize(mode: ModeArcade): object {

        const res: {[k: string]: any} = {
            maxScore: mode.maxScore,
            maxLevel: mode.maxLevel
        }

        if (!isNullOrUndefined(mode.graph)) {
            res["graph"] = new SerializerGraph().serialize(mode.graph)
        }

        return res
    }

}
