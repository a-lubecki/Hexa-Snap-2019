/**
 * Hexa Snap
 * © Aurélien Lubecki 2019
 * All Rights Reserved
 */

using System;
using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public abstract class Activity4 : BaseUIActivity {
	

    protected LineDrawer lineDrawerTarget;

	protected Text textTitle;
	protected Text textPrevious;
	protected Text textNext;
	protected Text textTarget;

	private Animation animationPrevious;
	private Animation animationNext;


    protected override MarkerBehavior getCurrentMarkerForInit(MarkerManager markerManager) {
		return markerManager.markerDTransition;
	}

    protected override Line newPushLine(BaseActivity next) {

        if (next is Activity10) {

            return new Line(
                Constants.newVector3(markerRef.posLeft, -1, -6, 0),
                Constants.newVector3(next.markerRef.posTop, -4, 2, 0),
                SegmentThickness.LARGE,
                1,
                false);
        }

        return null;
    }

    protected override bool hasAdBanner() {
        return false;
    }

    protected abstract string getTextTitle();

	protected abstract string getTextTarget();

	protected override void onCreate() {
		base.onCreate();
        
		textTitle = updateText("TextTitle", getTextTitle());

		textPrevious = updateText("TextPrevious", null);
		textPrevious.enabled = false;

		textNext = updateText("TextNext", null);
		textNext.enabled = false;

		textTarget = updateText("TextTarget", getTextTarget());
		textTarget.enabled = false;
        
		animationPrevious = textPrevious.GetComponent<Animation>();
		animationNext = textNext.GetComponent<Animation>();

        //animating to left
        float targetPosY = textTarget.transform.position.y;

        Line lineTarget = new Line(
            new Vector3(markerRef.posRight.x + 1, targetPosY, markerRef.posRight.z),
            new Vector3(markerRef.posLeft.x - 1, targetPosY, markerRef.posLeft.z),
            SegmentThickness.LARGE,
            0,
            false
        );

        GameObject goTarget = GameHelper.Instance.getPool().pickLineGameObject(lineTarget);
        goTarget.transform.SetAsFirstSibling();
        lineDrawerTarget = new LineDrawer(lineTarget);
        GameHelper.Instance.getLineDrawersManager().register(lineDrawerTarget);

    }

    protected override void onPreResume() {
        base.onPreResume();

        lineDrawerTarget.hide();

    }

    protected override void onDestroy() {
        base.onDestroy();

        GameHelper.Instance.getLineDrawersManager().unregister(lineDrawerTarget);
        GameHelper.Instance.getPool().storeLineGameObject(BaseModelBehavior.findModelBehavior<LineBehavior>(lineDrawerTarget.line));
        lineDrawerTarget = null;
    }

	public Vector3 getTitleLinePos() {

        Vector3 pos = textTitle.transform.position;
        pos.x = markerRef.posRight.x + 1;
		return pos;
	}

    protected void triggerPreviousNext(bool isReverse, string previous, string next, Action completion) {
        
		if (!string.IsNullOrEmpty(previous)) {

			if (previous.Equals(next)) {
				//no anim + no delay
				textPrevious.enabled = false;
				textNext.enabled = true;
				textNext.text = next;

                //show next lines
                Async.call(triggerStartLines(completion));
				return;
			}

			float delayPrevious = 0;
			if (isReverse) {
				delayPrevious += 0.2f;
			}

            Async.call(delayPrevious, () => {
				
				textPrevious.enabled = true;
				textPrevious.text = previous;

				Constants.playAnimation(animationPrevious, null, isReverse);
			});
		}

		if (!string.IsNullOrEmpty(next)) {

			float delayNext = 0;
			if (!isReverse) {
				delayNext += 0.2f;
			}

            Async.call(delayNext, () => {

				textNext.enabled = true;
				textNext.text = next;

				Constants.playAnimation(animationNext, null, isReverse);
			});
		}

        //show next lines
        Async.call(triggerStartLines(completion));
    }

	private IEnumerator triggerStartLines(Action completion) {

        yield return new WaitForSeconds(0.8f);
        
		lineDrawerTarget.drawAnimated(0.3f, InterpolatorCurve.LINEAR, null);
        GameHelper.Instance.getAudioManager().playSound("Activity.Push");
		
        yield return new WaitForSeconds(0.3f);
        
        textTarget.enabled = true;

        yield return new WaitForSeconds(1.3f);

        completion.Invoke();
	}

	protected string getTextTargetArcade(int level) {

		if (level < Constants.MAX_LEVEL_ARCADE) {
            return string.Format(Tr.get("Activity4.Text.Target.Level"), Constants.MAX_LEVEL_ARCADE);
        }

		if (level == Constants.MAX_LEVEL_ARCADE && !gameManager.isArcadeHarcoreModeUnlocked()) {
			return Tr.get("Activity4.Text.Target.Win");
		}

		if (level < Constants.MAX_LEVEL_HARDCORE) {	
			return string.Format(Tr.get("Activity4.Text.Target.Level"), Constants.MAX_LEVEL_HARDCORE);
		}

		if (level == Constants.MAX_LEVEL_HARDCORE && !gameManager.isArcadeHarcoreModeBeaten()) {
			return Tr.get("Activity4.Text.Target.Survive");
		}

		return Tr.get("Activity4.Text.Target.Best");
	}

}

