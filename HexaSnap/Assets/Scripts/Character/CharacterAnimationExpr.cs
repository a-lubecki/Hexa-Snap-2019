/**
 * Hexa Snap
 * © Aurélien Lubecki 2019
 * All Rights Reserved
 */

using System;


public class CharacterAnimationExpr : BaseCharacterAnimation {


    public readonly string nameImageFace;
    public readonly float durationSec;


    public CharacterAnimationExpr(string nameImageFace, float durationSec) : base(CharacterTimeline.EXPRESSION) {

        if (string.IsNullOrEmpty(nameImageFace)) {
            throw new ArgumentException();
        }
        if (durationSec <= 0) {
            throw new ArgumentException();
        }

        this.nameImageFace = nameImageFace;
        this.durationSec = durationSec;
    }

    public override void addToQueue(CharacterAnimatorQueue queue) {
        queue.enqueue(new QueueElementExpr(queue, this));
    }

}
