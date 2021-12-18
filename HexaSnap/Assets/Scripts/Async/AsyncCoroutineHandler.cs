/**
 * Hexa Snap
 * © Aurélien Lubecki 2019
 * All Rights Reserved
 */

using System;
using System.Collections;
using UnityEngine;


public class AsyncCoroutineHandler {


    private IEnumerator enumerator;
    private Coroutine coroutine;

    public string coroutineTag { get; private set; }


    public AsyncCoroutineHandler(IEnumerator enumerator, string coroutineTag = null) {

        this.enumerator = enumerator ?? throw new ArgumentException();

        if (string.IsNullOrEmpty(coroutineTag)) {
            this.coroutineTag = generateNewCoroutineTag();
        } else {
            this.coroutineTag = coroutineTag;
        }
    }

    public bool isStarted() {
        return (enumerator == null);
    }

    public bool isFinished() {
        return isStarted() && (coroutine == null);
    }

    public void start(MonoBehaviour b) {

        if (isStarted()) {
            return;
        }

        //free before to avoid twice possible starts
        var enumeratorRef = enumerator;
        enumerator = null;

        coroutine = b.StartCoroutine(callCoroutineThenFree(enumeratorRef));
    }

    private IEnumerator callCoroutineThenFree(IEnumerator enumeratorToCall) {

        //delay the call in case several interdependent coroutines mut be registered then started at the same time
        yield return new WaitForSeconds(0);

        //call the enumerator line by line to catch a possible exception
        while (true) {

            object enumeratorReturn = null;

            try {
                if (!enumeratorToCall.MoveNext()) {
                    break;
                }

                enumeratorReturn = enumeratorToCall.Current;

            } catch (Exception e) {

                //capture the stacktrace before freeing the pointers
                var stackTrace = "Error in async call delegate";

                markAsFinished();

                //rethrow the original stack trace for debugging purpose
                throw new Exception(stackTrace, e);
            }

            if (enumeratorReturn != null) {
                yield return enumeratorReturn;
            }
        }

        markAsFinished();
    }

    public void stop(MonoBehaviour b) {

        if (!isStarted()) {
            //disable future starts
            markAsFinished();
            return;
        }

        if (coroutine == null) {
            //already finished
            return;
        }

        //free before to avoid twice possible stops
        var coroutineRef = coroutine;
        markAsFinished();

        b.StopCoroutine(coroutineRef);
    }

    private void markAsFinished() {

        //mark as finished
        enumerator = null;
        coroutine = null;
    }


    private static int coroutineTagCounter = 0;

    private static string generateNewCoroutineTag() {

        coroutineTagCounter++;

        return "c" + coroutineTagCounter;
    }

}

