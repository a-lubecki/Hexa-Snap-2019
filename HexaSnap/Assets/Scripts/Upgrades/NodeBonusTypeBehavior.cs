/**
 * Hexa Snap
 * © Aurélien Lubecki 2019
 * All Rights Reserved
 */

using UnityEngine;
using UnityEngine.UI;


public class NodeBonusTypeBehavior : BaseNodeBehavior, NodeBonusTypeListener {

    public NodeBonusType nodeBonusType {
        get {
            return (NodeBonusType) model;
        }
    }


    private RawImage imagePercentage;
    private CanvasGroup imagePercentageAlpha;
    private Text textPercentage;


    protected override void onAwake() {
        base.onAwake();

        Transform trImagePercentage = transform.Find("ImagePercentage");

        imagePercentage = trImagePercentage.GetComponent<RawImage>();
        imagePercentageAlpha = trImagePercentage.GetComponent<CanvasGroup>();
        textPercentage = trImagePercentage.Find("TextPercentage").GetComponent<Text>();
    }

    protected override void onInit() {
        base.onInit();

        Transform trButton = transform.Find("Button");

        trButton.GetComponent<Image>().sprite = GameHelper.Instance.loadSpriteAsset(Constants.PATH_DESIGNS_OBJECTS + getBonusBackgroundImageName());

        trButton.Find("ImageIcon").GetComponent<RawImage>().texture = GameHelper.Instance.getBonusManager().getSpriteItemBonus(nodeBonusType.bonusType.getTag(null)).texture;

        RectTransform rectPercentage = imagePercentage.GetComponent<RectTransform>();

        if (!nodeBonusType.bonusType.isMalus) {
            //anchor to the left
            rectPercentage.pivot = new Vector2(1.15f, 0);
        } else {
            //anchor to the right
            rectPercentage.pivot = new Vector2(-0.15f, 0);
        }

        updatePercentageVisibility();
        updatePercentageText();
    }

    private string getBonusBackgroundImageName() {

        BonusType bonusType = nodeBonusType.bonusType;

        if (!bonusType.isMalus) {
            return "Item.Hexagon.Bonus";
        }

        return "Item.Hexagon.Malus";
    }

    private void updatePercentageVisibility() {
        
        imagePercentage.gameObject.SetActive(nodeBonusType.isZoneEnabled);
    }

    private void updatePercentageText() {

        textPercentage.text = nodeBonusType.getFormattedActivatePercentage() + "%";
        imagePercentageAlpha.alpha = (nodeBonusType.getActivatePercentage() <= 0) ? 0.25f : 1;
    }

    void NodeBonusTypeListener.onNodeZoneEnabled(NodeBonusType node) {

        updatePercentageVisibility();
    }

    void NodeBonusTypeListener.onNodeStateChange(NodeBonusType node, int slotPos) {

        updatePercentageText();
    }

}
