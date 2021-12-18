/**
 * Hexa Snap
 * © Aurélien Lubecki 2019
 * All Rights Reserved
 */

using System;


public abstract class BaseCharacterQueueElement {


    public bool isSelected { get; protected set; }
    public bool isCanceled { get; protected set; }

    private string currentCoroutineTag = null;

    public CharacterAnimatorQueue queue { get; protected set; }


    public BaseCharacterQueueElement(CharacterAnimatorQueue queue) {

        this.queue = queue ?? throw new ArgumentException();
    }

    public void onEnqueue() {

        processEnqueuedElement();
    }

    protected abstract void processEnqueuedElement();

    /**
     * Select when the element becomes the first item of the queue
     */
    public void onDequeue(Action completion) {

        if (isSelected) {
            //already selected
            return;
        }

        if (isCanceled) {
            return;
        }

        isSelected = true;

        OneShotDelayedAction delayedAction = new OneShotDelayedAction(completion);

        processSelectedElement(delayedAction);

        if (!delayedAction.willBeCalled) {
            //if the completion wasn't or won't be called, call it here
            completion();
        }
    }

    public virtual void onCancel() {

        isCanceled = true;

        if (currentCoroutineTag != null) {
            Async.cancel(currentCoroutineTag);
        }
    }

    protected abstract void processSelectedElement(OneShotDelayedAction completion);

    public void callCancelableDelayed(float delaySec, Action action) {

        if (isCanceled) {
            return;
        }

        currentCoroutineTag = Async.call(delaySec, () => {

            if (isCanceled) {
                return;
            }

            action.Invoke();
        }, currentCoroutineTag);
    }

    public abstract string getTag();

    public abstract float getTotalExecutionTimeSec();

}
