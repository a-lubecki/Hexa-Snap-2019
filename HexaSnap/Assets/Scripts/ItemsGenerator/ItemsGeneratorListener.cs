/**
 * Hexa Snap
 * © Aurélien Lubecki 2019
 * All Rights Reserved
 */

public interface ItemsGeneratorListener : BaseModelListener {

	void onItemsGeneratorStart(ItemsGenerator itemsGenerator);

	void onItemsGeneratorStop(ItemsGenerator itemsGenerator);

	void onItemsGeneratorAddItem(ItemsGenerator itemsGenerator, Item item);

	void onItemsGeneratorRemoveItem(ItemsGenerator itemsGenerator, Item item);

	void onItemsGeneratorDequeueItem(ItemsGenerator itemsGenerator, Item item);

	void onItemsGeneratorClearItems(ItemsGenerator itemsGenerator);

}

