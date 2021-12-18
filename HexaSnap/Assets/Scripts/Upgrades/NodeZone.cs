/**
 * Hexa Snap
 * © Aurélien Lubecki 2019
 * All Rights Reserved
 */
 
using UnityEngine;

public class NodeZone : BaseNode {

    private static NodeZoneListener to(BaseModelListener listener) {
        return (NodeZoneListener) listener;
    }


    public readonly Vector2 posInGraph;
    public readonly int height;
    public readonly int nbHexacoinsToUnlock;
    public readonly int nbSlotsByNode;

    public NodeZoneState state { get; private set; }


    public NodeZone(Graph graph, string tag, Vector2 posInGraph, int height, int nbHexacoinsToUnlock, int nbSlotsByNode) : base(graph, tag) {

        this.posInGraph = posInGraph;
        this.height = height;
        this.nbHexacoinsToUnlock = nbHexacoinsToUnlock;
        this.nbSlotsByNode = nbSlotsByNode;
    }

    public NodeZone(Graph graph, string tag, Vector2 posInGraph, int height, int nbHexacoinsToUnlock, int nbSlotsByNode, float linePercentageLevel, bool lineMustStartVerticalFirst) : base(graph, tag, linePercentageLevel, lineMustStartVerticalFirst) {

        this.posInGraph = posInGraph;
        this.height = height;
        this.nbHexacoinsToUnlock = nbHexacoinsToUnlock;
        this.nbSlotsByNode = nbSlotsByNode;
    }


    public void setState(NodeZoneState state) {

        if (this.state == state) {
            return;
        }

        NodeZoneState lastState = this.state;
        this.state = state;

        notifyListeners(listener => {
            to(listener).onNodeStateChange(this, lastState);
        });

    }

}


public enum NodeZoneState {
    DISABLED,//locked : can't unlock it
    LOCKED,//locked and unlockable
    ACTIVATED,//unlocked and active
    DEACTIVATED//unlocked and inactive
}
