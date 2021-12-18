/**
 * Hexa Snap
 * © Aurélien Lubecki 2019
 * All Rights Reserved
 */

using UnityEngine;


public abstract class TimeScaledModelBehavior : InGameModelBehavior {


	private float lastTimeScale;

	public Rigidbody2D timeScaledRigidBody { get; private set; }
	private float originalGravityScale;
	private float originalAngularDrag;

	protected abstract bool isPhysicsTimeScaled();


	protected override void onAwake() {
		base.onAwake ();

		timeScaledRigidBody = GetComponent<Rigidbody2D>();
		if (timeScaledRigidBody != null) {
			originalGravityScale = timeScaledRigidBody.gravityScale;
			originalAngularDrag = timeScaledRigidBody.angularDrag;
		}
	}

	protected override void onInit() {
		base.onInit();

		updateTimeScale();

	}

	private void updateTimeScale() {

		if (isPhysicsTimeScaled()) {
			lastTimeScale = getActivity().timeManager.getTotalTimeScalePhysics();
		} else {
			lastTimeScale = getActivity().timeManager.getTotalTimeScalePlay();
		}
	}

	protected bool isPaused() {
		return (lastTimeScale <= 0);
	}

	protected override void onLateUpdate() {
		base.onLateUpdate();

		//update the timescale field
		updateTimeScale();

	}

	protected virtual void updateTimeScaledRigidBody(RigidbodyType2D defaultType) {

		if (timeScaledRigidBody == null) {
			return;
		}

		if (lastTimeScale <= 0) {

			timeScaledRigidBody.bodyType = RigidbodyType2D.Static;

		} else {

			timeScaledRigidBody.bodyType = defaultType;

			if (defaultType == RigidbodyType2D.Dynamic && timeScaledRigidBody.constraints != RigidbodyConstraints2D.FreezePosition) {

				timeScaledRigidBody.velocity *= lastTimeScale;
				timeScaledRigidBody.gravityScale = originalGravityScale / lastTimeScale;
			}

			timeScaledRigidBody.angularDrag = originalAngularDrag / lastTimeScale;
		}
	}

}

