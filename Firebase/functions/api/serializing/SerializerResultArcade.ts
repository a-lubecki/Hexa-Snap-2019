/**
 * Hexa Snap
 * © Aurélien Lubecki 2019
 * All Rights Reserved
 */

import { ResultArcade } from "../model/ResultArcade"
import { Utils } from "../common/Utils"
import { BaseSerializerResult } from "./BaseSerializerResult"


export class SerializerResultArcade extends BaseSerializerResult {


    public deserializeSnapshot(snapshot: FirebaseFirestore.DocumentSnapshot): ResultArcade | undefined {

        const data = snapshot.data() as any

        if (!Utils.isValueCorrect(data, Object)) {
            return undefined
        }

        try {
            return new ResultArcade(
                new Date(parseInt(snapshot.id, 10) * 1000),
                data["score"],
                data["level"]
            )
        } catch (e) {
            console.error(e)
            return undefined
        }
    }

    public serialize(result: ResultArcade): object {

        return {
            score: result.score,
            level: result.level
        }
    }

}
