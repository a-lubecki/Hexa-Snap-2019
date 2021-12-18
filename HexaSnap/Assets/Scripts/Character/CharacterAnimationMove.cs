/**
 * Hexa Snap
 * © Aurélien Lubecki 2019
 * All Rights Reserved
 */

using System;


public class CharacterAnimationMove : BaseCharacterAnimation {


    public readonly string nameMoveAnimation;


    public CharacterAnimationMove(string nameMoveAnimation) : base(CharacterTimeline.MOVE) {

        if (string.IsNullOrEmpty(nameMoveAnimation)) {
            throw new ArgumentException();
        }

        this.nameMoveAnimation = nameMoveAnimation;
    }

    public override void addToQueue(CharacterAnimatorQueue queue) {
        queue.enqueue(new QueueElementMove(queue, this));
    }

}
