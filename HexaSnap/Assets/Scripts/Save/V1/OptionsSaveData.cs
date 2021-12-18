/**
 * Hexa Snap
 * © Aurélien Lubecki 2019
 * All Rights Reserved
 */

using System;
using UnityEngine;


[Serializable]
public class OptionsSaveData {


	protected bool passedOnboarding;

	protected bool isMusicDeactivated;
	protected bool isSoundsDeactivated;
    protected bool removedAds;

    protected SystemLanguage language;

    public bool hasPassedOnboarding() {
        return passedOnboarding;
    }

    public bool isMusicOptionDeactivated() {
        return isMusicDeactivated;
    }

    public bool isSoundsOptionDeactivated() {
        return isSoundsDeactivated;
    }

    public bool hasRemovedAds() {
        return removedAds;
    }

    public SystemLanguage getLanguage() {
        return language;
    }
}

