/**
 * Hexa Snap
 * © Aurélien Lubecki 2019
 * All Rights Reserved
 */

using System;


public class FirebaseFunctionCallException : Exception {


    public readonly int errorCode;


    public FirebaseFunctionCallException(int errorCode) : base() {
        this.errorCode = errorCode;
    }

}

