/**
 * Hexa Snap
 * © Aurélien Lubecki 2019
 * All Rights Reserved
 */

using UnityEngine;
using UnityEngine.UI;


public class HexacoinsWalletBehavior : BaseModelBehavior, HexacoinsWalletListener {

    public HexacoinsWallet hexacoinsWallet {
        get {
            return (HexacoinsWallet) model;
        }
    }


    private Text textTitle;
    private Text textDifference;
    private HexacoinBehavior hexacoinBehavior;
    private Animation animEarn;

    private int nbGlitteringMoreActions = 0;

    private bool isOnlyDisplayedOnChanges = false;
    private int totalDifference = 0;


    protected override void onAwake() {
        base.onAwake();

        textTitle = transform.Find("TextTitle").GetComponent<Text>();
        textDifference = transform.Find("TextDifference").GetComponent<Text>();
        hexacoinBehavior = GetComponentInChildren<HexacoinBehavior>();
        animEarn = transform.Find("Hexacoin").GetComponent<Animation>();
    }

    protected override void onInit() {
        base.onInit();

        updateNbHexacoins();

        //reset the diff text to hide it
        textDifference.text = "";
    }

    BaseModelBehavior BaseModelListener.getModelBehavior() {
		return this;
	}

	void HexacoinsWalletListener.onNbHexacoinsChanged(HexacoinsWallet hexacoinsWallet, int difference) {

        if (isOnlyDisplayedOnChanges) {

            reveal();

            Async.call(2, hideTemporary);
        }

        nbGlitteringMoreActions++;
        updateGlitteringMore();
        
        Async.call(0.1f, updateNbHexacoins);
        updateDifference(difference);

        Constants.playAnimation(animEarn, null, false);

        Async.call(1, () => {

            if (!isInit()) {
                return;
            }

            nbGlitteringMoreActions--;
            updateGlitteringMore();
        });
    }

    private void updateNbHexacoins() {

        textTitle.text = hexacoinsWallet.nbHexacoins.ToString();
    }

    private void updateDifference(int difference) {
        
        if (difference == 0) {
            //no diff to show
            return;
        }

        //display the difference text on top for a moment
        totalDifference += difference;

        if (totalDifference > 0) {
            textDifference.text = "+" + totalDifference.ToString();
        } else {
            textDifference.text = totalDifference.ToString();
        }

        //save diff to see if the current value is the same between now and the moment it will hide (overriden value by another hexacoins change)
        var lastDifference = totalDifference;

        //hide after a delay
        Async.call(1, () => {

            if (!isInit()) {
                return;
            }

            if (lastDifference != totalDifference) {
                //the difference has changed again between the diff display and now
                return;
            }

            //hide the text and reset the diff to show it the next time
            resetDifference();
        });
    }

    private void resetDifference() {

        textDifference.text = "";
        totalDifference = 0;
    }

    private void updateGlitteringMore() {
        
        hexacoinBehavior.setGlitteringMore(nbGlitteringMoreActions > 0);
    }

    public void setOnlyDisplayedOnChanges(bool displayed) {

        if (displayed == isOnlyDisplayedOnChanges) {
            return;
        }

        isOnlyDisplayedOnChanges = displayed;
        
        if (isOnlyDisplayedOnChanges) {
            hideTemporary();
        } else {
            reveal();
        }
        
    }

    public void startGlittering() {

        hexacoinBehavior.startGlittering();
    }

    public void stopGlittering() {

        hexacoinBehavior.stopGlittering();
    }

    public void reveal() {

        if (hexacoinBehavior.isActiveAndEnabled) {
            return;
        }

        textTitle.gameObject.SetActive(true);
        hexacoinBehavior.gameObject.SetActive(true);

        startGlittering();
    }

    public void hideTemporary() {

        if (!hexacoinBehavior.isActiveAndEnabled) {
            return;
        }

        stopGlittering();

        textTitle.gameObject.SetActive(false);
        hexacoinBehavior.gameObject.SetActive(false);

        resetDifference();
    }

}

