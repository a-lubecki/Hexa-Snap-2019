/**
 * Hexa Snap
 * © Aurélien Lubecki 2019
 * All Rights Reserved
 */

using System;
using UnityEngine;


public class GameHelper {
	

	private static GameHelper instance;
	
	private GameHelper() {}
	
	public static GameHelper Instance {

		get {
			if (instance == null) {
				instance = new GameHelper();
			}
			return instance;
		}
	}

	private static GameObject findGameObject(ref WeakReference reference, string gameObjectName) {
		return findGameObject(ref reference, gameObjectName, null);
	}

	private static GameObject findGameObject(ref WeakReference reference, string gameObjectName, GameObject parentGameObject) {

		//try with the stored ref
		if (reference != null) {

			object target = reference.Target;
			if (target != null) {

				//at this point the GameObject can be non-null but destroyed, there are no ways to know if it is null. The only way is to look for a trown exception from a property of GameObject.
				try {
					bool b = (target as GameObject).activeSelf;//trigger an exception if the game object has been destroyed
					return target as GameObject;

				} catch {
					reference = null;
					Debug.Log("NULL REF : " + gameObjectName);
				}

			}

		}

		//retrieve the game object then store it in the ref

		GameObject gameObject = null;

		if (parentGameObject == null) {
			
			gameObject = GameObject.Find(gameObjectName);

			if(gameObject == null) {
				throw new InvalidOperationException();
			}

		} else {
			
			Transform transform = parentGameObject.transform.Find(gameObjectName);

			if(transform == null) {
				throw new InvalidOperationException();
			}

			gameObject = transform.gameObject;
		}

		reference = new WeakReference(gameObject);

		return gameObject;
	}

	private static T findComponent<T>(GameObject gameObject) {
        return gameObject.GetComponent<T>();
	}


	private WeakReference mainScriptsManagerRef;
	private WeakReference canvasRef;
	private WeakReference eventSystemRef;
	private WeakReference mainCameraRef;
	private WeakReference poolRef;
	private WeakReference animationPoolRef;

    private WeakReference axis;
    private WeakReference limitZone;
    private WeakReference groundZone;
    private WeakReference itemsGenerator;
    private WeakReference bonusQueue;
	private WeakReference bonusStack;
    private WeakReference character;
    

    public GameObject getCanvasGameObject() {
		return findGameObject(ref canvasRef, Constants.GAME_OBJECT_NAME_CANVAS);
	}

    public GameObject getEventSystemGameObject() {
        return findGameObject(ref eventSystemRef, Constants.GAME_OBJECT_NAME_EVENT_SYSTEM);
    }

	public MainCameraBehavior getMainCameraBehavior() {
		return findComponent<MainCameraBehavior>(
			findGameObject(ref mainCameraRef, Constants.GAME_OBJECT_NAME_MAIN_CAMERA)
		);
	}

	public GameManager getGameManager() {
		return findComponent<GameManager>(
			findGameObject(ref mainScriptsManagerRef, Constants.GAME_OBJECT_NAME_MAIN_SCRIPTS_MANAGER)
		);
	}

    public Async getAsync() {
        return findComponent<Async>(
            findGameObject(ref mainScriptsManagerRef, Constants.GAME_OBJECT_NAME_MAIN_SCRIPTS_MANAGER)
        );
    }

	public InputsManager getInputsManager() {
		return findComponent<InputsManager>(
			findGameObject(ref mainScriptsManagerRef, Constants.GAME_OBJECT_NAME_MAIN_SCRIPTS_MANAGER)
		);
	}

	public LineDrawersManager getLineDrawersManager() {
		return findComponent<LineDrawersManager>(
			findGameObject(ref mainScriptsManagerRef, Constants.GAME_OBJECT_NAME_MAIN_SCRIPTS_MANAGER)
		);
	}

    public NativePopupManager getNativePopupManager() {
        return findComponent<NativePopupManager>(
            findGameObject(ref mainScriptsManagerRef, Constants.GAME_OBJECT_NAME_MAIN_SCRIPTS_MANAGER)
        );
    }

    public AdsManager getAdsManager() {
        return findComponent<AdsManager>(
            findGameObject(ref mainScriptsManagerRef, Constants.GAME_OBJECT_NAME_MAIN_SCRIPTS_MANAGER)
        );
    }

    public AudioManager getAudioManager() {
        return findComponent<AudioManager>(
            findGameObject(ref mainScriptsManagerRef, Constants.GAME_OBJECT_NAME_MAIN_SCRIPTS_MANAGER)
        );
    }


    public BonusManager getBonusManager() {
        return getGameManager().bonusManager;
    }

    public UpgradesManager getUpgradesManager() {
        return getGameManager().upgradesManager;
    }

    public PurchasesManager getPurchasesManager() {
        return getGameManager().purchasesManager;
    }

    public RewardsManager getRewardsManager() {
        return getGameManager().rewardsManager;
    }

    public HexacoinsWallet getHexacoinsWallet() {
        return getGameManager().hexacoinsWallet;
    }

    public GameObjectPoolBehavior getPool() {
        return findComponent<GameObjectPoolBehavior>(
            findGameObject(ref poolRef, Constants.GAME_OBJECT_NAME_POOL)
        );
    }

    public AnimationPoolBehavior getAnimationPool() {
        return findComponent<AnimationPoolBehavior>(
            findGameObject(ref animationPoolRef, Constants.GAME_OBJECT_NAME_POOL)
        );
    }

    public GameObject getAxis() {
        return findGameObject(ref axis, Constants.GAME_OBJECT_NAME_AXIS);
    }

    public GameObject getLimitZone() {
        return findGameObject(ref limitZone, Constants.GAME_OBJECT_NAME_LIMIT_ZONE);
    }

    public GameObject getGroundZone() {
        return findGameObject(ref groundZone, Constants.GAME_OBJECT_NAME_GROUND_ZONE);
    }

    public GameObject getItemsGenerator() {
        return findGameObject(ref itemsGenerator, Constants.GAME_OBJECT_NAME_ITEMS_GENERATOR);
    }

    public GameObject getBonusQueue() {
        return findGameObject(ref bonusQueue, Constants.GAME_OBJECT_NAME_BONUS_QUEUE);
    }

    public GameObject getBonusStack() {
        return findGameObject(ref bonusStack, Constants.GAME_OBJECT_NAME_BONUS_STACK);
    }

    public GameObject getCharacter() {
        return findGameObject(ref character, Constants.GAME_OBJECT_CHARACTER, getCanvasGameObject());
    }

    public CharacterAnimator getCharacterAnimator() {
        return getCharacter().GetComponent<CharacterAnimator>();
    }

    public UniqueDisplaySpeechesManager getUniqueDisplaySpeechesManager() {
        return getCharacter().GetComponent<UniqueDisplaySpeechesManager>();
    }


	private static T loadAsset<T>(string path) where T : UnityEngine.Object {

		if(string.IsNullOrEmpty(path)) {
			return null;
		}

		T asset = Resources.Load<T>(path) as T;
		if(asset == null) {
			throw new InvalidOperationException("Could not load " + typeof(T) + " asset : " + path);
		}

		return asset;
	}

	public Texture2D loadTexture2DAsset(string path) {
        return loadAsset<Texture2D>(path);
	}
	
	public Sprite loadSpriteAsset(string path) {
		return loadAsset<Sprite>(path);
	}

	public TextAsset loadTextAsset(string path) {
		return loadAsset<TextAsset>(path);
	}

    public AnimationClip loadAnimationClipAsset(string path) {
        return loadAsset<AnimationClip>(path);
    }

    public AudioClip loadAudioAsset(string path) {
        return loadAsset<AudioClip>(path);
    }

	public GameObject loadPrefabAsset(string prefabName) {
		return loadAsset<GameObject>(Constants.PATH_PREFABS + prefabName);
	}

    public RuntimeAnimatorController loadAnimatorController(string animatorName) {
        return loadAsset<RuntimeAnimatorController>(Constants.PATH_ANIMS + animatorName);
    }

    public Sprite loadMultiSpriteAsset(string imagePath, string spriteName) {

		if(string.IsNullOrEmpty(imagePath)) {
			return null;
		}
		if(string.IsNullOrEmpty(spriteName)) {
			return null;
		}

		UnityEngine.Object[] sprites = Resources.LoadAll(imagePath);
		if(sprites == null || sprites.Length <= 0) {
			throw new InvalidOperationException("Could not load multi image asset : " + imagePath);
		}

		foreach(UnityEngine.Object o in sprites) {

			if(!(o is Sprite)) {
				continue;
			}

			Sprite s = o as Sprite;
			if(spriteName.Equals(s.name)) {
				return s;
			}
		}

		throw new InvalidOperationException("Could not load image asset : " + imagePath + " => " + spriteName);
	}

}
