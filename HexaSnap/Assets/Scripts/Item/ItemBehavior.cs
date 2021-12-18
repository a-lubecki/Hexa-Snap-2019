/**
 * Hexa Snap
 * © Aurélien Lubecki 2019
 * All Rights Reserved
 */

using System;
using UnityEngine;


public class ItemBehavior : TimeScaledModelBehavior, ItemListener {


    private static Sprite[] spritesHexagon;
    private static Sprite[] spritesCircle;

    private static bool isSnapSoundPlaying = false;


    public Item item {
		get {
			return (Item) model;
		}
	}

	protected SpriteRenderer spriteRendererBackground;

    private CircleCollider2D colliderPhysicalTop;
    private CircleCollider2D colliderPhysicalBottom;
    private PolygonCollider2D colliderTriggerSelect;

    protected PositionInterpolator positionInterpolator;
    private Vector2 lastPos;

	private float delayBeforeUnregister;

    private Animation animationSelectable;


	protected override bool isPhysicsTimeScaled() {
		return true;	
	}

	protected override void onAwake() {
		base.onAwake();

		if (spritesHexagon == null || spritesCircle == null) {

			spritesHexagon = new Sprite[ItemTypeMethods.getLength()];
			spritesCircle = new Sprite[ItemTypeMethods.getLength()];

			for (int i = (int)ItemType.Type1 ; i <= (int)ItemType.Type100 ; i++) {

				int score = ItemTypeMethods.getScore((ItemType)i);

				spritesHexagon[i] = GameHelper.Instance.loadSpriteAsset(Constants.PATH_DESIGNS_OBJECTS + "Item.Hexagon." + score);
				spritesCircle[i] = GameHelper.Instance.loadSpriteAsset(Constants.PATH_DESIGNS_OBJECTS + "Item.Circle." + score);
			}
		}

		spriteRendererBackground = GetComponentInChildren<SpriteRenderer>();

        colliderPhysicalTop = GetComponents<CircleCollider2D>()[0];
        colliderPhysicalBottom = GetComponents<CircleCollider2D>()[1];
        colliderTriggerSelect = GetComponent<PolygonCollider2D>();

		positionInterpolator = new PositionInterpolator(transform);

        animationSelectable = GetComponent<Animation>();
	}

    protected override void onInit() {
        base.onInit();

		delayBeforeUnregister = 0;

		//reactivate the colliders that may have been deactivated previously with onItemDestroyRequest()
		foreach(Collider2D c in GetComponents<Collider2D>()) {
			c.enabled = true;
		}

		updateBackground();

		updateTranform();
        updateColliders();

        updateClickableAnimation();

        lastPos = transform.position;
	}
	
	protected override void onFixedUpdate() {
		base.onFixedUpdate();

		updateRigidBody();

        lastPos = transform.position;
	}

	protected override void onUpdate() {
		base.onUpdate();

		updateBackgroundAlpha();

        positionInterpolator.update();

        lastPos = transform.position;
	}

    protected override void onDeinit() {
		base.onDeinit();

		positionInterpolator.cancelInterpolation();
	}

	public Vector3 getLastAnimatedPos() {

		if (!isInit()) {
			return transform.position;
		}

		return positionInterpolator.getLastInterpolatedPos();
	}

	public float getAnimationRemainingTimeSec() {

		if (!isInit()) {
			return 0;
		}

		return positionInterpolator.getRemainingTimeSec();
	}

	public void animatePosition(PositionInterpolatorBundle bundle, bool cancelPreviousAnimation, Action<bool> completion = null) {
		animatePositions(new PositionInterpolatorBundle[] { bundle }, cancelPreviousAnimation, completion);
	}

	public void animatePositions(PositionInterpolatorBundle[] bundles, bool cancelPreviousAnimation, Action<bool> completion = null) {

		if (!isInit()) {
			return;
		}

        //as the item is animating, the background must be a circle, even if it's stacked in the model
        updateBackground();

        Action<bool> action = (hasReachedTheEnd) => {

            if (!isInit()) {
				return;
			}

			//then the background can be an hexagon again if item is stacked
			updateBackground();
            updateColliders();

            completion?.Invoke(hasReachedTheEnd);
        };

		if (cancelPreviousAnimation) {
			positionInterpolator.setNextPositions(bundles, action);
		} else {
			positionInterpolator.addNextPositions(bundles, action);
        }
    }

	public void cancelAnimationPosition() {

		if (!isInit()) {
			return;
		}

		positionInterpolator.cancelInterpolation();
    }

	public void setDelayBeforeUnregister(float delayBeforeUnregister) {

		if (delayBeforeUnregister < 0) {
			throw new ArgumentException();
		}

		this.delayBeforeUnregister = delayBeforeUnregister;
	}

	BaseModelBehavior BaseModelListener.getModelBehavior() {
		return this;
	}

	void ItemListener.onItemTypeChange(Item item) {

		updateBackground();
	}

    void ItemListener.onFallingItemCollide(Item item, Vector3 currentPos) {
		//do nothing
	}

	void ItemListener.onSnappedItemClick(Item item) {
		//do nothing
    }

    void ItemListener.onItemSelectableChange(Item item) {

        updateColliders();

        updateClickableAnimation();
    }

    void ItemListener.onItemSelect(Item item) {

        GameHelper.Instance.getAudioManager().playSound("Item.Select");
    }

    void ItemListener.onItemChangeZone(Item item) {

        updateBackground();

        updateTranform();
        updateColliders();

        playSnapSound(item);
    }

    private void playSnapSound(Item item) {

        if (isSnapSoundPlaying) {
            //already playing
            return;
        }

        string soundName = null;

        if (item.isSnapped() || (item.isStacked && !item.wasStacked)) {
            soundName = "Item.Snap." + Constants.newRandomInt(1, 4);
        } else if ((!item.isSnapped() && item.wasSnapped) || (!item.isStacked && item.wasStacked)) {
            soundName = "Item.Unsnap." + Constants.newRandomInt(1, 4);
        }

        if (soundName == null) {
            //nothing to play
            return;
        }

        //disable sound playing to avoid too much audio memory allocation during unsnapping
        isSnapSoundPlaying = true;

        GameHelper.Instance.getAudioManager().playSound(soundName);

        //enable snap sound gain after a moment
        Async.call(0.2f, () => isSnapSoundPlaying = false);
	}

    void ItemListener.onItemDestroyRequest(Item item, bool wasSnapped, ItemDestroyCause cause) {

		if (delayBeforeUnregister <= 0) {
			//trigger unregister now
			GameHelper.Instance.getPool().storeItemGameObject(this, item.itemType);
		
		} else {

			//disable colliders to avoid triggering events and unwanted collisition
			foreach(Collider2D c in GetComponents<Collider2D>()) {
				c.enabled = false;
			}

            Async.call(delayBeforeUnregister, () => {
				GameHelper.Instance.getPool().storeItemGameObject(this, item.itemType);
			});
		}
	}


	protected virtual void updateBackground() {

		if (!positionInterpolator.isInterpolating && item.isSnapped()) {
            spriteRendererBackground.sprite = getCurrentBackgroundSpriteHexagon();
		} else {
            spriteRendererBackground.sprite = getCurrentBackgroundSpriteCircle();
		}
	}

	protected virtual float updateBackgroundAlpha() {

		float alpha = Constants.getHighlightedAlpha(item.isClickable());
		Color color = spriteRendererBackground.color;

		if (alpha != color.a) {
			spriteRendererBackground.color = new Color(color.r, color.g, color.b, alpha);
		}

		return alpha;
	}

	//TODO put in another class (ex: AxisBehavior)
	protected virtual void updateTranform() {

        Transform axisTransform = getActivity().axisBehavior.transform;

		if (item.isSnapped()) {
			
			if (transform.parent != axisTransform) {
				//set as snapped

				//set velocity to zero, to litterally stop the item
				timeScaledRigidBody.velocity = Vector3.zero;
				timeScaledRigidBody.angularVelocity = 0;

				transform.SetParent(axisTransform);
                transform.localPosition = getActivity().axisBehavior.calculateGameObjectPositionInGrid(item.snapPosition, true);
				transform.localRotation = Quaternion.identity;
			}

		} else { 

			if (transform.parent == axisTransform) {
				//set as unsnapped
				transform.SetParent(null);
			}

            if (!item.hasBeenSnappedBefore) {
                //the item must stay vertical to have bottom and top circle colliders vertically aligned
                transform.rotation = Quaternion.identity;
                timeScaledRigidBody.freezeRotation = true;
            } else {
                //the item is free to rotate
                timeScaledRigidBody.freezeRotation = false;
            }
		}

    }

    private void updateClickableAnimation() {

        if (item == null) {
            return;
        }

        //notify the user that the item is clickable with an infinit anim
        if (item.isClickable()) {
            Constants.playAnimation(animationSelectable, null, false);
        } else {
            animationSelectable.Stop();
        }
    }

	protected void updateColliders() {

        bool isNotSelectedAndNotInterpolating = !item.isSelected && !positionInterpolator.isInterpolating;
        bool hasNotBeenSnappedbefore = !item.hasBeenSnappedBefore;

        colliderPhysicalTop.enabled = hasNotBeenSnappedbefore && isNotSelectedAndNotInterpolating;//top is only used for falling items
        colliderPhysicalBottom.enabled = isNotSelectedAndNotInterpolating;//bottom is used for falling and unsnapped items
        colliderTriggerSelect.enabled = isNotSelectedAndNotInterpolating && item.isSelectable;

        //change radius when snapped to have a fair collision detection
        if (hasNotBeenSnappedbefore) {
            colliderPhysicalTop.radius = 0.24f;
            colliderPhysicalTop.offset = new Vector2(0, 0.25f);
            colliderPhysicalBottom.radius = 0.24f;
            colliderPhysicalBottom.offset = new Vector2(0, -0.25f);
        } else {
            //top is ignored after fall
            colliderPhysicalBottom.radius = 0.48f;
            colliderPhysicalBottom.offset = Vector2.zero;
        }

		updateRigidBody();
	}

	private void updateRigidBody() {
		
		RigidbodyType2D newRigidbodyType = (item.isSelected || item.isSnapped() || item.isEnqueued || item.isStacked) ? RigidbodyType2D.Kinematic : RigidbodyType2D.Dynamic;

		//update rigidbody at the end because the timescale is prior
		updateTimeScaledRigidBody(newRigidbodyType);
	}

    protected override void onCollisionEnter(Collision2D collision) {
        base.onCollisionEnter(collision);

        if (!GameHelper.Instance.getGameManager().isGamePlaying) {
            return;
        }

        if (item.isSnapped()) {
            //only handle trigger with falling items
            return;
        }

        timeScaledRigidBody.velocity = Vector2.zero;

        //current item is falling, selfCollider is a capsule, otherCollider is a sphere
        var selfCollider = collision.otherCollider;
        var otherCollider = collision.collider;

        //calculate the optimal position as the collision detection is crap
        var collidedPosition = new Vector2(lastPos.x, transform.position.y);

        //get an empty position close to the colliding point
        if (otherCollider.gameObject == getActivity().axisBehavior.gameObject) {

            //collided on axis
            item.onFallingItemCollide(collidedPosition);
            return;
        }

        var otherItemBehavior = otherCollider.GetComponent<ItemBehavior>();
        if (otherItemBehavior != null) {

            //the other must be snapped
            var otherItem = otherItemBehavior.item;
            if (!otherItem.isSnapped()) {
                //the other must be snapped to become a hexagon
                return;
            }

            //collided with another item
            item.onFallingItemCollide(collidedPosition);
            return;
        }

        //else not managed for other objects
	}

    public bool isOverlappingItemForSelection(Collider2D[] hitColliders) {

        if (!item.isClickable()) {
            return false;
        }

		foreach (Collider2D c in hitColliders) {
            
            if (c == colliderTriggerSelect) {
                return true;
            }
		}

		return false;
	}

    public virtual Sprite getCurrentBackgroundSpriteHexagon() {
        return spritesHexagon[(int)item.itemType];
    }

    public virtual Sprite getCurrentBackgroundSpriteCircle() {
        return spritesCircle[(int)item.itemType];
    }

}

