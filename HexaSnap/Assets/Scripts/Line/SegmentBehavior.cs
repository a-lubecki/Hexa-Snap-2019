/**
 * Hexa Snap
 * © Aurélien Lubecki 2019
 * All Rights Reserved
 */

using UnityEngine;
using UnityEngine.UI;

public class SegmentBehavior : BaseModelBehavior, SegmentListener {

	public Segment segment {
		get {
			return (Segment) model;
		}
	}


	private bool wasHidden = false;
    
	private RectTransform rectTransform;
	private RawImage imageBody;
	private RawImage imageTail;

	private float totalDistance;


	protected override void onAwake() {
		base.onAwake();

		rectTransform = GetComponent<RectTransform>();
		imageBody = GetComponent<RawImage>();
		imageTail = transform.GetChild(0).GetComponent<RawImage>();
	}

    protected override void onInit() {
        base.onInit();

		//init as hidden
		imageBody.enabled = false;
		imageTail.enabled = false;

		transform.eulerAngles = new Vector3(0, 0, Constants.vectorToAngle(segment.posBegin.x - segment.posEnd.x, segment.posEnd.y - segment.posBegin.y));
		totalDistance = Vector3.Distance(segment.posBegin, segment.posEnd);

		updateAdvance();
        updateColor();
	}


	void SegmentListener.onAdvancePercentageUpdate(Segment segment) {

		updateAdvance();
	}

    void SegmentListener.onSegmentColorChange(Segment segment) {

        updateColor();
    }


    private void updateAdvance() {

		float percentage = segment.getAdvancePercentage();

		if (percentage <= 0) {

			if (!wasHidden) {
				//hide
				imageBody.enabled = false;
				imageTail.enabled = false;
			}

		} else {

			if (!wasHidden) {
				//reveal
				imageBody.enabled = true;
				imageTail.enabled = true;
			}

			//resize line
			rectTransform.sizeDelta = new Vector2(rectTransform.sizeDelta.x, percentage * totalDistance);
		}

	}
    
    private void updateColor() {
        
        imageBody.color = segment.color;
        imageTail.color = segment.color;
    }

}
