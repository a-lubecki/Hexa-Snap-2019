/**
 * Hexa Snap
 * © Aurélien Lubecki 2019
 * All Rights Reserved
 */

import { BaseResult } from "../model/BaseResult"


export abstract class BaseSerializerResult {


    abstract deserializeSnapshot(snapshot: FirebaseFirestore.DocumentSnapshot | undefined): BaseResult | undefined

    abstract serialize(result: BaseResult): object

}
