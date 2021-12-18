/**
 * Hexa Snap
 * © Aurélien Lubecki 2019
 * All Rights Reserved
 */

import * as chai from 'chai'
import { ResultArcade } from '../src/model/ResultArcade'
import { ResultTimeAttack } from '../src/model/ResultTimeAttack'
import { Player } from '../src/model/Player'
import { ModeArcade } from '../src/model/ModeArcade'
import { GameMode } from '../src/model/GameMode'
import { ModeTimeAttack } from '../src/model/ModeTimeAttack'
import { GraphNodesMap } from '../src/model/GraphNodeMap'
import { Graph } from '../src/model/Graph'

const expect = chai.expect


describe('Hexasnap Model', () => {

    describe('ResultArcade', () => {

        describe('constructor', () => {

            it('should not create bad ResultArcade with undefined date', () => {
                expect(function() { new ResultArcade(undefined, 0, 0) }).to.throw()
            })

            it('should not create bad ResultArcade with undefined score', () => {
                expect(function() { new ResultArcade(new Date(), undefined, 0) }).to.throw()
            })

            it('should not create bad ResultArcade with undefined level', () => {
                expect(function() { new ResultArcade(new Date(), 0, undefined) }).to.throw()
            })

            it('should not create bad ResultArcade with bad date type', () => {
                expect(function() { new ResultArcade("aaa" as any, 0, 0) }).to.throw()
            })

            it('should not create bad ResultArcade with bad score type', () => {
                expect(function() { new ResultArcade(new Date(), "aaa" as any, 0) }).to.throw()
            })

            it('should not create bad ResultArcade with bad level type', () => {
                expect(function() { new ResultArcade(new Date(), 0, "aaa" as any) }).to.throw()
            })

            it('should not create ResultArcade with bad score', () => {
                expect(function() { new ResultArcade(new Date(), -1, 0) }).to.throw()
            })

            it('should not create ResultArcade with bad level', () => {
                expect(function() { new ResultArcade(new Date(), 0, -1) }).to.throw()
            })

            it('should create ResultArcade with good date', () => {
                const date = new Date()
                expect(new ResultArcade(date, 0, 0)).to.have.property("date", date)
            })
        })
    })

    describe('ResultTimeAttack', () => {

        describe('constructor', () => {

            it('should not create bad ResultTimeAttack with undefined date', () => {
                expect(function() { new ResultTimeAttack(undefined, 0, 0) }).to.throw()
            })

            it('should not create bad ResultTimeAttack with undefined score', () => {
                expect(function() { new ResultTimeAttack(new Date(), undefined, 0) }).to.throw()
            })

            it('should not create bad ResultTimeAttack with undefined timeSec', () => {
                expect(function() { new ResultTimeAttack(new Date(), 0, undefined) }).to.throw()
            })

            it('should not create bad ResultTimeAttack with bad date type', () => {
                expect(function() { new ResultTimeAttack("aaa" as any, 0, 0) }).to.throw()
            })

            it('should not create bad ResultTimeAttack with bad score type', () => {
                expect(function() { new ResultTimeAttack(new Date(), "aaa" as any, 0) }).to.throw()
            })

            it('should not create bad ResultTimeAttack with bad timeSec type', () => {
                expect(function() { new ResultTimeAttack(new Date(), 0, "aaa" as any) }).to.throw()
            })

            it('should create ResultTimeAttack with good date', () => {
                const date = new Date()
                expect(new ResultTimeAttack(date, 0, 0)).to.have.property("date", date)
            })

            it('should not create ResultTimeAttack with bad score', () => {
                expect(function() { new ResultTimeAttack(new Date(), -1, 0) }).to.throw()
            })

            it('should not create ResultTimeAttack with bad timeSec', () => {
                expect(function() { new ResultTimeAttack(new Date(), 0, -1) }).to.throw()
            })
        })
    })

    describe('Player', () => {

        describe('constructor', () => {

            it('should not create bad Player with undefined hasRemovedAds', () => {
                expect(function() { new Player(undefined, 0) }).to.throw()
            })

            it('should not create bad Player with undefined nbHexacoins', () => {
                expect(function() { new Player(false, undefined) }).to.throw()
            })

            it('should not create bad Player with bad hasRemovedAds type', () => {
                expect(function() { new Player("aaa" as any, 0) }).to.throw()
            })

            it('should not create bad Player with bad nbHexacoins type', () => {
                expect(function() { new Player(false, "aaa" as any) }).to.throw()
            })

            it('should cnot reate Player with bad nbHexacoins', () => {
                expect(function() { new Player(true, -1) }).to.throw()
            })
        })

    })

    describe('ModeArcade', () => {

        describe('constructor', () => {

            it('should not create bad ModeArcade with undefined maxScore', () => {
                expect(function() { new ModeArcade(undefined, 0, null) }).to.throw()
            })

            it('should not create bad ModeArcade with undefined maxLevel', () => {
                expect(function() { new ModeArcade(0, undefined, null) }).to.throw()
            })

            it('should create ModeArcade with graph undefined', () => {
                expect(new ModeArcade(0, 0, undefined)).to.have.property("graph", null)
            })

            it('should create ModeArcade with graph null', () => {
                expect(new ModeArcade(0, 0, null)).to.have.property("graph", null)
            })

            it('should not create bad ModeArcade with bad maxScore type', () => {
                expect(function() { new ModeArcade("aaa" as any, 0, null) }).to.throw()
            })

            it('should not create bad ModeArcade with bad maxLevel type', () => {
                expect(function() { new ModeArcade(0, "aaa" as any, null) }).to.throw()
            })

            it('should create ModeArcade with bad graph type', () => {
                expect(function() { new ModeArcade(0, 0, "aaa" as any) }).to.throw()
            })

            it('should not create ModeArcade with bad maxScore', () => {
                expect(function() { new ModeArcade(-1, 0, null) }).to.throw()
            })

            it('should not create ModeArcade with bad maxLevel', () => {
                expect(function() { new ModeArcade(0, -1, null) }).to.throw()
            })

            it('should create ModeArcade with gameMode', () => {
                expect(new ModeArcade(0, 0, null)).to.have.property("gameMode", GameMode.ARCADE)
            })
        })
    })

    describe('ModeTimeAttack', () => {

        describe('constructor', () => {

            it('should not create bad ModeTimeAttack with undefined maxScore', () => {
                expect(function() { new ModeTimeAttack(undefined, 0, null) }).to.throw()
            })

            it('should not create bad ModeTimeAttack with undefined maxTimeSec', () => {
                expect(function() { new ModeTimeAttack(0, undefined, null) }).to.throw()
            })

            it('should create ModeTimeAttack with graph undefined', () => {
                expect(new ModeTimeAttack(0, 0, undefined)).to.have.property("graph", null)
            })

            it('should not create bad ModeTimeAttack with bad maxScore type', () => {
                expect(function() { new ModeTimeAttack("aaa" as any, 0, null) }).to.throw()
            })

            it('should not create bad ModeTimeAttack with bad maxTimeSec type', () => {
                expect(function() { new ModeTimeAttack(0, "aaa" as any, null) }).to.throw()
            })

            it('should create ModeTimeAttack with bad graph type', () => {
                expect(function() { new ModeTimeAttack(0, 0, "aaa" as any) }).to.throw()
            })

            it('should create ModeTimeAttack with graph null', () => {
                expect(new ModeTimeAttack(0, 0, null)).to.have.property("graph", null)
            })

            it('should not create ModeTimeAttack with bad maxScore', () => {
                expect(function() { new ModeTimeAttack(-1, 0, null) }).to.throw()
            })

            it('should not create ModeTimeAttack with bad maxTimeSec', () => {
                expect(function() { new ModeTimeAttack(0, -1, null) }).to.throw()
            })

            it('should create ModeTimeAttack with gameMode', () => {
                expect(new ModeTimeAttack(0, 0, null)).to.have.property("gameMode", GameMode.TIME_ATTACK)
            })
        })
    })

    describe('GraphNodesMap', () => {

        describe('constructor', () => {

            it('should not create GraphNodesMap with bad nodes type', () => {
                expect(function() { new GraphNodesMap("aaa" as any) }).to.throw()
            })

            it('should not create GraphNodesMap with bad nodes type', () => {
                expect(function() {

                    const map = new Map<string, number>()
                    map["aaa"] = 12
                    map["bbb"] = "bad"

                    return new GraphNodesMap(map)

                }).to.throw()
            })

            it('should create bad GraphNodesMap with undefined nodes', () => {

                const gnm = new GraphNodesMap(undefined)

                expect(gnm).to.have.property("nodes").that.is.empty
            })

            it('should create bad GraphNodesMap with empty nodes', () => {

                const map = new Map<string, number>()
                const gnm = new GraphNodesMap(map)

                expect(gnm).to.have.not.same.property("nodes", map)
                expect(gnm).to.have.property("nodes").that.is.empty
            })

            it('should create bad GraphNodesMap with some nodes', () => {

                const map = new Map<string, number>()
                map["aaa"] = 12
                map["bbb"] = 42

                const gnm = new GraphNodesMap(map)

                expect(gnm).to.have.not.same.property("nodes", map)
                expect(gnm).to.have.property("nodes").which.is.deep.equal(map)
            })
        })

        describe('getNodes', () => {

            it('should return empty map', () => {
                const gnm = new GraphNodesMap(undefined)
                expect(gnm.getNodes()).to.be.empty
            })

            it('should return not same map', () => {

                const map = new Map<string, number>()
                map["aaa"] = 12
                map["bbb"] = 42

                const gnm = new GraphNodesMap(map)

                expect(gnm.getNodes()).to.be.not.equal(map)
                expect(gnm.getNodes()).to.be.deep.equal(map)
            })
        })

        describe('getNodeNames', () => {

            it('should return not same keys array', () => {

                const map = new Map<string, number>()
                map["aaa"] = 12
                map["bbb"] = 42

                const gnm = new GraphNodesMap(map)

                expect(gnm.getNodeNames()).to.be.not.equal(map.keys())
                expect(gnm.getNodeNames()).to.be.deep.equal(Array.from(map.keys()))
            })
        })

        describe('getNode', () => {

            it('should fail with bad name type', () => {

                const map = new Map<string, number>()
                map["aaa"] = 12
                map["bbb"] = 42

                const gnm = new GraphNodesMap(map)

                expect(gnm.getNode(1000 as any)).to.be.equal(0)
            })

            it('should return no node with undefined', () => {

                const map = new Map<string, number>()
                map["aaa"] = 12
                map["bbb"] = 42

                const gnm = new GraphNodesMap(map)

                expect(gnm.getNode(undefined)).to.be.equal(0)
            })

            it('should return no existing node', () => {

                const map = new Map<string, number>()
                map["aaa"] = 12
                map["bbb"] = 42

                const gnm = new GraphNodesMap(map)

                expect(gnm.getNode("ccc")).to.be.equal(0)
            })

            it('should return one node', () => {

                const map = new Map<string, number>()
                map["aaa"] = 12
                map["bbb"] = 42

                const gnm = new GraphNodesMap(map)

                expect(gnm.getNode("bbb")).to.be.equal(42)
            })
        })
    })

    describe('Graph', () => {

        describe('Graph.constructor', () => {

            it('should not create Graph with undefined zones', () => {
                expect(function() { new Graph(undefined, new GraphNodesMap(), new GraphNodesMap()) }).to.throw()
            })

            it('should not create Graph with undefined activeNodes', () => {
                expect(function() { new Graph(0, undefined, new GraphNodesMap()) }).to.throw()
            })

            it('should not create Graph with undefined unlockedNodes', () => {
                expect(function() { new Graph(0, new GraphNodesMap(), undefined) }).to.throw()
            })

            it('should not create Graph with bad zones type', () => {
                expect(function() { new Graph("aaa" as any, new GraphNodesMap(), new GraphNodesMap()) }).to.throw()
            })

            it('should not create Graph with bad activeNodes type', () => {
                expect(function() { new Graph(0, "aaa" as any, new GraphNodesMap()) }).to.throw()
            })

            it('should not create Graph with bad unlockedNodes type', () => {
                expect(function() { new Graph(0, new GraphNodesMap(), "aaa" as any) }).to.throw()
            })

            it('should not create Graph with bad zones', () => {
                expect(function() { new Graph(-1, new GraphNodesMap(), new GraphNodesMap()) }).to.throw()
            })
        })
    })

})
