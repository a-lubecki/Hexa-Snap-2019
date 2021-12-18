/**
 * Hexa Snap
 * © Aurélien Lubecki 2019
 * All Rights Reserved
 */

using System;
using System.Collections.Generic;

[Serializable]
public class PropertiesSaveData {


    protected Dictionary<string, bool> propertiesBool;
    protected Dictionary<string, int> propertiesInt;
    protected Dictionary<string, DateTime> propertiesDateTime;
    protected Dictionary<string, string> propertiesString;


    public PropertiesSaveData(PropertyManager propertiesManager) {

        if (propertiesManager == null) {
			throw new ArgumentException();
		}

        propertiesBool = propertiesManager.getPropertiesBool();
        propertiesInt = propertiesManager.getPropertiesInt();
        propertiesDateTime = propertiesManager.getPropertiesDateTime();
        propertiesString = propertiesManager.getPropertiesString();
    }

    public Dictionary<string, bool> getPropertiesBool() {
        return new Dictionary<string, bool>(propertiesBool);
    }

    public Dictionary<string, int> getPropertiesInt() {
        return new Dictionary<string, int>(propertiesInt);
    }

    public Dictionary<string, DateTime> getPropertiesDateTime() {
        return new Dictionary<string, DateTime>(propertiesDateTime);
    }

    public Dictionary<string, string> getPropertiesString() {
        return new Dictionary<string, string>(propertiesString);
    }

}

