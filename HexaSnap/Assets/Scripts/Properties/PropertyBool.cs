/**
 * Hexa Snap
 * © Aurélien Lubecki 2019
 * All Rights Reserved
 */


public class PropertyBool : BaseProperty<bool> {
    

    public PropertyBool(string key) : base(key) {
    }

    public override bool has() {
        return PropertyManager.Instance.hasBool(key);
    }

    public override bool get() {
        return PropertyManager.Instance.findBool(key, false);
    }

    public override bool put(bool value) {
        PropertyManager.Instance.putBool(key, value);
        return value;
    }

}
