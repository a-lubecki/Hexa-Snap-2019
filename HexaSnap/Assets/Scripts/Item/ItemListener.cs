/**
 * Hexa Snap
 * © Aurélien Lubecki 2019
 * All Rights Reserved
 */

using UnityEngine;


public interface ItemListener : BaseModelListener {

	void onItemTypeChange(Item item);

    void onFallingItemCollide(Item item, Vector3 currentPos);

	void onSnappedItemClick(Item item);

    void onItemSelectableChange(Item item);

	void onItemSelect(Item item);

	void onItemChangeZone(Item item);

    void onItemDestroyRequest(Item item, bool wasSnapped, ItemDestroyCause cause);

}

