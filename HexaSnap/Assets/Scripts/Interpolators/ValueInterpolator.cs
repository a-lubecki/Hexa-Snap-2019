/**
 * Hexa Snap
 * © Aurélien Lubecki 2019
 * All Rights Reserved
 */

using System;
using UnityEngine;
using System.Collections.Generic;


public class ValueInterpolator {


	private ValueInterpolatorListener listener;

	public bool isInterpolating { get; private set; }

	private float lastValue;
	private float currentValue;

	private float beginTime;

	private List<ValueInterpolatorBundle> bundles = new List<ValueInterpolatorBundle>(); 

	public List<Action<bool>> completions = new List<Action<bool>>();

	public ValueInterpolator(ValueInterpolatorListener listener) {

		if (listener == null) {
			throw new ArgumentException();
		}

		this.listener = listener;
	}


	public float getLastInterpolatedValue() {

		if (!isInterpolating) {
			return currentValue;
		}

		return bundles[bundles.Count - 1].nextValue;
	}

	public float getRemainingTimeSec() {

		if (!isInterpolating) {
			return 0;
		}

		float elapsedTimeSec = getElapsedPercentage() * bundles[0].interpolationDurationSec;

		float totalTime = 0;
		foreach (ValueInterpolatorBundle bundle in bundles) {
			totalTime += bundle.interpolationDurationSec;
		}

		return totalTime - elapsedTimeSec;
	}

	/**
	 * Cancel all the current interpolations.
	 * If there are ones, the completions are invoked.
	 */
	public void cancelInterpolation() {
		cancelInterpolation(false);
	}

	private void cancelInterpolation(bool hasReachedTheEnd) {

		if (!isInterpolating) {
			//nothing to stop
			return;
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

	}


	private void changeCurrentValue(float v) {

		currentValue = v;

		listener.onValueChange(lastValue, currentValue, currentValue);

	}

	/**
	 * Cancel the current interpolations in the queue then replace them by a new one.
	 * 
	 * The completion Action is added to the other completions and will be called 
	 * at the very end of the animation if others actions are added.
	 */
	public void setNextValue(ValueInterpolatorBundle bundle, Action<bool> completion = null) {

		if (bundle.interpolationDurationSec < 0) {
			throw new ArgumentException();
		}

		if (bundle.interpolationDurationSec <= 0) {

			//cancel the last interpolation
			cancelInterpolation(false);

			lastValue = currentValue;

			//move now, no need to wait for the next frame
			changeCurrentValue(bundle.nextValue);

			if (completion != null) {
				completion(true);
			}

			return;
		}

		if (isInterpolating) {
			//if was interpolating, terminate the translation by applying the last end position
			changeCurrentValue(bundles[bundles.Count - 1].nextValue);

			//clear the list in order to insert only one bundle
			bundles.Clear();

		} else {

			//init a new animation
			isInterpolating = true;
		}

		lastValue = currentValue;
		beginTime = Time.realtimeSinceStartup;

		bundles.Add(bundle);

		if (completion != null) {
			completions.Add(completion);
		}
	}

	/**
	 * Cancel the current interpolations in the queue then replace them by several new ones.
	 * 
	 * The completion Action is added to the other completions and will be called 
	 * at the very end of the animation if others actions are added.
	 */
	public void setNextValues(ValueInterpolatorBundle[] newBundles, Action<bool> completion = null) {

		if (newBundles == null) {
			throw new ArgumentException();
		}
		if (newBundles.Length <= 0) {
			throw new ArgumentException();
		}

		//set the first interpolation to cancel the previous animation
		setNextValue(newBundles[0], completion);

		//then add the others
		int nbBundles = newBundles.Length;
		for (int i = 1 ; i < nbBundles ; i++) {
			addNextValue(newBundles[i]);
		}

	}

	/**
	 * Add an interpolation after the current ones.
	 * 
	 * The completion Action is added to the other completions and will be called at the very end of the animation.
	 */
	public void addNextValue(ValueInterpolatorBundle bundle, Action<bool> completion = null) {

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
			setNextValue(bundle, completion);
		}
	}

	/**
	 * Add several interpolations after the current ones.
	 * 
	 * The completion Action is added to the other completions and will be called at the very end of the animation.
	 */
	public void addNextValues(ValueInterpolatorBundle[] newBundles, Action<bool> completion = null) {

		if (newBundles == null) {
			throw new ArgumentException();
		}
		if (newBundles.Length <= 0) {
			throw new ArgumentException();
		}

		foreach (ValueInterpolatorBundle bundle in newBundles) {
			addNextValue(bundle);
		}

		if (completion != null) {
			completions.Add(completion);
		}
	}

	/**
	 * Add the same next position than the very last one during a delay.
	 * The completion Action is added to the other completions and will be called at the very end of the animation.
	 */
	public void addWait(float delaySec, Action<bool> completion = null) {

		float previousValue;

		if (isInterpolating) {
			//take the last pos of the last bundle
			previousValue = bundles[bundles.Count - 1].nextValue;

		} else {
			//take the current pos
			previousValue = currentValue;
		}

		addNextValue(new ValueInterpolatorBundle(previousValue, delaySec, InterpolatorCurve.LINEAR), completion);
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

		float elapsedPercentage = getElapsedPercentage();

		ValueInterpolatorBundle currentBundle = bundles[0];

		//distord the elapsed percentage with the curve
		elapsedPercentage = InterpolatorCurveMethods.applyFormula(currentBundle.curve, elapsedPercentage);

		//move the transform with the interpolation
		changeCurrentValue(lastValue + (currentBundle.nextValue - lastValue) * elapsedPercentage);

		if (elapsedPercentage >= 1) {

			//remove the bundle from the list as it's finished
			bundles.RemoveAt(0);

			//if at least one element is remaining, process it
			if (bundles.Count > 0) {

				lastValue = currentValue;
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
