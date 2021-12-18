/**
 * Hexa Snap
 * © Aurélien Lubecki 2019
 * All Rights Reserved
 */

public class Activity20a : Activity20 {

    protected override Graph getGraphForInit() {
        return GameHelper.Instance.getUpgradesManager().graphArcade;
    }

    protected override void saveGraph() {
        SaveManager.Instance.saveGraphArcade();
    }

    protected override bool onNodeZoneUnlock(NodeZone node) {

        if (base.onNodeZoneUnlock(node)) {
            return true;
        }

        if ("2".Equals(node.tag)) {

            GameHelper.Instance.getCharacterAnimator()
                      .show(this, true)
                      .enqueueTr("20a.Unlocked2");
        }

        if ("3".Equals(node.tag)) {

            GameHelper.Instance.getCharacterAnimator()
                      .show(this, true)
                      .enqueueTr("20a.Unlocked3");
        }

        if ("4".Equals(node.tag)) {

            GameHelper.Instance.getCharacterAnimator()
                      .show(this, true)
                      .enqueueTr("20a.Unlocked4");
        }

        if ("5".Equals(node.tag)) {

            GameHelper.Instance.getCharacterAnimator()
                      .show(this, true)
                      .enqueueTr("20a.Unlocked5");
        }

        if ("6".Equals(node.tag)) {

            GameHelper.Instance.getCharacterAnimator()
                      .show(this, true)
                      .enqueueTr("20a.Unlocked6");
        }

        return false;
    }

    protected override void trackZoneUnlocked(string zoneTag) {

        gameManager.trackNodeZoneState(zoneTag, T.Value.ACTIVATED);

        gameManager.trackMaxUnlockedNodeZone(T.Property.A_MAX_ZONE, zoneTag);

        TrackingManager.instance.prepareEvent(T.Event.A_NODE_UNLOCK)
                       .add(T.Param.TAG, zoneTag)
                       .track();
    }

    protected override void trackZoneActivated(string zoneTag) {

        gameManager.trackNodeZoneState(zoneTag, T.Value.ACTIVATED);

        TrackingManager.instance.prepareEvent(T.Event.A_NODE_ACTIVATE)
                       .add(T.Param.TAG, zoneTag)
                       .track();
    }

    protected override void trackZoneDeactivated(string zoneTag) {

        gameManager.trackNodeZoneState(zoneTag, T.Value.DEACTIVATED);

        TrackingManager.instance.prepareEvent(T.Event.A_NODE_DEACTIVATE)
                       .add(T.Param.TAG, zoneTag)
                       .track();
    }

}
