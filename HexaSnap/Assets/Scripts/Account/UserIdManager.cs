/**
 * Hexa Snap
 * © Aurélien Lubecki 2019
 * All Rights Reserved
 */

using System;
using System.Text;
using System.Security.Cryptography;
using System.Linq;
using UnityEngine;


public class UserIdManager {


    //singleton
    public static readonly UserIdManager Instance = new UserIdManager();


    private string anonymousUserId;

    public string getUserId() {

        var loggedUserId = LoginManager.Instance.getFirebaseUserId();
        if (loggedUserId != null) {
            return loggedUserId;
        }

        return getAnonymousUserId();
    }

    private string getAnonymousUserId() {

        if (anonymousUserId != null) {
            return anonymousUserId;
        }

        anonymousUserId = retrieveAnonymousUserId();

        return anonymousUserId;
    }

    private string retrieveAnonymousUserId() {

        //retrieve from cache
        var id = Prop.userId.get();

        if (id == null) {
            
            //if not saved, generate a new id then save it
            id = "a_" + Guid.NewGuid().ToString("N");

            //add to cache
            Prop.userId.put(id);
        }

        return id;
    }

    public string getPublicReferrer() {

        //transform string for tracking
        var userId = getUserId();

        var hash = new SHA1Managed().ComputeHash(Encoding.UTF8.GetBytes(userId));

        string anonymisedId = string.Concat(hash.Select(b => b.ToString("x2")));

        return anonymisedId.Substring(0, 10);
    }

}

