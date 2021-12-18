/**
 * Hexa Snap
 * © Aurélien Lubecki 2019
 * All Rights Reserved
 */

using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;


public class GoalCompletionBehavior : InGameModelBehavior, GoalCompletionListener {

    private static Texture imageBgType1;
    private static Texture imageBgType5;
    private static Texture imageBgType20;
    private static Texture imageBgType100;
    private static Texture imageBgBonus;
    private static Texture imageBgMalus;


    public GoalCompletion goalCompletion {
        get {
            return (GoalCompletion) model;
        }
    }

    public bool mustAnimateChanges = false;

    private Dictionary<LevelItemType, GameObject> subGoals = new Dictionary<LevelItemType, GameObject>();
    private Transform trBackground;


    protected override void onAwake() {
        base.onAwake();
        
        if (imageBgType1 == null) {
            imageBgType1 = GameHelper.Instance.loadTexture2DAsset(Constants.PATH_DESIGNS_OBJECTS + "Item.Hexagon.1");
            imageBgType5 = GameHelper.Instance.loadTexture2DAsset(Constants.PATH_DESIGNS_OBJECTS + "Item.Hexagon.5");
            imageBgType20 = GameHelper.Instance.loadTexture2DAsset(Constants.PATH_DESIGNS_OBJECTS + "Item.Hexagon.20");
            imageBgType100 = GameHelper.Instance.loadTexture2DAsset(Constants.PATH_DESIGNS_OBJECTS + "Item.Hexagon.100");
            imageBgBonus = GameHelper.Instance.loadTexture2DAsset(Constants.PATH_DESIGNS_OBJECTS + "Item.Hexagon.Bonus");
            imageBgMalus = GameHelper.Instance.loadTexture2DAsset(Constants.PATH_DESIGNS_OBJECTS + "Item.Hexagon.Malus");
        }

        trBackground = transform.Find("ImageBackground");

        //change the width of the goal container
        RectTransform rectTransform = GetComponent<RectTransform>();
        Vector2 containerSize = new Vector2(Constants.MAX_GOAL_ITEMS_FOR_LEVEL * 100, rectTransform.sizeDelta.y);
        rectTransform.sizeDelta = containerSize;
    }

    protected override void onInit() {
        base.onInit();

        updateGoalItems();
    }

    protected override void onDeinit() {
        base.onDeinit();

        subGoals.Clear();
    }

    void GoalCompletionListener.onGoalChange(GoalCompletion g) {

        updateGoalItems();
    }

    void GoalCompletionListener.onGoalCompletionChange(GoalCompletion g) {

        updateGoalItemsTexts();
    }

    private void updateGoalItems() {

        GameObjectPoolBehavior pool = GameHelper.Instance.getPool();

        //change sibling to avoid storing it to the pool
        int nbItems = transform.childCount;

        if (trBackground != null) {
            trBackground.SetAsLastSibling();
            nbItems--;
        }

        //remove all
        for (int i = 0 ; i < nbItems ; i++) {
            
            Transform t = transform.GetChild(0);
            if (t == trBackground) {
                throw new InvalidOperationException("Got the background transform and not a goal item");
            }

            pool.storeGoalItemGameObject(t.gameObject);
        }

        subGoals.Clear();

        //add all goal items
        List<LevelItemType> types = goalCompletion.getAvailableTypes();
        int nbTypes = types.Count;
        
        int pos = 0;
        foreach (LevelItemType type in types) {
            
            GameObject go = pool.pickGoalItemGameObject(transform, pos, nbTypes);
            go.GetComponentInChildren<RawImage>().texture = getBgImage(type);

            subGoals.Add(type, go);

            pos++;
        }
        
        updateGoalItemsTexts();
    }

    private Texture getBgImage(LevelItemType type) {

        switch (type) {
            case LevelItemType.Type1:
                return imageBgType1;

            case LevelItemType.Type5:
                return imageBgType5;

            case LevelItemType.Type20:
                return imageBgType20;

            case LevelItemType.Type100:
                return imageBgType100;

            case LevelItemType.Bonus:
                return imageBgBonus;

            case LevelItemType.Malus:
                return imageBgMalus;
        }

        throw new InvalidOperationException();
    }

    private void updateGoalItemsTexts() {

        List<LevelItemType> types = goalCompletion.getAvailableTypes();

        foreach (var e in subGoals) {

            LevelItemType type = e.Key;
            GameObject go = e.Value;

            int nbToRetrieve = goalCompletion.getRemainingNbItemsToFinish(type);

            Text textNbItems = go.GetComponentInChildren<Text>();

            var lastText = textNbItems.text;
            var currentText = "";

            if (nbToRetrieve <= 0) {
                
                currentText = "0";
                textNbItems.color = Constants.COLOR_GOAL_REACHED;

            } else {
                
                currentText = nbToRetrieve.ToString();
                textNbItems.color = Constants.COLOR_TITLE;
            }

            textNbItems.text = currentText;

            //animate changes
            if (mustAnimateChanges && !lastText.Equals(currentText)) {
                Constants.playAnimation(go.GetComponent<Animation>(), null, false);
            }
        }
    }

    public GameObject getNullableSubGoal(LevelItemType type) {

        GameObject go = null;
        subGoals.TryGetValue(type, out go);

        return go;
    }

}
