/**
 * Hexa Snap
 * © Aurélien Lubecki 2019
 * All Rights Reserved
 */

using System;


public class QueueElementDelay : BaseCharacterQueueElement  {


    private IQueueDelayListener delayListener;

    public float delaySec { get; private set; }


    public QueueElementDelay(CharacterAnimatorQueue queue, IQueueDelayListener delayListener, float delaySec) : base(queue) {

        if (delaySec <= 0) {
            throw new ArgumentException();
        }

        this.delayListener = delayListener;
        this.delaySec = delaySec;
    }

    public void setDelayListener(IQueueDelayListener delayListener) {
        this.delayListener = delayListener;
    }

    protected override void processEnqueuedElement() {
        //do nothing
    }

    protected override void processSelectedElement(OneShotDelayedAction completion) {

        if (delayListener != null) {
            delayListener.onDelayWait(queue);
        }

        completion.anticipateCall(true);

        callCancelableDelayed(delaySec, completion.callAction);
    }

    public override string getTag() {
        return "DELAY:" + delaySec;
    }

    public override float getTotalExecutionTimeSec() {
        return delaySec;
    }

}
