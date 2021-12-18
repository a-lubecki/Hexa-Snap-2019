/**
 * Hexa Snap
 * © Aurélien Lubecki 2019
 * All Rights Reserved
 */

using System.Collections.Generic;


public class CharacterAnimationSpeech : BaseCharacterAnimation {


    private readonly List<string> texts;
    public readonly CharacterBubblePosition bubblePos;


    public CharacterAnimationSpeech(List<string> texts, CharacterBubblePosition bubblePos) : base(CharacterTimeline.SPEECH) {

        //defensive copy
        this.texts = new List<string>(texts);
        this.bubblePos = bubblePos;
    }

    public override void addToQueue(CharacterAnimatorQueue queue) {

        foreach (string text in texts) {
            queue.enqueue(new QueueElementSpeech(queue, this, text));
        }
    }

}
