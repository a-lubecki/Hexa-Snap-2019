/**
 * Hexa Snap
 * © Aurélien Lubecki 2019
 * All Rights Reserved
 */

using System;
using UnityEngine;


public struct PositionInterpolatorBundle {

	public readonly Vector3 nextPos;
	public readonly float interpolationDurationSec;
	public readonly InterpolatorCurve curve;

	public PositionInterpolatorBundle(Vector3 nextPos, float interpolationDurationSec, InterpolatorCurve curve = InterpolatorCurve.LINEAR) {

        if (interpolationDurationSec < 0) {
            throw new ArgumentException();
        }

		this.nextPos = nextPos;
		this.interpolationDurationSec = interpolationDurationSec;
		this.curve = curve;

	}

}
