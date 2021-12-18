/**
 * Hexa Snap
 * © Aurélien Lubecki 2019
 * All Rights Reserved
 */

using System;
using System.Collections.Generic;
using UnityEngine;


public class DispatchQueue : MonoBehaviour {


    private static DispatchQueue instance;

    public static DispatchQueue Instance {
        get {
            return instance;
        }
    }


    private List<Action> pending = new List<Action>();

    private void Awake() {
        
        if (instance != null && instance != this) {
            Destroy(gameObject);
        } else {
            instance = this;
        }
    }

    private void Update() {
        InvokePending();
    }

    public void Invoke(Action fn) {

        lock (pending) {
            pending.Add(fn);
        }
    }

    private void InvokePending() {

        lock (pending) {

            while (pending.Count > 0) {

                var action = pending[0];

                pending.RemoveAt(0);

                //invoke after removing from queue to avoid infinite loop
                action();
            }
        }
    }

}