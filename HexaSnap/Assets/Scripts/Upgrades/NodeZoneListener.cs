/**
 * Hexa Snap
 * © Aurélien Lubecki 2019
 * All Rights Reserved
 */

public interface NodeZoneListener : BaseNodeListener {

    void onNodeStateChange(NodeZone node, NodeZoneState lastState);

}
