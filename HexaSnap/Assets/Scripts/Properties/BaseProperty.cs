/**
 * Hexa Snap
 * © Aurélien Lubecki 2019
 * All Rights Reserved
 */

using System;


public abstract class BaseProperty<T> {


    public string key { get; private set; }


    public BaseProperty(string key) {
        
        if (string.IsNullOrEmpty(key)) {
            throw new ArgumentException();
        }

        this.key = key;
    }


    public abstract bool has();

    public abstract T get();

    public abstract T put(T value);

}
