/**
 * Hexa Snap
 * © Aurélien Lubecki 2019
 * All Rights Reserved
 */

using UnityEngine;


public class MarkerBehavior : MonoBehaviour {


    public float safeAreaPaddingX { get; private set; }
    public float safeAreaPaddingY { get; private set; }

	public Vector3 posCenter { get; private set; }

	public Vector3 posTop { get; private set; }
	public Vector3 posRight { get; private set; }
	public Vector3 posBottom { get; private set; }
	public Vector3 posLeft { get; private set; }

	public Vector3 posTopLeft { get; private set; }
	public Vector3 posTopRight { get; private set; }
	public Vector3 posBottomLeft { get; private set; }
	public Vector3 posBottomRight { get; private set; }

	public Vector3 posCamera { get; private set; }

    public Vector3 posSafeAreaTop { get; private set; }
    public Vector3 posSafeAreaRight { get; private set; }
    public Vector3 posSafeAreaBottom { get; private set; }
    public Vector3 posSafeAreaLeft { get; private set; }

    public Vector3 posSafeAreaTopLeft { get; private set; }
    public Vector3 posSafeAreaTopRight { get; private set; }
    public Vector3 posSafeAreaBottomLeft { get; private set; }
    public Vector3 posSafeAreaBottomRight { get; private set; }

    private float totalWidth;
    private float totalHeight;


	void Awake() {
        
        float cameraSize = GameHelper.Instance.getMainCameraBehavior().GetComponent<Camera>().orthographicSize;
        RectTransform markerRectTransform = GetComponent<RectTransform>();
        Vector2 canvasSize = GameHelper.Instance.getCanvasGameObject().GetComponent<RectTransform>().sizeDelta;

        //change the marker width to have the same size as the screen
        markerRectTransform.sizeDelta = new Vector2(
            canvasSize.x * 10 * cameraSize,
            canvasSize.y * 10 * cameraSize
        );

        Vector2 markerSize = markerRectTransform.sizeDelta * 0.01f;
		posCenter = gameObject.transform.position;

        float dxHalf = 0.5f * markerSize.x;
        float dyHalf = 0.5f * markerSize.y;

		posTop = Constants.newVector3(posCenter, 0, dyHalf, 0);
		posRight = Constants.newVector3(posCenter, dxHalf, 0, 0);
		posBottom = Constants.newVector3(posCenter, 0, -dyHalf, 0);
		posLeft = Constants.newVector3(posCenter, -dxHalf, 0, 0);

		posTopLeft = Constants.newVector3(posCenter, -dxHalf, dyHalf, 0);
		posTopRight = Constants.newVector3(posCenter, dxHalf, dyHalf, 0);
		posBottomLeft = Constants.newVector3(posCenter, -dxHalf, -dyHalf, 0);
		posBottomRight = Constants.newVector3(posCenter, dxHalf, -dyHalf, 0);

        posCamera = new Vector3(posCenter.x, posCenter.y - getCameraYShift(cameraSize), Constants.Z_POS_CAMERA);

        totalWidth = posRight.x - posLeft.x;
        totalHeight = posTop.y - posBottom.y;

        //calculate the area padding to have
        safeAreaPaddingX = 0.15f * totalWidth;
        safeAreaPaddingY = 3f;//place for ads

        posSafeAreaTop = Constants.newVector3(posTop, 0, -safeAreaPaddingY, 0);
        posSafeAreaRight = Constants.newVector3(posRight, -safeAreaPaddingX, 0, 0);
        posSafeAreaBottom = Constants.newVector3(posBottom, 0, safeAreaPaddingY, 0);
        posSafeAreaLeft = Constants.newVector3(posLeft, safeAreaPaddingX, 0, 0);

        posSafeAreaTopLeft = Constants.newVector3(posTopLeft, safeAreaPaddingX, -safeAreaPaddingY, 0);
        posSafeAreaTopRight = Constants.newVector3(posTopRight, -safeAreaPaddingX, -safeAreaPaddingY, 0);
        posSafeAreaBottomLeft = Constants.newVector3(posBottomLeft, safeAreaPaddingX, safeAreaPaddingY, 0);
        posSafeAreaBottomRight = Constants.newVector3(posBottomRight, -safeAreaPaddingX, safeAreaPaddingY, 0);
	}

    /**
     * A shift to display the larger ads on large screens
     */
    private float getCameraYShift(float cameraSize) {
        
        var range = MainCameraBehavior.maxCamScale - MainCameraBehavior.minCamScale;
        var cursor = cameraSize - MainCameraBehavior.minCamScale;

        //bigger the screen is, bigger the shift is
        var offset = cursor / range;
        if (offset < 0) {
            offset = 0;
        }

        if (offset > 1) {
            offset = 1;
        }

        return 0.75f * offset;
    }

    public Vector3 newPosInPercentage(float dx, float dy) {
        
        return new Vector3(
            getPosXInPercentage(dx),
            getPosYInPercentage(dy),
            0
        );
    }

    public float getPosXInPercentage(float dx) {
        return posLeft.x + dx * totalWidth;
    }

    public float getPosYInPercentage(float dy) {
        return posBottom.y + dy * totalHeight;
    }

}

