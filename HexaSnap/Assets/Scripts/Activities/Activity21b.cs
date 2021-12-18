﻿/**
 * Hexa Snap
 * © Aurélien Lubecki 2019
 * All Rights Reserved
 */

public class Activity21b : Activity21 {


	protected override string getTitleForInit() {
		return Tr.get("Activity21b.Title");
	}

    protected override void trackSlotsUnlocked() {

        TrackingManager.instance.prepareEvent(T.Event.T_SLOTS_UNLOCK)
                       .add(T.Param.TAG, node.tag)
                       .add(T.Param.PERCENTAGE, getActivePercentage())
                       .track();
    }

    protected override void trackSlotsActivated() {

        TrackingManager.instance.prepareEvent(T.Event.T_SLOTS_ACTIVATE)
                       .add(T.Param.TAG, node.tag)
                       .add(T.Param.PERCENTAGE, getActivePercentage())
                       .track();
    }

    protected override void trackSlotsDeactivated() {

        TrackingManager.instance.prepareEvent(T.Event.T_SLOTS_DEACTIVATE)
                       .add(T.Param.TAG, node.tag)
                       .add(T.Param.PERCENTAGE, getActivePercentage())
                       .track();
    }

}
