/**
 * Hexa Snap
 * © Aurélien Lubecki 2019
 * All Rights Reserved
 */

using System;


public abstract class BaseCharacterAnimation {


    public CharacterTimeline timeline { get; private set; }


    protected BaseCharacterAnimation(CharacterTimeline timeline) {

        this.timeline = timeline;
    }

    public abstract void addToQueue(CharacterAnimatorQueue queue);

}
