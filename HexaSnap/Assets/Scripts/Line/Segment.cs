/**
 * Hexa Snap
 * © Aurélien Lubecki 2019
 * All Rights Reserved
 */

using System;
using UnityEngine;


public enum SegmentThickness {
	LARGE,
	THIN
}


public class Segment : BaseModel {

	private static SegmentListener to(BaseModelListener listener) {
		return (SegmentListener) listener;
	}

	public readonly Vector3 posBegin;
	public readonly Vector3 posEnd;
	public readonly float totalDistance;

	public readonly SegmentThickness thickness;

	private float advancePercentage = 0;

    public Color color { get; private set; }


    public Segment(Vector3 posBegin, Vector3 posEnd, SegmentThickness thickness) {

		this.posBegin = posBegin;
		this.posEnd = posEnd;
		totalDistance = Vector3.Distance(posBegin, posEnd);

		this.thickness = thickness;
	}

	public float getAdvancePercentage() {
		return advancePercentage;
	}


	public void updateAdvancePercentage(float advancePercentage) {

        if (advancePercentage < 0) {
            advancePercentage = 0;
        } else if (advancePercentage > 1) {
            advancePercentage = 1;
        }

		this.advancePercentage = advancePercentage;

		notifyListeners(listener => {
			to(listener).onAdvancePercentageUpdate(this);
		});

	}

    public void setColor(Color color) {

        this.color = color;

        notifyListeners(listener => {
            to(listener).onSegmentColorChange(this);
        });
    }
}

