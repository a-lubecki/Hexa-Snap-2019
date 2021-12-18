/**
* Hexa Snap
* © Aurélien Lubecki 2019
* All Rights Reserved
*/

using UnityEngine;


public class NodeBonusType : BaseNode {

    private static NodeBonusTypeListener to(BaseModelListener listener) {
        return (NodeBonusTypeListener)listener;
    }


    public readonly BonusType bonusType;

    public bool isZoneEnabled { get; private set; }
    private NodeSlotState[] slots;

    public NodeBonusType(Graph graph, string tag, BonusType bonusType, int nbSlots) : base(graph, tag) {

        this.bonusType = bonusType;
        slots = new NodeSlotState[nbSlots];
    }


    public int getNodeMaskUnlock() {

        int res = 0;

        int s = 0;
        foreach (NodeSlotState state in slots) {

            if (state == NodeSlotState.DEACTIVATED || state == NodeSlotState.ACTIVATED) {
                res += (1 << s);
            }

            s++;
        }

        return res;
    }

    public int getNodeMaskActivate() {

        int res = 0;

        int s = 0;
        foreach (NodeSlotState state in slots) {

            if (state == NodeSlotState.LOCKED || state == NodeSlotState.ACTIVATED) {
                res += (1 << s);
            }

            s++;
        }

        return res;
    }

    public int getNbSlots() {
        return slots.Length;
    }

    public NodeSlotState getState(int slotPos) {
        return slots[slotPos];
    }

    public void setZoneEnabled(bool enabled) {

        if (enabled == isZoneEnabled) {
            return;
        }

        isZoneEnabled = enabled;

        notifyListeners(listener => {
            to(listener).onNodeZoneEnabled(this);
        });

    }

    public void setState(int slotPos, NodeSlotState state) {

        slots[slotPos] = state;

        notifyListeners(listener => {
            to(listener).onNodeStateChange(this, slotPos);
        });

    }

    public int getNbUnlockedSlots() {

        int nbUnlocked = 0;

        foreach (NodeSlotState state in slots) {

            if (state != NodeSlotState.LOCKED) {
                nbUnlocked++;
            }
        }

        return nbUnlocked;
    }

    public float getUnlockPercentage() {

        return getNbUnlockedSlots() / (float)slots.Length;
    }

    public bool areAllSlotsUnlocked() {
        return getNbUnlockedSlots() >= getNbSlots();
    }

    public float getActivatePercentage() {

        int nbActivated = 0;

        foreach (NodeSlotState state in slots) {

            if (state != NodeSlotState.DEACTIVATED) {//if locked, the slot is activated
                nbActivated++;
            }
        }

        return nbActivated / (float)slots.Length;
    }

    public string getFormattedActivatePercentage() {

        return Mathf.FloorToInt(getActivatePercentage() * 100).ToString();
    }

}


public enum NodeSlotState {
    LOCKED,
    DEACTIVATED,
    ACTIVATED
}
