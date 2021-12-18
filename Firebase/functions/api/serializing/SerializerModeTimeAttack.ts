/**
 * Hexa Snap
 * © Aurélien Lubecki 2019
 * All Rights Reserved
 */

import { isNullOrUndefined } from "util"
import { SerializerGraph } from "./SerializerGraph"
import { Utils } from "../common/Utils"
import { ModeTimeAttack } from "../model/ModeTimeAttack"
import { BaseSerializerMode } from "./BaseSerializerMode"


export class SerializerModeTimeAttack extends BaseSerializerMode {


    public deserializeSnapshot(snapshot: FirebaseFirestore.DocumentSnapshot): ModeTimeAttack | undefined {

        return this.deserialize(snapshot.data())
    }

    public deserialize(data: any): ModeTimeAttack | undefined {

        if (!Utils.isValueCorrect(data, Object)) {
            return undefined
        }

        let maxScore = data["maxScore"]
        if (!Utils.isValueCorrect(maxScore, Number)) {
            maxScore = 0
        }

        let maxTimeSec = data["maxTimeSec"]
        if (!Utils.isValueCorrect(maxTimeSec, Number)) {
            maxTimeSec = 0
        }

        try {
            return new ModeTimeAttack(
                maxScore,
                maxTimeSec,
                new SerializerGraph().deserialize(data["graph"])
            )
        } catch (e) {
            console.error(e)
            return undefined
        }
    }

    public serialize(mode: ModeTimeAttack): object {

        const res: {[k: string]: any} = {
            maxScore: mode.maxScore,
            maxTimeSec: mode.maxTimeSec
        }

        if (!isNullOrUndefined(mode.graph)) {
            res["graph"] = new SerializerGraph().serialize(mode.graph)
        }

        return res
    }

}
