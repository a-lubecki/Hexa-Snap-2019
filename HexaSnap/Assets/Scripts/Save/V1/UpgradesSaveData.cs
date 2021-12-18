/**
 * Hexa Snap
 * © Aurélien Lubecki 2019
 * All Rights Reserved
 */

using System;

[Serializable]
public class UpgradesSaveData {

    protected UpgradeGraphSaveData graphArcadeData;
    protected UpgradeGraphSaveData graphTimeAttackData;

    public UpgradesSaveData(UpgradesManager upgradesManager) {

        graphArcadeData = new UpgradeGraphSaveData(upgradesManager.graphArcade);
        graphTimeAttackData = new UpgradeGraphSaveData(upgradesManager.graphTimeAttack);
    }

    public UpgradeGraphSaveData getGraphArcadeData() {
        return graphArcadeData;
    }

    public UpgradeGraphSaveData getGraphTimeAttack() {
        return graphTimeAttackData;
    }

}

