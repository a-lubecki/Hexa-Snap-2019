/**
 * Hexa Snap
 * Copyright © 2017 Aurélien Lubecki
 * All Rights Reserved
 */

package com.hexasnap.utils;

import android.app.Activity;
import android.content.Intent;
import android.net.Uri;
import androidx.core.content.FileProvider;
import android.text.TextUtils;

import java.io.File;

public class NativeShareManager {

    public static void share(CharSequence chooserTitle, CharSequence subject,
                             CharSequence message, CharSequence url, String imagePath) {

        Activity activity = NativeUtils.getContext();

        Intent sendIntent = new Intent();

        sendIntent.setAction(Intent.ACTION_SEND);

        if (TextUtils.isEmpty(imagePath)) {
            sendIntent.setType("text/plain");
        } else {
            sendIntent.setType("image/png");
        }

        sendIntent.putExtra(Intent.EXTRA_SUBJECT, subject);

        if (!TextUtils.isEmpty(message) || !TextUtils.isEmpty(url)) {

            CharSequence text;
            if (TextUtils.isEmpty(message)) {
                text = url;
            } else if (TextUtils.isEmpty(url)) {
                text = message;
            } else {
                text = message + "\n" + url;
            }

            sendIntent.putExtra(Intent.EXTRA_TEXT, text);
        }


        if (!TextUtils.isEmpty(imagePath)) {

            File imageFileToShare = new File(imagePath);

            if (imageFileToShare.exists()) {

                Uri uri = FileProvider.getUriForFile(activity,
                        activity.getString(R.string.SHARE_PROVIDER),
                        imageFileToShare);

                sendIntent.addFlags(Intent.FLAG_GRANT_READ_URI_PERMISSION);

                sendIntent.putExtra(Intent.EXTRA_STREAM, uri);
            }
        }

        Intent chooser = Intent.createChooser(sendIntent, chooserTitle);
        activity.startActivity(chooser);
    }

}
