/**
 * Hexa Snap
 * © Aurélien Lubecki 2019
 * All Rights Reserved
 */

import * as admin from 'firebase-admin'
import { Purchase } from '../model/Purchase'
import { SerializerPurchase } from '../serializing/SerializerPurchase'
import { isNullOrUndefined } from 'util'


const db = admin.firestore()

function document(purchaseId: string): FirebaseFirestore.DocumentReference {

    return db.collection("purchases")
        .doc(purchaseId)
}


export class DatabasePurchases {


    static updatePurchase(serializer: SerializerPurchase, purchase: Purchase): Promise<boolean> {

        const newPurchaseReference = serializer.serialize(purchase)

        if (isNullOrUndefined(newPurchaseReference)) {
            return Promise.resolve(false)
        }

        //create and fail if a document with the same id exist
        return document(purchase.purchaseId).set(
            newPurchaseReference,
            { merge: true }
        )
        .then(_ => true)
        .catch(e => {
            console.warn(`DatabasePurchases.updatePurchase(${purchase.purchaseId}) : ` + e)
            return false
        })
    }

    static async updatePlayer(purchaseId: string, userId: string): Promise<boolean> {

        return document(purchaseId).set(
            {
                userId: userId
            },
            { merge: true }
        )
        .then(_ => true)
        .catch(e => {
            console.warn(`DatabasePurchases.updatePlayer(${purchaseId}, ${userId}) : ` + e)
            return false
        })
    }

}
