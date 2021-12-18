/**
 * Hexa Snap
 * Copyright © 2017 Aurélien Lubecki
 * All Rights Reserved
 */

package com.hexasnap.utils;

import android.annotation.SuppressLint;
import android.app.Activity;
import android.content.Context;
import android.content.pm.PackageManager;
import android.content.pm.Signature;
import android.content.pm.SigningInfo;
import android.os.Build;
import android.util.Base64;

import com.unity3d.player.UnityPlayer;

import java.security.MessageDigest;
import java.util.ArrayList;
import java.util.List;

public class NativeUtils {

    public static int getTheme() {

        if (Build.VERSION.SDK_INT >= Build.VERSION_CODES.LOLLIPOP) {
            return android.R.style.Theme_Material_Light_Dialog;
        }

        return android.R.style.Theme_Holo_Dialog;
    }

    public static Activity getContext() {
        return UnityPlayer.currentActivity;
    }

    public static String getAppOrigin() {

        //obfuscated
    }

    @SuppressLint("PackageManagerGetSignatures")
    public static String[] getAppSignatures() {

        //obfuscated
    }

}
