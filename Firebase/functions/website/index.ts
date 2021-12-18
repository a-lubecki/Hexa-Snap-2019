/**
 * Hexa Snap
 * © Aurélien Lubecki 2019
 * All Rights Reserved
 */

import * as functions from 'firebase-functions'
import * as express from 'express'
import * as i18n from 'i18n'
import { isNullOrUndefined } from 'util'

const minifier = require('express-minify-html')
const app = express()

//init visible folder
app.use(express.static(__dirname + '/public', { maxAge: '1d' }))

//init templating
app.set('view engine', 'ejs');

//init i18n
i18n.configure({
    locales: ['en', 'fr'],
    queryParameter: 'lang',
    defaultLocale: 'en',
    directory: __dirname + '/locales',
    api: {
        '__': 'tr'
    }
})

app.use(i18n.init)

//init minify
app.use(minifier({
    override: true,
    exception_url: false,
    htmlMinifier: {
        removeComments: true,
        collapseWhitespace: true,
        collapseBooleanAttributes: true,
        removeAttributeQuotes: true,
        removeEmptyAttributes: true,
        minifyJS: true
    }
}))


let overridenLang: string | undefined = undefined

//pages generation
function renderPage(req: express.Request, res: express.Response, page: string) {

    //change locale for all requests if param detected
    const langParam = req.query.lang

    if (!isNullOrUndefined(langParam)) {
        overridenLang = langParam
    }

    if (!isNullOrUndefined(overridenLang)) {
        res.setLocale(overridenLang);
    }

    res.render(page, { req: req, res: res })
}

//pages generation
function handleRequest(path: string, page: string) {

    app.get(path, (req, res) => {
        renderPage(req, res, page)
    })
}

//global routing
handleRequest('/', 'page_home')
handleRequest('/terms', 'page_terms')
handleRequest('/privacy', 'page_privacy')
handleRequest('/legal', 'page_legal')

//special redirect for /about part included in home
app.get("/about", (_, res) => {
    res.redirect(301, "/#about")
})

//handle 404 for the rest of the pages
app.use((req, res) => {
    res.status(404)
    renderPage(req, res, 'page_404')
})

export const website = functions.https.onRequest(app);


/*

    => COMING SOON

    "rewrites": [
      {
        "source": "**",
        "destination": "/index.html"
      }
    ],

    => FINAL PROD

    "rewrites": [
      {
        "source": "**",
        "function": "website"
      }
    ],

*/