/**
 * Hexa Snap
 * © Aurélien Lubecki 2019
 * All Rights Reserved
 */

import * as functions from 'firebase-functions'
import { RequestCheck } from '../common/RequestCheck'
import { ResponseData } from '../common/ResponseData'
import { RequestParams } from '../common/RequestParams'
import { Purchase } from '../model/Purchase'
import { SerializerPurchase } from '../serializing/SerializerPurchase'
import { updatePlayer } from './updatePlayer'
import { SerializerPlayer } from '../serializing/SerializerPlayer'
import { DatabasePurchases } from '../db/DatabasePurchases'
import { isNullOrUndefined } from 'util'
import { Player } from '../model/Player'

/*
 * addInAppPurchase(transactionId, purchaseDate, purchaseTag, purchasePrice, purchaseNbHexacoins, purchaseIsRemovingAds, receipt, nbHexacoins, hasRemovedAds) => (nbHexacoins, hasRemovedAds)
 */

export const addInAppPurchase = functions.https.onRequest(async (req, res) => {

    console.log(JSON.stringify(req.body.data))

    const isLogged = !isNullOrUndefined(RequestParams.extractUserId(req))

    const extractors = new Map<string, any>()
    extractors.set("purchaserId", RequestParams.extractParamString)
    extractors.set("transactionId", RequestParams.extractParamString)
    extractors.set("purchaseDate", RequestParams.extractParamNumber)
    extractors.set("purchaseTag", RequestParams.extractParamString)
    extractors.set("purchasePrice", RequestParams.extractParamString)
    extractors.set("purchaseNbHexacoins", RequestParams.extractParamPositiveNumber)
    extractors.set("purchaseIsRemovingAds", RequestParams.extractParamBoolean)
    extractors.set("receipt", RequestParams.extractParamString)
    extractors.set("nbHexacoins", RequestParams.extractParamPositiveNumber)
    extractors.set("hasRemovedAds", RequestParams.extractParamBoolean)

    const params = await RequestCheck.checkFunctionOrFail(isLogged, req, res, extractors)
    const userId = params.get(RequestParams.paramUserId)
    const platform = params.get(RequestParams.paramPlatform)
    const purchaserId = params.get("purchaserId")
    const transactionId = params.get("transactionId")
    const purchaseDate = params.get("purchaseDate")
    const purchaseTag = params.get("purchaseTag")
    const purchasePrice = params.get("purchasePrice")
    const purchaseNbHexacoins = params.get("purchaseNbHexacoins")
    const purchaseIsRemovingAds = params.get("purchaseIsRemovingAds")
    const receipt = params.get("receipt")
    const nbHexacoins = params.get("nbHexacoins")
    const hasRemovedAds = params.get("hasRemovedAds")


    let newPurchase: Purchase | undefined

    try {
        //instanciate a new purchase to insert in db and mark it as created by the server with the current datetime
        newPurchase = new Purchase(
            userId,
            platform,
            purchaserId,
            transactionId,
            purchaseDate,
            purchaseTag,
            purchasePrice,
            purchaseNbHexacoins,
            purchaseIsRemovingAds,
            receipt,
            nbHexacoins,
            hasRemovedAds
        )

    } catch (e) {
        console.error(e)
        throw ResponseData.send400(res, "Can't create Purchase")
    }

    //add new purchase in db
    const ok = await DatabasePurchases.updatePurchase(
        new SerializerPurchase(userId, platform),
        newPurchase
    )

    if (!ok) {
        throw ResponseData.send500(res)
    }

    if (!isLogged) {

        //done for this anonymous user
        ResponseData.send200(res, new SerializerPlayer().serialize(
            new Player(
                false,
                newPurchase.hasRemovedAds || newPurchase.purchaseIsRemovingAds,
                newPurchase.nbHexacoins + newPurchase.purchaseNbHexacoins,
                undefined
            ),
            false
        ))
        return
    }

    //complete db for logged user

    const additionalPurchasesSyncState = new Map<string, boolean>()
    additionalPurchasesSyncState.set(newPurchase.purchaseId, true) //true because the purchase collection contains the userId (marked as synced)

    //add purchase for player purchases
    const newPlayer = await updatePlayer(
        res,
        userId,
        purchaseIsRemovingAds,
        nbHexacoins + purchaseNbHexacoins,
        undefined,//we don't take the last nb into account as the player has paid
        purchaseNbHexacoins,
        additionalPurchasesSyncState
    )

    ResponseData.send200(res, new SerializerPlayer().serialize(newPlayer, false))
})
