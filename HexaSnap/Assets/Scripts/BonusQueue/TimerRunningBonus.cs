/**
 * Hexa Snap
 * © Aurélien Lubecki 2019
 * All Rights Reserved
 */

using System;


public class TimerRunningBonus : GameTimer {

	public static readonly int nbSticks = 10;
    

	public ItemBonus itemBonus { get; private set; }


	public TimerRunningBonus(Activity10 activity, ItemBonus itemBonus, float totalDurationSec) : base(activity, false, totalDurationSec) {

		if (itemBonus == null) {
			throw new ArgumentException();
		}

		this.itemBonus = itemBonus;
	}
    
}

