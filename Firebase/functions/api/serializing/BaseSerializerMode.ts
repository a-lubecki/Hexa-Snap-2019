/**
 * Hexa Snap
 * © Aurélien Lubecki 2019
 * All Rights Reserved
 */

import { BaseMode } from "../model/BaseMode"


export abstract class BaseSerializerMode {


    abstract deserializeSnapshot(snapshot: FirebaseFirestore.DocumentSnapshot): BaseMode | undefined

    abstract deserialize(data: object): BaseMode | undefined

    abstract serialize(result: BaseMode): object

}
