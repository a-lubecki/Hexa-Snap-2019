/**
 * Hexa Snap
 * © Aurélien Lubecki 2019
 * All Rights Reserved
 */

using UnityEngine;

public class UpgradesManager {

    private static readonly int HEIGHT_BIG = 720;
    private static readonly int HEIGHT_NORMAL = 550;
    private static readonly int HEIGHT_SMALL = 450;


    public readonly Graph graphArcade = new Graph(2900);
    public readonly Graph graphTimeAttack = new Graph(2200);


    public UpgradesManager() {

        createGraphArcade();
        createGraphTimeAttack();
    }

    private void createGraphArcade() {

        BonusManager bonusManager = GameHelper.Instance.getBonusManager();

        Graph graph = graphArcade;

        graph.addNodeZone(Graph.ROOT, new NodeZone(graph, "1", Vector2.zero, HEIGHT_BIG, -1, 4));//first unlock is free
        graph.addNodeZone("1B2", new NodeZone(graph, "2", new Vector2(-300, -700), HEIGHT_NORMAL, 10, 6, 0.4f, false));
        graph.addNodeZone("1M2", new NodeZone(graph, "3", new Vector2(300, -800), HEIGHT_NORMAL, 20, 6, 0.39f, false));
        graph.addNodeZone("1B3", new NodeZone(graph, "4", new Vector2(-250, -1400), HEIGHT_SMALL, 30, 8, 1, true));
        graph.addNodeZone("1M3", new NodeZone(graph, "5", new Vector2(250, -1600), HEIGHT_SMALL, 40, 8, 1, true));
        graph.addNodeZone("5M1", new NodeZone(graph, "6", new Vector2(0, -2200), HEIGHT_SMALL, 50, 8, 0.9f, true));

        graph.addNodesBonusType(
            "1",
            bonusManager.bonusTypeAdjacentWipeout,
            bonusManager.bonusTypeRowWipeout,
            bonusManager.bonusTypeSimilarWipeout,
            bonusManager.bonusTypeProliferation,
            bonusManager.bonusTypeInversion,
            bonusManager.bonusTypeErosion
        );

        graph.addNodesBonusType(
            "2",
            bonusManager.bonusTypeChoiceBonus,
            bonusManager.bonusTypeExtension,
            bonusManager.bonusTypeChoiceMalus,
            bonusManager.bonusTypeLimitation
        );

        graph.addNodesBonusType(
            "3",
            bonusManager.bonusTypeMultiplier,
            bonusManager.bonusTypeShortage,
            bonusManager.bonusTypeDivider,
            bonusManager.bonusTypeProfusion
        );

        graph.addNodesBonusType(
            "4",
            bonusManager.bonusTypeRandomBonus,
            bonusManager.bonusTypeRandomMalus
        );

        graph.addNodesBonusType(
            "5",
            bonusManager.bonusTypeSlowDown,
            bonusManager.bonusTypeSpeedUp
        );

        graph.addNodesBonusType(
            "6",
            bonusManager.bonusTypeProgression,
            bonusManager.bonusTypeRegression
        );

    }

    private void createGraphTimeAttack() {

        BonusManager bonusManager = GameHelper.Instance.getBonusManager();

        Graph graph = graphTimeAttack;

        graph.addNodeZone(Graph.ROOT, new NodeZone(graph, "1", new Vector2(-50, 0), HEIGHT_BIG, -1, 4));//first unlock is free
        graph.addNodeZone("1M1", new NodeZone(graph, "2", new Vector2(280, -600), HEIGHT_NORMAL, 10, 6, 0.3f, false));
        graph.addNodeZone("1M2", new NodeZone(graph, "3", new Vector2(-280, -850), HEIGHT_NORMAL, 20, 6, 1, true));
        graph.addNodeZone("3M2", new NodeZone(graph, "4", new Vector2(0, -1500), HEIGHT_SMALL, 30, 8, 1, true));

        graph.addNodesBonusType(
            "1",
            bonusManager.bonusType5SecondsMore,
            bonusManager.bonusTypeMultiplier,
            bonusManager.bonusType5SecondsLess,
            bonusManager.bonusTypeDivider
        );

        graph.addNodesBonusType(
            "2",
            bonusManager.bonusType10SecondsMore,
            bonusManager.bonusTypeExtension,
            bonusManager.bonusType10SecondsLess,
            bonusManager.bonusTypeLimitation
        );

        graph.addNodesBonusType(
            "3",
            bonusManager.bonusType20SecondsMore,
            bonusManager.bonusTypeSlowDown,
            bonusManager.bonusType20SecondsLess,
            bonusManager.bonusTypeSpeedUp
        );

        graph.addNodesBonusType(
            "4",
            bonusManager.bonusType30SecondsMore,
            bonusManager.bonusType30SecondsLess
        );

    }

}

