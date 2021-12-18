/**
 * Hexa Snap
 * © Aurélien Lubecki 2019
 * All Rights Reserved
 */

using System.Collections.Generic;
using UnityEngine;


public class UniqueDisplaySpeechesManager : MonoBehaviour {


    private HashSet<string> tags = new HashSet<string>();


    public void addTags(string[] tags) {

        if (tags == null) {
            return;
        }

        this.tags.UnionWith(tags);
    }

    public bool addTag(string tag) {

        if (string.IsNullOrEmpty(tag)) {
            Debug.LogWarning("Try to add null tag in UniqueDisplaySpeechesManager");
            return false;
        }

        return tags.Add(tag);
    }

    public bool hasTag(string tag) {

        return tags.Contains(tag);
    }

    public string[] getTags() {

        string[] res = new string[tags.Count];
        tags.CopyTo(res);

        return res;
    }

}
