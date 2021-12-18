/**
 * Hexa Snap
 * © Aurélien Lubecki 2019
 * All Rights Reserved
 */


public class QueueElementJoin : BaseCharacterQueueElement  {


    private IQueueJoinListener joinListener;


    public QueueElementJoin(CharacterAnimatorQueue queue, IQueueJoinListener joinListener) : base(queue) {
        setJoinListener(joinListener);
    }

    public void setJoinListener(IQueueJoinListener joinListener) {
        this.joinListener = joinListener;
    }

    protected override void processEnqueuedElement() {

        if (joinListener == null) {
            //no listener, ignore
            return;
        }

        joinListener.onJoinEnqueue(queue);
    }

    protected override void processSelectedElement(OneShotDelayedAction completion) {

        if (joinListener == null) {
            //no listener, the join will be ignored and the elem will be dequeued normally
            return;
        }

        bool areAllJoined = joinListener.onJoinWait(queue);

        if (!areAllJoined) {
            
            //mark the completion as will-be-called but don't call it so that the elem will be stuck in the queue
            completion.anticipateCall(true);

            //the elem will be free from the queue the next time the select will be called => unselect it
            isSelected = false;
        }
    }

    public override string getTag() {
        return "JOIN";
    }

    public override float getTotalExecutionTimeSec() {
        return 0;
    }

}
