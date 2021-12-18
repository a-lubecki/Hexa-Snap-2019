/**
 * Hexa Snap
 * © Aurélien Lubecki 2019
 * All Rights Reserved
 */

using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using UnityEngine;


public class Async : MonoBehaviour {


    public static bool isRunning(string tag) {
        
        return GameHelper.Instance.getAsync().isCoroutineRunning(tag);
    }

    public static string call(float seconds, Action completion, string tag = null) {

        //save the current caller class::method in a field to debug some crashes
        //handleStackTraceForDebug(new StackTrace(true));

        return GameHelper.Instance.getAsync().callDelayed(seconds, completion, tag);
    }

    public static string call(IEnumerator enumerator, string tag = null) {

        //save the current caller class::method in a field to debug some crashes
        //handleStackTraceForDebug(new StackTrace(true));

        return GameHelper.Instance.getAsync().callCoroutine(enumerator, tag);
    }

    public static void cancel(string tag) {

        GameHelper.Instance.getAsync().cancelCoroutines(tag);
    }


    private Dictionary<string, AsyncCoroutineList> runningCoroutines = new Dictionary<string, AsyncCoroutineList>();//<tag, coroutines references>


    public bool isCoroutineRunning(string tag) {

        unregisterAllFinishedCoroutines();

        return runningCoroutines.ContainsKey(tag);
    }

    public string callDelayed(float delaySec, Action completion, string tag = null) {

        if (completion == null) {
            throw new Exception("Completion is null for call delayed");
        }

        //call the completion after the delay, if the delay is 0, the completion will still be called the next frame
        return callCoroutine(waitForSeconds(delaySec, completion), tag);
    }

    private IEnumerator waitForSeconds(float delaySec, Action completion) {

        if (delaySec > 0) {
            yield return new WaitForSeconds(delaySec);
        }

        completion.Invoke();
    }

    public string callCoroutine(IEnumerator enumerator, string tag = null) {

        if (enumerator == null) {
            throw new ArgumentException();
        }

        //clear before starting a new coroutine
        unregisterAllFinishedCoroutines();

        var h = new AsyncCoroutineHandler(enumerator, tag);

        registerRunningCoroutine(h);
        h.start(this);

        return h.coroutineTag;
    }

    public void cancelCoroutines(string tag) {

        if (string.IsNullOrEmpty(tag)) {
            //do nothing
            throw new ArgumentException();
        }

        if (runningCoroutines.ContainsKey(tag)) {

            foreach (var h in runningCoroutines[tag].extractRunningCoroutines()) {
                h.stop(this);
            }
        }

        //free the references
        unregisterAllFinishedCoroutines();
    }

    private void registerRunningCoroutine(AsyncCoroutineHandler h) {

        var tag = h.coroutineTag;

        AsyncCoroutineList list;

        if (runningCoroutines.ContainsKey(tag)) {
            list = runningCoroutines[tag];
        } else {
            list = new AsyncCoroutineList();
            runningCoroutines.Add(tag, list);
        }

        list.Add(h);
    }

    private void unregisterAllFinishedCoroutines() {

        if (runningCoroutines.Count <= 0) {
            //nothing to remove
            return;
        }
 
        var tagsToRemove = new HashSet<string>();

        foreach (var e in runningCoroutines) {

            var list = e.Value;
            list.removeFinishedCoroutines();

            //prepare to remove tag definitely if no more coroutines
            if (list.Count <= 0) {
                tagsToRemove.Add(e.Key);
            }
        }

        //free finished tags outside of the find loop to avoid concurrency
        foreach (var tag in tagsToRemove) {
            runningCoroutines.Remove(tag);
        }
    }

    /*
    //DEBUG CRASHES:

    private static int LAST_STACK_CALLER_POS = 1;

    private static string currentCoroutineCall = "";
    private static string lastCoroutineCall = "";

    public static string getCurrentCoroutineCall() {
        return currentCoroutineCall;
    }

    public static string getLastCoroutineCall() {
        return lastCoroutineCall;
    }

    private static void handleStackTraceForDebug(StackTrace st) {
        
        if (st.FrameCount <= LAST_STACK_CALLER_POS) {
            //cant access the caller
            return;
        }

        StackFrame currentFrame = st.GetFrame(0);
        StackFrame lastFrame = st.GetFrame(LAST_STACK_CALLER_POS);

        currentCoroutineCall = getFormattedFrame(currentFrame);
        lastCoroutineCall = getFormattedFrame(lastFrame);
    }

    private static string getFormattedFrame(StackFrame sf) {
        return sf.GetFileName() + " / " + sf.GetMethod() + " / " + sf.GetFileLineNumber();
    }
    */
}

