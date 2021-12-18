/**
 * Hexa Snap
 * © Aurélien Lubecki 2019
 * All Rights Reserved
 */

using System;


public class QueueElementUniqueDisplay : BaseCharacterQueueElement {


    public string tag { get; private set; }


    public QueueElementUniqueDisplay(CharacterAnimatorQueue queue, string tag) : base(queue) {

        if (string.IsNullOrEmpty(tag)) {
            throw new ArgumentException();
        }

        this.tag = tag;
    }

    protected override void processEnqueuedElement() {
        //do nothing
    }

    protected override void processSelectedElement(OneShotDelayedAction completion) {

        bool saved = GameHelper.Instance.getUniqueDisplaySpeechesManager().addTag(tag);
        if (saved) {
            SaveManager.Instance.saveCharacter();
        }
    }

    public override string getTag() {
        return "UNIQUE_DISPLAY:END";
    }

    public override float getTotalExecutionTimeSec() {
        return 0;
    }

}
