/**
 * Hexa Snap
 * © Aurélien Lubecki 2019
 * All Rights Reserved
 */

using System;
using System.Collections.Generic;
using UnityEngine;


public class PositionInterpolator {

	private Transform transform;

	public bool isInterpolating { get; private set; }

	private Vector3 lastPos;
	private float beginTime;

	private List<PositionInterpolatorBundle> bundles = new List<PositionInterpolatorBundle>(); 

	public List<Action<bool>> completions = new List<Action<bool>>();


	public PositionInterpolator(Transform transform) {
        this.transform = transform ?? throw new ArgumentException();
	}


	public Vector3 getLastInterpolatedPos() {
		
		if (!isInterpolating) {
			return transform.position;
		}

		return bundles[bundles.Count - 1].nextPos;
	}

	public float getRemainingTimeSec() {
		
		if (!isInterpolating) {
			return 0;
		}

		float elapsedTimeSec = getElapsedPercentage() * bundles[0].interpolationDurationSec;

		float totalTime = 0;
		foreach (PositionInterpolatorBundle bundle in bundles) {
			totalTime += bundle.interpolationDurationSec;
		}

		return totalTime - elapsedTimeSec;
	}

	/**
	 * Cancel all the current interpolations.
	 * If there are ones, the completions are invoked.
	 */
    public PositionInterpolator cancelInterpolation(bool hasReachedTheEnd = false) {

		if (!isInterpolating) {
			//nothing to stop
            return this;
		}

		isInterpolating = false;

		//release the structs in the list
		bundles.Clear();

		//retain the completions temporarily in order to release them before invoking them
		List<Action<bool>> completionsRef = new List<Action<bool>>(completions);

		//release the completion for the same reason
		completions.Clear();

		//call the completions normally
		foreach (Action<bool> completion in completionsRef) {
			completion(hasReachedTheEnd);
		}

        return this;
	}

	/**
	 * Cancel the current interpolations in the queue then replace them by a new one.
	 * 
	 * The completion Action is added to the other completions and will be called 
	 * at the very end of the animation if others actions are added.
	 */
    public PositionInterpolator setNextPosition(PositionInterpolatorBundle bundle, Action<bool> completion = null) {

		if (bundle.interpolationDurationSec < 0) {
			throw new ArgumentException();
		}

		if (bundle.interpolationDurationSec <= 0) {

			//cancel the last interpolation
			cancelInterpolation(false);

			//move now, no need to wait for the next frame
			transform.position = bundle.nextPos;

            completion?.Invoke(true);

            return this;
		}

		if (isInterpolating) {
			//if was interpolating, terminate the translation by applying the last end position
			transform.position = bundles[bundles.Count - 1].nextPos;

			//clear the list in order to insert only one bundle
			bundles.Clear();

		} else {

			//init a new animation
			isInterpolating = true;
		}

		lastPos = transform.position;
		beginTime = Time.realtimeSinceStartup;

		bundles.Add(bundle);

		if (completion != null) {
			completions.Add(completion);
		}

        return this;
	}

	/**
	 * Cancel the current interpolations in the queue then replace them by several new ones.
	 * 
	 * The completion Action is added to the other completions and will be called 
	 * at the very end of the animation if others actions are added.
	 */
    public PositionInterpolator setNextPositions(PositionInterpolatorBundle[] newBundles, Action<bool> completion = null) {

		if (newBundles == null) {
			throw new ArgumentException();
		}
		if (newBundles.Length <= 0) {
			throw new ArgumentException();
		}

		//set the first interpolation to cancel the previous animation
		setNextPosition(newBundles[0], completion);

		//then add the others
		int nbBundles = newBundles.Length;
		for (int i = 1 ; i < nbBundles ; i++) {
			addNextPosition(newBundles[i]);
		}

        return this;
	}

	/**
	 * Add an interpolation after the current ones.
	 * 
	 * The completion Action is added to the other completions and will be called at the very end of the animation.
	 */
    public PositionInterpolator addNextPosition(PositionInterpolatorBundle bundle, Action<bool> completion = null) {

		if (bundle.interpolationDurationSec < 0) {
			throw new ArgumentException();
		}

		if (isInterpolating) {
			//add the new interpolation to the queue, it will be triggered normally at its turn
			bundles.Add(bundle);

			if (completion != null) {
				completions.Add(completion);
			}

		} else {
			//if no current interpolation, the effect is the same as setting directly the interpolation
			setNextPosition(bundle, completion);
		}

        return this;
	}

	/**
	 * Add several interpolations after the current ones.
	 * 
	 * The completion Action is added to the other completions and will be called at the very end of the animation.
	 */
    public PositionInterpolator addNextPositions(PositionInterpolatorBundle[] newBundles, Action<bool> completion = null) {

		if (newBundles == null) {
			throw new ArgumentException();
		}
		if (newBundles.Length <= 0) {
			throw new ArgumentException();
		}

		foreach (PositionInterpolatorBundle bundle in newBundles) {
			addNextPosition(bundle);
		}

		if (completion != null) {
			completions.Add(completion);
		}

        return this;
	}

	/**
	 * Add the same next position than the very last one during a delay.
	 * The completion Action is added to the other completions and will be called at the very end of the animation.
	 */
    public PositionInterpolator addWait(float delaySec, Action<bool> completion = null) {

        if (delaySec < 0) {
            throw new ArgumentException();
        }

        if (delaySec == 0) {
            return this;
        }

		Vector3 previousPos;

		if (isInterpolating) {
			//take the last pos of the last bundle
			previousPos = bundles[bundles.Count - 1].nextPos;

		} else {
			//take the current pos
			previousPos = transform.position;
		}

		addNextPosition(new PositionInterpolatorBundle(previousPos, delaySec, InterpolatorCurve.LINEAR), completion);

        return this;
	}

	/**
	 * Update the global position of the GameObject with the current interpolation.
	 * Call this method in the OnUpdate() of a MonoBehavior.
	 * 
	 * The localPosition is not managed to avoid conflicts when the parent transform changes.
	 */
	public void update() {

		if (!isInterpolating) {
			//no changes to apply on transform
			return;
		}

		if (!transform.gameObject.activeSelf) {
			//can't apply changes on transform
			cancelInterpolation(false);
			return;
		}

		float elapsedPercentage = getElapsedPercentage();

		PositionInterpolatorBundle currentBundle = bundles[0];

		//distord the elapsed percentage with the curve
		elapsedPercentage = InterpolatorCurveMethods.applyFormula(currentBundle.curve, elapsedPercentage);

		//move the transform with the interpolation
		transform.position = Vector3.Lerp(lastPos, currentBundle.nextPos, elapsedPercentage);

		if (elapsedPercentage >= 1) {

			//remove the bundle from the list as it's finished
			bundles.RemoveAt(0);

			//if at least one element is remaining, process it
			if (bundles.Count > 0) {

				lastPos = transform.position;
				beginTime = Time.realtimeSinceStartup;

			} else {
				//else the interpolation is finished
				cancelInterpolation(true);
			}
		}
	}

	private float getElapsedPercentage() {
		
		float elapsedPercentage = (Time.realtimeSinceStartup - beginTime) / bundles[0].interpolationDurationSec;
		if (elapsedPercentage > 1) {
			return 1;
		}

		return elapsedPercentage;
	}
}

