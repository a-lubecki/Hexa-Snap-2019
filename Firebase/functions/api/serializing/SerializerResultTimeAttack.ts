/**
 * Hexa Snap
 * © Aurélien Lubecki 2019
 * All Rights Reserved
 */

import { Utils } from "../common/Utils"
import { BaseSerializerResult } from "./BaseSerializerResult"
import { ResultTimeAttack } from "../model/ResultTimeAttack"


export class SerializerResultTimeAttack extends BaseSerializerResult {


    public deserializeSnapshot(snapshot: FirebaseFirestore.DocumentSnapshot): ResultTimeAttack | undefined {

        const data = snapshot.data() as any

        if (!Utils.isValueCorrect(data, Object)) {
            return undefined
        }

        try {
            return new ResultTimeAttack(
                new Date(parseInt(snapshot.id, 10) * 1000),
                data["score"],
                data["timeSec"]
            )
        } catch (e) {
            console.error(e)
            return undefined
        }
    }

    public serialize(result: ResultTimeAttack): object {

        return {
            score: result.score,
            timeSec: result.timeSec
        }
    }

}
