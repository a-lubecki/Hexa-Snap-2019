/**
 * Hexa Snap
 * © Aurélien Lubecki 2019
 * All Rights Reserved
 */

using System;
using System.Collections.Generic;
using UnityEngine;


public class BaseGameObjectPoolBehavior : MonoBehaviour {

	//use a dictionary to avoid iterating over all the pool when searching for a gameobject
	private Dictionary<int, List<GameObject>> pool = new Dictionary<int, List<GameObject>>();

	//a tmp pool used to store the gameobjects processing for deinit, does not contains a lot of objects most of the time
	private List<GameObject> poolBuffer = new List<GameObject>();


	private List<GameObject> getPool(int tag) {

		if (pool.ContainsKey(tag)) {
			return pool[tag];
		}

		//else add a new list
		List<GameObject> newList = new List<GameObject>();
		pool.Add(tag, newList);

		return newList;
	}

	private GameObject createNewGameObject(string prefabName, string gameObjectName) {

		GameObject prefab = GameHelper.Instance.loadPrefabAsset(prefabName);

		GameObject newObject = GameObject.Instantiate(prefab) as GameObject;
		newObject.name = gameObjectName;

		return newObject;
	}


	protected GameObject pickGameObject(int tag, string prefabName, string gameObjectName, Transform parentTransform, bool isPosLocal, Vector3 pos) {

		if (string.IsNullOrEmpty(prefabName)) {
			throw new ArgumentException();
		}
		if (string.IsNullOrEmpty(gameObjectName)) {
			throw new ArgumentException();
		}

		GameObject res;

		List<GameObject> pool = getPool(tag);

		if (pool.Count <= 0) {

			res = createNewGameObject(prefabName, gameObjectName);

		} else {

			res = pool[pool.Count - 1];
			pool.RemoveAt(pool.Count - 1);
		}

		//change the position before setting the game object active to avoid collisions triggers
		res.transform.SetParent(parentTransform);

        if (isPosLocal) {

            RectTransform rectTransform = res.GetComponent<RectTransform>();
            if (rectTransform) {
                //UI only
                rectTransform.anchoredPosition = new Vector2(pos.x, pos.y);

            } else {
                //3D object
                res.transform.localPosition = pos;
            }

		} else {

            //3D object
            res.transform.position = pos;
		}

		res.transform.localScale = Vector3.one;

        res.SetActive(true);
        
        return res;
	}

	protected void storeGameObject(int tag, GameObject gameObject) {

		if (gameObject == null) {
			throw new ArgumentException();
		}

		List<GameObject> pool = getPool(tag);
		if (pool.Contains(gameObject)) {
			//already in pool
			return;
		}

		if (poolBuffer.Contains(gameObject)) {
			//already processing deinit
			return;
		}

		poolBuffer.Add(gameObject);

		gameObject.transform.SetParent(transform);
		gameObject.transform.position = Vector3.zero;
		gameObject.transform.localRotation = Quaternion.identity;

		gameObject.SetActive(false);

		//call the destruction on next frame to ensure all listeners are called
        Async.call(0.0001f, () => {

			BaseModelBehavior[] behaviors = gameObject.GetComponents<BaseModelBehavior>();
			if (behaviors != null) {

				foreach (BaseModelBehavior entityBehavior in behaviors) {
					entityBehavior.deinit();
				}
			}

			poolBuffer.Remove(gameObject);
			pool.Add(gameObject);
		});

	}


}

