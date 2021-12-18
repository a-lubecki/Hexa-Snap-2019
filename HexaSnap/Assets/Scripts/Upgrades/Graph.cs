/**
 * Hexa Snap
 * © Aurélien Lubecki 2019
 * All Rights Reserved
 */

using System;
using System.Collections.Generic;


public class Graph : BaseModel {

    public static readonly string ROOT = "0";


    private Dictionary<string, BaseNode> nodes = new Dictionary<string, BaseNode>();
    private Dictionary<string, List<string>> relations = new Dictionary<string, List<string>>();
    
    private List<string> sortedNodesZone = new List<string>();
    private Dictionary<string, List<string>> sortedNodesBonusType = new Dictionary<string, List<string>>();//<zone tag, sorted bonus type tags>

    private Dictionary<string, string> parentNodesZone = new Dictionary<string, string>();


    public readonly GraphPercentagesHolder graphPercentagesHolder;
    public readonly float height;


    public Graph(float height) : base() {

        graphPercentagesHolder = new GraphPercentagesHolder(this);
        this.height = height;
    }

    public void initNodesStates(UpgradeGraphSaveData graphData = null) {
        
        initNodesZone(graphData);

        updateNodesBonusTypeWithZoneState();
        updateReachableNodes();
    }

    private void initNodesZone(UpgradeGraphSaveData graphData) {

        var nodesZone = getSortedNodesZone();

        //init graph with data
        for (int z = 0; z < nodesZone.Count; z++) {
        
            var nz = nodesZone[z];

            var parent = getParentNode(nz.tag);

            if (parent == null) {

                //first node
                assignNodeState(graphData, nz, z);
                continue;
            }

            if (parent is NodeZone) {
                throw new NotSupportedException("Zone child of zone : not implemented");
            }

            var parentBonusType = parent as NodeBonusType;

            var parentZone = getParentNodeZone(parentBonusType.tag);

            if (parentZone != null &&
                (parentZone.state == NodeZoneState.DISABLED || parentZone.state == NodeZoneState.LOCKED)) {

                //disable zone if zone of parent node is also disabled
                nz.setState(NodeZoneState.DISABLED);
                continue;
            }

            //check if the parent nodes are reachable and allow this node to be unlocked
            var areParentsReachable = true;

            BaseNode p = parentBonusType;
            while (p != null && p is NodeBonusType) {

                if (!(p as NodeBonusType).areAllSlotsUnlocked()) {
                    areParentsReachable = false;
                    break;
                }

                p = getParentNode(p.tag);
            }

            if (!areParentsReachable) {
                nz.setState(NodeZoneState.DISABLED);
            } else {
                assignNodeState(graphData, nz, z);
            }
        }
    }

    private void assignNodeState(UpgradeGraphSaveData graphData, NodeZone nz, int z) {

        //set state
        if (graphData == null) {

            if (nz.nbHexacoinsToUnlock < 0) {
                nz.setState(NodeZoneState.ACTIVATED);
            } else {
                nz.setState(NodeZoneState.LOCKED);
            }

        } else {
            
            nz.setState(graphData.getNodeZoneState(z));
        }

        //init zone slots

        var isZoneLocked = (nz.state == NodeZoneState.DISABLED || nz.state == NodeZoneState.LOCKED);

        int n = 0;
        foreach (var nbt in getSortedNodesBonusType(nz.tag)) {

            int nbSlots = nbt.getNbSlots();

            if (!isZoneLocked && graphData != null) {
                //assign data only if the zone was unlocked once
                for (int s = 0; s < nbSlots; s++) {
                    nbt.setState(s, graphData.getNodeBonusTypeSlotState(nbSlots, z, n, s));
                }

            } else {
                //else, assign a default state, different between bonus and malus
                setDefaultNodesBonusState(nbt);
            }

            n++;
        }
    }

    public void updateNodesBonusTypeWithZoneState() {
        
        foreach (var nz in getSortedNodesZone()) {
            
            var isZoneEnabled = (nz.state == NodeZoneState.ACTIVATED);

            foreach (var nbt in getSortedNodesBonusType(nz.tag)) {
                nbt.setZoneEnabled(isZoneEnabled);
            }
        }
    }

    public void updateReachableNodes() {

        updateReachableNodes(ROOT, true);
    }

    private void updateReachableNodes(string parentTag, bool canBeReachable) {

        var children = getChildrenNodes(parentTag);
        if (children == null) {
            return;
        }

        foreach (var node in children) {

            node.setReachable(canBeReachable);

            var childrenReachable = canBeReachable;

            if (node is NodeZone) {
                NodeZoneState state = (node as NodeZone).state;
                childrenReachable = (childrenReachable && state != NodeZoneState.DISABLED && state != NodeZoneState.LOCKED);
            } else {
                NodeBonusType nbt = (node as NodeBonusType);
                childrenReachable = (childrenReachable && nbt.areAllSlotsUnlocked());
            }

            updateReachableNodes(node.tag, childrenReachable);
        }
    }

    private void addRelation(string parentTag, string nodeTag) {

        if (parentTag == null || parentTag.Length <= 0) {
            throw new ArgumentException();
        }
        if (nodeTag == null || nodeTag.Length <= 0) {
            throw new ArgumentException();
        }
        if (parentTag.Equals(nodeTag)) {
            throw new ArgumentException();
        }

        List<string> children;

        if (!relations.ContainsKey(parentTag)) {

            children = new List<string>();
            relations.Add(parentTag, children);

        } else {
            
            if (parentTag.Equals(ROOT)) {
                throw new InvalidOperationException("The ROOT node can only have one child");
            }

            children = relations[parentTag];

            if (children.Contains(nodeTag)) {
                throw new InvalidOperationException("The node already has a parent");
            }
        }

        children.Add(nodeTag);
    }


    private void addNode(string zoneTag, string parentTag, BaseNode n) {

        string tag = n.tag;

        if (nodes.ContainsKey(tag)) {
            throw new ArgumentException();
        }

        nodes.Add(tag, n);

        if (n is NodeZone) {

            if (!sortedNodesZone.Contains(tag)) {
                sortedNodesZone.Add(tag);
            }

        } else if (n is NodeBonusType) {

            List<string> sortedNodes;

            if (sortedNodesBonusType.ContainsKey(zoneTag)) {
                sortedNodes = sortedNodesBonusType[zoneTag];
            } else {
                sortedNodes = new List<string>();
                sortedNodesBonusType.Add(zoneTag, sortedNodes);
            }

            if (!sortedNodes.Contains(tag)) {
                sortedNodes.Add(tag);
            }

        } else {
            throw new NotSupportedException();
        }

        addRelation(parentTag, tag);

        parentNodesZone[tag] = zoneTag;
    }

    public void addNodeZone(string parentTag, BaseNode n) {
        addNode(null, parentTag, n);
    }

    public void addNodesBonusType(string zoneTag, params BonusType[] bonusTypes) {

        if (zoneTag == null || zoneTag.Length <= 0) {
            throw new ArgumentException();
        }

        string lastMalusParent = zoneTag;
        string lastBonusParent = zoneTag;

        int iMalus = 1;
        int iBonus = 1;

        int nbSlots = (nodes[zoneTag] as NodeZone).nbSlotsByNode;

        foreach (BonusType b in bonusTypes) {

            NodeBonusType n;

            if (b.isMalus) {

                n = new NodeBonusType(this, zoneTag + "M" + iMalus, b, nbSlots);
                addNode(zoneTag, lastMalusParent, n);

                lastMalusParent = n.tag;
                iMalus++;

            } else {

                n = new NodeBonusType(this, zoneTag + "B" + iBonus, b, nbSlots);
                addNode(zoneTag, lastBonusParent, n);

                lastBonusParent = n.tag;
                iBonus++;
            }
            
        }

    }

    public NodeZone getRootNode() {
        return getNode("1") as NodeZone;
    }

    public BaseNode getNode(string tag) {
        
        if (tag == null || tag.Length <= 0) {
            throw new ArgumentException();
        }

        if (!nodes.ContainsKey(tag)) {
            throw new InvalidOperationException();
        }

        return nodes[tag];
    }

    public List<BaseNode> getNodes() {
        return new List<BaseNode>(nodes.Values);
    }

    public HashSet<NodeZone> getNodesZone() {

        HashSet<NodeZone> res = new HashSet<NodeZone>();

        foreach (BaseNode n in nodes.Values) {

            if (n is NodeZone) {
                res.Add(n as NodeZone);
            }
        }

        return res;
    }

    public HashSet<NodeBonusType> getNodesBonusType() {

        HashSet<NodeBonusType> res = new HashSet<NodeBonusType>();

        foreach (BaseNode n in nodes.Values) {

            if (n is NodeBonusType) {
                res.Add(n as NodeBonusType);
            }
        }

        return res;
    }

    public List<NodeZone> getSortedNodesZone() {

        List<NodeZone> res = new List<NodeZone>();

        foreach (string tag in sortedNodesZone) {
            res.Add(nodes[tag] as NodeZone);
        }

        return res;
    }

    public NodeZone getParentNodeZone(string nodeTag) {

        if (!parentNodesZone.ContainsKey(nodeTag)) {
            return null;
        }

        return nodes[parentNodesZone[nodeTag]] as NodeZone;
    }

    public List<NodeBonusType> getSortedNodesBonusType(string zoneTag) {
        
        List<NodeBonusType> res = new List<NodeBonusType>();

        foreach (string tag in sortedNodesBonusType[zoneTag]) {
            res.Add(nodes[tag] as NodeBonusType);
        }

        return res;
    }

    public BaseNode getParentNode(string childTag) {

        foreach (KeyValuePair<string, List<string>> e in relations) {

            if (e.Value.Contains(childTag)) {

                string parentTag = e.Key;
                if (parentTag.Equals(ROOT)) {
                    return null;
                }

                return nodes[e.Key];
            }
        }

        return null;
    }

    public bool hasChildrenNodes(string parentTag) {

        if (!relations.ContainsKey(parentTag)) {
            return false;
        }

        List<string> childrenTags = relations[parentTag];
        if (childrenTags == null || childrenTags.Count <= 0) {
            return false;
        }

        return true;
    }

    public List<BaseNode> getChildrenNodes(string parentTag) {
        
        if (!hasChildrenNodes(parentTag)) {
            return null;
        }

        List<string> childrenTags = relations[parentTag];

        List<BaseNode> res = new List<BaseNode>(childrenTags.Count);
        foreach (string tag in childrenTags) {

            res.Add(nodes[tag]);
        }

        return res;
    }


    public void setDefaultNodesBonusStates(NodeZone nz) {

        foreach (var nbt in getSortedNodesBonusType(nz.tag)) {
            setDefaultNodesBonusState(nbt);
        }
    }

    private void setDefaultNodesBonusState(NodeBonusType nbt) {

        var defaultState = nbt.bonusType.isMalus ? NodeSlotState.LOCKED : NodeSlotState.ACTIVATED;

        var nbSlots = nbt.getNbSlots();
        for (int s = 0; s < nbSlots; s++) {
            nbt.setState(s, defaultState);
        }
    }

    public void updateChildrenStatesAfterUnlock(BaseNode node) {

        bool areParentsReachable = true;

        BaseNode p = node;
        while (p != null && p is NodeBonusType) {

            if (!(p as NodeBonusType).areAllSlotsUnlocked()) {
                areParentsReachable = false;
                break;
            }

            p = getParentNode(p.tag);
        }

        if (!areParentsReachable) {
            return;
        }

        updateChildrenStates(node);

        updateNodesBonusTypeWithZoneState();
        updateReachableNodes();
    }

    private void updateChildrenStates(BaseNode node) {

        List<BaseNode> children = getChildrenNodes(node.tag);
        if (children == null) {
            return;
        }

        foreach (BaseNode n in children) {

            if (n is NodeZone) {

                NodeZone nz = n as NodeZone;
                nz.setState(NodeZoneState.LOCKED);

                continue;
            }
            
            NodeBonusType nbt = n as NodeBonusType;
            if (!nbt.areAllSlotsUnlocked()) {
                continue;
            }

            updateChildrenStates(n);
        }

    }

    public void updateGraphPercentagesHolder() {

        graphPercentagesHolder.updatePercentages();
    }

    public int getZonesMask() {

        List<NodeZone> nodesZone = getSortedNodesZone();
        int nbNodesZone = nodesZone.Count;

        int zonesMask = 0;

        int z = 0;
        foreach (NodeZone nz in nodesZone) {

            if (nz.state == NodeZoneState.DEACTIVATED || nz.state == NodeZoneState.ACTIVATED) {
                zonesMask += 1 << (z * 2);//bit 0, 2, 4, 6...
            }
            if (nz.state == NodeZoneState.ACTIVATED) {
                zonesMask += 1 << (z * 2 + 1);//bit 1, 3, 5, 7...
            }

            z++;
        }

        // [6ad 5ad 4ad 3ad 2ad 1ad] => bit 0 : zone 1 débloquée, bit 1 : zone 1 activée (et débloquée), bit 2 : zone 2 débloquée...
        return zonesMask;
    }

    public int[] getUnlockedNodesMask() {

        List<NodeZone> nodesZone = getSortedNodesZone();
        int nbNodesZone = nodesZone.Count;

        int[] unlockedNodes = new int[nbNodesZone];

        int z = 0;
        foreach (NodeZone nz in nodesZone) {

            int unlockedMask = 0;

            //only calculate slots mask if the zone was activated once
            if (nz.state == NodeZoneState.ACTIVATED || nz.state == NodeZoneState.DEACTIVATED) {

                int nbSlots = nz.nbSlotsByNode;

                int n = 0;
                foreach (NodeBonusType nbt in getSortedNodesBonusType(nz.tag)) {

                    unlockedMask += nbt.getNodeMaskUnlock() << (n * nbSlots);

                    n++;
                }
            }

            // [6666 5555 4444 3333 2222 1111] => bit 0-3 : node 1 + slot 0-3 débloqué, bit 4-7 : node 2 + slot 0-3 débloqué... 
            unlockedNodes[z] = unlockedMask;

            z++;
        }

        return unlockedNodes;
    }

    public int[] getActiveNodesMask() {

        List<NodeZone> nodesZone = getSortedNodesZone();
        int nbNodesZone = nodesZone.Count;

        int[] activeNodes = new int[nbNodesZone];

        int z = 0;
        foreach (NodeZone nz in nodesZone) {

            int activeMask = 0;

            //only calculate slots mask if the zone was activated once
            if (nz.state == NodeZoneState.ACTIVATED || nz.state == NodeZoneState.DEACTIVATED) {

                int nbSlots = nz.nbSlotsByNode;

                int n = 0;
                foreach (NodeBonusType nbt in getSortedNodesBonusType(nz.tag)) {

                    activeMask += nbt.getNodeMaskActivate() << (n * nbSlots);

                    n++;
                }
            }

            // [6666 5555 4444 3333 2222 1111] => bit 0-3 : node 0 / slot 0-3 débloqué, bit 4-7 : node 1 / slot 0-3 débloqué... 
            activeNodes[z] = activeMask;

            z++;
        }

        return activeNodes;
    }

}

