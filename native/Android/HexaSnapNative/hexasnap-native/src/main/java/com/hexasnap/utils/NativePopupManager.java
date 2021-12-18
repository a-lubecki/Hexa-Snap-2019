/**
 * Hexa Snap
 * Copyright © 2017 Aurélien Lubecki
 * All Rights Reserved
 */

package com.hexasnap.utils;

import android.app.AlertDialog;
import android.content.DialogInterface;
import android.text.TextUtils;
import android.view.ContextThemeWrapper;
import android.view.KeyEvent;

import com.unity3d.player.UnityPlayer;


//jar found in : C:\Program Files\Unity\Editor\Data\PlaybackEngines\AndroidPlayer\Variations\mono\Release\Classes\classes.jar

public class NativePopupManager {


    private static void callNativeCallBack(DialogInterface dialog, int buttonPos) {

        dialog.dismiss();
        UnityPlayer.UnitySendMessage("MainScriptsManager", "onPopupButtonClick", String.valueOf(buttonPos));

    }


    private static DialogInterface.OnKeyListener keyListener = new DialogInterface.OnKeyListener() {

        @Override
        public boolean onKey(DialogInterface dialog, int keyCode, KeyEvent event) {

            if (keyCode == KeyEvent.KEYCODE_BACK) {
                callNativeCallBack(dialog, -1);
            }

            return false;
        }
    };


    public static void showAlertDialog(CharSequence title, CharSequence message, CharSequence negativeButtonText, CharSequence positiveButtonText) {

        AlertDialog.Builder dialog = new AlertDialog.Builder(new ContextThemeWrapper(NativeUtils.getContext(), NativeUtils.getTheme()));

        dialog.setTitle(title);
        dialog.setMessage(message);

        if (!TextUtils.isEmpty(positiveButtonText)) {

            dialog.setPositiveButton(positiveButtonText, new DialogInterface.OnClickListener() {

                @Override
                public void onClick(DialogInterface dialog, int which) {
                    callNativeCallBack(dialog, 0);
                }
            });
        }

        if (TextUtils.isEmpty(negativeButtonText)) {
            negativeButtonText = dialog.getContext().getText(android.R.string.ok);
        }

        dialog.setNegativeButton(negativeButtonText, new DialogInterface.OnClickListener() {

            @Override
            public void onClick(DialogInterface dialog, int which) {
                callNativeCallBack(dialog, -1);
            }
        });

        dialog.setOnKeyListener(keyListener);
        dialog.setCancelable(false);
        dialog.show();

    }

    public static void showActionSheetDialog(CharSequence title, CharSequence message, CharSequence negativeButtonText, CharSequence[] buttonTexts) {

        AlertDialog.Builder dialog = new AlertDialog.Builder(new ContextThemeWrapper(NativeUtils.getContext(), NativeUtils.getTheme()));

        dialog.setTitle(title);
        dialog.setMessage(message);

        if (buttonTexts != null) {

            dialog.setItems(buttonTexts, new DialogInterface.OnClickListener() {

                @Override
                public void onClick(DialogInterface dialog, int which) {
                    callNativeCallBack(dialog, which);
                }
            });
        }

        if (TextUtils.isEmpty(negativeButtonText)) {
            negativeButtonText = dialog.getContext().getText(android.R.string.cancel);
        }

        dialog.setNegativeButton(negativeButtonText, new DialogInterface.OnClickListener() {

            @Override
            public void onClick(DialogInterface dialog, int which) {
                callNativeCallBack(dialog, -1);
            }
        });

        dialog.setOnKeyListener(keyListener);
        dialog.setCancelable(false);
        dialog.show();

    }

}

