/**
 * Hexa Snap
 * © Aurélien Lubecki 2019
 * All Rights Reserved
 */

using System;
using UnityEngine;


public class GoalItemGhostBehavior : MonoBehaviour {


    private SpriteRenderer spriteRenderer;
    protected PositionInterpolator positionInterpolator;


    private void Awake() {

        spriteRenderer = GetComponent<SpriteRenderer>();

        positionInterpolator = new PositionInterpolator(transform);
    }

    private void Update() {

        if (isActiveAndEnabled) {
            positionInterpolator.update();
        }
    }

    private void OnDisable() {
        
        positionInterpolator.cancelInterpolation();
    }

    private void OnDestroy() {

        positionInterpolator.cancelInterpolation();
    }

    public void updateItem(Item item) {
        
        spriteRenderer.sprite = BaseModelBehavior.findModelBehavior<ItemBehavior>(item).getCurrentBackgroundSpriteHexagon();
        transform.localScale = new Vector3(0.5f, 0.5f, 0.5f);
    }

    public void animatePosition(PositionInterpolatorBundle bundle, Action<bool> completion = null) {
        
        if (!isActiveAndEnabled) {
            return;
        }

        positionInterpolator.setNextPosition(bundle, completion);
    }

}

