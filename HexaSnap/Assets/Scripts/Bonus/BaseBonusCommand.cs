/**
 * Hexa Snap
 * © Aurélien Lubecki 2019
 * All Rights Reserved
 */

using UnityEngine;


public abstract class BaseBonusCommand {


    public virtual string[] getAllMultipleTagSuffixes() {
        return null;
    }

    public virtual string getItemTagSuffix(ItemBonus item) {
        return null;
    }

    public virtual void onItemBonusSnapped(ItemBonus item) {//used for bonus row wipeout (to update row direction)

    }

    public virtual void onItemBonusUsed(ItemBonus item) {
        
    }

    public virtual object newSpecificBonusObject() {
        return null;
    }

    public virtual bool mustMoveItemBeforeDestroying(ItemBonus item) {
        return false;
    }

    public virtual ItemSnapPosition getImpactPos(Axis axis, ItemBonus item) {//used to know where the item must move (when stacked or chosen bonus)

        ItemSnapPosition pos = item.snapPosition;
        if (pos != null) {
            return pos;
        }

        return item.impactPos;
    }

    protected void playFX(string animName, float freezeTimeSec, ItemBonus itemBonus, Vector3 pos = new Vector3(), float angleDegrees = 0) {

        //stop time during the FX play
        TimeManager timeManager = itemBonus.activity.timeManager;

        string tag = "bonus_select_" + itemBonus.id;
        timeManager.pause(tag);

        Constants.playFX(animName, pos, angleDegrees);

        Async.call(freezeTimeSec, () => {

            timeManager.resume(tag);
        });

    }

}
