/**
 * Hexa Snap
 * © Aurélien Lubecki 2019
 * All Rights Reserved
 */

public struct ValueInterpolatorBundle {

	public readonly float nextValue;
	public readonly float interpolationDurationSec;
	public readonly InterpolatorCurve curve;

	public ValueInterpolatorBundle(float nextValue, float interpolationDurationSec, InterpolatorCurve curve = InterpolatorCurve.LINEAR) {

		this.nextValue = nextValue;
		this.interpolationDurationSec = interpolationDurationSec;
		this.curve = curve;

	}

}
