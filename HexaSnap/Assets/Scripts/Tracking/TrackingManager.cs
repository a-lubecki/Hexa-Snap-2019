/**
 * Hexa Snap
 * © Aurélien Lubecki 2019
 * All Rights Reserved
 */

using System;
using Firebase.Analytics;


public class TrackingManager {


    //singleton
    public static readonly TrackingManager instance = new TrackingManager();


    private TrackingManager() {
    }

    public void setEnabled(bool isEnabled) {
        FirebaseAnalytics.SetAnalyticsCollectionEnabled(isEnabled);
    }

    public void updateUserId(string userId) {

        if (userId == null) {
            //ignore
            return;
        }

        FirebaseAnalytics.SetUserId(userId);
    }

    public void setUserProperty(string name, int property) {

        setUserProperty(name, property.ToString());
    }

    public void setUserProperty(string name, string property) {

        if (name == null || name.Length <= 0) {
            throw new ArgumentException();
        }

        FirebaseAnalytics.SetUserProperty(name, property);
    }

    public void trackEvent(string name) {
        
        prepareEvent(name).track();
    }

    public TrackingEvent prepareEvent(string name) {
        
        return new TrackingEvent(name);
    }

}

