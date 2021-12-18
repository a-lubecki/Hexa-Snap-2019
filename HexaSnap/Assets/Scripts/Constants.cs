/**
 * Hexa Snap
 * © Aurélien Lubecki 2019
 * All Rights Reserved
 */

using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using UnityEngine;


public class Constants {

	public static readonly float COROUTINE_FIXED_UPDATE_S = 0.005f;

	public static readonly string COROUTINE_TAG_ITEM_GENERATION = "itemGeneration";
	public static readonly string COROUTINE_TAG_MOVE_CAMERA = "moveCamera";

    public static readonly string PATH_RES = "";
    public static readonly string PATH_TRANSLATIONS = PATH_RES + "Translations/";
    public static readonly string PATH_ANIMS = PATH_RES + "Animations/";
    public static readonly string PATH_PREFABS = PATH_RES + "Prefabs/";
    public static readonly string PATH_DESIGNS = PATH_RES + "Designs/";
    public static readonly string PATH_DESIGNS_OBJECTS = PATH_DESIGNS + "Objects/";
	public static readonly string PATH_DESIGNS_BONUS = PATH_DESIGNS + "Bonus/";
	public static readonly string PATH_DESIGNS_CHARACTER = PATH_DESIGNS + "Character/";
	public static readonly string PATH_DESIGNS_MENUS = PATH_DESIGNS + "Menus/";
    public static readonly string PATH_DESIGNS_SOUNDS = PATH_DESIGNS + "Sounds/";
    public static readonly string PATH_DESIGNS_MUSICS = PATH_DESIGNS + "Musics/";

	public static readonly string PREFAB_NAME_ITEM = "Item";
	public static readonly string PREFAB_NAME_ITEM_BONUS = "ItemBonus";
	public static readonly string PREFAB_NAME_TIMER_RUNNING_BONUS = "TimerRunningBonus";
	public static readonly string PREFAB_NAME_TIMER_PROGRESS_STICK = "TimerProgressStick";
	public static readonly string PREFAB_NAME_MENU_IN_GAME = "MenuInGame";
	public static readonly string PREFAB_NAME_MENU_BUTTON = "MenuButton";
	public static readonly string PREFAB_NAME_LINE = "Line";
	public static readonly string PREFAB_NAME_SEGMENT_THIN = "Segment.THIN";

    public static readonly string PREFAB_NAME_SEGMENT_LARGE = "Segment.LARGE";
	public static readonly string PREFAB_NAME_ACTIVITY_TITLE = "ActivityTitle";
    public static readonly string PREFAB_NAME_ACTIVITY_DIALOG_BACKGROUND = "ActivityDialogBackground";
	public static readonly string PREFAB_NAME_ACTIVITY_DIALOG = "ActivityDialog";
    public static readonly string PREFAB_NAME_GRAPH = "Graph";
    public static readonly string PREFAB_NAME_NODE_ZONE = "NodeZone";
    public static readonly string PREFAB_NAME_NODE_BONUS_TYPE = "NodeBonusType";
    public static readonly string PREFAB_NAME_NODE_SLOT_LOCKED = "NodeSlot.Locked";
    public static readonly string PREFAB_NAME_NODE_SLOT_ACTIVATED = "NodeSlot.Activated";
    public static readonly string PREFAB_NAME_GOAL_ITEM = "GoalItem";
    public static readonly string PREFAB_NAME_GOAL_ITEM_GHOST = "GoalItemGhost";
    public static readonly string PREFAB_NAME_HEXACOIN_GLITTER = "HexacoinGlitter";
    public static readonly string PREFAB_NAME_FLYING_SCORE = "FlyingScore";
    public static readonly string PREFAB_NAME_FLYING_HEXACOIN = "FlyingHexacoin";
    public static readonly string PREFAB_NAME_FX_BONUS_SELECT = "FX.BonusSelect";
    public static readonly string PREFAB_NAME_IN_APP_PURCHASE = "InAppPurchase";
    public static readonly string PREFAB_NAME_ONBOARDING_CONTROLS_INDICATOR = "OnboardingControlsIndicator";

    public static readonly string GAME_OBJECT_NAME_MAIN_SCRIPTS_MANAGER = "MainScriptsManager";
	public static readonly string GAME_OBJECT_NAME_CANVAS = "Canvas";
	public static readonly string GAME_OBJECT_NAME_EVENT_SYSTEM = "EventSystem";
	public static readonly string GAME_OBJECT_NAME_MAIN_CAMERA = "MainCamera";
	public static readonly string GAME_OBJECT_NAME_MENU_PAUSE = "DialogPause";
	public static readonly string GAME_OBJECT_NAME_MENU_BONUS_CHOICE = "DialogBonusChoice";
	public static readonly string GAME_OBJECT_NAME_MENU_BUTTON = "MenuButton";
	public static readonly string GAME_OBJECT_NAME_BONUS_QUEUE = "BonusQueue";
	public static readonly string GAME_OBJECT_NAME_BONUS_STACK = "BonusStack";
	public static readonly string GAME_OBJECT_NAME_AXIS = "Axis";
    public static readonly string GAME_OBJECT_NAME_LIMIT_ZONE = "LimitZone";
    public static readonly string GAME_OBJECT_NAME_GROUND_ZONE = "GroundZone";
	public static readonly string GAME_OBJECT_NAME_ITEMS_GENERATOR = "ItemsGenerator";
	public static readonly string GAME_OBJECT_NAME_HEXACOINS_WALLET = "HexacoinsWallet";
	public static readonly string GAME_OBJECT_NAME_POOL = "Pool";
	public static readonly string GAME_OBJECT_NAME_ITEM = "Item";
	public static readonly string GAME_OBJECT_NAME_TIMER_RUNNING_BONUS = "TimerRunningBonus";
	public static readonly string GAME_OBJECT_NAME_TIMER_PROGRESS_STICK = "TimerProgressStick";
	public static readonly string GAME_OBJECT_NAME_BUTTON_ROTATE_CLOCKWISE = "ButtonRotateClockwise";
	public static readonly string GAME_OBJECT_NAME_BUTTON_ROTATE_COUNTER_CLOCKWISE = "ButtonRotateCounterClockwise";
	public static readonly string GAME_OBJECT_NAME_LINE = "Line";
	public static readonly string GAME_OBJECT_NAME_SEGMENT = "Segment";
	public static readonly string GAME_OBJECT_NAME_ACTIVITY_DIALOG_BACKGROUND = "ActivityDialogBackground";
	public static readonly string GAME_OBJECT_NAME_ACTIVITY_DIALOG = "ActivityDialog";
    public static readonly string GAME_OBJECT_CHARACTER = "Character";

    public static readonly MusicInfo MUSIC_INFO_MENU_DEFAULT = new MusicInfo("Menu.Default", 46.9f, 94.9f);
    public static readonly MusicInfo MUSIC_INFO_MENU_HARDCORE = new MusicInfo("Menu.Hardcore", 18.9f, 134.1f);
    public static readonly MusicInfo MUSIC_INFO_GAME_ARCADE_LEVEL1TO5 = new MusicInfo("Game.Arcade.Level1To5", 0f, 137.1f);
    public static readonly MusicInfo MUSIC_INFO_GAME_ARCADE_LEVEL16TO20 = new MusicInfo("Game.Arcade.Level16To20", 0f, 185.4f);
    public static readonly MusicInfo MUSIC_INFO_GAME_ARCADE_LEVEL21TO30 = new MusicInfo("Game.Arcade.Level21To30", 0f, 92.3f);
    public static readonly MusicInfo MUSIC_INFO_GAME_TIMEATTACK = new MusicInfo("Game.TimeAttack", 32f, 128f);

	public static readonly float Z_POS_CAMERA = -100;
	public static readonly float Z_POS_LINES = 0;

	//space between 2 hexagons on the x axis when they are snapped
	public static readonly int HEXAGON_DEFAULT_SIZE = 1;
	public static readonly float SNAPPED_HEXAGONS_DISTANCE_X = Mathf.Cos(30 * Mathf.Deg2Rad) * HEXAGON_DEFAULT_SIZE;
	public static readonly float MIN_SNAPPING_SIZE_PERCENTAGE = 0.3f;

	public static readonly int MAX_ROTATION_FORCE = 6000;

	public static readonly int MAX_LEVEL_ARCADE = 20;
    public static readonly int MAX_LEVEL_HARDCORE = 100;
    public static readonly int MAX_LEVEL_TIME_ATTACK = 10;

    public static readonly int MAX_GOAL_ITEMS_FOR_LEVEL = 5;//max 5 on 6 types

    public static readonly int INITIAL_TIME_ATTACK_TIME_S = 180;//3 min
    public static readonly int TARGET_TIME_ATTACK_TIME_S = 300;//5 min

    public static readonly float MAX_GENERATION_FREQUENCE_S = 1.7f;
    public static readonly float MIN_GENERATION_FREQUENCE_S = 0.55f;
    public static readonly float STEP_10_LEVELS_GENERATION_FREQUENCE_S = 0.01f;//level 100 is 0.47
    public static readonly float ONBOARDING_LEVEL_1_GENERATION_FREQUENCE_S = 1.9f;//default is 1.70
    public static readonly float ONBOARDING_LEVEL_2_GENERATION_FREQUENCE_S = 1.7f;//default is 1.42
    public static readonly float ONBOARDING_LEVEL_3_GENERATION_FREQUENCE_S = 1.4f;//default is 1.31, next is 1.22

	public static readonly float DELAY_BETWEEN_ITEMS_SELECTION_S = 0.6f;
	public static readonly float DELAY_MALUS_TRIGGER_S = 30f;

	#if DEBUG
	public static readonly float MAX_PERCENTAGE_BONUS_GENERATION = 0.08f;
	#else
	public static readonly float MAX_PERCENTAGE_BONUS_GENERATION = 0.08f;
	#endif

	public static readonly int NB_ITEMS_TO_SELECT_GROUP = 3;
    public static readonly int NB_ITEMS_TO_SHAKE_DEVICE = 6;

    public static readonly float ITEM_TO_GOAL_ANIM_TIME_SEC = 0.2f;


    public static readonly int LEVEL_WHEN_BONUS_AVAILABLE_FOR_ONBOARDING = 2;

    public static readonly int NB_HEXACOINS_TO_UNLOCK_SLOT = 5;

    public static readonly int BLOCKING_TIME_MIN_REWARD_DAILY = 24 * 60; //24h
    public static readonly int BLOCKING_TIME_MIN_REWARD_AD_VIDEO = 15; //15min


	public static readonly float INTERPOLATION_ACTIVITY_PUSH = 0.5f;
	public static readonly float INTERPOLATION_ACTIVITY_POP = 0.4f;
	public static readonly float INTERPOLATION_LINE_PUSH = INTERPOLATION_ACTIVITY_PUSH - 0.02f;
	public static readonly float INTERPOLATION_LINE_POP = INTERPOLATION_ACTIVITY_PUSH + 0.02f;

	public static readonly float INTERPOLATION_ITEMS_GENERATOR_IN = 0.3f;
	public static readonly float INTERPOLATION_ITEMS_GENERATOR_MOVE = 0.1f;
	public static readonly float INTERPOLATION_ITEMS_GENERATOR_OUT = 0.3f;
	public static readonly float INTERPOLATION_STACK_IN = 0.5f;
	public static readonly float INTERPOLATION_STACK_MOVE = 0.5f;
	public static readonly float INTERPOLATION_STACK_OUT = 0.3f;
	public static readonly float INTERPOLATION_TIMER_QUEUE_IN = 0.5f;
	public static readonly float INTERPOLATION_TIMER_QUEUE_MOVE = 0.3f;
	public static readonly float INTERPOLATION_TIMER_QUEUE_OUT = 0.5f;


    private static readonly Color COLOR_WHITE = new Color(1.00f, 1.00f, 1.00f); //#ffffff
    private static readonly Color COLOR_SILVER = new Color(0.75f, 0.75f, 0.75f); //#bebebe
    private static readonly Color COLOR_NOBEL = new Color(0.71f, 0.71f, 0.71f); //#b4b4b4
    private static readonly Color COLOR_GRAY = new Color(0.51f, 0.51f, 0.51f); //#828282
    private static readonly Color COLOR_DOVE_GRAY = new Color(0.43f, 0.43f, 0.43f); //#6e6e6e
    private static readonly Color COLOR_TUNDORA = new Color(0.27f, 0.27f, 0.27f); //#464646
    private static readonly Color COLOR_COD_GRAY = new Color(0.12f, 0.12f, 0.12f); //#1e1e1e
    private static readonly Color COLOR_BLACK = new Color(0.00f, 0.00f, 0.00f); //#000000
    private static readonly Color COLOR_LAVENDER_BLUSH = new Color(1.00f, 0.92f, 0.93f); //#ffebee
    private static readonly Color COLOR_SUNSET_ORANGE = new Color(1.00f, 0.32f, 0.32f); //#ff5252
    private static readonly Color COLOR_HAWKES_BLUE = new Color(0.89f, 0.95f, 0.99f); //#e3f2fd
    private static readonly Color COLOR_DODGER_BLUE = new Color(0.27f, 0.54f, 1.00f); //#448aff
    private static readonly Color COLOR_GALLERY = new Color(0.94f, 0.94f, 0.94f); //#f0f0f0
    private static readonly Color COLOR_ALTO = new Color(0.84f, 0.84f, 0.84f); //#d7d7d7
    private static readonly Color COLOR_DUSTY_GRAY = new Color(0.59f, 0.59f, 0.59f); //#969696
    private static readonly Color COLOR_FAIR_PINK = new Color(1.00f, 0.92f, 0.92f); //#ffeaea
    private static readonly Color COLOR_RED_BERRY = new Color(0.60f, 0.00f, 0.00f); //#990000
    private static readonly Color COLOR_BLEACH_WHITE = new Color(1.00f, 0.96f, 0.84f); //#fef4d7
    private static readonly Color COLOR_AMBER = new Color(0.97f, 0.76f, 0.00f); //#f8c100
    private static readonly Color COLOR_FACEBOOK = new Color(0.23f, 0.35f, 0.60f); //#3b5998

    public static readonly Color COLOR_BACKGROUND_DEFAULT = COLOR_GALLERY;
    public static readonly Color COLOR_BACKGROUND_FREEZE = COLOR_WHITE;
    public static readonly Color COLOR_TITLE = COLOR_COD_GRAY;
	public static readonly Color COLOR_TITLE_NEGATIVE = COLOR_WHITE;
	public static readonly Color COLOR_SUBTITLE = COLOR_TUNDORA;
	public static readonly Color COLOR_MENU_BUTTON_FG = COLOR_TUNDORA;
	public static readonly Color COLOR_MENU_BUTTON_HIGHLIGHT = COLOR_RED_BERRY;
	public static readonly Color COLOR_MENU_BUTTON_DIALOG = COLOR_WHITE;
    public static readonly Color COLOR_BONUS = COLOR_DODGER_BLUE;
    public static readonly Color COLOR_MALUS = COLOR_SUNSET_ORANGE;
    public static readonly Color COLOR_LINE_DEFAULT = COLOR_COD_GRAY;
    public static readonly Color COLOR_LINE_INACTIVE = COLOR_ALTO;
    public static readonly Color COLOR_GOAL_REACHED = COLOR_RED_BERRY;
    public static readonly Color COLOR_ARCADE_HARDCORE = COLOR_RED_BERRY;
    public static readonly Color COLOR_BONUS_MULTIPLIER = COLOR_AMBER;


    public static readonly string URL_SITE_HEXASNAP = "https://hexasnap.com/";
    public static readonly string URL_SITE_HEXASNAP_TERMS = URL_SITE_HEXASNAP + "terms";
    public static readonly string URL_SITE_HEXASNAP_PRIVACY = URL_SITE_HEXASNAP + "privacy";
    public static readonly string URL_SITE_HEXASNAP_LEGAL = URL_SITE_HEXASNAP + "legal";
    public static readonly string URL_SITE_HEXASNAP_ABOUT = URL_SITE_HEXASNAP + "about";

    public static readonly string URL_FACEBOOK_HEXASNAP = "https://www.facebook.com/hexasnap";

    public static readonly string DYNAMIC_LINK_DOMAIN = "hexasnap.page.link";
    public static readonly string DYNAMIC_LINK_SOCIAL_TITLE = "Hexa Snap - Free game on iOS and Android";
    public static readonly string DYNAMIC_LINK_SOCIAL_DESCRIPTION = "Hexa Snap is the new arcade scoring game on iOS and Android. Download it for free now!";
    public static readonly string DYNAMIC_LINK_SOCIAL_IMAGE_URL = "https://hexasnap.com/images/hexa-snap-banner.png";
    public static readonly string DYNAMIC_LINK_GA_SOURCE = "app";
    public static readonly string DYNAMIC_LINK_GA_MEDIUM = "share";
    public static readonly string URL_DYNAMIC_LINK_FALLBACK = "https://" + DYNAMIC_LINK_DOMAIN + "/app";

    public static readonly string APP_PACKAGE = "com.hexasnap";

    public static readonly string URL_STORE_GOOGLE = "market://details?id=" + APP_PACKAGE;
    public static readonly string URL_STORE_APPLE = "itms-apps://apps.apple.com/app/id1455788690";
    public static readonly string URL_STORE_STEAM = "TODO";

    public static readonly string AD_MOB_ID_ANDROID = "XXXXXXXXX";
    public static readonly string AD_MOB_ID_IOS = "XXXXXXXXX";

    //https://developers.google.com/admob/unity/test-ads?hl=fr
    public static readonly string AD_MOB_BOTTOM_BANNER_ANDROID = "XXXXXXXXX";
    public static readonly string AD_MOB_BOTTOM_BANNER_IOS = "XXXXXXXXX";
    public static readonly string AD_MOB_BOTTOM_BANNER_TEST = "XXXXXXXXX";

    public static readonly string AD_MOB_REWARDED_ADS_ANDROID = "XXXXXXXXX";
    public static readonly string AD_MOB_REWARDED_ADS_IOS = "XXXXXXXXX";
    public static readonly string AD_MOB_REWARDED_ADS_TEST = "XXXXXXXXX";


    private static Dictionary<ItemSnapDirection, int> anglesMapping = new Dictionary<ItemSnapDirection, int>() {
        { ItemSnapDirection.TOP, 0},
        { ItemSnapDirection.RIGHT_TOP, 60},
        { ItemSnapDirection.RIGHT_BOTTOM, 120},
        { ItemSnapDirection.BOTTOM, 180},
        { ItemSnapDirection.LEFT_BOTTOM, 240},
        { ItemSnapDirection.LEFT_TOP, 300}
    };

    public static ItemSnapDirection[] getAllDirections() {
        return anglesMapping.Keys.ToArray();
    }

	public static float getAngle(ItemSnapDirection direction) {
		return anglesMapping[direction];
	}

	public static ItemSnapDirection findDirection(float angle) {

		angle = normalizeAngle(angle);

		foreach (KeyValuePair<ItemSnapDirection, int> entry in anglesMapping) {

			float refAngle = entry.Value;

			if (refAngle - 30 <= angle && angle <= refAngle + 30) {
				return entry.Key;
			}
		}

		return ItemSnapDirection.TOP;
	}

	public static float vectorToAngle(float x, float y) {

		if(y == 0) {
			return (x > 0) ? 90 : -90;
		}

		return (int)(Mathf.Atan2(x, y) * Mathf.Rad2Deg);
	}

	public static Vector2 angleToVector(float angleDegrees, int force) {

		float angleRad = angleDegrees * Mathf.Deg2Rad;

		return new Vector2(Mathf.Sin(angleRad) * force, Mathf.Cos(angleRad) * force);
	}

	public static float normalizeAngle(float angleDegrees) {

		angleDegrees = angleDegrees % 360;

		if (angleDegrees < 0) {
			angleDegrees += 360;
		}

		return angleDegrees;
	}


	public static float getHighlightedAlpha(bool isHighlighted) {

		if (!isHighlighted) {
			return 1;
		}

		float durationSec = 0.7f;//every x seconds

		float alphaMin = 0.6f;
		float alphaMax = 1;

		float timeInPeriod = Time.time % durationSec;
		float timePercentage = timeInPeriod / durationSec;

		if (timePercentage < 0.15f) {
			//decreasing
			return alphaMin + (alphaMax - alphaMin) * (1 - timePercentage);
		}

		if (timePercentage < 0.3f) {
			//increasing
			return alphaMin + (alphaMax - alphaMin) * timePercentage;
		}

		return 1;
	}


    public static bool isAnimationPlaying(Animation anim, string animationName) {

        if (animationName == null && anim.GetClipCount() > 1) {
            throw new ArgumentException("More than 1 clip in the anim, don't know which one is playing");
        }

        if (animationName == null) {
            return anim.isPlaying;
        }

        return anim.IsPlaying(animationName);
    }

	public static void playAnimation(Animation anim, string animationName, bool isReversed) {

        if (anim == null) {
            throw new ArgumentException();
        }

		if (animationName == null && anim.GetClipCount() > 1) {
			throw new ArgumentException("More than 1 clip in the anim, don't know which one to play");
		}

		anim.Stop();
		anim.Rewind();

		if (animationName == null) {
			animationName = anim.clip.name;//first one
		}

		AnimationState state = anim[animationName];
		state.speed = isReversed ? -1 : 1;
		state.time = isReversed ? state.length : 0;

		anim.Play(animationName);
	}

    public static void playFX(string animName, Vector3 pos, float angleDegrees = 0) {

        GameObjectPoolBehavior pool = GameHelper.Instance.getPool();

        GameObject goFx = pool.pickFXGameObject(animName, pos, angleDegrees);

        Async.call(goFx.GetComponent<Animator>().GetCurrentAnimatorStateInfo(0).length, () => {
            pool.storeFXGameObject(goFx);
        });

    }


    public static Vector3 newVector3(Vector3 vector, float dx, float dy, float dz) {
        return new Vector3(vector.x + dx, vector.y + dy, vector.z + dz);
    }

    private static CultureInfo getApplicationCultureInfo() {

        var language = Application.systemLanguage;

        return CultureInfo.GetCultures(CultureTypes.AllCultures).
            FirstOrDefault(x => x.EnglishName == Enum.GetName(language.GetType(), language));
    }

    public static string getDisplayableScore(int score) {
        return score.ToString("N0", getApplicationCultureInfo());
    }

    public static string getDisplayableTimeSec(float timeSec) {
        return getDisplayableTimeMs((long) (timeSec * 1000));
    }

    public static string getDisplayableTimeMs(long timeMs) {

        TimeSpan timeSpan = new TimeSpan(timeMs * 10000);
        return string.Format("{0:0}:{1:00}", Math.Floor(timeSpan.TotalMinutes), timeSpan.Seconds);
    }


    /**
     * New bool.
     */
    public static bool newRandomBool() {
        return newRandomFloat(0, 2) > 1;
    }

    /**
     * New int between 0 and max (inclusive) int.
     */
    public static int newRandomPositiveInt() {
        return newRandomInt(0, int.MaxValue);
    }

    /**
     * New int between min (inclusive) and max (inclusive) int. Can be negative.
     */
    public static int newRandomInt() {
        return newRandomInt(int.MinValue, int.MaxValue);
    }

    /**
     * New int between min (inclusive) and max (inclusive).
     */
    public static int newRandomInt(int min, int max) {
        return (int)newRandomFloat(min, max);
    }

    /**
     * New int between 0 and array length - 1.
     */
    public static int newRandomPosInArray(int arrayLength) {
        return newRandomInt(0, arrayLength - 1);
    }

    /**
     * New float between 0 and max (inclusive) float.
     */
    public static float newRandomPositiveFloat() {
        return newRandomFloat(0, int.MaxValue);
    }

    /**
     * New float between min (inclusive) and max (inclusive) float. Can be negative.
     */
    public static float newRandomFloat() {
        return newRandomFloat(float.MinValue, float.MaxValue);
    }

    /**
     * New float between min (inclusive) and max (inclusive).
     */
    public static float newRandomFloat(float min, float max) {
        if (min >= max) {
            throw new ArgumentException();
        }
        return UnityEngine.Random.Range(min, max);
    }

}
