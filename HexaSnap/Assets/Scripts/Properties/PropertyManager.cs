/**
 * Hexa Snap
 * © Aurélien Lubecki 2019
 * All Rights Reserved
 */

using System;
using System.Collections.Generic;


[Serializable]
public class PropertyManager {


    private static PropertyManager instance;

    private PropertyManager() { }

    public static PropertyManager Instance {

        get {
            if (instance == null) {
                instance = new PropertyManager();
            }
            return instance;
        }
    }


    protected Dictionary<string, bool> propertiesBool = new Dictionary<string, bool>();
    protected Dictionary<string, int> propertiesInt = new Dictionary<string, int>();
    protected Dictionary<string, DateTime> propertiesDateTime = new Dictionary<string, DateTime>();
    protected Dictionary<string, string> propertiesString = new Dictionary<string, string>();


    public void init(Dictionary<string, bool> propertiesBool, Dictionary<string, int> propertiesInt, 
                     Dictionary<string, DateTime> propertiesDateTime, Dictionary<string, string> propertiesString) {

        if (propertiesBool == null) {
            this.propertiesBool = new Dictionary<string, bool>();
        } else {
            this.propertiesBool = propertiesBool;
        }

        if (propertiesInt == null) {
            this.propertiesInt = new Dictionary<string, int>();
        } else {
            this.propertiesInt = propertiesInt;
        }

        if (propertiesDateTime == null) {
            this.propertiesDateTime = new Dictionary<string, DateTime>();
        } else {
            this.propertiesDateTime = propertiesDateTime;
        }

        if (propertiesString == null) {
            this.propertiesString = new Dictionary<string, string>();
        } else {
            this.propertiesString = propertiesString;
        }
    }

    public void putBool(string key, bool value) {

        if (key == null) {
            throw new ArgumentException();
        }

        propertiesBool[key] = value;

        save();
    }

    public bool hasBool(string key) {

        if (string.IsNullOrEmpty(key)) {
            return false;
        }

        return propertiesBool.ContainsKey(key);
    }

    public bool findBool(string key, bool defaultValue) {
        
        if (!hasBool(key)) {
            return defaultValue;
        }

        return propertiesBool[key];
    }

    public Dictionary<string, bool> getPropertiesBool() {
        return new Dictionary<string, bool>(propertiesBool);
    }

    public void putInt(string key, int value) {

        if (key == null) {
            throw new ArgumentException();
        }

        propertiesInt[key] = value;

        save();
    }

    public bool hasInt(string key) {

        if (string.IsNullOrEmpty(key)) {
            return false;
        }

        return propertiesInt.ContainsKey(key);
    }

    public int findInt(string key, int defaultValue) {

        if (!hasInt(key)) {
            return defaultValue;
        }

        return propertiesInt[key];
    }

    public Dictionary<string, int> getPropertiesInt() {
        return new Dictionary<string, int>(propertiesInt);
    }

    public void putDateTime(string key, DateTime value) {

        if (key == null) {
            throw new ArgumentException();
        }

        propertiesDateTime[key] = value;

        save();
    }

    public bool hasDateTime(string key) {

        if (string.IsNullOrEmpty(key)) {
            return false;
        }

        return propertiesDateTime.ContainsKey(key);
    }

    public DateTime findDateTime(string key, DateTime defaultValue) {

        if (!hasDateTime(key)) {
            return defaultValue;
        }

        return propertiesDateTime[key];
    }

    public Dictionary<string, DateTime> getPropertiesDateTime() {
        return new Dictionary<string, DateTime>(propertiesDateTime);
    }

    public void putString(string key, string value) {

        if (key == null) {
            throw new ArgumentException();
        }

        if (value == null) {
            propertiesString.Remove(key);
        } else {
            propertiesString[key] = value;
        }

        save();
    }

    public bool hasString(string key) {

        if (string.IsNullOrEmpty(key)) {
            return false;
        }

        return propertiesString.ContainsKey(key);
    }

    public string findString(string key) {

        if (!hasString(key)) {
            return null;
        }

        return propertiesString[key];
    }

    public Dictionary<string, string> getPropertiesString() {
        return new Dictionary<string, string>(propertiesString);
    }

    private void save() {

        GameSaverLocal.instance.saveProperties();

        //commit
        GameSaverLocal.instance.saveAllToFile();
    }

}

