/**
 * Hexa Snap
 * © Aurélien Lubecki 2019
 * All Rights Reserved
 */

using System;
using System.Collections.Generic;
using System.Linq;


public class QueueJoinManager : IQueueJoinListener  {


    private HashSet<CharacterAnimatorQueue> registeredJoiningQueues = new HashSet<CharacterAnimatorQueue>();
    private HashSet<CharacterAnimatorQueue> waitingQueues = new HashSet<CharacterAnimatorQueue>();

    private bool isResolvingJoin = false;



    void IQueueJoinListener.onJoinEnqueue(CharacterAnimatorQueue queue) {

        if (isResolvingJoin) {
            //avoid changing data during resolve
            return;
        }

        registeredJoiningQueues.Add(queue);
    }

    bool IQueueJoinListener.onJoinWait(CharacterAnimatorQueue queue) {

        if (isResolvingJoin) {
            //avoid changing data during resolve
            return true;
        }

        if (!registeredJoiningQueues.Contains(queue)) {
            UnityEngine.Debug.LogWarning("A join must be registered before waiting to join");
        }

        waitingQueues.Add(queue);

        bool allQueuesJoined = true;

        //check if all registered queues are waiting (all joined)
        foreach (var q in registeredJoiningQueues) {
            
            if (!waitingQueues.Contains(q)) {
                allQueuesJoined = false;
                break;
            }
        }

        if (allQueuesJoined) {

            isResolvingJoin = true;

            resolveJoin();

            isResolvingJoin = false;
        }

        return allQueuesJoined;
    }

    private void resolveJoin() {

        //copy before clearing data
        HashSet<CharacterAnimatorQueue> queuesCopy = new HashSet<CharacterAnimatorQueue>(registeredJoiningQueues);

        //keep the queues that have another join for next time
        registeredJoiningQueues = new HashSet<CharacterAnimatorQueue>(registeredJoiningQueues.Where(q => q.getNbElements(typeof(QueueElementJoin)) > 1));
        waitingQueues.Clear();

        //remove all join first elem from the queue
        foreach (CharacterAnimatorQueue q in queuesCopy) {
            q.dequeue();
        }
    }

}
