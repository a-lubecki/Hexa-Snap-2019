/**
 * Hexa Snap
 * © Aurélien Lubecki 2019
 * All Rights Reserved
 */

using System;
using UnityEngine;


public class QueueElementExpr : BaseCharacterQueueElement  {


    private CharacterAnimationExpr expr;


    public QueueElementExpr(CharacterAnimatorQueue queue, CharacterAnimationExpr expr) : base(queue) {

        this.expr = expr ?? throw new ArgumentException();
    }

    protected override void processEnqueuedElement() {
        //do nothing
    }

    protected override void processSelectedElement(OneShotDelayedAction completion) {

        completion.anticipateCall(true);

        GameHelper.Instance.getCharacterAnimator().setExpression(loadTextureFromExpr(expr.nameImageFace));

        callCancelableDelayed(expr.durationSec, completion.callAction);
    }

    public override string getTag() {
        return "EXPR";
    }

    public override float getTotalExecutionTimeSec() {
        return expr.durationSec;
    }

    public static Texture2D loadTextureFromExpr(string nameImageFace) {
        return GameHelper.Instance.loadTexture2DAsset(Constants.PATH_DESIGNS_CHARACTER + "Face." + nameImageFace);
    }

}
