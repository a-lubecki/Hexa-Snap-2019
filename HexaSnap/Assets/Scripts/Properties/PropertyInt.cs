/**
 * Hexa Snap
 * © Aurélien Lubecki 2019
 * All Rights Reserved
 */


public class PropertyInt : BaseProperty<int> {
    

    public PropertyInt(string key) : base(key) {
    }

    public override bool has() {
        return PropertyManager.Instance.hasInt(key);
    }

    public override int get() {
        return PropertyManager.Instance.findInt(key, 0);
    }

    public override int put(int value) {
        PropertyManager.Instance.putInt(key, value);
        return value;
    }

    public int add(int value) {
        return put(get() + value);
    }

    public int increment() {
        return add(1);
    }

    public int putIfGreater(int value) {

        int currentValue = get();

        if (value > currentValue) {
            return put(value);
        }

        return currentValue;
    }

}
