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
import { SerializerGraph } from '../src/serializing/SerializerGraph'

const expect = chai.expect

/*
describe('Hexasnap Serializing', () => {

    describe('SerializerGraph', () => {

        let s: SerializerGraph

        beforeEach(() => {
            s = new SerializerGraph()
        })

        describe('deserialize', () => {

            it('should return undefined with undefined data', () => {

                const res = s.deserialize(undefined)
                expect(res).to.be.null
            })

            it('should return default with empty data', () => {

                const res = s.deserialize({})
                expect(res.zones).to.equal(0)
                expect(res.activeNodes["nodes"]).to.be.empty
                expect(res.unlockedNodes["nodes"]).to.be.empty
            })

            it('should return undefined with bad zone type', () => {

                const data = {
                    zones: "aaa"
                }

                const res = s.deserialize(data)
                expect(res.zones).to.equal(0)
                expect(res.activeNodes["nodes"]).to.be.empty
                expect(res.unlockedNodes["nodes"]).to.be.empty
            })

            it('should return graph with good zone', () => {

                const data = {
                    zones: 42
                }

                const res = s.deserialize(data)
                expect(res.zones).to.equal(42)
                expect(res.activeNodes["nodes"]).to.be.empty
                expect(res.unlockedNodes["nodes"]).to.be.empty
            })

            //TODO

        })
    })

})
*/