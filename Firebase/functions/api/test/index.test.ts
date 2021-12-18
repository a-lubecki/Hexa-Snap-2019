/**
 * Hexa Snap
 * © Aurélien Lubecki 2019
 * All Rights Reserved
 */

//tuto : https://medium.com/@FizzyInTheHall/run-typescript-mocha-tests-in-visual-studio-code-58e62a173575

import * as admin from 'firebase-admin'
import * as sinon from 'sinon'

const test = require('firebase-functions-test')()
/*
const key = functions.config().stripe.key
test.mockConfig({ stripe: { key: '23wr42ewr34' }})
*/


describe('Hexasnap Cloud Functions', () => {

    //let hexasnap
    let adminInitStub

    admin.initializeApp()

    before(() => {

        // If index.js calls admin.initializeApp at the top of the file,
        // we need to stub it out before requiring index.js. This is because the
        // functions will be executed as a part of the require process.
        // Here we stub admin.initializeApp to be a dummy function that doesn't do anything.
        adminInitStub = sinon.stub(admin, 'initializeApp')

        // Now we can require index.js and save the exports inside a namespace called myFunctions.
        //hexasnap = require('../index')
    })

    after(() => {

        // Restore admin.initializeApp() to its original method.
        adminInitStub.restore()


        // Do other cleanup tasks.
        test.cleanup()
    })


    //TODO test https functions


})
