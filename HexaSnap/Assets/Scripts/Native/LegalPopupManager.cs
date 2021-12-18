/**
 * Hexa Snap
 * © Aurélien Lubecki 2019
 * All Rights Reserved
 */

using System;
using UnityEngine;


public class LegalPopupManager {
    

    public static void show() {
        
        TrackingManager.instance.trackEvent(T.Event.SHOW_POPUP_LEGAL);

        GameHelper.Instance.getNativePopupManager().show(
            null,
            null,
            Tr.get("P.Close"),
            null,
            new string[] {
                Tr.get("P3a.Terms"),
                Tr.get("P3a.Privacy"),
                Tr.get("P3a.LegalInfo")
            },
            new Action[] {
                () => {
                    
                    TrackingManager.instance.trackEvent(T.Event.REDIRECT_TERMS);

                    Application.OpenURL(Constants.URL_SITE_HEXASNAP_TERMS);
                },
                () => {

                    TrackingManager.instance.trackEvent(T.Event.REDIRECT_PRIVACY);

                    Application.OpenURL(Constants.URL_SITE_HEXASNAP_PRIVACY);
                },
                () => {

                    TrackingManager.instance.trackEvent(T.Event.REDIRECT_LEGAL);

                    Application.OpenURL(Constants.URL_SITE_HEXASNAP_LEGAL);
                }
            }
        );

    }

}