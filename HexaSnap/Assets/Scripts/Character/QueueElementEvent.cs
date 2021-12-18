/**
 * Hexa Snap
 * © Aurélien Lubecki 2019
 * All Rights Reserved
 */

using System;


public class QueueElementEvent : BaseCharacterQueueElement  {


    private Action<QueueElementEvent, Action> actionToProcess;


    public QueueElementEvent(CharacterAnimatorQueue queue, Action<QueueElementEvent, Action> actionToProcess) : base(queue) {
        
        this.actionToProcess = actionToProcess ?? throw new ArgumentException();
    }

    protected override void processEnqueuedElement() {
        //do nothing
    }

    protected override void processSelectedElement(OneShotDelayedAction completion) {

        completion.anticipateCall(true);

        actionToProcess(this, completion.callAction);
    }

    public override string getTag() {
        return "EVENT";
    }

    public override float getTotalExecutionTimeSec() {
        return 0;
    }

}
