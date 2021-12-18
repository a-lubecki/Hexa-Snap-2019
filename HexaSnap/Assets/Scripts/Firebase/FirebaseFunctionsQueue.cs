/**
 * Hexa Snap
 * © Aurélien Lubecki 2019
 * All Rights Reserved
 */

using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class FirebaseFunctionsQueue {


    private static readonly string COROUTINE_TAG_SEND_DELAYED = "functionQueueSendDelayed";


    private readonly FirebaseFunctionsManager manager;

    private readonly List<FirebaseFunctionCall> queue = new List<FirebaseFunctionCall>();

    private FirebaseFunctionCall processingCall;

    private bool isProcessingSendDelayed;


    public FirebaseFunctionsQueue(FirebaseFunctionsManager manager) {

        if (manager == null) {
            throw new ArgumentNullException();
        }

        this.manager = manager;
    }


    public void prepareCall(FirebaseFunctionCall call) {

        Debug.Log("FirebaseFunctionsQueue.prepareCall : " + call.methodName);

        var forceProcessTrigger = false;

        //find a similar call to merge
        var foundCallPos = queue.FindIndex((c) => c.methodName.Equals(call.methodName));

        if (foundCallPos >= 0) {

            Debug.Log("FirebaseFunctionsQueue.prepareCall => strategy = " + call.mergeStrategy);

            //remove first if found
            if (call.mergeStrategy == FirebaseFunctionCallMergeStrategy.REMOVE_PREVIOUS) {
                
                queue.RemoveAt(foundCallPos);

                if (foundCallPos == 0) {
                    //if the next item to process is removed, force trigger to update timer
                    forceProcessTrigger = true;
                }
            }
        }

        //add the new call to the end of the queue
        queue.Add(call);

        if (call.isPrior) {

            //call everything in queue now rather than wait, then this call at the end
            queue.ForEach(c => c.isPrior = true);

            forceProcessTrigger = true;
        }

        if (forceProcessTrigger || !isProcessingSendDelayed) {
            //trigger sending
            processSendDelayed();
        }
    }

    public bool isProcessingCall() {
        return processingCall != null;
    }
    
    protected void processSendDelayed() {

        if (queue.Count <= 0) {
            //no calls to process
            return;
        }

        if (isProcessingCall()) {
            //already processing call
            return;
        }

        var call = queue[0];

        Debug.Log("FirebaseFunctionsQueue.processSendDelayed : " + call.methodName + ", isPrior=" + call.isPrior);

        stopProcessingSendDelayed();

        //wait for 3 sec before sending call except if the call is prior
        var nbSecToWait = 3;

        if (call.isPrior) {
            nbSecToWait = 0;
        }

        //wait for 3 sec then process
        startProcessingSendDelayed(nbSecToWait);
    }

    private void startProcessingSendDelayed(int nbSecToWait) {

        if (isProcessingSendDelayed) {
            //already sendng
            return;
        }

        isProcessingSendDelayed = true;

        Async.call(processSend(nbSecToWait), COROUTINE_TAG_SEND_DELAYED);
    }

    private void stopProcessingSendDelayed() {

        isProcessingSendDelayed = false;

        Async.cancel(COROUTINE_TAG_SEND_DELAYED);
    }

    private IEnumerator processSend(int nbSecToWait) {

        yield return new WaitForSeconds(nbSecToWait);

        isProcessingSendDelayed = false;

        if (queue.Count <= 0) {
            //no calls to process
            yield break;
        }

        if (isProcessingCall()) {
            //already processing call
            yield break;
        }

        processingCall = queue[0];
        queue.RemoveAt(0);

        Debug.Log("FirebaseFunctionsQueue.processSend : " + processingCall.methodName + " => start");

        var processingCallRef = processingCall;

        processingCall.processCall(() => {

            if (processingCallRef != processingCall) {
                //current call has changed because of timeout
                return;
            }

            Debug.Log("FirebaseFunctionsQueue.processSend : " + processingCall.methodName + " => completion");

            //on finish, set as not processing
            processingCall = null;

            //stop timeout as the call succeeded
            stopProcessingSendDelayed();

            //trigger the next call
            processSendDelayed();
        });

        //manage timeout
        yield return new WaitForSeconds(15);

        if (processingCallRef != processingCall) {
            //current call has been processed
            yield break;
        }

        Debug.Log("FirebaseFunctionsQueue.processSend : " + processingCall.methodName + " => timeout");

        processingCall.onError?.Invoke(new TimeoutException());

        //set as not processing any more
        processingCall = null;

        //trigger the next call if the call timed out
        processSendDelayed();
    }

}

