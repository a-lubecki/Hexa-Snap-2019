/**
 * Hexa Snap
 * © Aurélien Lubecki 2019
 * All Rights Reserved
 */

using System;
using System.Collections.Generic;
using UnityEngine;

public class LineDrawersManager : MonoBehaviour {
		

	private List<LineDrawer> drawers = new List<LineDrawer>();

	void Update() {
        
		if (drawers.Count <= 0) {
			//optimize call
			return;
		}
		
		//copy the list to avoid concurrent modifications exceptions
		foreach (LineDrawer d in new List<LineDrawer>(drawers)) {
            d.valueInterpolator.update();
        }
        
	}


	public void register(LineDrawer d) {

		if (d == null) {
			throw new ArgumentException();
		}

		if (!drawers.Contains(d)) {
			drawers.Add(d);
		}
	}

	public void unregister(LineDrawer d) {

		if (d == null) {
			throw new ArgumentException();
		}

        drawers.Remove(d);
    }
   
}

