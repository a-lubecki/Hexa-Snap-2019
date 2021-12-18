/**
 * Hexa Snap
 * © Aurélien Lubecki 2019
 * All Rights Reserved
 */

using System;
using UnityEngine;
using System.Collections.Generic;


public abstract class BaseModelBehavior : MonoBehaviour, BaseModelListener {

	public static T findModelBehavior<T>(BaseModel model) where T : BaseModelBehavior {

		using(IEnumerator<BaseModelListener> e = model.getListenersEnumerator()) {

			while(e.MoveNext()) {

				BaseModelListener l = e.Current;

				BaseModelBehavior b = l.getModelBehavior();
				if (b != null) {
					//found
					return (T)b;
				}
			}
		}

		return null;
	}

	public static GameObject findGameObject(BaseModel model) {

		BaseModelBehavior b = findModelBehavior<BaseModelBehavior>(model);

		if (b == null) {
			return null;
		}

		return b.gameObject;
	}

	public static Transform findTransform(BaseModel model) {

		BaseModelBehavior b = findModelBehavior<BaseModelBehavior>(model);

		if (b == null) {
			return null;
		}

		return b.transform;
	}


	protected BaseModel model { get; private set; }

	private bool isAwakeCalledOnce = false;

	private static bool isQuittingApplication = false;
	 

	public BaseModelBehavior getModelBehavior() {
		return this;
	}

	public bool isInit() { 
        return (model != null && isAwakeCalledOnce && isActiveAndEnabled);
	}

	protected void Awake() {
		attemptAwake();
    }

    private void attemptAwake() {

		if (isAwakeCalledOnce) {
			return;
		}
		if (!isActiveAndEnabled) {
			return;
		}

		isAwakeCalledOnce = true;

		onAwake();
	}

    protected void OnEnable() {

        if (model == null) {
            return;
        }

        //reinit with the current model
        init(model);
    }

	/**
	 * Link the behavior to a model
	 */
	public void init(BaseModel model) {

		if (model == null) {
			throw new ArgumentException();
        }

		attemptAwake();

		this.model = model;

        if (isActiveAndEnabled) {

            //the model must be active to be init
            onInit();

            model.addListener(this);
        }
	}

	/**
	 * Remove link between behavior and model
	 */
	public void deinit() {

		if (model == null) {
			return;
		}

        model.removeListener(this);
		model = null;

        if (!isQuittingApplication) {
            //clean only if not quitting to avoid crashing the game because of other objects dependencies in deinit() subclasses
            onDeinit();
        }
    }

    protected void OnDisable() {

        if (model == null) {
            return;
        }

        //deinit but keep the model, it will be reinit in OnEnable()
        model.removeListener(this);

        if (!isQuittingApplication) {
            
            //call onDeinit after a frame to avoid crashes
            Async.call(0, () => {

                if (!isQuittingApplication) {
                    onDeinit();
                }
            });
        }
    }

    /**
	 * https://docs.unity3d.com/ScriptReference/MonoBehaviour.OnApplicationQuit.html
	 */
    protected void OnApplicationQuit() {
		isQuittingApplication = true;
	}

	protected void OnDestroy() {
	
		deinit();
	}

	protected void FixedUpdate() {

        if (isInit()) {
            attemptAwake();
            onFixedUpdate();
        }
	}

	protected void Update() {

        if (isInit()) {
            attemptAwake();
            onUpdate();
		}
	}

	protected void LateUpdate() {

        if (isInit()) {
			attemptAwake();
			onLateUpdate();
		}
	}

	protected void OnCollisionEnter2D(Collision2D collision) {

		if (isInit()) {
			attemptAwake();
			onCollisionEnter(collision);
		}
	}

	protected void OnCollisionExit2D(Collision2D collision) {

		if (isInit()) {
			attemptAwake();
			onCollisionExit(collision);
		}
	}

	protected void OnCollisionStay2D(Collision2D collision) {

		if (isInit()) {
			attemptAwake();
			onCollisionStay(collision);
		}
	}

	protected void OnTriggerEnter2D(Collider2D collider) {

		if (isInit()) {
			attemptAwake();
			onTriggerEnter(collider);
		}
	}

	protected void OnTriggerExit2D(Collider2D collider) {

		if (isInit()) {
			attemptAwake();
			onTriggerExit(collider);
		}
	}

	protected void OnTriggerStay2D(Collider2D collider) {

		if (isInit()) {
			attemptAwake();
			onTriggerStay(collider);
		}
	}


	/**
	 * Called only once, when the entity is ready. Used to init fields that won't change
	 */
	protected virtual void onAwake() {
		//override if necessary
	}

	/**
	 * Called when the item is initialized with an entity. The entity field is not set yet.
	 */
	protected virtual void onInit() {
		//override if necessary
	}

	/**
	 * Called when the behavior is attached. The entity field can't be null.
	 */
	public virtual void onBehaviorAttached() {
		//override if necessary
	}

	/**
	 * Called during FixedUpdate (for physics updates) when the entity is active and enabled.
	 */
	protected virtual void onFixedUpdate() {
		//override if necessary
	}

	/**
	 * Called during Update (for other than physics or camera updates) when the entity is active and enabled.
	 */
	protected virtual void onUpdate() {
		//override if necessary
	}

	/**
	 * Called during LateUpdate (for camera updates) when the entity is active and enabled.
	 */
	protected virtual void onLateUpdate() {
		//override if necessary
	}

	/**
	 * Called when the behavior is detached. The entity field is about to be nulled.
	 */
	public virtual void onBehaviorDetached() {
		//override if necessary
	}

	/**
	 * Called when the item is deinit. The entity field has been nulled.
	 */
	protected virtual void onDeinit() {
		//override if necessary
	}


	protected virtual void onCollisionEnter(Collision2D collision) {
		//override if necessary
	}	

	protected virtual void onCollisionExit(Collision2D collision) {
		//override if necessary
	}

	protected virtual void onCollisionStay(Collision2D collision) {
		//override if necessary
	}

	protected virtual void onTriggerEnter(Collider2D collider) {
		//override if necessary
	}

	protected virtual void onTriggerExit(Collider2D collider) {
		//override if necessary
	}

	protected virtual void onTriggerStay(Collider2D collider) {
		//override if necessary
	}

}

