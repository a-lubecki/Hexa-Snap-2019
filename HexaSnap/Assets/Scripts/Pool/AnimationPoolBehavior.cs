/**
 * Hexa Snap
 * © Aurélien Lubecki 2019
 * All Rights Reserved
 */

using UnityEngine;
using System;
using System.Collections.Generic;

public class AnimationPoolBehavior : MonoBehaviour {

    //use a dictionary to avoid iterating over all the pool when searching for a gameobject
    private Dictionary<string, List<RuntimeAnimatorController>> pool = new Dictionary<string, List<RuntimeAnimatorController>>();


    private List<RuntimeAnimatorController> getPool(string tag) {

        if (pool.ContainsKey(tag)) {
            return pool[tag];
        }

        //else add a new list
        List<RuntimeAnimatorController> newList = new List<RuntimeAnimatorController>();
        pool.Add(tag, newList);

        return newList;
    }
    
    public RuntimeAnimatorController pickAnimatorController(string animName) {

        if (string.IsNullOrEmpty(animName)) {
            throw new ArgumentException();
        }

        RuntimeAnimatorController res;

        List<RuntimeAnimatorController> pool = getPool(animName);

        if (pool.Count <= 0) {

            res = GameHelper.Instance.loadAnimatorController(animName);

        } else {

            res = pool[pool.Count - 1];
            pool.RemoveAt(pool.Count - 1);
        }
        
        return res;
    }

    public void storeAnimatorController(RuntimeAnimatorController controller) {

        if (controller == null) {
            throw new ArgumentException();
        }
        
        List<RuntimeAnimatorController> pool = getPool(controller.name);
        if (pool.Contains(controller)) {
            //already in pool
            return;
        }
        
        pool.Add(controller);
    }

}
