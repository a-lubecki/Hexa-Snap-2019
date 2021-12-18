/**
 * Hexa Snap
 * © Aurélien Lubecki 2019
 * All Rights Reserved
 */

using System;
using UnityEngine;


public class GameObjectPoolBehavior : BaseGameObjectPoolBehavior {

	private static readonly int TAG_ITEM = 7844;
	private static readonly int TAG_ITEM_BONUS = 1445;
	private static readonly int TAG_TIMER_RUNNING_BONUS = 8898;
	private static readonly int TAG_TIMER_PROGRESS_STICK = 3251;
	private static readonly int TAG_MENU_BUTTON = 8581;
	private static readonly int TAG_LINE = 4625;
	private static readonly int TAG_SEGMENT_THIN = 7895;
	private static readonly int TAG_SEGMENT_LARGE = 5405;
    private static readonly int TAG_DIALOG_BACKGROUND = 1364;
	private static readonly int TAG_DIALOG = 7856;
	private static readonly int TAG_GOAL_ITEM = 1478;
    private static readonly int TAG_GOAL_ITEM_GHOST = 7796;
    private static readonly int TAG_HEXACOIN_GLITTER = 4787;
    private static readonly int TAG_FLYING_SCORE = 6641;
    private static readonly int TAG_FLYING_HEXACOIN = 2451;
    private static readonly int TAG_FX = 1773;
    private static readonly int TAG_FX_BONUS_SELECT = 4546;
    

    public GameObject pickItemGameObject(Item item, Transform parentTransform, bool isPosLocal, Vector3 pos) {

		if (item == null) {
			throw new ArgumentException();
		}

		GameObject res;

		if (item.itemType != ItemType.Bonus) {
        	res = pickGameObject(TAG_ITEM, Constants.PREFAB_NAME_ITEM, Constants.GAME_OBJECT_NAME_ITEM, parentTransform, isPosLocal, pos);
		} else {
        	res = pickGameObject(TAG_ITEM_BONUS, Constants.PREFAB_NAME_ITEM_BONUS, Constants.GAME_OBJECT_NAME_ITEM, parentTransform, isPosLocal, pos);
		}

        res.GetComponent<ItemBehavior>().init(item);

        return res;
	}

	public void storeItemGameObject(ItemBehavior itemBehavior, ItemType type) {

		if (type != ItemType.Bonus) {
			storeGameObject(TAG_ITEM, itemBehavior.gameObject);
		} else {
			storeGameObject(TAG_ITEM_BONUS, itemBehavior.gameObject);
		}
	}

	public GameObject pickTimerRunningBonusGameObject(TimerRunningBonus timer, Transform parentTransform, bool isPosLocal, Vector3 pos) {

		if (timer == null) {
			throw new ArgumentException();
		}

		GameObject res = pickGameObject(TAG_TIMER_RUNNING_BONUS, Constants.PREFAB_NAME_TIMER_RUNNING_BONUS, Constants.GAME_OBJECT_NAME_TIMER_RUNNING_BONUS, parentTransform, isPosLocal, pos);
		res.GetComponent<TimerRunningBonusBehavior>().init(timer);

		return res;
	}

	public void storeTimerRunningBonusGameObject(TimerRunningBonusBehavior timerBehavior) {

		storeGameObject(TAG_TIMER_RUNNING_BONUS, timerBehavior.gameObject);
	}

	public GameObject pickTimerProgressStickGameObject(Transform parentTransform, bool isPosLocal, Vector3 pos) {

		return pickGameObject(TAG_TIMER_PROGRESS_STICK, Constants.PREFAB_NAME_TIMER_PROGRESS_STICK, Constants.GAME_OBJECT_NAME_TIMER_PROGRESS_STICK, parentTransform, isPosLocal, pos);
	}

	public void storeTimerProgressStickGameObject(GameObject gameObject) {

		storeGameObject(TAG_TIMER_PROGRESS_STICK, gameObject);
	}

    public GameObject pickMenuButtonGameObject(BaseMenuButton menuButton, MenuButtonClickListener clickListener, Transform parentTransform, bool isPosLocal, Vector3 pos) {

		if (menuButton == null) {
			throw new ArgumentException();
		}

		GameObject res = pickGameObject(TAG_MENU_BUTTON, Constants.PREFAB_NAME_MENU_BUTTON, Constants.GAME_OBJECT_NAME_MENU_BUTTON, parentTransform, isPosLocal, pos);

        MenuButtonBehavior menuButtonBehavior = res.GetComponent<MenuButtonBehavior>();
        menuButtonBehavior.init(menuButton);

        menuButtonBehavior.clickListener = clickListener;

		return res;
	}

	public void storeMenuButtonGameObject(MenuButtonBehavior menuButtonBehavior) {

        menuButtonBehavior.clickListener = null;

		storeGameObject(TAG_MENU_BUTTON, menuButtonBehavior.gameObject);
	}

	public GameObject pickLineGameObject(Line line) {

		if (line == null) {
			throw new ArgumentException();
		}
        
        GameObject res = pickGameObject(TAG_LINE, Constants.PREFAB_NAME_LINE, Constants.GAME_OBJECT_NAME_LINE, GameHelper.Instance.getCanvasGameObject().transform, false, line.getBeginPosition());
		res.GetComponent<LineBehavior>().init(line);

		return res;
	}

	public void storeLineGameObject(LineBehavior lineBehavior) {

		storeGameObject(TAG_LINE, lineBehavior.gameObject);
	}

	public GameObject pickSegmentGameObject(Segment segment, LineBehavior lineBehavior) {

		if (segment == null) {
			throw new ArgumentException();
		}

		string prefabName = (segment.thickness == SegmentThickness.THIN) ? Constants.PREFAB_NAME_SEGMENT_THIN : Constants.PREFAB_NAME_SEGMENT_LARGE;
        int tag = (segment.thickness == SegmentThickness.THIN) ? TAG_SEGMENT_THIN : TAG_SEGMENT_LARGE;

        GameObject res = pickGameObject(tag, prefabName, Constants.GAME_OBJECT_NAME_SEGMENT, lineBehavior.transform, false, segment.posBegin);
		res.GetComponent<SegmentBehavior>().init(segment);

		return res;
	}

	public void storeSegmentGameObject(SegmentBehavior segmentBehavior) {

        int tag = (segmentBehavior.segment.thickness == SegmentThickness.THIN) ? TAG_SEGMENT_THIN : TAG_SEGMENT_LARGE;
        
		storeGameObject(tag, segmentBehavior.gameObject);
	}

	public GameObject pickDialogBackgroundGameObject(Transform parent) {

		return pickGameObject(TAG_DIALOG_BACKGROUND, Constants.PREFAB_NAME_ACTIVITY_DIALOG_BACKGROUND, Constants.GAME_OBJECT_NAME_ACTIVITY_DIALOG_BACKGROUND, parent, true, Vector3.zero);
	}

	public void storeDialogBackgroundGameObject(GameObject gameObject) {

		if (!Constants.GAME_OBJECT_NAME_ACTIVITY_DIALOG_BACKGROUND.Equals(gameObject.name)) {
			throw new ArgumentException();
		}

		storeGameObject(TAG_DIALOG_BACKGROUND, gameObject);

	}

	public GameObject pickDialogGameObject(Transform parent) {

		return pickGameObject(TAG_DIALOG, Constants.PREFAB_NAME_ACTIVITY_DIALOG, Constants.GAME_OBJECT_NAME_ACTIVITY_DIALOG, parent, true, Vector3.zero);
	}

	public void storeDialogGameObject(GameObject gameObject) {

		if (!Constants.GAME_OBJECT_NAME_ACTIVITY_DIALOG.Equals(gameObject.name)) {
			throw new ArgumentException();
		}

		storeGameObject(TAG_DIALOG, gameObject);

	}

    public GameObject pickGoalItemGameObject(Transform parentTransform, int index, int nbItems) {

        return pickGameObject(TAG_GOAL_ITEM, Constants.PREFAB_NAME_GOAL_ITEM, Constants.PREFAB_NAME_GOAL_ITEM, parentTransform, true, new Vector3(index * 80 - (nbItems - 1) * 40, 0, 0));
    }

    public void storeGoalItemGameObject(GameObject gameObject) {

        storeGameObject(TAG_GOAL_ITEM, gameObject);
    }

    public GameObject pickGoalItemGhostGameObject(Item item, Vector3 pos) {
        
        var go = pickGameObject(TAG_GOAL_ITEM_GHOST, Constants.PREFAB_NAME_GOAL_ITEM_GHOST, Constants.PREFAB_NAME_GOAL_ITEM_GHOST, null, false, pos);
        go.GetComponent<GoalItemGhostBehavior>().updateItem(item);

        return go;
    }

    public void storeGoalItemGhostGameObject(GameObject gameObject) {

        storeGameObject(TAG_GOAL_ITEM_GHOST, gameObject);
    }

    public GameObject pickHexacoinGlitterGameObject(Transform parentTransform, bool isPosLocal, Vector3 pos) {

        return pickGameObject(TAG_HEXACOIN_GLITTER, Constants.PREFAB_NAME_HEXACOIN_GLITTER, Constants.PREFAB_NAME_HEXACOIN_GLITTER, parentTransform, isPosLocal, pos);
    }

    public void storeHexacoinGlitterGameObject(GameObject gameObject) {

        storeGameObject(TAG_HEXACOIN_GLITTER, gameObject);
    }

    public GameObject pickFlyingGameObject(bool isScore, Transform parentTransform, bool isPosLocal, Vector3 pos) {

        if (isScore) {
            return pickGameObject(TAG_FLYING_SCORE, Constants.PREFAB_NAME_FLYING_SCORE, Constants.PREFAB_NAME_FLYING_SCORE, parentTransform, isPosLocal, pos);
        }

        return pickGameObject(TAG_FLYING_HEXACOIN, Constants.PREFAB_NAME_FLYING_HEXACOIN, Constants.PREFAB_NAME_FLYING_HEXACOIN, parentTransform, isPosLocal, pos);
    }

    public void storeFlyingGameObject(bool isScore, GameObject gameObject) {

        if (isScore) {
            storeGameObject(TAG_FLYING_SCORE, gameObject);
        } else {
            storeGameObject(TAG_FLYING_HEXACOIN, gameObject);
        }

    }

    public GameObject pickFXGameObject(string animName, Vector3 pos, float angleDegrees = 0) {

        if (string.IsNullOrEmpty(animName)) {
            throw new ArgumentException();
        }
        
        GameObject go = pickGameObject(TAG_FX, "FX.Default", "FX.Default", null, false, pos);

        Animator animator = go.GetComponent<Animator>();
        
        RuntimeAnimatorController controller = GameHelper.Instance.getAnimationPool().pickAnimatorController(animName);
        animator.runtimeAnimatorController = controller;

        go.transform.eulerAngles = new Vector3(0, 0, angleDegrees);

        return go;
    }

    public void storeFXGameObject(GameObject go) {

        if (go == null) {
            throw new ArgumentException();
        }

        Animator animator = go.GetComponent<Animator>();
        if (animator == null) {
            throw new ArgumentException("Can only store FX objects with an animator");
        }
        
        GameHelper.Instance.getAnimationPool().storeAnimatorController(animator.runtimeAnimatorController);
        animator.runtimeAnimatorController = null;

        storeGameObject(TAG_FX, go);
    }

    public GameObject pickFXBonusSelectGameObject(ItemBonusBehavior itemBonusBehavior) {

        GameObject go = pickFXBonusSelectGameObject(itemBonusBehavior.getSpriteIcon(), itemBonusBehavior.transform.position);
        go.transform.eulerAngles = new Vector3(0, 0, itemBonusBehavior.getIconAngleDegrees());

        return go;
    }

    public GameObject pickFXBonusSelectGameObject(MenuButtonItemBonus menuButton, Vector3 pos) {
        return pickFXBonusSelectGameObject(menuButton.spriteFg, pos);
    }

    private GameObject pickFXBonusSelectGameObject(Sprite spriteIcon, Vector3 pos) {

        GameObject go = pickGameObject(TAG_FX_BONUS_SELECT, Constants.PREFAB_NAME_FX_BONUS_SELECT, Constants.PREFAB_NAME_FX_BONUS_SELECT, GameHelper.Instance.getCanvasGameObject().transform, false, pos);

        //set the same sprite as the item
        go.GetComponent<SpriteRenderer>().sprite = spriteIcon;

        return go;
    }

    public void storeFXBonusSelectGameObject(GameObject go) {

        storeGameObject(TAG_FX_BONUS_SELECT, go);
    }

}

