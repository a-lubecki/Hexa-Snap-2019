/**
 * Hexa Snap
 * © Aurélien Lubecki 2019
 * All Rights Reserved
 */

using System.Collections.Generic;
using UnityEngine;


public class InputActionSelectItem : InGameInputAction {
	
	public override KeyCode[] getDefaultActionKeys() {
		return null;
	}

	public override int[] getDefaultActionMouseButtons() {
		return new int[] {
			0
		};
	}

	public override bool areActionMouseButtonsReleased() {
		return true;
	}


    private Vector2? initialTouchPos;

    public override bool isActionDone() {

        if (!base.execute()) {
            return false;
        }

        //assign initial pos
        if (hasStartedClicking()) {
            initialTouchPos = getClickPosition();
        }

        if (!initialTouchPos.HasValue) {
            //invalid if no initial point
            return false;
        }

        return true;
    }

    public override bool execute() {

		if (!base.execute()) {
			return false;
		}
        
        if (!hasFinishedClicking()) {
            return false;
        }

        if (!initialTouchPos.HasValue) {
            //invalid if no initial point
            return false;
        }

        Vector2? clickPosition = getClickPosition();
        if (!clickPosition.HasValue) {
            return false;
        }

        if (Vector2.Distance(clickPosition.Value, initialTouchPos.Value) > 0.5) {
            //dragged, not clicked
            return false;
        }

        Collider2D[] hitColliders = Physics2D.OverlapPointAll(clickPosition.Value);

		foreach (Collider2D c in hitColliders) {

            var possibleItem = c.GetComponent<ItemBehavior>();
            if (possibleItem != null && possibleItem.isOverlappingItemForSelection(hitColliders)) {
                //clicking on an available item
                possibleItem.item.onItemClick();
                return true;
            }
		}

		return false;
    }

}

