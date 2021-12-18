/**
 * Hexa Snap
 * © Aurélien Lubecki 2019
 * All Rights Reserved
 */

using System;
using System.Linq;
using System.Collections.Generic;
using Firebase.Analytics;


public class TrackingEvent {


    private String name;

    private Dictionary<string, Parameter> parameters = new Dictionary<string, Parameter>();


    public TrackingEvent(string name) {

        if (name == null || name.Length <= 0) {
            throw new ArgumentException();
        }

        this.name = name;
    }

    public TrackingEvent add(string name, int value) {

        if (name == null || name.Length <= 0) {
            throw new ArgumentException();
        }

        parameters.Add(name, new Parameter(name, value));

        return this;
    }

    public TrackingEvent add(string name, string value) {

        if (name == null || name.Length <= 0) {
            throw new ArgumentException();
        }

        if (value == null) {
            remove(name);
        } else {
            parameters.Add(name, new Parameter(name, value));
        }

        return this;
    }

    public TrackingEvent remove(string name) {

        if (name == null || name.Length <= 0) {
            throw new ArgumentException();
        }

        parameters.Remove(name);

        return this;
    }

    public void track() {
        
        FirebaseAnalytics.LogEvent(name, parameters.Values.ToArray());
    }

}

