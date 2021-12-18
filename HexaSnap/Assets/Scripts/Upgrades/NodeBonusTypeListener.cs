/**
 * Hexa Snap
 * © Aurélien Lubecki 2019
 * All Rights Reserved
 */

public interface NodeBonusTypeListener : BaseNodeListener {

    void onNodeZoneEnabled(NodeBonusType node);

    void onNodeStateChange(NodeBonusType node, int slotPos);

}
