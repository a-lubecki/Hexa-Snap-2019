/**
 * Hexa Snap
 * © Aurélien Lubecki 2019
 * All Rights Reserved
 */


public class Activity4c : Activity4 {
	

	protected override string[] getPrefabNamesToLoad() {
		return new string[] { "Activity4" };
	}

	protected override string getTextTitle() {
		return Tr.get("Activity4c.Text.Timer");
	}

	protected override string getTextTarget() {
		return Tr.get("Activity4c.Text.Target.Time");
	}


    protected override void onResume(bool isFirst) {
		base.onResume(isFirst); 
        
        triggerPreviousNext(
            false,
            null, 
            Constants.getDisplayableTimeSec(Constants.INITIAL_TIME_ATTACK_TIME_S),
            () => {

			    push(new Activity10b());
			    pop();
		});
	}

}


