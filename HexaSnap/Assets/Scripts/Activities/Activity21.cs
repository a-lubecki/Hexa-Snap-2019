/**
 * Hexa Snap
 * © Aurélien Lubecki 2019
 * All Rights Reserved
 */

using System;
using UnityEngine;
using UnityEngine.UI;


public abstract class Activity21 : BaseUIActivity {


    //used to know if the graph must be saved on exit
    private bool hasChangedGraph = false;

    private Transform trImageBackground;

    private MenuButtonBehavior buttonAll;
    private Text textChances;
    private Text textPercentage;

    private RawImage imageLineTop;
    private RawImage imageLineBottom;
    private RawImage imageLineMiddleM;
    private RawImage imageLineMiddleL;
    private RawImage imageLineMiddleR;
    private RectTransform trLineMiddleM;
    private RectTransform trLineMiddleL;
    private RectTransform trLineMiddleR;

    private Button buttonNbHexacoins;
    private Text textUnlockAll;

    private GameObject[] goSlots;


    private Graph graph;
    protected NodeBonusType node { get; private set; }
    private BonusType bonusType;
    private int nbSlots;
    private bool has2Lines;


    protected override MarkerBehavior getCurrentMarkerForInit(MarkerManager markerManager) {
        return markerManager.markerHBonusSpecific;
    }

    protected override string[] getPrefabNamesToLoad() {
        return new string[] { "Activity21" };
    }

    protected override bool hasDefaultHexacoinWallet() {
        return true;
    }

    protected abstract void trackSlotsUnlocked();

    protected abstract void trackSlotsActivated();

    protected abstract void trackSlotsDeactivated();


    protected override void onCreate() {
        base.onCreate();

        BundlePush21 b = (BundlePush21) bundlePush;

        graph = b.graph;
        if (graph == null) {
            throw new InvalidOperationException();
        }

        node = b.selectedNode;
        if (node == null) {
            throw new InvalidOperationException();
        }

        bonusType = node.bonusType;
        nbSlots = node.getNbSlots();
        has2Lines = (nbSlots > 4);


        RawImage imageBonusBackground = findChildTransform("ImageBonusBackground").GetComponent<RawImage>();
        imageBonusBackground.texture = GameHelper.Instance.loadTexture2DAsset(Constants.PATH_DESIGNS_OBJECTS + getBonusBackgroundImageName());

        RawImage imageBonusIcon = imageBonusBackground.transform.Find("ImageBonusIcon").GetComponent<RawImage>();
        imageBonusIcon.texture = GameHelper.Instance.getBonusManager().getSpriteItemBonus(bonusType.getTag(null)).texture;


        Color bonusColor = getBonusColor();

        Text textBonusTitle = updateText("TextBonusTitle", bonusType.title);
        textBonusTitle.color = bonusColor;

        updateText("TextBonusDescription", bonusType.description);

        Text textTypeTitle = updateText("TextTypeTitle", getTypeTitle());
        textTypeTitle.color = bonusColor;

        Text textTypeDescription = updateText("TextTypeDescription", getTypeDescription());
        textTypeDescription.color = bonusColor;

        trImageBackground = findChildTransform("ImageBackground");
        RawImage imageBackground = trImageBackground.GetComponent<RawImage>();
        imageBackground.texture = GameHelper.Instance.loadTexture2DAsset(Constants.PATH_DESIGNS_BONUS + getBackgroundImageName());

        RawImage imageBorder = trImageBackground.Find("ImageBorder").GetComponent<RawImage>();
        imageBorder.texture = GameHelper.Instance.loadTexture2DAsset(Constants.PATH_DESIGNS_BONUS + getBorderImageName());

        Transform trImagePercentageBackground = trImageBackground.Find("ImagePercentageBackground");

        textChances = trImagePercentageBackground.Find("TextChances").GetComponent<Text>();
        textChances.text = Tr.get("Activity21.Item.Chances");

        textPercentage = trImagePercentageBackground.Find("TextPercentage").GetComponent<Text>();

        //add slots
        goSlots = new GameObject[nbSlots];

        for (int i = 0 ; i < nbSlots ; i++) {
            createNewSlotGameObject(i);
        }

        imageLineTop = trImageBackground.Find("LineTop").GetComponent<RawImage>();
        imageLineBottom = trImageBackground.Find("LineBottom").GetComponent<RawImage>();
        imageLineMiddleM = trImageBackground.Find("LineMiddleM").GetComponent<RawImage>();
        imageLineMiddleL = trImageBackground.Find("LineMiddleL").GetComponent<RawImage>();
        imageLineMiddleR = trImageBackground.Find("LineMiddleR").GetComponent<RawImage>();
        trLineMiddleM = imageLineMiddleM.GetComponent<RectTransform>();
        trLineMiddleL = imageLineMiddleL.GetComponent<RectTransform>();
        trLineMiddleR = imageLineMiddleR.GetComponent<RectTransform>();

        buttonAll = createButtonGameObject(
            this,
            trImageBackground,
            trImageBackground.Find("PosAll").position,
            MenuButtonSmall.newButtonDefault(
                "MenuButton.PlusPlus"
            )
        );

        buttonNbHexacoins = trImageBackground.Find("ButtonNbHexacoins").GetComponent<Button>();
        buttonNbHexacoins.onClick.AddListener(tryUnlockAll);

        textUnlockAll = trImageBackground.Find("TextUnlockAll").GetComponent<Text>();
        textUnlockAll.text = Tr.get("Activity21.Button.Activate");
        textUnlockAll.color = node.isReachable ? Constants.COLOR_TITLE_NEGATIVE : Constants.COLOR_TITLE;

        updatePercentageText();
        updateSlots();
        updateNbHexacoinsToUnlock();
    }

    protected override void onResume(bool isFirst) {
        base.onResume(isFirst);

        synchronizeSlotsAnims();
    }

    protected override void onPrePause(bool isLast) {
        base.onPrePause(isLast);

        if (isLast && hasChangedGraph) {

            //save graph nodes states now in case the player quit the game
            saveGraph();

            hasChangedGraph = false;
        }
    }


    private void saveGraph() {

        if (graph == gameManager.upgradesManager.graphArcade) {
            SaveManager.Instance.saveGraphArcade();
        } else if (graph == gameManager.upgradesManager.graphTimeAttack) {
            SaveManager.Instance.saveGraphTimeAttack();
        }
    }

    private string getBonusBackgroundImageName() {

        if (!bonusType.isMalus) {
            return "Item.Hexagon.Bonus";
        }

        return "Item.Hexagon.Malus";
    }

    private string getTypeTitle() {

        if (!bonusType.isMalus) {

            if (bonusType.isInstant) {
                return Tr.get("Activity21a.INSTANT");
            }

            return Tr.get("Activity21a.PERSISTENT");
        }

        if (bonusType.isInstant) {
            return Tr.get("Activity21b.INSTANT");
        }

        return Tr.get("Activity21b.PERSISTENT");
    }

    private string getTypeDescription() {

        if (!bonusType.isMalus) {

            if (bonusType.isInstant) {
                return Tr.get("Activity21a.INSTANT.Description");
            }

            return Tr.get("Activity21a.PERSISTENT.Description");
        }

        if (bonusType.isInstant) {
            return Tr.get("Activity21b.INSTANT.Description");
        }

        return Tr.get("Activity21b.PERSISTENT.Description");
    }

    private Color getBonusColor() {

        if (!bonusType.isMalus) {
            return Constants.COLOR_BONUS;
        }

        return Constants.COLOR_MALUS;
    }

    private string getBackgroundImageName() {

        if (!bonusType.isMalus) {
            return "Specific.Bonus.Background";
        }

        return "Specific.Malus.Background";
    }

    private string getBorderImageName() {

        if (!bonusType.isMalus) {
            return "Specific.Bonus.Border";
        }

        return "Specific.Malus.Border";
    }

    protected int getActivePercentage() {
        return Mathf.FloorToInt(node.getActivatePercentage() * 100);
    }

    private bool canActivateAll() {
        return (getActivePercentage() < 100);
    }

    private int getNbHexacoinsToUnlockAll() {

        int nbSlotsToUnlock = nbSlots - node.getNbUnlockedSlots();
        if (nbSlotsToUnlock <= 0) {
            return 0;
        }

        return nbSlotsToUnlock * Constants.NB_HEXACOINS_TO_UNLOCK_SLOT;
    }

    private void updatePercentageText() {

        textPercentage.text = node.getFormattedActivatePercentage();

        if (canActivateAll()) {
            (buttonAll.menuButton as MenuButtonSmall).changeSpriteBg("MenuButton.PlusPlus");
            textUnlockAll.text = Tr.get("Activity21.Button.Activate");
        } else {
            (buttonAll.menuButton as MenuButtonSmall).changeSpriteBg("MenuButton.MinusMinus");
            textUnlockAll.text = Tr.get("Activity21.Button.Deactivate");
        }
    }

    private void updateSlots() {

        //show 1 or 2 lines
        imageLineMiddleM.gameObject.SetActive(!has2Lines);
        imageLineMiddleL.gameObject.SetActive(has2Lines);
        imageLineMiddleR.gameObject.SetActive(has2Lines);

        //hide the bottom line if there are no next nodes
        imageLineBottom.gameObject.SetActive(graph.hasChildrenNodes(node.tag));

        int nbUnlocked = 0;

        if (!has2Lines) {
            
            int nbFirstUnlocked = 0;

            for (int i = 0 ; i < nbSlots ; i++) {

                if (node.getState(i) == NodeSlotState.LOCKED) {
                    break;
                }

                nbFirstUnlocked++;
            }

            nbUnlocked = nbFirstUnlocked;

            float advancePercentage = (nbSlots - nbFirstUnlocked) / (float)(nbSlots + 1);
            trLineMiddleM.anchorMin = new Vector2(trLineMiddleM.anchorMin.x, advancePercentage);

        } else {

            int nbFirstUnlockedL = 0;

            int nbHalfSlots = (int) (nbSlots / 2f);

            for (int i = 0 ; i < nbHalfSlots ; i++) {

                if (node.getState(i) == NodeSlotState.LOCKED) {
                    break;
                }

                nbFirstUnlockedL++;
            }

            float advancePercentageL = (nbHalfSlots - nbFirstUnlockedL) / (float) (nbHalfSlots + 1);
            trLineMiddleL.anchorMin = new Vector2(trLineMiddleL.anchorMin.x, advancePercentageL);

            int nbFirstUnlockedR = 0;

            for (int i = nbHalfSlots ; i < nbSlots ; i++) {

                if (node.getState(i) == NodeSlotState.LOCKED) {
                    break;
                }

                nbFirstUnlockedR++;
            }

            nbUnlocked = nbFirstUnlockedL + nbFirstUnlockedR;

            float advancePercentageR = (nbHalfSlots - nbFirstUnlockedR) / (float) (nbHalfSlots + 1);
            trLineMiddleR.anchorMin = new Vector2(trLineMiddleR.anchorMin.x, advancePercentageR);
        }

        for (int i = 0 ; i < nbSlots ; i++) {
            updateSlotAlpha(i);
        }

        //update line colors :
        bool areAllSlotsActivated = (nbUnlocked == nbSlots);

        Color colorTop = node.isReachable ? Constants.COLOR_LINE_DEFAULT : Constants.COLOR_LINE_INACTIVE;
        Color colorBottom = node.isReachable && areAllSlotsActivated ? Constants.COLOR_LINE_DEFAULT : Constants.COLOR_LINE_INACTIVE;

        imageLineTop.color = colorTop;
        imageLineMiddleM.color = colorTop;
        imageLineMiddleL.color = colorTop;
        imageLineMiddleR.color = colorTop;
        imageLineBottom.color = colorBottom;
    }

    private void updateNbHexacoinsToUnlock() {

        bool isActive = false;

        if (bonusType.isMalus && !node.areAllSlotsUnlocked()) {
            isActive = true;
        }

        buttonNbHexacoins.gameObject.SetActive(isActive);
        buttonAll.gameObject.SetActive(!isActive);

        if (isActive) {
            buttonNbHexacoins.GetComponentInChildren<Text>().text = getNbHexacoinsToUnlockAll().ToString();
        }
    }

    public void createNewSlotGameObject(int slotPos) {

        if (goSlots[slotPos] != null) {
            //free the last
            GameObject.Destroy(goSlots[slotPos]);
        }

        bool isLocked = (node.getState(slotPos) == NodeSlotState.LOCKED);

        string prefabName;
        if (isLocked) {
            prefabName = Constants.PREFAB_NAME_NODE_SLOT_LOCKED;
        } else {
            prefabName = Constants.PREFAB_NAME_NODE_SLOT_ACTIVATED;
        }

        GameObject prefab = GameHelper.Instance.loadPrefabAsset(prefabName);
        GameObject goSlot = GameObject.Instantiate(prefab, trImageBackground);

        goSlots[slotPos] = goSlot;

        RawImage image = goSlot.GetComponent<RawImage>();

        string imageSuffix = isLocked ? "Locked" : "Activated";
        image.texture = GameHelper.Instance.loadTexture2DAsset(Constants.PATH_DESIGNS_BONUS + "Specific.Slot." + nbSlots + "." + imageSuffix);
        image.SetNativeSize();

        if (isLocked) {
            //add price for 1 slot unlock
            goSlot.GetComponentInChildren<Text>().text = Constants.NB_HEXACOINS_TO_UNLOCK_SLOT.ToString();
        }

        goSlot.GetComponent<Button>().onClick.AddListener(() => {
            onSlotClick(slotPos);
        });


        RectTransform transform = goSlot.GetComponent<RectTransform>();
        
        int nbLineSlots = !has2Lines ? nbSlots : (int) (nbSlots / 2f);
        int lineSlotPos = !has2Lines ? slotPos : ((slotPos < nbLineSlots) ? slotPos : (slotPos - nbLineSlots));

        float x = !has2Lines ? 0.5f : (slotPos < nbLineSlots ? 1.1f : -0.1f);
        float y = 0.5f + 1.1f * (lineSlotPos - (nbLineSlots - 1) / 2f);
        transform.pivot = new Vector2(x, y);

        updateSlotAlpha(slotPos);
    }

    private void updateSlotAlpha(int slotPos) {

        RawImage image = goSlots[slotPos].GetComponent<RawImage>();

        float alpha = (node.getState(slotPos) == NodeSlotState.DEACTIVATED) ? 0.25f : 1;
        image.color = new Color(1, 1, 1, alpha);
    }

    protected override void onButtonClick(MenuButtonBehavior menuButton) {

        if (menuButton == buttonAll) {

            if (canActivateAll()) {
                setStateForAllSlots(NodeSlotState.ACTIVATED);
            } else {
                setStateForAllSlots(NodeSlotState.DEACTIVATED);
            }

            hasChangedGraph = true;

        } else {

            base.onButtonClick(menuButton);
        }
    }

    private void onSlotClick(int slotPos) {

        if (node.getState(slotPos) == NodeSlotState.LOCKED) {
            tryUnlockSlot(slotPos);
        } else {
            swapSlot(slotPos);
        }
    }


    private void swapSlot(int slotPos) {

        if (node.getState(slotPos) == NodeSlotState.ACTIVATED) {

            node.setState(slotPos, NodeSlotState.DEACTIVATED);

            GameHelper.Instance.getAudioManager().playSound("Bonus.Deactivate");

            trackSlotsDeactivated();
        
        } else {
            
            node.setState(slotPos, NodeSlotState.ACTIVATED);

            GameHelper.Instance.getAudioManager().playSound("Bonus.Activate");

            trackSlotsActivated();
        }

        updateSlots();
        updatePercentageText();

        graph.updateGraphPercentagesHolder();

        hasChangedGraph = true;
    }

    private void tryUnlockSlot(int slotPos) {

        //save unlock now
        int nbHexacoinsToPay = Constants.NB_HEXACOINS_TO_UNLOCK_SLOT;

        //track button click
        TrackingManager.instance.prepareEvent(T.Event.HEXACOIN_PAY_START)
                       .add(T.Param.NB_HEXACOINS, nbHexacoinsToPay)
                       .add(T.Param.ORIGIN, getActivityName())
                       .add(T.Param.REASON, T.Value.PAY_REASON_UNLOCK_SLOT)
                       .track();
        
        HexacoinsWallet wallet = GameHelper.Instance.getHexacoinsWallet();
        if (!wallet.canPayHexacoins(nbHexacoinsToPay)) {

            //fallback on shop if can't pay
            push(new Activity22());
            return;
        }

        node.setState(slotPos, NodeSlotState.DEACTIVATED);
        createNewSlotGameObject(slotPos);

        if (node.areAllSlotsUnlocked()) {
            //last one
            graph.updateChildrenStatesAfterUnlock(node);
        }

        saveGraph();

        gameManager.payHexacoins(nbHexacoinsToPay, getActivityName(), T.Value.PAY_REASON_UNLOCK_SLOT);

        updateSlots();
        updateNbHexacoinsToUnlock();
        updatePercentageText();

        graph.updateGraphPercentagesHolder();

        synchronizeSlotsAnims();

        GameHelper.Instance.getAudioManager().playSound("Bonus.Unlock");

        trackSlotsUnlocked();
    }

    private void tryUnlockAll() {

        int nbHexacoinsToPay = getNbHexacoinsToUnlockAll();

        //track button click
        TrackingManager.instance.prepareEvent(T.Event.HEXACOIN_PAY_START)
                       .add(T.Param.NB_HEXACOINS, nbHexacoinsToPay)
                       .add(T.Param.ORIGIN, getActivityName())
                       .add(T.Param.REASON, T.Value.PAY_REASON_UNLOCK_SLOT)
                       .track();
        
        HexacoinsWallet wallet = GameHelper.Instance.getHexacoinsWallet();
        if (!wallet.canPayHexacoins(nbHexacoinsToPay)) {

            //fallback on shop if can't pay
            push(new Activity22());
            return;
        }

        for (int i = 0 ; i < nbSlots ; i++) {

            if (node.getState(i) != NodeSlotState.LOCKED) {
                continue;
            }

            node.setState(i, NodeSlotState.DEACTIVATED);
            createNewSlotGameObject(i);
        }

        graph.updateChildrenStatesAfterUnlock(node);

        saveGraph();

        //save unlock now
        gameManager.payHexacoins(nbHexacoinsToPay, getActivityName(), T.Value.PAY_REASON_UNLOCK_SLOT);

        updateSlots();
        updateNbHexacoinsToUnlock();
        updatePercentageText();

        graph.updateGraphPercentagesHolder();

        GameHelper.Instance.getAudioManager().playSound("Bonus.Unlock");

        trackSlotsUnlocked();
    }

    private void setStateForAllSlots(NodeSlotState state) {
        
        for (int i = 0 ; i < nbSlots ; i++) {

            if (node.getState(i) == NodeSlotState.LOCKED) {
                continue;
            }

            node.setState(i, state);
        }

        updateSlots();
        updatePercentageText();

        graph.updateGraphPercentagesHolder();

        if (state == NodeSlotState.DEACTIVATED) {

            GameHelper.Instance.getAudioManager().playSound("Bonus.Deactivate");

            trackSlotsDeactivated();

        } else if (state == NodeSlotState.ACTIVATED) {
            
            GameHelper.Instance.getAudioManager().playSound("Bonus.Activate");

            trackSlotsActivated();
        }
    }

    private void synchronizeSlotsAnims() {

        for (int i = 0; i < nbSlots; i++) {

            GameObject goSlot = goSlots[i];
            float delay = i * 0.05f;

            //deactivating then reactivating the slot after a frame will cause the anim to be reset
            Async.call(delay, () => {

                if (goSlot == null) {
                    //object has been destroyed between the call and now
                    return;
                }

                goSlot.SetActive(false);
                goSlot.SetActive(true);
            });
        }
    }

}

public class BundlePush21 : BaseBundle {

	public Graph graph;
    public NodeBonusType selectedNode;

}
