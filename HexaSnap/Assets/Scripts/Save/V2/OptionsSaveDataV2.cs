/**
 * Hexa Snap
 * © Aurélien Lubecki 2019
 * All Rights Reserved
 */

using System;
using UnityEngine;


[Serializable]
public class OptionsSaveDataV2 {


	protected bool passedOnboarding;

    protected bool isMusicDeactivated;
    protected bool isSoundsDeactivated;
    protected bool isControlsDragHorizontal;
    protected bool removedAds;


    public OptionsSaveDataV2(OptionsSaveData previousData) {

        if (previousData == null) {
            throw new ArgumentException();
        }

        passedOnboarding = previousData.hasPassedOnboarding();

        isMusicDeactivated = previousData.isMusicOptionDeactivated();
        isSoundsDeactivated = previousData.isSoundsOptionDeactivated();
        isControlsDragHorizontal = true;//the first version of the app had the horizontal drag by default
        removedAds = previousData.hasRemovedAds();
    }

    public OptionsSaveDataV2(GameManager gameManager) {
        
		if (gameManager == null) {
			throw new ArgumentException();
		}

		passedOnboarding = gameManager.hasPassedOnboarding;

		isMusicDeactivated = gameManager.isMusicOptionDeactivated;
		isSoundsDeactivated = gameManager.isSoundsOptionDeactivated;
        isControlsDragHorizontal = gameManager.isControlsOptionDragHorizontal;
        removedAds = gameManager.hasRemovedAds;
	}

    public bool hasPassedOnboarding() {
        return passedOnboarding;
    }

    public bool isMusicOptionDeactivated() {
        return isMusicDeactivated;
    }

    public bool isSoundsOptionDeactivated() {
        return isSoundsDeactivated;
    }

    public bool isControlsOptionDragHorizontal() {
        return isControlsDragHorizontal;
    }

    public bool hasRemovedAds() {
        return removedAds;
    }

}

