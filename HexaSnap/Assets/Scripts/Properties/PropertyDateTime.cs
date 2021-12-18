/**
 * Hexa Snap
 * © Aurélien Lubecki 2019
 * All Rights Reserved
 */

using System;


public class PropertyDateTime : BaseProperty<DateTime> {
    

    public PropertyDateTime(string key) : base(key) {
    }

    public override bool has() {
        return PropertyManager.Instance.hasDateTime(key);
    }

    public override DateTime get() {
        return PropertyManager.Instance.findDateTime(key, DateTime.Now);
    }

    public override DateTime put(DateTime value) {
        PropertyManager.Instance.putDateTime(key, value);
        return value;
    }

    public DateTime putNow() {
        return put(DateTime.Now);
    }

}
