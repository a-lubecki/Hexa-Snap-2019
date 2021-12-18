/**
 * Hexa Snap
 * © Aurélien Lubecki 2019
 * All Rights Reserved
 */

using System;
using UnityEngine;


public class NativePopupManager : MonoBehaviour {


    private Action cancelAction;
    private Action[] actions;


    public void show(string title, string message, string cancelMessage, Action cancelAction) {

        show(title, message, cancelMessage, cancelAction, null, (Action[]) null);
    }

    public void show(string title, string message, string cancelMessage, Action cancelAction, string buttonMessage, Action action) {

        show(title, message, cancelMessage, cancelAction, new string[] { buttonMessage }, new Action[] { action });
    }

    public void show(string title, string message, string cancelMessage, Action cancelAction, string[] buttonsMessage, Action[] actions) {

        if (buttonsMessage == null) {

            if (actions != null && actions.Length > 0) {
                throw new ArgumentException();
            }

        } else if (actions == null) {

            if (buttonsMessage != null && buttonsMessage.Length > 0) {
                throw new ArgumentException();
            }

        } else if (actions.Length != buttonsMessage.Length) {
            throw new ArgumentException();
        }

        //pause the game if playing
        var gameManager = GameHelper.Instance.getGameManager();
        if (gameManager.isGamePlaying) {
            //trigger pause menu
            gameManager.getNullableInGameActivity()?.pauseGame();
        }


        if (string.IsNullOrEmpty(cancelMessage)) {
            cancelMessage = Tr.get("P.Close");
        }

        this.cancelAction = cancelAction;
        this.actions = actions;


        if (actions == null || actions.Length <= 1) {

            string button = null;
            if (buttonsMessage != null && buttonsMessage.Length == 1) {
                button = buttonsMessage[0];
            }

            NativeCallsManager.showAlertDialog(title, message, cancelMessage, button);

        } else {

            NativeCallsManager.showActionSheetDialog(title, message, cancelMessage, buttonsMessage);
        }
    }

    public void onPopupButtonClick(string buttonId) {

        //callback called from native code
        Action currentAction = null;

        int pos = int.Parse(buttonId);
        if (pos < 0) {
            
            currentAction = cancelAction;

        } else {
            
            if (actions != null) {
                currentAction = actions[pos];
            }
        }

        //free actions array for memory management before calling the delegate
        actions = null;
        cancelAction = null;

        //call delegate
        currentAction?.Invoke();
    }

}
