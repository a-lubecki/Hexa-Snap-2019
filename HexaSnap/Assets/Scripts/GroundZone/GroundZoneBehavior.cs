/**
 * Hexa Snap
 * © Aurélien Lubecki 2019
 * All Rights Reserved
 */

using UnityEngine;

public class GroundZoneBehavior : MonoBehaviour {


	void OnTriggerEnter2D(Collider2D collider) {

		if (!Constants.GAME_OBJECT_NAME_ITEM.Equals(collider.name)) {
			return;
		}

		ItemBehavior itemBehavior = collider.gameObject.GetComponent<ItemBehavior>();
        itemBehavior.item.destroy(ItemDestroyCause.System);
	}

}
