/**
 * Hexa Snap
 * © Aurélien Lubecki 2019
 * All Rights Reserved
 */

#if UNITY_EDITOR

using UnityEngine;
using UnityEditor;
using System;
using System.Collections.Generic;
using System.IO;
using Firebase.Auth;
using Firebase.Extensions;


[InitializeOnLoad]
public class HexaSnapEditor {


    static HexaSnapEditor() {

        //init all
        foreach (string name in FIRST_MENU_NAMES) {

            if (EditorPrefs.GetBool(name)) {

                //delay call in order to wait for the menu populating
                EditorApplication.delayCall += () => {
                    Menu.SetChecked(name, true);
                };
                break;
            }
        }
    }


    public const string PATH = "Hexa Snap/";
    public const string PATH_LANGUAGE = PATH + "Language/";
    public const string PATH_FIRST = PATH + "First activity/";
    public const string PATH_FILE_STORAGE = PATH + "File Storage/";
    public const string PATH_FILE_TEST = PATH + "Test/";

    public const string FIRST_MENU_NAME_S = PATH_FIRST + "ActivityS";
    public const string FIRST_MENU_NAME_0 = PATH_FIRST + "Activity0";
    public const string FIRST_MENU_NAME_1a = PATH_FIRST + "Activity1a";
    public const string FIRST_MENU_NAME_10a = PATH_FIRST + "Activity10a";
    public const string FIRST_MENU_NAME_10b = PATH_FIRST + "Activity10b";
    public const string FIRST_MENU_NAME_20a = PATH_FIRST + "Activity20a";


    private static string LANGUAGE_FIELD_NAME = "newLanguage";

    [MenuItem(PATH_LANGUAGE + "English")]
    private static void changeLanguageEnglish() {

        EditorPrefs.SetString(LANGUAGE_FIELD_NAME, "en");
    }

    [MenuItem(PATH_LANGUAGE + "French")]
    private static void changeLanguageFrench() {

        EditorPrefs.SetString(LANGUAGE_FIELD_NAME, "fr");
    }

    public static String getLanguageToChange() {

        return EditorPrefs.GetString(LANGUAGE_FIELD_NAME);
    }

    public static void resetLanguageToChange() {

        EditorPrefs.DeleteKey(LANGUAGE_FIELD_NAME);
    }


    public static readonly string[] FIRST_MENU_NAMES = new string[] {
        FIRST_MENU_NAME_S,
        FIRST_MENU_NAME_0,
        FIRST_MENU_NAME_1a,
        FIRST_MENU_NAME_10a,
        FIRST_MENU_NAME_10b,
        FIRST_MENU_NAME_20a
    };


    [MenuItem(FIRST_MENU_NAME_S)]
    private static void newMenuS() {

        check(
            FIRST_MENU_NAMES,
            FIRST_MENU_NAME_S
        );
    }

    [MenuItem(FIRST_MENU_NAME_0)]
    private static void newMenu0() {

        check(
            FIRST_MENU_NAMES,
            FIRST_MENU_NAME_0
        );
    }

    [MenuItem(FIRST_MENU_NAME_1a)]
    private static void newMenu1a() {

        check(
            FIRST_MENU_NAMES,
            FIRST_MENU_NAME_1a
        );
    }

    [MenuItem(FIRST_MENU_NAME_10a)]
    private static void newMenu10a() {

        check(
            FIRST_MENU_NAMES,
            FIRST_MENU_NAME_10a
        );
    }

    [MenuItem(FIRST_MENU_NAME_10b)]
    private static void newMenu10b() {

        check(
            FIRST_MENU_NAMES,
            FIRST_MENU_NAME_10b
        );
    }

    [MenuItem(FIRST_MENU_NAME_20a)]
    private static void newMenu20a() {

        check(
            FIRST_MENU_NAMES,
            FIRST_MENU_NAME_20a
        );
    }


    private static void check(string[] menuNames, string name) {

        for (int i = 0; i < menuNames.Length; i++) {

            string n = menuNames[i];

            bool isChecked = (n.Equals(name));

            Menu.SetChecked(n, isChecked);
            EditorPrefs.SetBool(n, isChecked);
        }

    }

    public static BaseActivity newCheckedActivity() {

        string chosenName = null;

        foreach (string name in FIRST_MENU_NAMES) {

            if (EditorPrefs.GetBool(name)) {
                chosenName = name;
                break;
            }
        }

        if (chosenName != null) {

            if (chosenName == FIRST_MENU_NAME_S) {
                return new ActivityS();
            }

            if (chosenName == FIRST_MENU_NAME_0) {
                return new Activity0();
            }

            if (chosenName == FIRST_MENU_NAME_1a) {
                return new Activity1a();
            }

            if (chosenName == FIRST_MENU_NAME_10a) {

                BundlePushE10a b = new BundlePushE10a {
                    level = 1
                };

                return new Activity10a().setBundlePush(b);
            }

            if (chosenName == FIRST_MENU_NAME_10b) {
                return new Activity10b();
            }

            if (chosenName == FIRST_MENU_NAME_20a) {
                return new Activity20a();
            }

        }

        //default activity
        return new Activity1a();
    }


    [MenuItem(PATH_FILE_STORAGE + "Clear")]
    private static void newMenuClear() {

        GameSaverLocal.instance.deleteSave();
    }

    [MenuItem(PATH_FILE_STORAGE + "Add 1 Hexacoin")]
    private static void newMenuAddH1() {

        addHexacoins(1);
    }

    [MenuItem(PATH_FILE_STORAGE + "Add 10 Hexacoins")]
    private static void newMenuAddH10() {

        addHexacoins(10);
    }

    [MenuItem(PATH_FILE_STORAGE + "Add 100 Hexacoins")]
    private static void newMenuAddH100() {

        addHexacoins(100);
    }

    [MenuItem(PATH_FILE_STORAGE + "Add 1 Level")]
    private static void newMenuAddL1() {

        addLevels(1);
    }

    [MenuItem(PATH_FILE_STORAGE + "Add 10 Levels")]
    private static void newMenuAddL10() {

        addLevels(10);
    }

    private static string HEXACOINS_FIELD_NAME = "newHexacoins";

    public static void addHexacoins(int nb) {

        int nbHexacoins = getHexacoinsToAdd();
        nbHexacoins += nb;

        EditorPrefs.SetInt(HEXACOINS_FIELD_NAME, nbHexacoins);
    }

    public static int getHexacoinsToAdd() {

        return EditorPrefs.GetInt(HEXACOINS_FIELD_NAME, 0);
    }

    public static void resetHexacoinsToAdd() {

        EditorPrefs.SetInt(HEXACOINS_FIELD_NAME, 0);
    }


    private static string LEVELS_FIELD_NAMES = "newLevels";

    public static void addLevels(int nb) {

        int nbLevels = getLevelsToAdd();
        nbLevels += nb;

        EditorPrefs.SetInt(LEVELS_FIELD_NAMES, nbLevels);
    }

    public static int getLevelsToAdd() {

        return EditorPrefs.GetInt(LEVELS_FIELD_NAMES, 0);
    }

    public static void resetLevelsToAdd() {

        EditorPrefs.SetInt(LEVELS_FIELD_NAMES, 0);
    }


    private static void endCall(Dictionary<object, object> response) {
        Debug.Log("End call : " + response);
    }

    private static void onDone() {
        Debug.Log("Call DONE");
    }

    private static void onError(Exception e) {
        Debug.Log("Call ERROR : " + e);
    }

    [MenuItem(PATH_FILE_TEST + "Login")]
    private static void login() {

        FirebaseInitManager.instance.init(null);

        FirebaseAuth.DefaultInstance.SignOut();

        var credential = FacebookAuthProvider.GetCredential("XXXXXX");
        FirebaseAuth.DefaultInstance.SignInAndRetrieveDataWithCredentialAsync(credential).ContinueWithOnMainThread(task => {

            Debug.Log("Firebase login completed : " + task?.IsCompleted);
        });
    }

    private static Dictionary<string, object> newArgs(params object[] otherArgs) {

        var res = new Dictionary<string, object> {
            { "platform", "editor" },
            { "origin", "editor" },
            { "signatures", "editor" },
            { "userId", "XXXXXXX" }
        };

        if (otherArgs != null) {
            var length = otherArgs.Length;
            for (int i = 0; i < length; i += 2) {
                res[otherArgs[i] as string] = otherArgs[i + 1];
            }
        }

        return res;
    }

    private static void call(string functionName, Dictionary<string, object> args) {

        new FirebaseFunctionCall(
            false,
            FirebaseFunctionCallMergeStrategy.KEEP_PREVIOUS,
            functionName,
            false,
            args,
            endCall,
            onDone,
            onError
        ).processCall(null);
    }

    [MenuItem(PATH_FILE_TEST + "getTimeShift/❌ void")]
    private static void getTimeShift_void() {

        call("getTimeShift", new Dictionary<string, object>());
    }

    [MenuItem(PATH_FILE_TEST + "getTimeShift/❌ default args")]
    private static void getTimeShift_default() {

        call("getTimeShift", newArgs());
    }

    [MenuItem(PATH_FILE_TEST + "getTimeShift/❌ bad")]
    private static void getTimeShift_bad() {

        call("getTimeShift", newArgs(
            "dateTimeSec", new Dictionary<object, object>()
        ));
    }

    [MenuItem(PATH_FILE_TEST + "getTimeShift/✅ 0 sec")]
    private static void getTimeShift_0() {

        call("getTimeShift", newArgs(
            "dateTimeSec", DateTimeOffset.Now.ToUnixTimeSeconds()
        ));
    }

    [MenuItem(PATH_FILE_TEST + "getTimeShift/✅ 3600 sec")]
    private static void getTimeShift_3600() {

        call("getTimeShift", newArgs(
            "dateTimeSec", DateTimeOffset.Now.AddHours(1).ToUnixTimeSeconds()
        ));
    }

    [MenuItem(PATH_FILE_TEST + "getTimeShift/✅ -3600 sec")]
    private static void getTimeShift_m3600() {

        call("getTimeShift", newArgs(
            "dateTimeSec", DateTimeOffset.Now.AddHours(-1).ToUnixTimeSeconds()
        ));
    }

    [MenuItem(PATH_FILE_TEST + "updateResultArcade/❌ bad")]
    private static void updateResultArcade_bad() {

        call("updateResultArcade", newArgs(
            "score", new Dictionary<object, object>(),
            "level", new Dictionary<object, object>()
        ));
    }

    [MenuItem(PATH_FILE_TEST + "updateResultArcade/❌ score 0 + level 0")]
    private static void updateResultArcade_s0_l0() {

        call("updateResultArcade", newArgs(
            "score", 0,
            "level", 0
        ));
    }

    [MenuItem(PATH_FILE_TEST + "updateResultArcade/❌ score -100 + level 1")]
    private static void updateResultArcade_sm100_l1() {

        call("updateResultArcade", newArgs(
            "score", -100,
            "level", 1
        ));
    }

    [MenuItem(PATH_FILE_TEST + "updateResultArcade/✅ score 0 + level 1")]
    private static void updateResultArcade_s0_l1() {

        call("updateResultArcade", newArgs(
            "score", 0,
            "level", 1
        ));
    }

    [MenuItem(PATH_FILE_TEST + "updateResultArcade/✅ score 100 + level 1")]
    private static void updateResultArcade_s100_l1() {

        call("updateResultArcade", newArgs(
            "score", 100,
            "level", 1
        ));
    }

    [MenuItem(PATH_FILE_TEST + "updateResultArcade/✅ score 200 + level 20")]
    private static void updateResultArcade_s200_l20() {

        call("updateResultArcade", newArgs(
            "score", 200,
            "level", 20
        ));
    }

    [MenuItem(PATH_FILE_TEST + "updateResultTimeAttack/❌ bad")]
    private static void updateResultTimeAttack_bad() {

        call("updateResultTimeAttack", newArgs(
            "score", new Dictionary<object, object>(),
            "timeSec", new Dictionary<object, object>()
        ));
    }

    [MenuItem(PATH_FILE_TEST + "updateResultTimeAttack/❌ score 0 + timeSec -10")]
    private static void updateResultTimeAttack_s0_tm10() {

        call("updateResultTimeAttack", newArgs(
            "score", 0,
            "timeSec", -10
        ));
    }

    [MenuItem(PATH_FILE_TEST + "updateResultTimeAttack/❌ score -100 + timeSec 0")]
    private static void updateResultTimeAttack_sm100_t0() {

        call("updateResultTimeAttack", newArgs(
            "score", -100,
            "timeSec", 0
        ));
    }

    [MenuItem(PATH_FILE_TEST + "updateResultTimeAttack/✅ score 0 + timeSec 0")]
    private static void updateResultTimeAttack_s0_t0() {

        call("updateResultTimeAttack", newArgs(
            "score", 0,
            "timeSec", 0
        ));
    }

    [MenuItem(PATH_FILE_TEST + "updateResultTimeAttack/✅ score 100 + timeSec 0")]
    private static void updateResultTimeAttack_s100_t0() {

        call("updateResultTimeAttack", newArgs(
            "score", 100,
            "timeSec", 0
        ));
    }

    [MenuItem(PATH_FILE_TEST + "updateResultTimeAttack/✅ score 200 + timeSec 20")]
    private static void updateResultTimeAttack_s200_t20() {

        call("updateResultTimeAttack", newArgs(
            "score", 200,
            "timeSec", 20
        ));
    }

    [MenuItem(PATH_FILE_TEST + "updateWallet/❌ bad")]
    private static void updateWallet_bad() {

        call("updateWallet", newArgs(
            "nbHexacoins", new Dictionary<object, object>(),
            "lastRemoteNbHexacoins", new Dictionary<object, object>()
        ));
    }

    [MenuItem(PATH_FILE_TEST + "updateWallet/❌ -100 + last 0")]
    private static void updateWallet_m100_last_0() {

        call("updateWallet", newArgs(
            "nbHexacoins", -100,
            "lastRemoteNbHexacoins", 0
        ));
    }

    [MenuItem(PATH_FILE_TEST + "updateWallet/❌ 100 + last -50")]
    private static void updateWallet_100_last_m50() {

        call("updateWallet", newArgs(
            "nbHexacoins", 100,
            "lastRemoteNbHexacoins", -50
        ));
    }

    [MenuItem(PATH_FILE_TEST + "updateWallet/✅ 100 + last 0")]
    private static void updateWallet_100_last_0() {

        call("updateWallet", newArgs(
            "nbHexacoins", 100,
            "lastRemoteNbHexacoins", 0
        ));
    }

    [MenuItem(PATH_FILE_TEST + "updateWallet/✅ 100 + last 50")]
    private static void updateWallet_100_last_50() {

        call("updateWallet", newArgs(
            "nbHexacoins", 100,
            "lastRemoteNbHexacoins", 50
        ));
    }

    [MenuItem(PATH_FILE_TEST + "updateGraphArcade/❌ bad1")]
    private static void updateGraphArcade_bad1() {

        call("updateGraphArcade", newArgs(
            "zones", new Dictionary<object, object>(),
            "unlockedNodes", new int[] { 0, 0 },
            "activeNodes", new int[] { 0, 0 }
        ));
    }

    [MenuItem(PATH_FILE_TEST + "updateGraphArcade/❌ bad2")]
    private static void updateGraphArcade_bad2() {

        call("updateGraphArcade", newArgs(
            "zones", 0,
            "unlockedNodes", new Dictionary<object, object>(),
            "activeNodes", new Dictionary<object, object>()
        ));
    }

    [MenuItem(PATH_FILE_TEST + "updateGraphArcade/❌ zones 0 nodes bad")]
    private static void updateGraphArcade_z0_nbad() {

        call("updateGraphArcade", newArgs(
            "zones", 0,
            "unlockedNodes", new object[] { new Dictionary<object, object>(), 0 },
            "activeNodes", new object[] { new Dictionary<object, object>(), 0 }
        ));
    }

    [MenuItem(PATH_FILE_TEST + "updateGraphArcade/❌ zones -3 + nodes 0")]
    private static void updateGraphArcade_zm3_n0() {

        call("updateGraphArcade", newArgs(
            "zones", -3,
            "unlockedNodes", new int[] { 0, 0 },
            "activeNodes", new int[] { 0, 0 }
        ));
    }

    [MenuItem(PATH_FILE_TEST + "updateGraphArcade/❌ zones 0 + nodes -10")]
    private static void updateGraphArcade_z0_n10() {

        call("updateGraphArcade", newArgs(
            "zones", 0,
            "unlockedNodes", new int[] { -10, 0 },
            "activeNodes", new int[] { -10, 0 }
        ));
    }

    [MenuItem(PATH_FILE_TEST + "updateGraphArcade/✅ zones 0 + nodes 0")]
    private static void updateGraphArcade_z0() {

        call("updateGraphArcade", newArgs(
            "zones", 0,
            "unlockedNodes", new int[] { 0, 0 },
            "activeNodes", new int[] { 0, 0 }
        ));
    }

    [MenuItem(PATH_FILE_TEST + "updateGraphArcade/✅ zones 3 nodes 10")]
    private static void updateGraphArcade_z3() {

        call("updateGraphArcade", newArgs(
            "zones", 3,
            "unlockedNodes", new int[] { 10, 0 },
            "activeNodes", new int[] { 10, 0 }
        ));
    }

    [MenuItem(PATH_FILE_TEST + "addInAppPurchase/❌ bad")]
    private static void addInAppPurchase_bad() {

        call("addInAppPurchase", newArgs(
            "purchaseDate", new Dictionary<object, object>(),
            "purchaseTag", new Dictionary<object, object>(),
            "transactionId", new Dictionary<object, object>(),
            "purchasePrice", new Dictionary<object, object>(),
            "purchaseNbHexacoins", new Dictionary<object, object>(),
            "purchaseIsRemovingAds", new Dictionary<object, object>(),
            "receipt", new Dictionary<object, object>(),
            "lastRemoteNbHexacoins", new Dictionary<object, object>(),
            "nbHexacoins", new Dictionary<object, object>(),
            "hasRemovedAds", new Dictionary<object, object>()
        ));
    }

    [MenuItem(PATH_FILE_TEST + "addInAppPurchase/❌ void tag")]
    private static void addInAppPurchase_void_tag() {

        call("addInAppPurchase", newArgs(
            "purchaseDate", 1546364432,
            "purchaseTag", "",
            "transactionId", "123456789",
            "purchasePrice", "0,99€",
            "purchaseNbHexacoins", 40,
            "purchaseIsRemovingAds", false,
            "receipt", "AAAAAAAAAAA",
            "lastRemoteNbHexacoins", 0,
            "nbHexacoins", 100,
            "hasRemovedAds", false
        ));
    }

    [MenuItem(PATH_FILE_TEST + "addInAppPurchase/❌ void transaction id")]
    private static void addInAppPurchase_void_transaction_id() {

        call("addInAppPurchase", newArgs(
            "purchaseDate", 1546364432,
            "purchaseTag", "com.hexasnap.add40",
            "transactionId", "",
            "purchasePrice", "0,99€",
            "purchaseNbHexacoins", 40,
            "purchaseIsRemovingAds", false,
            "receipt", "AAAAAAAAAAA",
            "lastRemoteNbHexacoins", 0,
            "nbHexacoins", 100,
            "hasRemovedAds", false
        ));
    }

    [MenuItem(PATH_FILE_TEST + "addInAppPurchase/❌ -40 hexacoins")]
    private static void addInAppPurchase_m40_hex() {

        call("addInAppPurchase", newArgs(
            "purchaseDate", 1546364432,
            "purchaseTag", "com.hexasnap.add40",
            "transactionId", "123456789",
            "purchasePrice", "0,99€",
            "purchaseNbHexacoins", -40,
            "purchaseIsRemovingAds", false,
            "receipt", "AAAAAAAAAAA",
            "lastRemoteNbHexacoins", 0,
            "nbHexacoins", 100,
            "hasRemovedAds", false
        ));
    }

    [MenuItem(PATH_FILE_TEST + "addInAppPurchase/❌ -100 remote nb hexacoins")]
    private static void addInAppPurchase_m100_nb_remote_hex() {

        call("addInAppPurchase", newArgs(
            "purchaseDate", 1546364432,
            "purchaseTag", "com.hexasnap.add40",
            "transactionId", "123456789",
            "purchasePrice", "0,99€",
            "purchaseNbHexacoins", 40,
            "purchaseIsRemovingAds", false,
            "receipt", "AAAAAAAAAAA",
            "lastRemoteNbHexacoins", -100,
            "nbHexacoins", 100,
            "hasRemovedAds", false
        ));
    }

    [MenuItem(PATH_FILE_TEST + "addInAppPurchase/❌ -100 nb hexacoins")]
    private static void addInAppPurchase_m100_nb_hex() {

        call("addInAppPurchase", newArgs(
            "purchaseDate", 1546364432,
            "purchaseTag", "com.hexasnap.add40",
            "transactionId", "123456789",
            "purchasePrice", "0,99€",
            "purchaseNbHexacoins", 40,
            "purchaseIsRemovingAds", false,
            "receipt", "AAAAAAAAAAA",
            "lastRemoteNbHexacoins", 0,
            "nbHexacoins", -100,
            "hasRemovedAds", false
        ));
    }

    [MenuItem(PATH_FILE_TEST + "addInAppPurchase/✅ +40 hexacoins same transaction id")]
    private static void addInAppPurchase_40_hex_same_id() {

        call("addInAppPurchase", newArgs(
            "purchaseDate", 1546364432,
            "purchaseTag", "com.hexasnap.add40",
            "transactionId", "123456789",
            "purchasePrice", "0,99€",
            "purchaseNbHexacoins", 40,
            "purchaseIsRemovingAds", false,
            "receipt", "AAAAAAAAAAA",
            "lastRemoteNbHexacoins", 0,
            "nbHexacoins", 100,
            "hasRemovedAds", false
        ));
    }

    [MenuItem(PATH_FILE_TEST + "addInAppPurchase/✅ +1000 hexacoins same transaction id")]
    private static void addInAppPurchase_1000_hex_same_id() {

        call("addInAppPurchase", newArgs(
            "purchaseDate", 1546364432,
            "purchaseTag", "com.hexasnap.add1000",
            "transactionId", "123456789",
            "purchasePrice", "8,99€",
            "purchaseNbHexacoins", 1000,
            "purchaseIsRemovingAds", true,
            "receipt", "AAAAAAAAAAA",
            "lastRemoteNbHexacoins", 0,
            "nbHexacoins", 100,
            "hasRemovedAds", false
        ));
    }

    [MenuItem(PATH_FILE_TEST + "addInAppPurchase/✅ +40 hexacoins random transaction id")]
    private static void addInAppPurchase_40_hex_random_id() {

        call("addInAppPurchase", newArgs(
            "purchaseDate", 1546364432,
            "purchaseTag", "com.hexasnap.add40",
            "transactionId", Constants.newRandomInt(100000, 1000000).ToString(),
            "purchasePrice", "0,99€",
            "purchaseNbHexacoins", 40,
            "purchaseIsRemovingAds", false,
            "receipt", "AAAAAAAAAAA",
            "lastRemoteNbHexacoins", 0,
            "nbHexacoins", 100,
            "hasRemovedAds", false
        ));
    }

    [MenuItem(PATH_FILE_TEST + "addInAppPurchase/✅ +1000 hexacoins random transaction id")]
    private static void addInAppPurchase_1000_hex_random_id() {

        call("addInAppPurchase", newArgs(
            "purchaseDate", 1546364432,
            "purchaseTag", "com.hexasnap.add1000",
            "transactionId", Constants.newRandomInt(100000, 1000000).ToString(),
            "purchasePrice", "8,99€",
            "purchaseNbHexacoins", 1000,
            "purchaseIsRemovingAds", true,
            "receipt", "AAAAAAAAAAA",
            "lastRemoteNbHexacoins", 0,
            "nbHexacoins", 100,
            "hasRemovedAds", false
        ));
    }

    [MenuItem(PATH_FILE_TEST + "synchronize/player/❌ bad")]
    private static void synchronize_player_bad() {

        call("synchronize", newArgs());
    }

    [MenuItem(PATH_FILE_TEST + "synchronize/player/✅ bad 1")]
    private static void synchronize_player_bad_1() {

        var args = newArgs();

        args["player"] = new Dictionary<string, object> {
            { "lastRemoteNbHexacoins", new Dictionary<string, object>() },
            { "nbHexacoins", 0 },
            { "hasRemovedAds", false },
        };

        call("synchronize", args);
    }

    [MenuItem(PATH_FILE_TEST + "synchronize/player/✅ bad 2")]
    private static void synchronize_player_bad_2() {

        var args = newArgs();

        args["player"] = new Dictionary<string, object> {
            { "lastRemoteNbHexacoins", 0 },
            { "nbHexacoins", new Dictionary<string, object>() },
            { "hasRemovedAds", new Dictionary<string, object>() },
        };

        call("synchronize", args);
    }

    [MenuItem(PATH_FILE_TEST + "synchronize/player/✅ void")]
    private static void synchronize_player_void() {

        call("synchronize", newArgs(
            "player",
            new Dictionary<string, object>()
        ));
    }

    [MenuItem(PATH_FILE_TEST + "synchronize/player/✅ -20 hexacoins + last 0 hexacoins")]
    private static void synchronize_player_m20_hexacoins_last_0_hexacoins() {

        var args = newArgs();

        args["player"] = new Dictionary<string, object> {
            { "lastRemoteNbHexacoins", 0 },
            { "nbHexacoins", -20 },
            { "hasRemovedAds", false },
        };

        call("synchronize", args);
    }

    [MenuItem(PATH_FILE_TEST + "synchronize/player/✅ 0 hexacoins + last -20 hexacoins")]
    private static void synchronize_player_0_hexacoins_last_m20_hexacoins() {

        var args = newArgs();

        args["player"] = new Dictionary<string, object> {
            { "lastRemoteNbHexacoins", -20 },
            { "nbHexacoins", 0 },
            { "hasRemovedAds", false },
        };

        call("synchronize", args);
    }

    [MenuItem(PATH_FILE_TEST + "synchronize/player/✅ 0 hexacoins")]
    private static void synchronize_player_0_hexacoins() {

        var args = newArgs();

        args["player"] = new Dictionary<string, object> {
            { "lastRemoteNbHexacoins", 0 },
            { "nbHexacoins", 0 },
            { "hasRemovedAds", false },
        };

        call("synchronize", args);
    }

    [MenuItem(PATH_FILE_TEST + "synchronize/player/✅ 100 hexacoins + last 50 hexacoins")]
    private static void synchronize_player_100_hexacoins_last_50_hexacoins() {

        var args = newArgs();

        args["player"] = new Dictionary<string, object> {
            { "lastRemoteNbHexacoins", 50 },
            { "nbHexacoins", 100 },
            { "hasRemovedAds", true },
        };

        call("synchronize", args);
    }

    [MenuItem(PATH_FILE_TEST + "synchronize/player/✅ 100 hexacoins + last 5000 hexacoins")]
    private static void synchronize_player_100_hexacoins_last_5000_hexacoins() {

        var args = newArgs();

        args["player"] = new Dictionary<string, object> {
            { "lastRemoteNbHexacoins", 5000 },
            { "nbHexacoins", 100 },
            { "hasRemovedAds", true },
        };

        call("synchronize", args);
    }

    [MenuItem(PATH_FILE_TEST + "synchronize/arcade/✅ bad")]
    private static void synchronize_arcade_bad() {

        var args = newArgs();

        args["player"] = new Dictionary<string, object> {
            { "arcade", 0 }
        };

        call("synchronize", args);
    }

    [MenuItem(PATH_FILE_TEST + "synchronize/arcade/✅ score bad")]
    private static void synchronize_arcade_score_bad() {

        var args = newArgs();

        args["player"] = new Dictionary<string, object> {
            { "arcade", new Dictionary<string, object> {
                    { "maxScore", new Dictionary<string, object>() },
                    { "maxLevel", new Dictionary<string, object>() }
            }}
        };

        call("synchronize", args);
    }

    [MenuItem(PATH_FILE_TEST + "synchronize/arcade/✅ no graph")]
    private static void synchronize_arcade_no_graph() {

        var args = newArgs();

        args["player"] = new Dictionary<string, object> {
            { "arcade", new Dictionary<string, object> {
                    { "maxScore", 1000 },
                    { "maxLevel", 11 }
            }}
        };

        call("synchronize", args);
    }

    [MenuItem(PATH_FILE_TEST + "synchronize/arcade/✅ graph bad")]
    private static void synchronize_arcade_graph_bad() {

        var args = newArgs();

        args["player"] = new Dictionary<string, object> {
            { "arcade", new Dictionary<string, object> {
                    { "maxScore", 1000 },
                    { "maxLevel", 11 },
                    { "graph", "aaa" }
            }}
        };

        call("synchronize", args);
    }

    [MenuItem(PATH_FILE_TEST + "synchronize/arcade/✅ graph no score")]
    private static void synchronize_arcade_no_score() {

        var args = newArgs();

        args["player"] = new Dictionary<string, object> {
            { "arcade", new Dictionary<string, object> {
                    { "graph", new Dictionary<string, object>() }
            }}
        };

        call("synchronize", args);
    }

    [MenuItem(PATH_FILE_TEST + "synchronize/arcade/✅ score 0")]
    private static void synchronize_arcade_score0() {

        var args = newArgs();

        args["player"] = new Dictionary<string, object> {
            { "arcade", new Dictionary<string, object> {
                    { "maxScore", 0 },
                    { "maxLevel", 1 },
                    { "graph", new Dictionary<string, object> {
                            { "zones", 3 },
                            { "unlockedNodes", new int[] { 3, 3 } },
                            { "activeNodes", new int[] { 3, 3 } }
                    }}
            }}
        };

        call("synchronize", args);
    }

    [MenuItem(PATH_FILE_TEST + "synchronize/arcade/✅ graph 0")]
    private static void synchronize_arcade_graph0() {

        var args = newArgs();

        args["player"] = new Dictionary<string, object> {
            { "arcade", new Dictionary<string, object> {
                    { "maxScore", 100 },
                    { "maxLevel", 10 },
                    { "graph", new Dictionary<string, object> {
                            { "zones", 0 },
                            { "unlockedNodes", new int[] { 0, 0 } },
                            { "activeNodes", new int[] { 0, 0 } }
                    }}
            }}
        };

        call("synchronize", args);
    }

    [MenuItem(PATH_FILE_TEST + "synchronize/purchases/✅ 1 purchase same id")]
    private static void synchronize_1_purchase_same() {

        var pendingPurchases = new List<object>();

        pendingPurchases.Add(new Dictionary<string, object> {
            { "transactionId", "123456789" },
            { "purchaseDate", 1546364432 },
            { "purchaseTag", "com.hexasnap.add1000" },
            { "purchasePrice", "8,99€" },
            { "purchaseNbHexacoins", 1000 },
            { "purchaseIsRemovingAds", true },
            { "receipt", "AAAAAAAAAAA" },
            { "nbHexacoinsToConsume", 1000 },
            { "nbHexacoins", 100 },
            { "hasRemovedAds", false }
        });

        var args = newArgs();

        args["player"] = new Dictionary<string, object> {
            { "pendingPurchases", pendingPurchases }
        };

        call("synchronize", args);
    }

    [MenuItem(PATH_FILE_TEST + "synchronize/purchases/✅ 1 purchase random id")]
    private static void synchronize_1_purchase_random() {

        var pendingPurchases = new List<object>();

        pendingPurchases.Add(new Dictionary<string, object> {
            { "transactionId", Constants.newRandomInt(100000, 1000000).ToString() },
            { "purchaseDate", 1546364432 },
            { "purchaseTag", "com.hexasnap.add1000" },
            { "purchasePrice", "8,99€" },
            { "purchaseNbHexacoins", 1000 },
            { "purchaseIsRemovingAds", true },
            { "receipt", "AAAAAAAAAAA" },
            { "nbHexacoinsToConsume", 1000 },
            { "nbHexacoins", 100 },
            { "hasRemovedAds", false }
        });

        var args = newArgs();

        args["player"] = new Dictionary<string, object> {
            { "pendingPurchases", pendingPurchases }
        };

        call("synchronize", args);
    }


    private static readonly string PATH_ANDROID_BUILD_TXT = "./../export/android_build_passwords.txt";

    [MenuItem(PATH + "Set Android signing")]
    public static void BuildAndroid() {

        if (!File.Exists(PATH_ANDROID_BUILD_TXT)) {

            EditorUtility.DisplayDialog(
                "Missing Build Config",
                "In the project folder create " + PATH_ANDROID_BUILD_TXT + " then inside write 2 lines:lines\n1. The keystore password\n2. The key password",
                "OK"
            );
            return;
        }

        StreamReader configReader = new StreamReader(PATH_ANDROID_BUILD_TXT);
        string storePassword = configReader.ReadLine();
        string keyPassword = configReader.ReadLine();
        configReader.Close();

        PlayerSettings.keystorePass = storePassword;
        PlayerSettings.keyaliasPass = keyPassword;
    }

}

#endif