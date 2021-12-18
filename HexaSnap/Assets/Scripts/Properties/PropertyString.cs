/**
 * Hexa Snap
 * © Aurélien Lubecki 2019
 * All Rights Reserved
 */


public class PropertyString : BaseProperty<string> {
    

    public PropertyString(string key) : base(key) {
    }

    public override bool has() {
        return PropertyManager.Instance.hasString(key);
    }

    public override string get() {
        return PropertyManager.Instance.findString(key);
    }

    public override string put(string value) {
        PropertyManager.Instance.putString(key, value);
        return value;
    }

}
