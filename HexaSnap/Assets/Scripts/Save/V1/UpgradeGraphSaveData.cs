/**
 * Hexa Snap
 * © Aurélien Lubecki 2019
 * All Rights Reserved
 */

using System;

[Serializable]
public class UpgradeGraphSaveData {

    protected int zones;
    protected int[] unlockedNodes;//by zone
    protected int[] activeNodes;//by zone
    
    public UpgradeGraphSaveData(Graph g) {

        zones = g.getZonesMask();
        unlockedNodes = g.getUnlockedNodesMask();
        activeNodes = g.getActiveNodesMask();
    }

    public UpgradeGraphSaveData(int zones, int[] unlockedNodes, int[] activeNodes) {

        if (zones < 0) {
            throw new ArgumentException();
        }

        if (unlockedNodes == null) {
            throw new ArgumentException();
        }

        if (activeNodes == null) {
            throw new ArgumentException();
        }

        this.zones = zones;
        this.unlockedNodes = unlockedNodes;
        this.activeNodes = activeNodes;
    }

    public int getZonesMask() {
        return zones;
    }

    public int getNbZones() {
        return unlockedNodes.Length;
    }

    public int getUnlockedNodeMask(int zone) {

        if (zone < 0 || zone >= unlockedNodes.Length) {
            return 0;
        }

        return unlockedNodes[zone];
    }

    public int getActiveNodeMask(int zone) {

        if (zone < 0 || zone >= activeNodes.Length) {
            return 0;
        }

        return activeNodes[zone];
    }

    public NodeZoneState getNodeZoneState(int nodeZonePos) {

        bool isUnlocked = ((zones >> (nodeZonePos * 2)) & 1) == 1;
        if (!isUnlocked) {
            return NodeZoneState.LOCKED;
        }

        bool isActive = ((zones >> (nodeZonePos * 2 + 1)) & 1) == 1;
        if (isActive) {
            return NodeZoneState.ACTIVATED;
        }

        return NodeZoneState.DEACTIVATED;
    }

    public NodeSlotState getNodeBonusTypeSlotState(int nbSlots, int nodeZonePos, int nodeBonusTypePos, int slotPos) {
        
        int maskUnlock = getUnlockedNodeMask(nodeZonePos);

        bool isUnlocked = ((maskUnlock >> (nodeBonusTypePos * nbSlots + slotPos)) & 1) == 1;
        if (!isUnlocked) {
            return NodeSlotState.LOCKED;
        }

        int maskActive = getActiveNodeMask(nodeZonePos);

        bool isActive = ((maskActive >> (nodeBonusTypePos * nbSlots + slotPos)) & 1) == 1;
        if (isActive) {
            return NodeSlotState.ACTIVATED;
        }

        return NodeSlotState.DEACTIVATED;
    }

}

