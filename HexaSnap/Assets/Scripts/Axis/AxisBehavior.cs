/**
 * Hexa Snap
 * © Aurélien Lubecki 2019
 * All Rights Reserved
 */

using System;
using System.Linq;
using System.Collections.Generic;
using UnityEngine;


public class AxisBehavior : TimeScaledModelBehavior, AxisListener {

	public Axis axis {
		get {
			return (Axis) model;
		}
	}


	private Sprite spriteDefault;
	private Sprite spriteBlocked;

    private CircleCollider2D colliderPhysical;
    private SpriteRenderer spriteRenderer;

    private float? newAngle;
    private float newForceClockwise = 0;
    private float newForceCounterClockwise = 0;


    protected override bool isPhysicsTimeScaled() {
		return true;	
	}

	protected override void onAwake() {
		base.onAwake();

        colliderPhysical = GetComponent<CircleCollider2D>();
		spriteRenderer = GetComponent<SpriteRenderer>();

		spriteDefault = GameHelper.Instance.loadSpriteAsset(Constants.PATH_DESIGNS_OBJECTS + "Axis.Default");
		spriteBlocked = GameHelper.Instance.loadSpriteAsset(Constants.PATH_DESIGNS_OBJECTS + "Axis.Blocked");
	}

	public float getAngleDegrees() {
		return Constants.normalizeAngle(transform.eulerAngles.z);
	}

    protected override void onFixedUpdate() {
        base.onFixedUpdate();

        if (newAngle.HasValue) {

            //advance to the angle
            float finalAngle = Constants.normalizeAngle(newAngle.Value);
            float currentRotation = Constants.normalizeAngle(transform.rotation.eulerAngles.z);

            float distance = Mathf.Min(
                Mathf.Abs(finalAngle - currentRotation),
                Mathf.Abs(finalAngle - 360 - currentRotation),
                Mathf.Abs(finalAngle + 360 - currentRotation)
            );
            
            float advanceValue = 20;

            if (distance > advanceValue) {

                //advance a little
                float advance = (finalAngle > currentRotation) ? advanceValue : -advanceValue;

                //manage the < 0 vs > 360 case
                if (Mathf.Abs(finalAngle - currentRotation) > 180) {
                    advance = -advance;
                }

                transform.rotation = Quaternion.Euler(0, 0, currentRotation + advance);

            } else {
                
                //end the advance
                transform.rotation = Quaternion.Euler(0, 0, finalAngle);

                //free the angle to avoid several same calls
                newAngle = null;
            }

        } else {

            //add force to the axis
            var defaultType = GameHelper.Instance.getGameManager().isGamePlaying ? RigidbodyType2D.Dynamic : RigidbodyType2D.Static;
            updateTimeScaledRigidBody(defaultType);

            if (getActivity().timeManager.getTotalTimeScalePlay() <= 0) {
                
                colliderPhysical.enabled = true;

            } else {

                timeScaledRigidBody.AddTorque((newForceCounterClockwise - newForceClockwise) * Time.fixedDeltaTime, ForceMode2D.Force);

                newForceClockwise = 0;
                newForceCounterClockwise = 0;
            }
        }
    }

    protected override void onUpdate() {
        base.onUpdate();

        if (getActivity().timeManager.getTotalTimeScalePlay() <= 0) {
            spriteRenderer.sprite = spriteBlocked;
        } else {
            spriteRenderer.sprite = spriteDefault;
        }
    }

    protected override void onLateUpdate() {
        base.onLateUpdate();
        
        //avoid bugs with items pushing dawn the axis
        transform.position = Vector3.zero;
    }

    BaseModelBehavior BaseModelListener.getModelBehavior() {
		return this;
	}
    
    void AxisListener.onRotationAngleChange(Axis axis, float angle) {

        newAngle = angle;
    }

    void AxisListener.onRotationForceClockwiseChange(Axis axis, float force) {
        
        newForceClockwise += force;
    }

    void AxisListener.onRotationForceCounterClockwiseChange(Axis axis, float force) {

        newForceCounterClockwise += force;
    }


	public Vector3 calculateGameObjectPositionInGrid(ItemSnapPosition snappingPos, bool isLocal) {

        var res = new Vector3(
			snappingPos.pos * Constants.SNAPPED_HEXAGONS_DISTANCE_X, //x
			1 + snappingPos.level - 0.5f * snappingPos.pos, //y
			0);

		//rotate point with final rotation
		res = Quaternion.AngleAxis(60 * (int)snappingPos.direction, Vector3.back) * res;

		if (isLocal) {
			return res;
		}

		//add the current axis rotation
		return Quaternion.AngleAxis(-transform.localRotation.eulerAngles.z, Vector3.back) * res;
	}

    public ItemSnapPosition findFreeAdjacentPosition(Vector3 initialItemPos) {

        var bestPos = findSnapPositionFromWorldPosition(initialItemPos);

        if (canSnap(bestPos)) {
            return bestPos;
        }

        //else try to find another position close to the axis or close to another item (very bad case, almost impossible)

        var checkedPositions = new HashSet<ItemSnapPosition>();
        checkedPositions.Add(bestPos);

        var currentSiblingPositions = new HashSet<ItemSnapPosition>();
        currentSiblingPositions.Add(bestPos);

        return findFreeAdjacentPosition(initialItemPos, currentSiblingPositions, checkedPositions);
    }

    public ItemSnapPosition findFreeAdjacentPosition(Vector3 initialItemPos, HashSet<ItemSnapPosition> currentSiblingPositions, HashSet<ItemSnapPosition> checkedPositions) {

        var newSiblingPositions = new HashSet<ItemSnapPosition>();
        var snappablePositions = new HashSet<ItemSnapPosition>();

        foreach (var currentPos in currentSiblingPositions) {
            
            //expand
            foreach (var siblingPos in currentPos.newSiblingItemPositions()) {
                
                if (checkedPositions.Contains(siblingPos)) {
                    //already seen
                    continue;
                }

                //mark as seen for performance
                checkedPositions.Add(siblingPos);

                if (canSnap(siblingPos)) {
                    //close to another item, can snap, cut the search
                    snappablePositions.Add(siblingPos);
                    continue;
                }

                //keep the sibling for recursive call
                newSiblingPositions.Add(siblingPos);
            }
        }

        if (snappablePositions.Count > 0) {
            //found
            return getClosestItemSnapPosition(snappablePositions, initialItemPos);
        }

        //retry by expanding search to other siblings
        return findFreeAdjacentPosition(initialItemPos, newSiblingPositions, checkedPositions);
    }

    private bool canSnap(ItemSnapPosition pos) {

        if (axis.hasSnappedItemAt(pos)) {
            //there is already an item here
            return false;
        }

        //there is room for this item, check if close to axis or another item

        if (pos.level <= 0) {
            //close to the axis, can snap
            return true;
        }

        //check if close to a snapped item
        foreach (var siblingPos in pos.newSiblingItemPositions()) {
            
            if (axis.hasSnappedItemAt(siblingPos)) {
                //close to another item, can snap
                return true;
            }
        }

        return false;
    }

    private ItemSnapPosition findSnapPositionFromWorldPosition(Vector3 itemPos) {

        var firstPos = new ItemSnapPosition(ItemSnapDirection.TOP, 0, 0);
        if (itemPos.x == 0 && itemPos.y == 0) {
            return firstPos;
        }

        //progress in the axis positions to get the closest possible pos
        return findNextPosition(firstPos, itemPos);
    }

    private ItemSnapPosition findNextPosition(ItemSnapPosition currentPos, Vector3 itemPos) {
        
        var siblingPositions = new HashSet<ItemSnapPosition>();
        siblingPositions.Add(currentPos);
        siblingPositions.UnionWith(currentPos.newSiblingItemPositions());

        //check if sibling are close
        var closestPos = getClosestItemSnapPosition(siblingPositions, itemPos);
        if (closestPos == currentPos) {
            //found
            return currentPos;
        }

        //retry with closestPos
        return findNextPosition(closestPos, itemPos);
    }

    private ItemSnapPosition getClosestItemSnapPosition(HashSet<ItemSnapPosition> positions, Vector3 itemPos) {

        if (positions.Count <= 0) {
            throw new ArgumentException();
        }

        var minDistance = float.MaxValue;
        ItemSnapPosition res = null;

        foreach (var pos in positions) {

            var dist = Vector2.Distance(calculateGameObjectPositionInGrid(pos, false), itemPos);

            if (dist < minDistance) {
                //closer distance than the previous
                minDistance = dist;
                res = pos;
            }
        }

        return res;
    }

    /**
     * Scale a collider to let the items fall if the axis is purely horizontal
     */
	public void bump() {

        float originalX = colliderPhysical.offset.x;
        float originalRadius = colliderPhysical.radius;

        colliderPhysical.offset = new Vector2(originalX - 0.5f, colliderPhysical.offset.y);
        colliderPhysical.radius = originalRadius * 1.5f;

        //reset the position and scale after a short delay
        Async.call(0.5f, () => {
            colliderPhysical.offset = new Vector2(originalX, colliderPhysical.offset.y);
            colliderPhysical.radius = originalRadius;
        });
	}

}

