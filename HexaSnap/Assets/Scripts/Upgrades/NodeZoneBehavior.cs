/**
 * Hexa Snap
 * © Aurélien Lubecki 2019
 * All Rights Reserved
 */

using System;
using UnityEngine;
using UnityEngine.UI;


public class NodeZoneBehavior : BaseNodeBehavior, NodeZoneListener {

    public NodeZone nodeZone {
        get {
            return (NodeZone) model;
        }
    }


    private static Sprite spriteBgDisabled;
    private static Sprite spriteBgLocked;

    private static Sprite spriteFgLocked;
    private static Sprite spriteFgDeactivated;

    private RectTransform trButton;

    private Image imageBackground;
    private Text textZone;
    private Image imageLock;
    private Text textUnlock;
    private RawImage imageHexacoins;
    private Text textHexacoins;
    private Text textTuto;

    private Animation animationAlpha;
    private Animation animationNodeClickable;

    private BadgeBehavior badge;

    private bool isAnimating;


    protected override void onAwake() {
        base.onAwake();

        if (spriteBgDisabled == null) {
            spriteBgDisabled = GameHelper.Instance.loadSpriteAsset(Constants.PATH_DESIGNS_BONUS + "Node.Zone.Disabled");
            spriteBgLocked = GameHelper.Instance.loadSpriteAsset(Constants.PATH_DESIGNS_BONUS + "Node.Zone.Locked");
            spriteFgLocked = GameHelper.Instance.loadSpriteAsset(Constants.PATH_DESIGNS_BONUS + "Node.Lock");
            spriteFgDeactivated = GameHelper.Instance.loadSpriteAsset(Constants.PATH_DESIGNS_BONUS + "Node.Sign");
        }

        trButton = GetComponentInChildren<Button>().GetComponent<RectTransform>();

        imageBackground = trButton.GetComponent<Image>();
        Transform trImageNode = trButton.Find("ImageNode");
        textZone = trImageNode.Find("TextZone").GetComponent<Text>();
        imageLock = trButton.Find("ImageLock").GetComponent<Image>();
        textUnlock = trButton.Find("TextUnlock").GetComponent<Text>();
        imageHexacoins = trButton.Find("ImageHexacoins").GetComponent<RawImage>();
        textHexacoins = trButton.Find("TextHexacoins").GetComponent<Text>();
        textTuto = trButton.Find("TextTuto").GetComponent<Text>();

        textUnlock.text = Tr.get("Activity20.Section.Unlock");

        animationAlpha = trButton.GetComponent<Animation>();
        animationNodeClickable = trImageNode.GetComponent<Animation>();

        badge = transform.Find("BadgeAnchor").Find("Badge").GetComponent<BadgeBehavior>();
    }

    protected override void onInit() {
        base.onInit();

        trButton.sizeDelta = new Vector2(trButton.sizeDelta.x, nodeZone.height);

        textZone.text = nodeZone.tag;
        textHexacoins.text = nodeZone.nbHexacoinsToUnlock.ToString();

        updateState();
    }

    private void updateState() {

        imageBackground.enabled = false;
        imageLock.enabled = false;
        textUnlock.enabled = false;
        imageHexacoins.enabled = false;
        textHexacoins.enabled = false;
        textTuto.enabled = false;

        badge.setText(null);

        switch (nodeZone.state) {

            case NodeZoneState.DISABLED:

                imageBackground.enabled = true;
                imageBackground.sprite = spriteBgDisabled;

                imageLock.enabled = true;
                imageLock.sprite = spriteFgLocked;
                imageLock.SetNativeSize();

                break;

            case NodeZoneState.LOCKED:

                imageBackground.enabled = true;
                imageBackground.sprite = spriteBgLocked;

                textUnlock.enabled = true;
                imageHexacoins.enabled = true;
                textHexacoins.enabled = true;

                if (nodeZone.tag.Equals("1")) {
                    textTuto.enabled = true;
                    textTuto.text = Tr.get("Activity20.Section.Tuto");
                }

                //show a badge on the zone as it can be unlocked
                badge.setText("!");

                break;

            case NodeZoneState.ACTIVATED:

                break;

            case NodeZoneState.DEACTIVATED:

                imageBackground.enabled = true;
                imageBackground.sprite = spriteBgLocked;

                imageLock.enabled = true;
                imageLock.sprite = spriteFgDeactivated;
                imageLock.SetNativeSize();

                if (nodeZone.tag.Equals("1")) {
                    textTuto.enabled = true;
                    textTuto.text = Tr.get("Activity20.Section.Activate");
                }

                break;

            default:
                throw new NotImplementedException();
        }

        updateClickableAnimation();
    }


    public override void onNodeClick() {

        if (!isAnimating) {
            base.onNodeClick();
        }

    }

    void NodeZoneListener.onNodeStateChange(NodeZone node, NodeZoneState lastState) {
        
        if ((lastState == NodeZoneState.LOCKED || lastState == NodeZoneState.DEACTIVATED) && node.state == NodeZoneState.ACTIVATED) {

            //animate disappearing then update
            isAnimating = true;

            updateState();
            imageBackground.enabled = true;
            
            Constants.playAnimation(animationAlpha, null, false);

            Async.call(animationAlpha.clip.length, () => {

                imageBackground.enabled = false;

                isAnimating = false;
            });

            return;
        }

        if (lastState == NodeZoneState.ACTIVATED && node.state == NodeZoneState.DEACTIVATED) {

            //update + animate appearing
            isAnimating = true;

            imageBackground.enabled = true;

            Constants.playAnimation(animationAlpha, null, true);

            Async.call(animationAlpha.clip.length, () => {

                updateState();

                isAnimating = false;
            });

            return;
        }

        updateState();

    }

    private void updateClickableAnimation() {

        if (nodeZone == null) {
            return;
        }

        //notify the user that the button is clickable with an infinit anim
        if (nodeZone.state != NodeZoneState.DISABLED) {
            Constants.playAnimation(animationNodeClickable, null, false);
        } else {
            animationNodeClickable.Stop();
        }
    }

}
