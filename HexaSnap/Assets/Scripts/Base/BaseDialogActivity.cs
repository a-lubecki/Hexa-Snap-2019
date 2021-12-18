/**
 * Hexa Snap
 * © Aurélien Lubecki 2019
 * All Rights Reserved
 */

using System;
using UnityEngine;


public abstract class BaseDialogActivity : BaseUIActivity, DialogAnimationListener {


	private Animation animationBackground;
	private Animation animationDialog;


    public override bool isOverlay() {
        return true;
    }

	protected override MarkerBehavior getCurrentMarkerForInit(MarkerManager markerManager) {

		MarkerBehavior previousMarkerRef = getPreviousMarkerRef();
		if (previousMarkerRef == null) {
            throw new InvalidOperationException("Can't push a dialog activity as root : this=" + GetType());
		}

		return previousMarkerRef;
	}

    protected override bool hasAdBanner() {
        return false;
    }


	protected abstract string getDialogAnimationName();

	protected override void onCreate() {
		base.onCreate();

		Transform markerRefTransform = markerRef.transform;

		GameObjectPoolBehavior pool = GameHelper.Instance.getPool();

		GameObject goBackground = pool.pickDialogBackgroundGameObject(markerRefTransform);
		goBackground.GetComponent<DialogBehavior>().listener = this;
		animationBackground = goBackground.GetComponent<Animation>();

		GameObject goDialog = pool.pickDialogGameObject(markerRefTransform);
		goDialog.GetComponent<DialogBehavior>().listener = this;
		animationDialog = goDialog.GetComponent<Animation>();


		//place the background before the current elements in the tree (under them)
		string[] prefabNamesToLoad = getPrefabNamesToLoad();

		if (prefabNamesToLoad != null) {

			int firstLoadedPos = markerRefTransform.childCount - prefabNamesToLoad.Length;

			goBackground.transform.SetSiblingIndex(firstLoadedPos - 2);
			goDialog.transform.SetSiblingIndex(firstLoadedPos - 1);

			//hide during the animation
			foreach (string name in prefabNamesToLoad) {
				
				getLoadedGameObject(name).SetActive(false);
			}
		}

        playDialogAnimsShow();
    }

	protected override void onPrePause(bool isLast) {
		base.onPrePause(isLast);

		if (isLast) {
			playDialogAnimsHide();
		}

	}

	protected virtual void onDialogShown() {
		//override this if necessary
	}

	private void playAnimationBackground(bool isReversed) {
		Constants.playAnimation(animationBackground, null, isReversed);
	}

	protected void playAnimationDialog(bool isReversed) {
		Constants.playAnimation(animationDialog, getDialogAnimationName(), isReversed);
	}

	protected void stopAnimationDialog() {

        string animationName = getDialogAnimationName();

        animationDialog.Stop(animationName);

        //reset the time of the looping anim
        if (animationDialog[animationName].wrapMode == WrapMode.Loop) {
            animationDialog.GetClip(animationName).SampleAnimation(animationDialog.gameObject, 0);
        }
	}

	private void playDialogAnimsShow() {

        playAnimationBackground(false);

        playAnimationDialog(false);
	}

	private void playDialogAnimsHide() {

		playAnimationBackground(true);

		playAnimationDialog(true);
	}


	void DialogAnimationListener.onDialogAnimationStart(Animation anim, bool isReversed) {
		//do nothing
	}

	void DialogAnimationListener.onDialogAnimationFinish(Animation anim, bool isReversed) {

		if (isReversed) {
			//animation hide

			if (anim == animationBackground) {

				GameHelper.Instance.getPool().storeDialogBackgroundGameObject(animationBackground.gameObject);

				//release
				animationBackground = null;

			} else if (anim == animationDialog) {

				GameHelper.Instance.getPool().storeDialogGameObject(animationDialog.gameObject);

				//release
				animationDialog = null;
			}

		} else {
			//animation show

			if (anim == animationDialog) {

				string[] prefabNamesToLoad = getPrefabNamesToLoad();

				if (prefabNamesToLoad != null) {
					//reveal after animation
					foreach (string name in prefabNamesToLoad) {

						GameObject go = getLoadedGameObject(name);

						//center and activate
						go.transform.position = animationDialog.transform.position;
						go.SetActive(true);
					}
				}

				onDialogShown();
			}

		}

	}

}

