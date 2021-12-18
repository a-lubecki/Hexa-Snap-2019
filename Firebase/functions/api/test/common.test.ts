/**
 * Hexa Snap
 * © Aurélien Lubecki 2019
 * All Rights Reserved
 */

import * as admin from 'firebase-admin'
import * as chai from 'chai'
import * as sinon from 'sinon'
import * as httpMocks from 'node-mocks-http'
import { RequestCheck } from '../src/common/RequestCheck'
import { RequestParams } from '../src/common/RequestParams'
import { Utils } from '../src/common/Utils'
import { GraphNodesMap } from '../src/model/GraphNodeMap'
import { GameMode } from '../src/model/GameMode'

const expect = chai.expect


describe('Hexasnap Common', () => {

    let adminInitStub: any
    let req: any

    before(() => {
        adminInitStub = sinon.stub(admin, 'initializeApp')
    })

    after(() => {
        adminInitStub.restore()
    })

    beforeEach(() => {

        if (this.sinon == null) {
            this.sinon = sinon.sandbox.create()
        } else {
            this.sinon.restore()
        }

        req = httpMocks.createRequest()
    })

    describe('RequestCheck', () => {

        describe('checkAuthToken', () => {

            let authTokenStub

            describe('uid: undefined', () => {

                before(() => {
                    authTokenStub = sinon.stub(admin.auth(), 'verifyIdToken').returns(Promise.resolve(
                        {
                            uid: "undefined"
                        }
                    ))
                })

                after(() => {
                    authTokenStub.restore()
                })

                it('should check and fail', async () => {

                    const ok = await RequestCheck["checkAuthToken"](
                        "tokenTest",
                        "user1234"
                    )

                    expect(ok).to.be.false
                })
            })

            describe('uid: "user1234"', () => {

                before(() => {
                    authTokenStub = sinon.stub(admin.auth(), 'verifyIdToken').returns(Promise.resolve(
                        {
                            uid: "user1234"
                        }
                    ))
                })

                after(() => {
                    authTokenStub.restore()
                })

                it('should precheck id token', async () => {

                    const idToken = await admin.auth().verifyIdToken("123456789")

                    expect(idToken.uid).to.equal("user1234")
                })

                it('should check and succeed with undefined token', async () => {

                    const ok = await RequestCheck["checkAuthToken"](
                        undefined,
                        "user1234"
                    )

                    expect(ok).to.be.false
                })

                it('should check and succeed with bad token type', async () => {

                    const ok = await RequestCheck["checkAuthToken"](
                        1000 as any,
                        "user1234"
                    )

                    expect(ok).to.be.false
                })

                it('should check and succeed with undefined userId', async () => {

                    const ok = await RequestCheck["checkAuthToken"](
                        "tokenTest",
                        undefined
                    )

                    expect(ok).to.be.false
                })

                it('should check and succeed with bad userId type', async () => {

                    const ok = await RequestCheck["checkAuthToken"](
                        undefined,
                        1000 as any
                    )

                    expect(ok).to.be.false
                })

                it('should check and succeed with bad userId', async () => {

                    const ok = await RequestCheck["checkAuthToken"](
                        "tokenTest",
                        "badToken"
                    )

                    expect(ok).to.be.false
                })

                it('should check and succeed with good userId', async () => {

                    const ok = await RequestCheck["checkAuthToken"](
                        "tokenTest",
                        "user1234"
                    )

                    expect(ok).to.be.true
                })
            })
        })

        describe('checkRequiredParamsOrFail', () => {

            it('should check and fail with undefined array', () => {

                const func = () => {
                    RequestCheck.checkRequiredParamsOrFail(req, undefined, new Map())
                }

                expect(func).to.throw
            })

            it('should check and fail with bad array type', () => {

                const func = () => {
                    RequestCheck.checkRequiredParamsOrFail(req, 10 as any, new Map())
                }

                expect(func).to.throw
            })

            it('should check and succeed with empty array', () => {

                const params = RequestCheck.checkRequiredParamsOrFail(req, new Map(), new Map())

                expect(params).to.be.undefined
            })
/*
            it('should check and fail with one returning undefined', () => {

                const extractors = new Map<string, any>()
                extractors["aaa"] = () => "test"
                extractors["bbb"] = () => undefined

                const params = RequestCheck.checkRequiredParamsOrFail(req, res, extractors, new Map())

                expect(params).to.be.undefined
                expect(res.statusCode).to.equal(200)
                expect(res._isEndCalled()).to.be.false
            })

            it('should check and succeed with all ok', () => {

                const extractors = new Map<string, any>()
                extractors["aaa"] = () => "test1"
                extractors["bbb"] = () => "test2"

                const params = RequestCheck.checkRequiredParamsOrFail(req, res, extractors, new Map())

                expect(params).to.deep.equal(extractors)
                expect(res.statusCode).to.equal(200)
                expect(res._isEndCalled()).to.be.false
            })*/
        })
    })

    describe('RequestParams', () => {

        describe('extractParam', () => {

            describe('Request no body', () => {

                it('should extract undefined for not existing param', () => {

                    const value = RequestParams.extractParam(req, "notExistingParam")
                    expect(value).to.be.undefined
                })

                it('should extract undefined for bad param type', () => {

                    const value = RequestParams.extractParam(req, 1000 as any)
                    expect(value).to.be.undefined
                })
            })

            describe('Request no data', () => {

                beforeEach(() => {

                    req.body = {}
                })

                it('should extract undefined for not existing param', () => {

                    const value = RequestParams.extractParam(req, "notExistingParam")
                    expect(value).to.be.undefined
                })
            })

            describe('Request with data', () => {

                beforeEach(() => {

                    req.body = {
                        data: {
                            nullValue: null,
                            origin: "originTest"
                        }
                    }
                })

                it('should extract undefined for not existing param', () => {

                    const value = RequestParams.extractParam(req, "notExistingParam")
                    expect(value).to.be.undefined
                })

                it('should extract null for not nullValue param', () => {

                    const value = RequestParams.extractParam(req, "nullValue")
                    expect(value).to.be.null
                })

                it('should extract existing param', () => {

                    const value = RequestParams.extractParam(req, "origin")
                    expect(value).to.equal("originTest")
                })
            })
        })

        describe('extractParamString', () => {

            it('should extract null param', () => {

                req.body = {
                    data: {
                        param: null
                    }
                }

                const value = RequestParams.extractParamString(req, "param")
                expect(value).to.be.null
            })

            it('should extract undefined for bad param type', () => {

                const value = RequestParams.extractParamString(req, 1000 as any)
                expect(value).to.be.undefined
            })

            it('should fail to extract existing param', () => {

                req.body = {
                    data: {
                        param: 0
                    }
                }

                const value = RequestParams.extractParamString(req, "param")
                expect(value).to.be.null
            })

            it('should fail to extract empty param', () => {

                req.body = {
                    data: {
                        param: ""
                    }
                }

                const value = RequestParams.extractParamString(req, "param")
                expect(value).to.be.null
            })

            it('should extract existing param', () => {

                req.body = {
                    data: {
                        param: "paramTest"
                    }
                }

                const value = RequestParams.extractParamString(req, "param")
                expect(value).to.equal("paramTest")
            })
        })

        describe('extractParamNumber', () => {

            it('should fail to extract param', () => {

                req.body = {
                    data: {
                        param: "aaa"
                    }
                }

                const value = RequestParams.extractParamNumber(req, "param")
                expect(value).to.be.null
            })

            it('should extract existing param', () => {

                req.body = {
                    data: {
                        param: 10
                    }
                }

                const value = RequestParams.extractParamNumber(req, "param")
                expect(value).to.equal(10)
            })
        })

        describe('extractParamPositiveNumber', () => {

            it('should fail to extract negative number', () => {

                req.body = {
                    data: {
                        param: -10
                    }
                }

                const value = RequestParams.extractParamPositiveNumber(req, "param")
                expect(value).to.be.null
            })

            it('should extract existing positive number', () => {

                req.body = {
                    data: {
                        param: 10
                    }
                }

                const value = RequestParams.extractParamPositiveNumber(req, "param")
                expect(value).to.equal(10)
            })
        })

        describe('extractParamNumberArray', () => {

            it('should fail to extract array', () => {

                req.body = {
                    data: {
                        param: 0
                    }
                }

                const value = RequestParams.extractParamNumberArray(req, "param")
                expect(value).to.be.null
            })

            it('should extract existing array with bad values', () => {

                req.body = {
                    data: {
                        param: ["aaa", "bbb"]
                    }
                }

                const value = RequestParams.extractParamNumberArray(req, "param")
                expect(value).to.be.null
            })
/*
            it('should extract existing array with number values', () => {

                req.body = {
                    data: {
                        param: [1, 2]
                    }
                }

                const value = RequestParams.extractParamNumberArray(req, "param")
                expect(value).to.deep.equal([1, 2])
            })*/
        })

        describe('extractParamObject', () => {

            it('should fail to extract object', () => {

                req.body = {
                    data: {
                        param: 0
                    }
                }

                const value = RequestParams.extractParamObject(req, "param")
                expect(value).to.be.null
            })

            it('should extract existing array with object', () => {

                req.body = {
                    data: {
                        param: {}
                    }
                }

                const value = RequestParams.extractParamObject(req, "param")
                expect(value).to.deep.equal({})
            })
        })
    })

    describe('Utils', () => {

        describe('isTypeCorrect', () => {

            it('should throw with undefined typeName', () => {
                const func = () => { Utils.isTypeCorrect(true, undefined) }
                expect(func).to.throw
            })

            it('should throw with bad type for typeName', () => {
                const func = () => { Utils.isTypeCorrect(true, {}) }
                expect(func).to.throw
            })

            it('should return false for Number with Boolean', () => {
                const value = Utils.isTypeCorrect(10, Boolean)
                expect(value).to.be.false
            })

            it('should return false for String with Number', () => {
                const value = Utils.isTypeCorrect("aaa", Number)
                expect(value).to.be.false
            })

            it('should return true for Boolean', () => {
                const value = Utils.isTypeCorrect(true, Boolean)
                expect(value).to.be.true
            })

            it('should return true for Number', () => {
                const value = Utils.isTypeCorrect(42, Number)
                expect(value).to.be.true
            })

            it('should return true for String', () => {
                const value = Utils.isTypeCorrect("aaa", String)
                expect(value).to.be.true
            })

            it('should return true for Object', () => {
                const value = Utils.isTypeCorrect({}, Object)
                expect(value).to.be.true
            })

            it('should return true for Map', () => {
                const value = Utils.isTypeCorrect(new Map, Map)
                expect(value).to.be.true
            })

            it('should return true for GraphNodesMap', () => {
                const value = Utils.isTypeCorrect(new GraphNodesMap(null), GraphNodesMap)
                expect(value).to.be.true
            })

            it('should return true for GameMode', () => {
                const value = Utils.isTypeCorrect(GameMode.ARCADE, GameMode)
                expect(value).to.be.true
            })
        })

        describe('isValueCorrect', () => {

            it('should return false for undefined', () => {
                const value = Utils.isValueCorrect(undefined, Boolean)
                expect(value).to.be.false
            })

            it('should return true for Boolean', () => {
                const value = Utils.isValueCorrect(true, Boolean)
                expect(value).to.be.true
            })
        })

        describe('newObjectFromMap', () => {

            it('should fail for bad type', () => {
                const func = () => Utils.newObjectFromMap(10 as any)
                expect(func).to.throw
            })

            it('should return empty from undefined', () => {
                const value = Utils.newObjectFromMap(undefined)
                expect(value).to.be.empty
            })

            it('should return empty from undefined', () => {
                const value = Utils.newObjectFromMap(undefined)
                expect(value).to.be.empty
            })

            it('should return empty from empty', () => {
                const value = Utils.newObjectFromMap(new Map())
                expect(value).to.be.empty
            })

            it('should return equivalent object from map', () => {

                const obj = {
                    key1: 42,
                    key2: 80
                }
                const map = new Map<string, number>()
                map["key1"] = 42
                map["key2"] = 80

                const value = Utils.newObjectFromMap(map)
                expect(value).to.deep.equal(obj)
            })
        })

        describe('newMapFromObject', () => {

            it('should fail for bad type', () => {
                const func = () => Utils.newMapFromObject(10 as any)
                expect(func).to.throw
            })

            it('should return empty from undefined', () => {
                const value = Utils.newMapFromObject(undefined)
                expect(value).to.be.empty
            })

            it('should return empty from empty', () => {
                const value = Utils.newMapFromObject({})
                expect(value).to.be.empty
            })

            it('should return equivalent object from map', () => {

                const obj = {
                    key1: 42,
                    key2: 80
                }
                const map = new Map<string, number>()
                map["key1"] = 42
                map["key2"] = 80

                const value = Utils.newMapFromObject(obj)
                expect(value).to.deep.equal(map)
            })
        })

        describe('areEquals', () => {

            it('should return false from undefined and undefined', () => {
                const ok = Utils.areEquals(undefined, undefined)
                expect(ok).to.be.false
            })

            it('should return false for bad types', () => {
                const ok = Utils.areEquals(10 as any, 10 as any)
                expect(ok).to.be.false
            })

            it('should return false from undefined and empty', () => {
                const ok = Utils.areEquals(undefined, new Map())
                expect(ok).to.be.false
            })

            it('should return false from empty and undefined', () => {
                const ok = Utils.areEquals(new Map(), undefined)
                expect(ok).to.be.false
            })

            it('should return false from different maps keys', () => {

                const map1 = new Map<string, number>()
                map1["key1"] = 42
                map1["key2"] = 80

                const map2 = new Map<string, number>()
                map2["key1"] = 42
                map2["key3"] = 80

                const ok = Utils.areEquals(map1, map2)
                expect(ok).to.be.false
            })

            it('should return false from different maps values', () => {

                const map1 = new Map<string, number>()
                map1["key1"] = 42
                map1["key2"] = 80

                const map2 = new Map<string, number>()
                map2["key1"] = 42
                map2["key2"] = 100

                const ok = Utils.areEquals(map1, map2)
                expect(ok).to.be.false
            })

            it('should return true from empty and empty', () => {
                const ok = Utils.areEquals(new Map(), new Map())
                expect(ok).to.be.true
            })

            it('should return true from equivalent maps', () => {

                const map1 = new Map<string, number>()
                map1["key1"] = 42
                map1["key2"] = 80

                const map2 = new Map<string, number>()
                map2["key1"] = 42
                map2["key2"] = 80

                const ok = Utils.areEquals(map1, map2)
                expect(ok).to.be.true
            })

            it('should return true from same maps', () => {

                const map = new Map<string, number>()
                map["key1"] = 42
                map["key2"] = 80

                const ok = Utils.areEquals(map, map)
                expect(ok).to.be.true
            })
        })

        describe('checkDocumentData', () => {

            it('should fail with undefined keys', () => {

                const func = function() {
                    Utils.checkDocumentData({}, undefined, [Object])
                }

                expect(func).to.throw
            })

            it('should fail with undefined types', () => {

                const func = function() {
                    Utils.checkDocumentData({}, ["test"], undefined)
                }

                expect(func).to.throw
            })

            it('should fail with empty keys and types', () => {

                const func = function() {
                    Utils.checkDocumentData({}, [], [])
                }

                expect(func).to.throw
            })

            it('should fail with more keys than types', () => {

                const func = function() {
                    Utils.checkDocumentData({}, ["test"], [])
                }

                expect(func).to.throw
            })

            it('should fail with more types than keys', () => {

                const func = function() {
                    Utils.checkDocumentData({}, [], [Object])
                }

                expect(func).to.throw
            })

            it('should return empty map for empty data', () => {
                const map = Utils.checkDocumentData({}, ["test"], [Object])
                expect(map).to.be.empty
            })

            it('should return empty map for empty value', () => {

                const document = {
                    test: {}
                }

                const map = Utils.checkDocumentData(document, ["test"], [Object])
                expect(map).to.be.empty
            })

            it('should return map with null value', () => {

                const document = {
                    test: null
                }

                const expectedMap = new Map<string, any>()
                expectedMap["test"] = null

                const map = Utils.checkDocumentData(document, ["test"], [Object])
                expect(map).to.deep.equal(expectedMap)
            })

            it('should return empty map because of bad types', () => {

                const document = {
                    test: "aaa"
                }

                const map = Utils.checkDocumentData(document, ["test"], [Object])
                expect(map).to.be.empty
            })

            it('should return filled map', () => {

                const document = {
                    test: {
                        id: 1234
                    }
                }

                const expectedMap = new Map<string, any>()
                expectedMap["test"] = {
                    id: 1234
                }

                const map = Utils.checkDocumentData(document, ["test"], [Object])
                expect(map).to.deep.equal(expectedMap)
            })
        })

    })

})
