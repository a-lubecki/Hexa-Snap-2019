/**
 * Hexa Snap
 * © Aurélien Lubecki 2019
 * All Rights Reserved
 */

using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Facebook.Unity;


public class FirebaseFunctionsManager {


    public static bool isSendingDisabled;

    //singleton
    public static readonly FirebaseFunctionsManager instance = new FirebaseFunctionsManager();

    private FirebaseFunctionsManager() {
        queue = new FirebaseFunctionsQueue(this);
    }


    private readonly FirebaseFunctionsQueue queue;

    private GameManager gameManager {
        get {
            return GameHelper.Instance.getGameManager();
        }
    }

    private string appOrigin;
    private List<string> appSignatures;


    private void sendHttpsCall(FirebaseFunctionCall call) {
        
        if (!FirebaseInitManager.instance.hasResolvedDependencies()) {
            //not ready
            call.resolveCallCallback(new InvalidOperationException());
            return;
        }

        if (isSendingDisabled) {
            //not available, nothing to send
            call.resolveCallCallback();
            return;
        }

        if (call.needsToBeLogged && !LoginManager.Instance.isLoggedInFacebook()) {
            //not logged, nothing to send
            call.resolveCallCallback();
            return;
        }

        //prepare sending
        queue.prepareCall(call);
    }

    private string getAppOrigin() {

        if (appOrigin == null) {
            #if UNITY_EDITOR
            appOrigin = "test";
            #else
            if (Debug.isDebugBuild) {
                appOrigin = "test";
            } else {
                appOrigin = NativeCallsManager.getAppOrigin();
            }
            #endif
        }

        return appOrigin;
    }

    private List<string> getAppSignatures() {

        if (appSignatures == null) {

            string[] s;

            #if UNITY_EDITOR
            s = new string[] { "test" };
            #else
            if (Debug.isDebugBuild) {
                s = new string[] { "test" };
            } else {
                s = NativeCallsManager.getAppSignatures();
            }
            #endif

            if (s != null) {
                appSignatures = new List<string>(s);
            }
        }

        return appSignatures;
    }

    private Dictionary<string, object> newDefaultArgs() {

        Dictionary<string, object> res = new Dictionary<string, object>();

        string platform;
        #if UNITY_ANDROID
        platform = "android";
        #elif UNITY_IPHONE
        platform = "ios";
        #else
        platform = "unknown";
        #endif
        res["platform"] = platform;

        string origin = getAppOrigin();
        if (!string.IsNullOrEmpty(origin)) {
            res["origin"] = origin;
        }

        List<string> signatures = getAppSignatures();
        if (signatures != null && signatures.Count > 0) {
            res["signatures"] = String.Join(",", signatures);
        }

        string userId = LoginManager.Instance.getFirebaseUserId();
        if (!string.IsNullOrEmpty(userId)) {
            res["userId"] = userId;
        }

        return res;
    }

    public void retrieveTimeShift(Action<int> onDone, Action<Exception> onError) {

        if (onDone == null) {
            //the result won't be handled, do nothing
            return;
        }

        var args = newDefaultArgs();
        args["dateTimeSec"] = DateTime.Now.ToUniversalTime().TotalSeconds();

        //no need to be logged for this method
        sendHttpsCall(new FirebaseFunctionCall(
            true,
            FirebaseFunctionCallMergeStrategy.REMOVE_PREVIOUS,
            "getTimeShift",
            false,
            args,
            (data) => {
                    
                int timeShiftSec;
                if (extractInt(data, "timeShiftSec", out timeShiftSec)) {
                    onDone(timeShiftSec);
                }
            },
            null,
            onError
        ));
    }

    public void updateResultArcade(int score, int level) {

        var args = newDefaultArgs();
        args["score"] = score;
        args["level"] = level;

        sendHttpsCall(new FirebaseFunctionCall(
            false,
            FirebaseFunctionCallMergeStrategy.KEEP_PREVIOUS,
            "updateResultArcade",
            true,
            args,
            updateArcadeScore,
            null,
            null
        ));
    }

    public void updateResultTimeAttack(int score, float timeSec) {

        var args = newDefaultArgs();
        args["score"] = score;
        args["timeSec"] = timeSec;

        sendHttpsCall(new FirebaseFunctionCall(
            false,
            FirebaseFunctionCallMergeStrategy.KEEP_PREVIOUS,
            "updateResultTimeAttack",
            true,
            args,
            updateTimeAttackScore,
            null,
            null
        ));
    }

    public void updateWallet() {

        var args = newDefaultArgs();
        args["lastRemoteNbHexacoins"] = gameManager.hexacoinsWallet.lastRemoteNbHexacoins;
        args["nbHexacoins"] = gameManager.hexacoinsWallet.nbHexacoins;

        sendHttpsCall(new FirebaseFunctionCall(
            false,
            FirebaseFunctionCallMergeStrategy.REMOVE_PREVIOUS,
            "updateWallet",
            true,
            args,
            (data) => updateNbHexacoins(data, args),
            null,
            null
        ));
    }

    public void updateGraphArcade() {

        updateGraph(
            "updateGraphArcade",
            gameManager.upgradesManager.graphArcade
        );
    }

    public void updateGraphTimeAttack() {

        updateGraph(
            "updateGraphTimeAttack",
            gameManager.upgradesManager.graphTimeAttack
        );
    }

    private void updateGraph(string name, Graph graph) {
        
        var args = newDefaultArgs();
        args["zones"] = graph.getZonesMask();
        args["unlockedNodes"] = graph.getUnlockedNodesMask();
        args["activeNodes"] = graph.getActiveNodesMask();

        sendHttpsCall(new FirebaseFunctionCall(
            false,
            FirebaseFunctionCallMergeStrategy.REMOVE_PREVIOUS,
            name,
            true,
            args,
            (data) => updateGraph(graph, data, args),
            null,
            null
        ));
    }

    public void addInAppPurchase(Purchase purchase, Action onDone, Action<Exception> onError) {
        
        var args = newDefaultArgs();
        args["purchaserId"] = purchase.purchaserId;
        args["transactionId"] = purchase.transactionId;
        args["purchaseDate"] = purchase.purchaseDate.TotalSeconds();
        args["purchaseTag"] = purchase.purchaseTag;
        args["purchasePrice"] = purchase.purchasePrice;
        args["purchaseNbHexacoins"] = purchase.purchaseNbHexacoins;
        args["purchaseIsRemovingAds"] = purchase.purchaseIsRemovingAds;
        args["receipt"] = purchase.receipt;
        args["nbHexacoins"] = gameManager.hexacoinsWallet.nbHexacoins;
        args["hasRemovedAds"] = gameManager.hasRemovedAds;

        sendHttpsCall(new FirebaseFunctionCall(
            true,
            FirebaseFunctionCallMergeStrategy.REMOVE_PREVIOUS,
            "addInAppPurchase",
            false,
            args,
            (data) => {

                int nbHexacoins;
                if (extractInt(data, "nbHexacoins", out nbHexacoins)) {

                    //update remote hexacoins only if the user has an account so that the user can keep its hexacoins when logging in
                    if (args.ContainsKey("userId")) {
                        gameManager.hexacoinsWallet.updateLastRemoteHexacoins(nbHexacoins);
                    }

                    //IAP tracking for purchase
                    gameManager.updateHexacoins(nbHexacoins, "Activity23", T.Value.EARN_REASON_IAP);
                }

                var hasRemovedAds = false;
                if (extractBool(data, "hasRemovedAds", out hasRemovedAds)) {

                    if (hasRemovedAds) {
                        gameManager.removeAds();
                    }
                }
            },
            onDone,
            onError
        ));
    }

    public void deleteAccount(Action onDone, Action<Exception> onError) {

        var args = newDefaultArgs();

        sendHttpsCall(new FirebaseFunctionCall(
            true,
            FirebaseFunctionCallMergeStrategy.REMOVE_PREVIOUS,
            "deleteAccount",
            true,
            args,
            null,
            onDone,
            onError
        ));
    }

    /*
     * synchronize(userId, hasRemovedAds, arcade, timeAttack) : (nbHexacoins, hasRemovedAds, arcade, timeAttack)
        // body :
        {
            "data" : {
                "userId" : "azertyuiop",
                "platform" : "android",
                "origin" : "azertyuiop",
                "signatures" : "azertyuiop,qsdfghjklm",
                "player": {
                    "lastRemoteNbHexacoins" : 0,
                    "nbHexacoins" : 0,
                    "hasRemovedAds" : false,
                    "arcade" : {
                        "maxScore" : 0,
                        "maxLevel" : 0
                        "graph" : {
                            "zones" : 0,
                            "unlockedNodes" : [],
                            "activeNodes" : []
                        }
                    },
                    "timeAttack" : {
                        "maxScore" : 0,
                        "maxTimeSec" : 0
                        "graph" : {
                            "zones" : 0,
                            "unlockedNodes" : [],
                            "activeNodes" : []
                        }
                    },
                    "pendingPurchases" : [
                        "123456",
                        "678910",
                        "111213"
                    ]
                }
            }
        }
     */
    public void synchronize(Action<bool> onDone, Action<Exception> onError) {

        var graphArcade = gameManager.upgradesManager.graphArcade;
        var graphTimeAttack = gameManager.upgradesManager.graphTimeAttack;

        //prepare pending purchases string array to send
        var pendingPurchases = gameManager.purchasesManager.getPendingPurchases().Select((p) => p.transactionId).ToArray();

        var args = newDefaultArgs();
        args["player"] = new Dictionary<string, object> {
            { "lastRemoteNbHexacoins", gameManager.hexacoinsWallet.lastRemoteNbHexacoins },
            { "nbHexacoins", gameManager.hexacoinsWallet.nbHexacoins },
            { "hasRemovedAds", gameManager.hasRemovedAds },
            { "arcade", new Dictionary<string, object> {
                    { "maxScore", gameManager.maxArcadeScore },
                    { "maxLevel", gameManager.maxArcadeLevel },
                    { "graph", new Dictionary<string, object> {
                            { "zones", graphArcade.getZonesMask() },
                            { "unlockedNodes", graphArcade.getUnlockedNodesMask() },
                            { "activeNodes", graphArcade.getActiveNodesMask() }
                    }}
            }},
            { "timeAttack", new Dictionary<string, object> {
                    { "maxScore", gameManager.maxTimeAttackScore },
                    { "maxTimeSec", gameManager.maxTimeAttackTimeSec },
                    { "graph", new Dictionary<string, object> {
                            { "zones", graphTimeAttack.getZonesMask() },
                            { "unlockedNodes", graphTimeAttack.getUnlockedNodesMask() },
                            { "activeNodes", graphTimeAttack.getActiveNodesMask() }
                    }}
            }},
            { "pendingPurchases", pendingPurchases }
        };

        sendHttpsCall(new FirebaseFunctionCall(
            true,
            FirebaseFunctionCallMergeStrategy.REMOVE_PREVIOUS,
            "synchronize",
            true,
            args,
            data => {
                
                var playerData = endSynchronize(data);

                bool hasBeenCreated = false;
                extractBool(playerData, "hasBeenCreated", out hasBeenCreated);

                onDone(hasBeenCreated);
            },
            null,
            onError
        ));
    }

    private Dictionary<object, object> endSynchronize(Dictionary<object, object> data) {
        
        Dictionary<object, object> player;

        if (!extractDictionary(data, "player", out player)) {
            return null;
        }

        updateNbHexacoins(player);

        var hasRemovedAds = false;
        if (extractBool(player, "hasRemovedAds", out hasRemovedAds)) {

            if (hasRemovedAds) {
                gameManager.removeAds();
            }
        }

        Dictionary<object, object> modeArcade;
        Dictionary<object, object> graphArcade = null;

        if (extractDictionary(player, "arcade", out modeArcade)) {

            updateArcadeScore(modeArcade);

            extractDictionary(modeArcade, "graph", out graphArcade);
        }

        Dictionary<object, object> modeTimeAttack;
        Dictionary<object, object> graphTimeAttack = null;

        if (extractDictionary(player, "timeAttack", out modeTimeAttack)) {

            updateTimeAttackScore(modeTimeAttack);

            extractDictionary(modeTimeAttack, "graph", out graphTimeAttack);
        }

        var upgradesManager = GameHelper.Instance.getUpgradesManager();

        updateGraph(upgradesManager.graphArcade, graphArcade);
        updateGraph(upgradesManager.graphTimeAttack, graphTimeAttack);

        //remove all the purchases that are in the response because they are saved on the server
        string[] savedPurchases;
        if (extractStringArray(data, "savedPurchases", out savedPurchases)) {

            foreach (var transactionId in savedPurchases) {
                gameManager.purchasesManager.removePendingPurchase(transactionId);
            }

            SaveManager.Instance.saveShopItems();
        }

        return player;
    }


    private void updateNbHexacoins(Dictionary<object, object> player, Dictionary<string, object> sentArgs = null) {

        if (player == null) {
            return;
        }

        int nbHexacoins;
        if (!extractInt(player, "nbHexacoins", out nbHexacoins)) {
            return;
        }

        gameManager.hexacoinsWallet.updateLastRemoteHexacoins(nbHexacoins);

        if (sentArgs != null) {
            
            var nbHexacoinsSent = sentArgs["nbHexacoins"] as int? ?? 0;
            if (nbHexacoins == nbHexacoinsSent) {

                //save the wallet for changed last remote hexacoins
                SaveManager.Instance.saveHexacoinsWallet();

                //assign data only if the nb hexacoins are different between sent and received
                //it avoids reseting the nb if the player earn some hexacoins between the data sending and receipt
                return;
            }
        }

        gameManager.updateHexacoins(nbHexacoins);
    }

    private void updateArcadeScore(Dictionary<object, object> modeArcade) {

        if (modeArcade == null) {
            return;
        }

        int maxScore;
        if (extractInt(modeArcade, "maxScore", out maxScore)) {
            gameManager.updateMaxArcadeScore(maxScore);
        }

        int maxLevel;
        if (extractInt(modeArcade, "maxLevel", out maxLevel)) {
            gameManager.updateMaxArcadeLevel(maxLevel);
        }

        SaveManager.Instance.saveBestScores();
    }

    private void updateTimeAttackScore(Dictionary<object, object> modeTimeAttack) {

        if (modeTimeAttack == null) {
            return;
        }

        int maxScore;
        if (extractInt(modeTimeAttack, "maxScore", out maxScore)) {
            gameManager.updateMaxTimeAttackScore(maxScore);
        }

        float maxTimeSec;
        if (extractFloat(modeTimeAttack, "maxTimeSec", out maxTimeSec)) {
            gameManager.updateMaxTimeAttackTimeSec(maxTimeSec);
        }

        SaveManager.Instance.saveBestScores();
    }

    private void updateGraph(Graph currentGraph, Dictionary<object, object> graph, Dictionary<string, object> sentArgs = null) {

        if (graph == null || graph.Count <= 0) {
            return;
        }

        int zones;
        extractInt(graph, "zones", out zones);

        int[] unlockedNodes;
        extractIntArray(graph, "unlockedNodes", out unlockedNodes);

        if (unlockedNodes == null) {
            unlockedNodes = new int[0];
        }

        int[] activeNodes;
        extractIntArray(graph, "activeNodes", out activeNodes);

        if (activeNodes == null) {
            activeNodes = new int[0];
        }

        if (sentArgs != null) {

            var zonesSent = sentArgs["zones"] as int? ?? 0;
            var unlockedNodesSent = sentArgs["unlockedNodes"] as int[] ?? new int[0];
            var activeNodesSent = sentArgs["activeNodes"] as int[] ?? new int[0];

            if (zones == zonesSent && 
                unlockedNodes.SequenceEqual(unlockedNodesSent) &&
                activeNodes.SequenceEqual(activeNodesSent)) {
                //assign data only if the graph is different between sent and received
                //it avoids reseting the graph if the player unlock nodes between the data sending and receipt
                return;
            }
        }

        gameManager.initGraph(currentGraph, new UpgradeGraphSaveData(
            zones,
            unlockedNodes,
            activeNodes
        ));

        SaveManager.Instance.saveGraphs();
    }

    private bool extractBool<K>(Dictionary<K, object> data, K key, out bool valueToAssign) {

        valueToAssign = false;

        if (data == null) {
            return false;
        }

        if (!data.ContainsKey(key)) {
            return false;
        }

        var value = data[key] as bool?;

        if (value == null || !value.HasValue) {
            return false;
        }

        valueToAssign = value.Value;

        return true;
    }

    private bool extractNumber<K>(Dictionary<K, object> data, K key, out long valueToAssign) {

        valueToAssign = 0;

        if (data == null) {
            return false;
        }

        if (!data.ContainsKey(key)) {
            return false;
        }

        var value = data[key] as long?;

        if (value == null || !value.HasValue) {
            return false;
        }

        valueToAssign = value.Value;

        return true;
    }

    private bool extractInt<K>(Dictionary<K, object> data, K key, out int valueToAssign) {

        valueToAssign = 0;

        long value = 0;

        if (!extractNumber(data, key, out value)) {
            return false;
        }

        valueToAssign = (int)value;

        return true;
    }

    private bool extractFloat<K>(Dictionary<K, object> data, K key, out float valueToAssign) {

        valueToAssign = 0;

        long value = 0;

        if (!extractNumber(data, key, out value)) {
            return false;
        }

        valueToAssign = (float)value;

        return true;
    }

    private bool extractString<K>(Dictionary<K, object> data, K key, out string valueToAssign) {

        valueToAssign = null;

        if (data == null) {
            return false;
        }

        if (!data.ContainsKey(key)) {
            return false;
        }

        var value = data[key] as string;

        if (value == null) {
            return false;
        }

        valueToAssign = value;

        return true;
    }

    private bool extractDictionary<K>(Dictionary<K, object> data, K key, out Dictionary<object, object> dictionaryToAssign) {

        dictionaryToAssign = null;

        if (!data.ContainsKey(key)) {
            return false;
        }

        dictionaryToAssign = data[key] as Dictionary<object, object>;

        return (dictionaryToAssign != null);
    }

    private bool extractIntArray<K>(Dictionary<K, object> data, K key, out int[] arrayToAssign) {

        return extractArray(
            data,
            key,
            out arrayToAssign,
            (value) => {
                return (int)(long)value;
            }
        );
    }

    private bool extractStringArray<K>(Dictionary<K, object> data, K key, out string[] arrayToAssign) {

        return extractArray(
            data,
            key,
            out arrayToAssign,
            (value) => {
                return (string)value;
            }
        );
    }

    private bool extractArray<K, T>(Dictionary<K, object> data, K key, out T[] arrayToAssign, Func<object, T> convertValue) {

        arrayToAssign = null;

        if (data == null) {
            return false;
        }

        if (!data.ContainsKey(key)) {
            return false;
        }

        var list = data[key] as List<object>;

        if (list == null) {
            return false;
        }

        foreach (object val in list) {

            //types of every elem must be correct before assigning value
            if (!(val is long)) {
                return false;
            }
        }

        arrayToAssign = new T[list.Count];

        for (int i = 0 ; i < list.Count ; i++) {

            arrayToAssign[i] = convertValue(list[i]);
        }

        return true;
    }

}

