/**
 * Hexa Snap
 * © Aurélien Lubecki 2019
 * All Rights Reserved
 */

using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;


public abstract class Activity20 : BaseUIActivity, BaseNodeSelectListener {


    private Text textRecap;

    private BaseNode selectedNode;

    protected abstract Graph getGraphForInit();

    public GraphBehavior graphBehavior { get; private set; }
    public Graph graph {
        get {
            return graphBehavior.graph;
        }
    }

    //used to know if the graph must be saved on exit
    private bool hasChangedGraph = false;

    protected override MarkerBehavior getCurrentMarkerForInit(MarkerManager markerManager) {
        return markerManager.markerGModeSpecificCredits;
    }

    protected override string[] getPrefabNamesToLoad() {
        return new string[] { "Activity20" };
    }

    protected override string getTitleForInit() {
        return Tr.get("Activity20.Title");
    }

    protected override Line newPushLine(BaseActivity next) {

        if (next is Activity21) {

            return new Line(
                BaseModelBehavior.findTransform(selectedNode).position,
                next.markerRef.posSafeAreaBottomLeft,
                SegmentThickness.LARGE,
                1,
                true);
        }

        return null;
    }

    protected override bool hasDefaultHexacoinWallet() {
        return true;
    }

    protected override Vector3 getCharacterPosInside() {
        return Constants.newVector3(markerRef.posTopLeft, 2, -2.5f, 0);
    }

    protected override CharacterSituation getFirstCharacterSituation(bool isFirstResume) {

        if (!GameHelper.Instance.getUniqueDisplaySpeechesManager().hasTag("20.NoMalus") &&
            graph.getNodesBonusType().All(n => n.areAllSlotsUnlocked())) {

            //if all slots of maluses unlocked, display super congratulations
            return new CharacterSituation()
                .enqueueTr("20.NoMalus")
                .enqueueMove(CharacterRes.MOVE_SPIRAL)
                .enqueueExpression(CharacterRes.EXPR_SUNGLASSES, 7)
                .enqueueUniqueDisplay("20.NoMalus");
        }

        if (isFirstResume) {
            return new CharacterSituation()
                .enqueueTr("20.Default");
        }

        return null;
    }

    protected abstract void saveGraph();

    protected abstract void trackZoneUnlocked(string zoneTag);

    protected abstract void trackZoneActivated(string zoneTag);

    protected abstract void trackZoneDeactivated(string zoneTag);


    protected override void onCreate() {
        base.onCreate();

        Transform trScrollView = findChildTransform("ScrollView");
        Transform trScrollViewContent = trScrollView.Find("Viewport").Find("Content");

        GameObject prefabGraph = GameHelper.Instance.loadPrefabAsset(Constants.PREFAB_NAME_GRAPH);
        graphBehavior = GameObject.Instantiate(prefabGraph, trScrollViewContent).GetComponent<GraphBehavior>();
        graphBehavior.init(getGraphForInit());

        RectTransform rt = trScrollViewContent.GetComponent<RectTransform>();
        rt.sizeDelta = new Vector2(rt.sizeDelta.x, graph.height);

        foreach (BaseNode n in graph.getNodes()) {
            n.nodeSelectListener = this;
        }

        textRecap = trScrollView.Find("TextRecap").GetComponent<Text>();

        SpecificDeviceManager.Instance.adaptScroll(trScrollView.GetComponent<ScrollRect>());
    }

    protected override void onPreResume() {
        base.onPreResume();

        updateTextRecap();
    }

    protected override void onPrePause(bool isLast) {
        base.onPrePause(isLast);

        if (isLast && hasChangedGraph) {

            //save graph nodes states now in case the player quit the game
            saveGraph();

            hasChangedGraph = false;
        }
    }

    protected override void onDestroy() {
        base.onDestroy();

        graphBehavior.deinit();
    }

    private void updateTextRecap() {

        int nbBonus = 0;
        int nbMalus = 0;

        //nbBonus and nbMalus are 0 if the graph has only random bonus
        foreach (NodeZone nz in graph.getSortedNodesZone()) {

            if (nz.state != NodeZoneState.ACTIVATED) {
                continue;
            }

            foreach (NodeBonusType nbt in graph.getSortedNodesBonusType(nz.tag)) {

                if (nbt.getActivatePercentage() > 0) {

                    if (!nbt.bonusType.isMalus) {
                        nbBonus++;
                    } else {
                        nbMalus++;
                    }
                }
            }
        }

        string strNbBonus;
        if (nbBonus == 0) {
            strNbBonus = Tr.get("Activity20.Text.Bonus.0");
        } else if (nbBonus == 1) {
            strNbBonus = Tr.get("Activity20.Text.Bonus.1");
        } else {
            strNbBonus = string.Format(Tr.get("Activity20.Text.Bonus.+"), nbBonus);
        }

        string strNbMalus;
        if (nbMalus == 0) {
            strNbMalus = Tr.get("Activity20.Text.Malus.0");
        } else if (nbMalus == 1) {
            strNbMalus = Tr.get("Activity20.Text.Malus.1");
        } else {
            strNbMalus = string.Format(Tr.get("Activity20.Text.Malus.+"), nbMalus);
        }

        textRecap.text = string.Format(Tr.get("Activity20.Text.Recap"), strNbBonus, strNbMalus);
    }

    void BaseNodeSelectListener.onNodeSelect(BaseNode node) {

        selectedNode = node;

        //if it's a bonus node, show the detail screen
        if (node is NodeBonusType) {

            onNodeBonusTypeSelect(node as NodeBonusType);

        } else {

            //else if it's a zone node, unlock / activate / deactivate all the bonus nodes of the zone node
            onNodeZoneSelect(node as NodeZone);
        }
    }

    private void onNodeZoneSelect(NodeZone node) {

        HexacoinsWallet wallet = GameHelper.Instance.getHexacoinsWallet();

        switch (node.state) {

            case NodeZoneState.LOCKED:

                int nbHexacoinsToUnlock = node.nbHexacoinsToUnlock;

                //track button click
                TrackingManager.instance.prepareEvent(T.Event.HEXACOIN_PAY_START)
                               .add(T.Param.NB_HEXACOINS, nbHexacoinsToUnlock)
                               .add(T.Param.ORIGIN, getActivityName())
                               .add(T.Param.REASON, T.Value.PAY_REASON_UNLOCK_ZONE)
                               .track();

                if (wallet.canPayHexacoins(nbHexacoinsToUnlock)) {

                    node.setState(NodeZoneState.ACTIVATED);

                    graph.setDefaultNodesBonusStates(node);
                    graph.updateChildrenStatesAfterUnlock(node);
                    saveGraph();

                    //save now in order to save the paid hexacoins
                    gameManager.payHexacoins(nbHexacoinsToUnlock, getActivityName(), T.Value.PAY_REASON_UNLOCK_ZONE);

                    graph.updateGraphPercentagesHolder();
                    updateTextRecap();

                    //display speeches
                    onNodeZoneUnlock(node);

                    GameHelper.Instance.getAudioManager().playSound("Bonus.Unlock");

                    trackZoneUnlocked(node.tag);

                } else {

                    //propose to buy hexacoins if the player has not enough coins
                    push(new Activity22());
                }

                break;

            case NodeZoneState.ACTIVATED:

                node.setState(NodeZoneState.DEACTIVATED);
                graph.updateNodesBonusTypeWithZoneState();

                graph.updateGraphPercentagesHolder();
                updateTextRecap();

                hasChangedGraph = true;

                GameHelper.Instance.getAudioManager().playSound("Bonus.Deactivate");

                trackZoneDeactivated(node.tag);

                break;

            case NodeZoneState.DEACTIVATED:

                node.setState(NodeZoneState.ACTIVATED);
                graph.updateNodesBonusTypeWithZoneState();

                graph.updateGraphPercentagesHolder();
                updateTextRecap();

                hasChangedGraph = true;

                GameHelper.Instance.getAudioManager().playSound("Bonus.Activate");

                trackZoneActivated(node.tag);

                break;

            case NodeZoneState.DISABLED:

                //explain why the node is disabled
                GameHelper.Instance.getCharacterAnimator()
                          .show(this, true)
                          .enqueueTr("20.Disabled");

                break;
        }

    }

    /**
     * Display speeches then return if a speech has been triggered or not
     */
    protected virtual bool onNodeZoneUnlock(NodeZone node) {

        HashSet<NodeZone> nodes = graph.getNodesZone();

        if (nodes.Any(n => n.state == NodeZoneState.DISABLED) && !nodes.Any(n => n.state == NodeZoneState.LOCKED) &&
            !GameHelper.Instance.getUniqueDisplaySpeechesManager().hasTag("20.Blocked")) {

            //after unlocking all the first zones, display speech about next disabled zone and red lines
            GameHelper.Instance.getCharacterAnimator()
                      .show(this, true)
                      .enqueueTr("20.Blocked")
                      .enqueueUniqueDisplay("20.Blocked");

            return true;
        }

        if ((!nodes.Any(n => n.state == NodeZoneState.DISABLED || n.state == NodeZoneState.LOCKED)) &&
            !GameHelper.Instance.getUniqueDisplaySpeechesManager().hasTag("20.AllBonus")) {

            //if all groups unlocked, display congratulations
            GameHelper.Instance.getCharacterAnimator()
                      .show(this, true)
                      .enqueueTr("20.AllBonus")
                      .enqueue(new CharacterAnimationMove(CharacterRes.MOVE_SPIRAL))
                      .enqueue(new CharacterAnimationExpr(CharacterRes.EXPR_SUNGLASSES, 10))
                      .enqueueUniqueDisplay("20.AllBonus");

            return true;
        }

        return false;
    }

    private void onNodeBonusTypeSelect(NodeBonusType node) {

        BonusType bonusType = node.bonusType;

        BundlePush21 b = new BundlePush21 {
            graph = graph,
            selectedNode = node
        };

        BaseActivity a;
        if (!bonusType.isMalus) {
            a = new Activity21a();
        } else {
            a = new Activity21b();
        }

        push(a.setBundlePush(b));
    }

}