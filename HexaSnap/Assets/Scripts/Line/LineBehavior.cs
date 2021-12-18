/**
 * Hexa Snap
 * © Aurélien Lubecki 2019
 * All Rights Reserved
 */

using System.Collections.Generic;
using UnityEngine;


public class LineBehavior : BaseModelBehavior {


	public Line line {
		get {
			return (Line) model;
		}
	}

    protected override void onInit() {
        base.onInit();
        
		//move line to the right place
		transform.position = line.getBeginPosition();

		GameObjectPoolBehavior pool = GameHelper.Instance.getPool();

		//update segments
		int nbSegments = line.getNbSegments();

		for (int i = 0 ; i < nbSegments ; i++) {
		
			pool.pickSegmentGameObject(line.getSegment(i), this);
		}

	}


	protected override void onDeinit() {
		base.onDeinit();

		//remove all segments game objects
		GameObjectPoolBehavior pool = GameHelper.Instance.getPool();

		//copy the list to avoid concurrency
		List<SegmentBehavior> segmentBehaviors = new List<SegmentBehavior>();
		foreach (Transform t in transform) {
			segmentBehaviors.Add(t.GetComponent<SegmentBehavior>());
		}

		foreach (SegmentBehavior sb in segmentBehaviors) {
			pool.storeSegmentGameObject(sb);
		}

	}


}
