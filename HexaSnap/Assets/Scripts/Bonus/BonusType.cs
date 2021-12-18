/**
 * Hexa Snap
 * © Aurélien Lubecki 2019
 * All Rights Reserved
 */

using System;


public class BonusType {


    private string tag; //used to load images + translations + tracking

    public bool hasIcon { get; private set; }

    public string title { get; private set; }
    public string description { get; private set; }

    public bool isMalus { get; private set; }
    public bool isInstant { get; private set; }

    public bool canBeRegisteredForGoals { get; private set; }

    public bool isDirectionBased { get; private set; }

    private BaseBonusCommand bonusCommand; //the behavior of the bonus in-game


    public BonusType(bool isMalus) {

        //void bonus type
        if (isMalus) {
            this.tag = "VOID_MALUS";
        } else {
            this.tag = "VOID_BONUS";
        }

        this.hasIcon = false;

        this.isMalus = isMalus;
        this.isInstant = true;

        canBeRegisteredForGoals = true;
    }

    public BonusType(string tag, bool isMalus, bool isInstant, bool canBeRegisteredForGoals,
        bool isDirectionBased, BaseBonusCommand bonusCommand) {

        if (string.IsNullOrEmpty(tag)) {
            throw new ArgumentException();
        }

        this.tag = tag;

        this.hasIcon = true;

        this.title = Tr.get("Bonus." + tag + ".title");
        this.description = Tr.get("Bonus." + tag + ".description");

        this.isMalus = isMalus;
        this.isInstant = isInstant;

        this.canBeRegisteredForGoals = canBeRegisteredForGoals;

        this.isDirectionBased = isDirectionBased;

        this.bonusCommand = bonusCommand;
    }

    public string getTag(ItemBonus item) {

        if (bonusCommand == null) {
            return tag;
        }

        string suffix = bonusCommand.getItemTagSuffix(item);
        if (suffix == null) {
            return tag;
        }

        return tag + "." + suffix;
    }

    public string[] getAllTags() {

        if (bonusCommand == null) {
            return new string[] { tag };
        }

        string[] tagsSuffixes = bonusCommand.getAllMultipleTagSuffixes();
        if (tagsSuffixes == null) {
            return new string[] { tag };
        }

        string[] res = new string[tagsSuffixes.Length];

        int i = 0;
        foreach (string suffix in tagsSuffixes) {
            res[i++] = tag + "." + suffix;
        }

        return res;
    }

    public void onItemBonusSnapped(ItemBonus item) {

        if (bonusCommand == null) {
            return;
        }

        bonusCommand.onItemBonusSnapped(item);
    }

    public void onItemBonusUsed(ItemBonus item) {

        if (bonusCommand == null) {
            return;
        }

        bonusCommand.onItemBonusUsed(item);
    }

    public object newSpecificBonusObject() {

        if (bonusCommand == null) {
            return null;
        }

        return bonusCommand.newSpecificBonusObject();
    }

    public bool mustMoveItemToImpactPos(ItemBonus item) {

        if (bonusCommand == null) {
            return false;
        }

        return bonusCommand.mustMoveItemBeforeDestroying(item);
    }

    public ItemSnapPosition getImpactPos(Axis axis, ItemBonus itemBonus) {

        if (bonusCommand == null) {
            throw new InvalidOperationException();
        }

        return bonusCommand.getImpactPos(axis, itemBonus);
    }

}

