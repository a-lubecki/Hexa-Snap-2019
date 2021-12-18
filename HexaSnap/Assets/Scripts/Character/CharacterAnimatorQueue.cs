/**
 * Hexa Snap
 * © Aurélien Lubecki 2019
 * All Rights Reserved
 */

using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;


public class CharacterAnimatorQueue : IQueueJoinListener, IQueueDelayListener {


    public bool isDequeuing { get; private set; }
    public float? startDequeueTime { get; private set; }

    private IAnimatorQueueListener listener;
    private IQueueJoinListener joinListener;
    private IQueueDelayListener delayListener;

    private LinkedList<BaseCharacterQueueElement> queue = new LinkedList<BaseCharacterQueueElement>();


    public void setListener(IAnimatorQueueListener listener) {

        this.listener = listener;
    }

    public void setJoinListener(IQueueJoinListener joinListener) {

        this.joinListener = joinListener;

        foreach (BaseCharacterQueueElement elem in queue) {

            if (elem is QueueElementJoin) {
                (elem as QueueElementJoin).setJoinListener(this);
            }
        }
    }

    public void setDelayListener(IQueueDelayListener delayListener) {

        this.delayListener = delayListener;

        foreach (BaseCharacterQueueElement elem in queue) {

            if (elem is QueueElementDelay) {
                (elem as QueueElementDelay).setDelayListener(this);
            }
        }
    }

    public void clear() {

        foreach (BaseCharacterQueueElement elem in queue) {
            elem.onCancel();
        }

        isDequeuing = false;

        queue.Clear();
    }

    public void enqueueUniqueDisplay(string tag) {

        if (string.IsNullOrEmpty(tag)) {
            throw new ArgumentException();
        }

        enqueue(new QueueElementUniqueDisplay(this, tag));
    }

    public int getNbElements(Type type = null) {

        if (type == null) {
            return queue.Count;
        }

        return queue.Count(elem => elem.GetType().Equals(type));
    }

    public void enqueue(BaseCharacterQueueElement elem) {

        if (elem == null) {
            throw new ArgumentException();
        }

        queue.AddLast(elem);

        elem.onEnqueue();
    }

    public void enqueueDelay(float delaySec) {

        if (delaySec <= 0) {
            throw new ArgumentException();
        }

        if (queue.Count > 1) {

            //merge the last delay with the new, only if the current delay was not started (not first element)
            BaseCharacterQueueElement lastElem = queue.Last();

            if (lastElem is QueueElementDelay) {
                //merge delays
                delaySec += (lastElem as QueueElementDelay).delaySec;
                queue.RemoveLast();
            }
        }

        enqueue(new QueueElementDelay(this, this, delaySec));
    }

    public void enqueueJoin() {
        
        enqueue(new QueueElementJoin(this, this));
    }

    public void enqueueEvent(Action action) {

        if (action == null) {
            throw new ArgumentException();
        }

        queue.AddLast(new QueueElementEvent(this, (elem, completion) => {
            
            action();
            completion();
        }));
    }

    public void dequeue() {
        
        if (!hasElements()) {
            //no elements to dequeue

            listener?.onNeedDefaultAnim(this);
            return;
        }

        isDequeuing = true;

        startDequeueTime = Time.realtimeSinceStartup;

        //select then dequeue if there is another elem
        queue.First().onDequeue(() => {
            
            endDequeue();

            if (hasElements()) {
                queue.RemoveFirst();
            }
        });
    } 

    private void endDequeue() {
        
        isDequeuing = false;

        startDequeueTime = null;

        //recursive call on the next frame to avoid recursive bug
        Async.call(0, dequeue);
    }

    public bool hasElements() {
        return (queue.Count > 0);
    }

    public float getRemainingTimeForCurrentElement() {

        if (!startDequeueTime.HasValue) {
            return 0;
        }

        if (!hasElements()) {
            return 0;
        }

        var res = (startDequeueTime.Value + queue.First().getTotalExecutionTimeSec()) - Time.realtimeSinceStartup;
        if (res < 0) {
            return 0;
        }

        return res;
    }

    public void skipCurrent() {

        if (!hasElements()) {
            return;
        }

        queue.First().onCancel();

        queue.RemoveFirst();

        endDequeue();
    }

    public void skipUntilNextJoinOrEnd() {

        while (hasElements() && !(queue.First() is QueueElementJoin)) {

            queue.First().onCancel();

            queue.RemoveFirst();
        }

        endDequeue();
    }

    void IQueueJoinListener.onJoinEnqueue(CharacterAnimatorQueue queue) {

        if (joinListener != null) {
            joinListener.onJoinEnqueue(queue);
        }
    }

    bool IQueueJoinListener.onJoinWait(CharacterAnimatorQueue queue) {

        if (listener != null) {
            listener.onNeedDefaultAnim(queue);
        }

        bool allJoined = false;
        if (joinListener != null) {
            allJoined = joinListener.onJoinWait(queue);
        }

        return allJoined;
    }

    void IQueueDelayListener.onDelayWait(CharacterAnimatorQueue queue) {

        if (listener != null) {
            listener.onNeedDefaultAnim(queue);
        }

        if (delayListener != null) {
            delayListener.onDelayWait(queue);
        }
    }

    public override string ToString() {

        string res = "";

        foreach (BaseCharacterQueueElement elem in queue) {
            res += "[" + elem.getTag() + "]";
        }

        return res;
    }

}
