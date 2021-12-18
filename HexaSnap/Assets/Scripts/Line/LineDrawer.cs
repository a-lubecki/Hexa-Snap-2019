/**
 * Hexa Snap
 * © Aurélien Lubecki 2019
 * All Rights Reserved
 */

using System;


public class LineDrawer : ValueInterpolatorListener {


	public readonly Line line;

	public readonly ValueInterpolator valueInterpolator;


	public LineDrawer(Line line) {

		if (line == null) {
			throw new ArgumentException();
		}

		this.line = line;


		valueInterpolator = new ValueInterpolator(this);
	}


	void Update() {

		valueInterpolator.update();

	}


	public void drawAnimated(float duration, InterpolatorCurve curve, Action<bool> completion = null) {

		cancelAnimation();

		hide();

		//reset interpolator
		valueInterpolator.setNextValue(new ValueInterpolatorBundle(0, 0));

		valueInterpolator.setNextValue(new ValueInterpolatorBundle(1, duration, curve), completion);
	}

	public void eraseAnimated(float duration, InterpolatorCurve curve, Action<bool> completion = null) {

		cancelAnimation();

		show();

		//reset interpolator
		valueInterpolator.setNextValue(new ValueInterpolatorBundle(1, 0));

		valueInterpolator.setNextValue(new ValueInterpolatorBundle(0, duration, curve), completion);
	}

	public void show() {

		cancelAnimation();

		line.updateAdvancePercentage(1);
	}

	public void hide() {

		cancelAnimation();

		line.updateAdvancePercentage(0);
	}


	private void cancelAnimation() {

		valueInterpolator.cancelInterpolation();

	}

	void ValueInterpolatorListener.onValueChange(float beginValue, float endValue, float currentValue) {

		line.updateAdvancePercentage(currentValue);

	}

}

