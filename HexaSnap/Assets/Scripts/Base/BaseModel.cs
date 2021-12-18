/**
 * Hexa Snap
 * © Aurélien Lubecki 2019
 * All Rights Reserved
 */

using System;
using System.Collections.Generic;

public abstract class BaseModel {


	public readonly int id;

	private HashSet<BaseModelListener> listeners = new HashSet<BaseModelListener>();


	protected BaseModel() {
        id = Constants.newRandomPositiveInt();
	}

	public IEnumerator<BaseModelListener> getListenersEnumerator() {
		return listeners.GetEnumerator();
	}

	protected void notifyListeners(Action<BaseModelListener> action) {

		if (action == null) {
			throw new ArgumentException();
		}

		//create a copy of the listeners to avoid any concurrent modification
		List<BaseModelListener> listenersCopy = new List<BaseModelListener>(listeners);

		foreach (BaseModelListener listener in listenersCopy) {
			action(listener);
		}
	}

	public void addListener(BaseModelListener listener) {

		if (listener == null) {
			throw new ArgumentException();
		}

        if (listeners.Contains(listener)) {
            //already added
            return;
        }

		listeners.Add(listener);
	}

	public void removeListener(BaseModelListener listener) {

		if (listener == null) {
			throw new ArgumentException();
		}

		listeners.Remove(listener);
	}

	public void clearListeners() {

		listeners.Clear();
	}


}

