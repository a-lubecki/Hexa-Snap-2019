/**
 * Hexa Snap
 * © Aurélien Lubecki 2019
 * All Rights Reserved
 */

using System;


public class QueueElementMove : BaseCharacterQueueElement  {


    private CharacterAnimationMove move;
    private float durationSec;


    public QueueElementMove(CharacterAnimatorQueue queue, CharacterAnimationMove move) : base(queue) {

        this.move = move ?? throw new ArgumentException();
    }

    protected override void processEnqueuedElement() {
        //do nothing
    }

    protected override void processSelectedElement(OneShotDelayedAction completion) {

        completion.anticipateCall(true);

        var moveAnimationClip = GameHelper.Instance.loadAnimationClipAsset(Constants.PATH_ANIMS + "Character." + move.nameMoveAnimation);
        durationSec = moveAnimationClip.length;

        GameHelper.Instance.getCharacterAnimator().playMove(moveAnimationClip);

        callCancelableDelayed(durationSec, completion.callAction);
    }

    public override string getTag() {
        return "MOVE";
    }

    public override float getTotalExecutionTimeSec() {
        return durationSec;
    }

}
