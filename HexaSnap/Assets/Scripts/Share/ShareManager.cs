/**
 * Hexa Snap
 * © Aurélien Lubecki 2019
 * All Rights Reserved
 */

using System;
using System.Threading.Tasks;
using UnityEngine;
using Firebase.DynamicLinks;
using Firebase.Extensions;


public class ShareManager {


    private static readonly string PARAM_REFERRER = "referrer";


    public static readonly ShareManager instance = new ShareManager();

    private ShareManager() {
        
    }

    public void onOpenFromDynamicLink(Uri url) {

        string referrer = extractParam(url, PARAM_REFERRER);

        //track the dynamic link opening
        TrackingEvent e = TrackingManager.instance.prepareEvent(T.Event.OPEN_FROM_LINK);

        if (!string.IsNullOrEmpty(referrer)) {
            //referrer was found
            e.add(T.Param.REFERRER, referrer);
        }

        e.track();
    }

    private static string extractParam(Uri url, string paramName) {

        var query = url.Query;
        if (string.IsNullOrEmpty(query)) {
            //no params
            return null;
        }

        if (query.StartsWith("?", StringComparison.Ordinal)) {
            query = query.Substring(1);
        }

        var paramList = query.Split('&');
        if (paramList == null) {
            return null;
        }

        foreach (var p in paramList) {

            if (p.StartsWith(paramName + "=", StringComparison.Ordinal)) {
                //found
                return p.Substring(paramName.Length + 1, p.Length - paramName.Length - 1);
            }
        }
     
        //not found in params
        return null;
    }

    public void retrieveShareUrl(Action<string> completion) {

        //check if the referrer is the same as the one used to generate the store dynamic link
        string referrer = UserIdManager.Instance.getPublicReferrer();
        if (referrer.Equals(Prop.referrerForShareUrl.get())) {
            
            string localShareUrl = Prop.shareUrl.get();
            if (localShareUrl != null) {

                //the link is found and the referrer used to generate it is the same
                completion(localShareUrl);
                return;
            }
        }

        //else generate a short url through the remote

        var isUrlRetrieved = false;

        var components = new DynamicLinkComponents(newDynamicLinkUrl(referrer), Constants.DYNAMIC_LINK_DOMAIN) {
            AndroidParameters = new AndroidParameters(Constants.APP_PACKAGE),
            IOSParameters = new IOSParameters(Constants.APP_PACKAGE),
            SocialMetaTagParameters = new SocialMetaTagParameters() {
                Title = Constants.DYNAMIC_LINK_SOCIAL_TITLE,
                Description = Constants.DYNAMIC_LINK_SOCIAL_DESCRIPTION,
                ImageUrl = new Uri(Constants.DYNAMIC_LINK_SOCIAL_IMAGE_URL)
            },
            GoogleAnalyticsParameters = new GoogleAnalyticsParameters() {
                Source = Constants.DYNAMIC_LINK_GA_SOURCE,
                Medium = Constants.DYNAMIC_LINK_GA_MEDIUM,
                Campaign = referrer
            }
        };

        var options = new DynamicLinkOptions { PathLength = DynamicLinkPathLength.Unguessable };
        DynamicLinks.GetShortLinkAsync(components, options).ContinueWithOnMainThread((task) => {

            if (task.IsCanceled) {
                Debug.LogError("GetShortLinkAsync was canceled.");
                return;
            }

            //mark as done to avoid calling the completion twice
            isUrlRetrieved = true;

            onDynamicLinkTaskCompleted(task, referrer, completion);
        });

        //if url was not retrieved from remote before 5sec, use the default dynamic link
        Async.call(5, () => {

            if (isUrlRetrieved) {
                //already retrieved
                return;
            }

            //mark as done to avoid calling the completion twice
            isUrlRetrieved = true;

            completion(Constants.URL_DYNAMIC_LINK_FALLBACK);
        });

    }

    private Uri newDynamicLinkUrl(string referrer) {
        
        return new Uri("https://hexasnap.com?" + PARAM_REFERRER + "=" + referrer);
    }

    private void onDynamicLinkTaskCompleted(Task<ShortDynamicLink> task, string referrer, Action<string> completion) {

        if (task.IsFaulted) {

            Debug.LogError("GetShortLinkAsync encountered an error: " + task.Exception);

            //call fallback
            completion(Constants.URL_DYNAMIC_LINK_FALLBACK);
            return;
        }

        //short link has been created
        var link = task.Result;

        //debug warning
        foreach (var w in link.Warnings) {
            Debug.LogWarning(w);
        }

        var shareUrl = link.Url.OriginalString;

        Debug.LogFormat("Generated short link {0}", shareUrl);

        //add url to local properties
        Prop.shareUrl.put(shareUrl);
        Prop.referrerForShareUrl.put(referrer);

        //done
        completion(shareUrl);
    }

}

