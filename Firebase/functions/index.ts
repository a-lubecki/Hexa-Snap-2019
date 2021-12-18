/**
 * Hexa Snap
 * © Aurélien Lubecki 2019
 * All Rights Reserved
 */

import * as admin from 'firebase-admin'

admin.initializeApp()


//export all functions
export * from './api/https/getTimeShift'
export * from './api/https/updateResultArcade'
export * from './api/https/updateResultTimeAttack'
export * from './api/https/updateWallet'
export * from './api/https/updateGraphArcade'
export * from './api/https/updateGraphTimeAttack'
export * from './api/https/addInAppPurchase'
export * from './api/https/synchronize'
export * from './api/https/deleteAccount'


//init hosting
export * from './website/index'



//=== deploy ===
//- firebase deploy --only functions:synchronize
//- firebase functions:log

//=== debug ===
//- functions start
//- functions deploy --timeout 3600s synchronize --trigger-http
//- functions inspect synchronize
//- (click on green arrow)
//- functions call synchronize --data='{ "data" : { "userId" : "TOTO", "origin" : "TEST", "signatures" : "123456789", "player" : { "nbHexacoins" : 0, "hasRemovedAds" : false, "arcade" : { "maxScore" : 0, "maxLevel" : 0, "graph" : { "zones" : 0, "unlockedNodes" : [], "activeNodes" : [] } }, "timeAttack" : { "maxScore" : 0, "maxTimeSec" : 0, "graph" : { "zones" : 0, "unlockedNodes" : [], "activeNodes" : [] } } } } }'

//=== website local ===
//- firebase deploy --only functions:home
//- firebase serve
// browser => http://localhost:5000/home
