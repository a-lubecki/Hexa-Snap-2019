/**
 * Hexa Snap
 * © Aurélien Lubecki 2019
 * All Rights Reserved
 */

using System;
using UnityEngine;


public class MainCameraBehavior : MonoBehaviour {

    public static readonly float minRatio = 1.5f;//iphone SE / Blackberry KEY 2
    public static readonly float maxRatio = 2.165f;//iphone X / Oppo Find X
    public static readonly float minCamScale = 11f;
    public static readonly float maxCamScale = 12.5f;

    private Camera cam;
	private PositionInterpolator positionInterpolator;

    private Vector3 initialCamPos;
    public float screenRatio { get; private set; }

    private bool isShakingHorizontal = false;
	private bool isShakingVertical = false;


    void Awake() {

        RectTransform canvasRectTransform = GameHelper.Instance.getCanvasGameObject().GetComponent<RectTransform>();
        cam = GetComponent<Camera>();

        Screen.autorotateToPortrait = false;
        Screen.autorotateToPortraitUpsideDown = false;
        Screen.autorotateToLandscapeLeft = false;
        Screen.autorotateToLandscapeRight = false;

        Screen.orientation = ScreenOrientation.Portrait;

        //change the current canvas size to adapt to the screen size
        float minSize = Mathf.Min(Screen.width, Screen.height);
        float maxSize = Mathf.Max(Screen.width, Screen.height);
        screenRatio = maxSize / minSize;

        float currentHeight = canvasRectTransform.sizeDelta.y;
        canvasRectTransform.sizeDelta = new Vector2((currentHeight / screenRatio), currentHeight);

        //change the camera scale to have spaces on top/bottom when the screen size is too high (ex : iPhone X)
        float a = (maxCamScale - minCamScale) / (maxRatio - minRatio);
        float b = maxCamScale - (a * maxRatio);
        cam.orthographicSize = a * screenRatio + b;


        positionInterpolator = new PositionInterpolator(transform);
        
        cam.backgroundColor = Constants.COLOR_BACKGROUND_DEFAULT;


        //init all markers
        canvasRectTransform.GetChild(0).gameObject.SetActive(true);
    }

    public void initInGameCamera() {

        initialCamPos = transform.position;
    }

    void LateUpdate() {

		//move
        positionInterpolator?.update();

        //change background color
        if (cam != null) {
            
            GameManager gameManager = GameHelper.Instance.getGameManager();
            Activity10 activity = gameManager.getNullableInGameActivity();

            Color color = Constants.COLOR_BACKGROUND_DEFAULT;

            if (activity != null && gameManager.isGamePlaying && activity.timeManager.getTotalTimeScalePlay() <= 0) {
                color = Constants.COLOR_BACKGROUND_FREEZE;
            }

            cam.backgroundColor = color;
        }
	}

    public Vector3 getClickPosInScene(Vector3 mousePosition) {

        //remove the current position due to the shaking
        Vector3 mousePos = cam.ScreenToWorldPoint(mousePosition);
        Vector3 camPos = transform.position;

        return initialCamPos - camPos + mousePos;
    }

    public void animatePosition(PositionInterpolatorBundle bundle, Action<bool> completion = null) {
		
		positionInterpolator.setNextPosition(bundle, completion);

	}

	private Vector3 newShakePos(Vector3 initialPos, float extraForce, bool isHorizontal, bool isVertical) {
		
		float r = extraForce * 0.2f;
		float hForce = isHorizontal ? extraForce : 0;
		float vForce = isVertical ? extraForce : 0;

		return Constants.newVector3(
            initialPos,
            hForce + r * Constants.newRandomFloat(-1, 1),
            vForce + r * Constants.newRandomFloat(-1, 1),
			0
        );
	}

	private Vector3 newHorizontalShakePos(Vector3 initialPos, float force) {
		return newShakePos(initialPos, force, true, false);
	}

	private Vector3 newVerticalShakePos(Vector3 initialPos, float force) {
		return newShakePos(initialPos, force, false, true);
	}

	public void shakeHorizontal() {

		if (isShakingHorizontal) {
			//avoid multiples calls to the position interpolator for performances
			return;
		}

		isShakingHorizontal = true;

		Vector3 initialPos = positionInterpolator.getLastInterpolatedPos();

		positionInterpolator.setNextPositions(
			new PositionInterpolatorBundle[] {
				new PositionInterpolatorBundle(
					newHorizontalShakePos(initialPos, 0.15f),
					0.04f,
					InterpolatorCurve.EASE_OUT
				),
				new PositionInterpolatorBundle(
					newHorizontalShakePos(initialPos, -0.15f),
					0.08f,
					InterpolatorCurve.EASE_IN_OUT
				),
				new PositionInterpolatorBundle(
					newHorizontalShakePos(initialPos, 0.1f),
					0.08f,
					InterpolatorCurve.EASE_IN_OUT
				),
				new PositionInterpolatorBundle(
					newHorizontalShakePos(initialPos, -0.1f),
					0.04f,
					InterpolatorCurve.EASE_IN_OUT
				),
				new PositionInterpolatorBundle(
					newHorizontalShakePos(initialPos, 0.05f),
					0.04f,
					InterpolatorCurve.EASE_IN_OUT
				),
				new PositionInterpolatorBundle(
					newHorizontalShakePos(initialPos, -0.05f),
					0.02f,
					InterpolatorCurve.EASE_IN_OUT
				),
				new PositionInterpolatorBundle(//return to first pos
					initialPos,
					0.01f,
					InterpolatorCurve.EASE_IN_OUT
				)
			},
            (_) => {

				isShakingHorizontal = false;
			}
		);

	}

	public void shakeVertical() {

		if (isShakingVertical) {
			//avoid multiples calls to the position interpolator for performances
			return;
		}

		isShakingVertical = true;

		Vector3 initialPos = positionInterpolator.getLastInterpolatedPos();

		positionInterpolator.setNextPositions(
			new PositionInterpolatorBundle[] {
				new PositionInterpolatorBundle(
					newVerticalShakePos(initialPos, 0.15f),
					0.04f,
					InterpolatorCurve.EASE_OUT
				),
				new PositionInterpolatorBundle(
					newVerticalShakePos(initialPos, -0.05f),
					0.08f,
					InterpolatorCurve.EASE_IN_OUT
				),
				new PositionInterpolatorBundle(
					newVerticalShakePos(initialPos, 0.05f),
					0.08f,
					InterpolatorCurve.EASE_IN_OUT
				),
				new PositionInterpolatorBundle(//return to first pos
					initialPos,
					0.01f,
					InterpolatorCurve.EASE_IN_OUT
				)
			},
            (_) => {

				isShakingVertical = false;
			}
		);

	}


	public static bool isPortrait() {
		
		if (Input.deviceOrientation == DeviceOrientation.Portrait ||
			Input.deviceOrientation == DeviceOrientation.PortraitUpsideDown ||
			Screen.orientation == ScreenOrientation.Portrait ||
			Screen.orientation == ScreenOrientation.PortraitUpsideDown) {
			return true;
		}

		if (Input.deviceOrientation == DeviceOrientation.LandscapeLeft ||
			Input.deviceOrientation == DeviceOrientation.LandscapeRight) {
			return false;
		}

		return Screen.width < Screen.height;
	}


}
