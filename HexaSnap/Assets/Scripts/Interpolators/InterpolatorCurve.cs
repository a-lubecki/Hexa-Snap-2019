/**
 * Hexa Snap
 * © Aurélien Lubecki 2019
 * All Rights Reserved
 */

using System;
using UnityEngine;

/**
 * https://chicounity3d.wordpress.com/2014/05/23/how-to-lerp-like-a-pro/
 */
public enum InterpolatorCurve {
	LINEAR,
	EASE_IN,
	EASE_OUT,
	EASE_IN_OUT
}

public class InterpolatorCurveMethods {

	public static float applyFormula(InterpolatorCurve curve, float value) {

		switch (curve) {

		case InterpolatorCurve.LINEAR:
			return value;

		case InterpolatorCurve.EASE_IN:
			return value * value;

		case InterpolatorCurve.EASE_OUT:
			return Mathf.Sqrt(value);

		case InterpolatorCurve.EASE_IN_OUT:
			return value * value * value * (value * (6f * value - 15f) + 10f);
		}


		throw new NotSupportedException("The curve is not managed yet : " + curve);
	}

}
