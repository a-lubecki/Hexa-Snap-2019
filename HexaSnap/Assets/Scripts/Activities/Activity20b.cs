/**
 * Hexa Snap
 * © Aurélien Lubecki 2019
 * All Rights Reserved
 */

public class Activity20b : Activity20 {

    protected override Graph getGraphForInit() {
        return GameHelper.Instance.getUpgradesManager().graphTimeAttack;
    }

    protected override CharacterSituation getFirstCharacterSituation(bool isFirstResume) {

        if (!GameHelper.Instance.getUniqueDisplaySpeechesManager().hasTag("20b.Onboarding")) {

            return new CharacterSituation()
                .enqueueTr("20b.Onboarding")
                .enqueueDelayMove(5)
                .enqueueMove(CharacterRes.MOVE_SHRINK)
                .enqueueExpression(CharacterRes.EXPR_SURPRISED, 3)
                .enqueueExpression(CharacterRes.EXPR_MEME_POKERFACE, 5.5f)
                .enqueueExpression(CharacterRes.EXPR_CUTE, 3)
                .enqueueUniqueDisplay("20b.Onboarding");
        }

        return base.getFirstCharacterSituation(isFirstResume);
    }

    protected override void saveGraph() {
        SaveManager.Instance.saveGraphTimeAttack();
    }

    protected override bool onNodeZoneUnlock(NodeZone node) {

        if (base.onNodeZoneUnlock(node)) {
            return true;
        }

        if ("2".Equals(node.tag)) {

            GameHelper.Instance.getCharacterAnimator()
                      .show(this, true)
                      .enqueueTr("20b.Unlocked2");
        }

        if ("3".Equals(node.tag)) {

            GameHelper.Instance.getCharacterAnimator()
                      .show(this, true)
                      .enqueueTr("20b.Unlocked3");
        }

        if ("4".Equals(node.tag)) {

            GameHelper.Instance.getCharacterAnimator()
                      .show(this, true)
                      .enqueueTr("20b.Unlocked4");
        }

        return false;
    }

    protected override void trackZoneUnlocked(string zoneTag) {

        gameManager.trackNodeZoneState(zoneTag, T.Value.ACTIVATED);

        gameManager.trackMaxUnlockedNodeZone(T.Property.T_MAX_ZONE, zoneTag);

        TrackingManager.instance.prepareEvent(T.Event.T_NODE_UNLOCK)
                       .add(T.Param.TAG, zoneTag)
                       .track();
    }

    protected override void trackZoneActivated(string zoneTag) {

        gameManager.trackNodeZoneState(zoneTag, T.Value.ACTIVATED);

        TrackingManager.instance.prepareEvent(T.Event.T_NODE_ACTIVATE)
                       .add(T.Param.TAG, zoneTag)
                       .track();
    }

    protected override void trackZoneDeactivated(string zoneTag) {

        gameManager.trackNodeZoneState(zoneTag, T.Value.DEACTIVATED);

        TrackingManager.instance.prepareEvent(T.Event.T_NODE_DEACTIVATE)
                       .add(T.Param.TAG, zoneTag)
                       .track();
    }


}
