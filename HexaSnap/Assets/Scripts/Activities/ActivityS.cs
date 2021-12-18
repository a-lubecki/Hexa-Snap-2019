/**
 * Hexa Snap
 * © Aurélien Lubecki 2019
 * All Rights Reserved
 */

using UnityEngine;
using UnityEngine.UI;
using System.Collections;


public class ActivityS : BaseUIActivity {

    private Transform trImageTransition;

    private RawImage imageHexagonTop;
    private RawImage imageCircleTop;
    private RawImage imageHexagonBottom;
    private RawImage imageCircleBottom;

    private RawImage textHexaLight;
    private RawImage textHexaDark;
    private RawImage textSnapLight;

    protected PositionInterpolator positionInterpolatorTop;
    protected PositionInterpolator positionInterpolatorBottom;


	protected override MarkerBehavior getCurrentMarkerForInit(MarkerManager markerManager) {
		return markerManager.marker0;
	}

	protected override string[] getPrefabNamesToLoad() {
		return new string[] { "ActivityS" };
	}

    protected override bool hasAdBanner() {
        return false;
    }

    protected override void onCreate() {
		base.onCreate();

        trImageTransition = findChildTransform("ImageTransition");

        Transform trHexagonTop = findChildTransform("HexagonTop");
        imageHexagonTop = trHexagonTop.GetComponent<RawImage>();
        imageCircleTop = trHexagonTop.Find("CircleTop").GetComponent<RawImage>();

        Transform trHexagonBottom = findChildTransform("HexagonBottom");
        imageHexagonBottom = trHexagonBottom.GetComponent<RawImage>();
        imageCircleBottom = trHexagonBottom.Find("CircleBottom").GetComponent<RawImage>();

        textHexaLight = trHexagonTop.Find("TextHexaLight").GetComponent<RawImage>();
        textHexaDark = trHexagonTop.Find("TextHexaDark").GetComponent<RawImage>();
        textSnapLight = trHexagonBottom.Find("TextSnapLight").GetComponent<RawImage>();

        positionInterpolatorTop = new PositionInterpolator(imageCircleTop.transform);
        positionInterpolatorBottom = new PositionInterpolator(imageCircleBottom.transform);

        //interpolate the anim
        Async.call(interpolate());

        //animate all
        Async.call(animateSplashScreen());
	}

    protected override void onDestroy() {
        base.onDestroy();

        positionInterpolatorTop.cancelInterpolation();
    }

    private IEnumerator interpolate() {

        float totalTime = 0;

        while (totalTime <= 5) {

            yield return new WaitForEndOfFrame();

            positionInterpolatorTop.update();
            positionInterpolatorBottom.update();

            totalTime += Time.deltaTime;
        }
    }

    private IEnumerator animateSplashScreen() {
        
        textHexaDark.enabled = false;

        yield return new WaitForSeconds(0.25f);

        GameHelper.Instance.getAudioManager().playSound("Character.Bubble.Hide");

        yield return new WaitForSeconds(trImageTransition.GetComponent<Animation>().clip.length);

        //hide the transition image after anim
        trImageTransition.gameObject.SetActive(false);

        GameHelper.Instance.getAudioManager().playSound("Item.Generation");


        float animDurationSec = 0.5f;

        //snap the circleTop to the hexagonBottom
        positionInterpolatorTop.setNextPosition(
            new PositionInterpolatorBundle(
                imageHexagonTop.transform.position,
                animDurationSec,
                InterpolatorCurve.EASE_IN
            ),
            (_) => {

                imageCircleTop.enabled = false;
                imageHexagonTop.enabled = true;

                textHexaLight.enabled = false;
                textHexaDark.enabled = true;

                GameHelper.Instance.getAudioManager().playSound("Item.Snap.1");
            }
        );


        yield return new WaitForSeconds(animDurationSec + 1.5f);

        //change hexagons to circles
        imageHexagonTop.enabled = false;
        imageCircleTop.enabled = true;
        imageHexagonBottom.enabled = false;
        imageCircleBottom.enabled = true;

        //hide texts
        textHexaLight.enabled = false;
        textHexaDark.enabled = false;
        textSnapLight.enabled = false;


        //trigger falling
        positionInterpolatorTop.setNextPosition(
            new PositionInterpolatorBundle(
                Constants.newVector3(imageHexagonTop.transform.position, 0, -22, 0),
                animDurationSec,
                InterpolatorCurve.EASE_IN
            ),
            null
        );

        positionInterpolatorBottom.setNextPosition(
            new PositionInterpolatorBundle(
                Constants.newVector3(imageHexagonBottom.transform.position, 0, -22, 0),
                animDurationSec,
                InterpolatorCurve.EASE_IN
            ),
            null
        );

        GameHelper.Instance.getAudioManager().playSound("Item.Unsnap.1");
        GameHelper.Instance.getAudioManager().playSound("Item.Unsnap.2");


        yield return new WaitForSeconds(2f);


        //hide the circles to not see them on push
        imageCircleTop.enabled = false;
        imageCircleBottom.enabled = false;

        if (!gameManager.hasPassedOnboarding) {
            pushAsRoot(new Activity0());
        } else {
            pushAsRoot(new Activity1a());
        }

    }


}

