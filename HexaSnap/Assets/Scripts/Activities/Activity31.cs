/**
 * Hexa Snap
 * © Aurélien Lubecki 2019
 * All Rights Reserved
 */

public class Activity31 : BaseDialogActivity {


    private MenuButtonBehavior buttonInfo;
    private MenuButtonBehavior buttonCancel;
    private MenuButtonBehavior buttonContinue;

    private MenuButtonBehavior buttonCancelSync;

    private bool isSyncing = false;

    private string originActivityName = "";


    protected override string[] getPrefabNamesToLoad() {
        return new string[] { "Activity31", "Activity31Sync" };
    }

    protected override string getDialogAnimationName() {

        if (isSyncing) {
            return "ActivityDialog.LoginSync";
        }

        return "ActivityDialog.LoginLegal";
    }

    protected override void onCreate() {
        base.onCreate();

        originActivityName = ((BundlePush31)bundlePush).originActivityName;
                                  
        buttonInfo = createButtonGameObject(
            this,
            "PosInfo",
            MenuButtonSmall.newButtonDefault(
                "MenuButton.Info"
            )
        );

        updateText("TextLegal", Tr.get("Activity31.Text.Legal"));

        buttonCancel = createButtonGameObject(
            this,
            "PosCancel",
            MenuButtonIcon.newButtonDialog(
                Tr.get("Activity31.Button.Cancel"),
                "MenuButton.Close"
            )
        );

        buttonContinue = createButtonGameObject(
            this,
            "PosContinue",
            MenuButtonIcon.newButtonDialog(
                Tr.get("Activity31.Button.Continue"),
                "MenuButton.Play"
            )
        );


        updateText("TextSync", Tr.get("Activity31.Text.Sync"));

        buttonCancelSync = createButtonGameObject(
            this,
            "PosCancelSync",
            MenuButtonIcon.newButtonDialog(
                Tr.get("Activity31.Button.Cancel"),
                "MenuButton.Close"
            )
        );

        TrackingManager.instance.prepareEvent(T.Event.LOGIN_START)
                       .add(T.Param.ORIGIN, originActivityName)
                       .track();
    }

    protected override void onDialogShown() {
        base.onDialogShown();

        setSyncing(false);
    }

    protected override void onButtonClick(MenuButtonBehavior menuButton) {

        if (menuButton == buttonInfo) {

            LegalPopupManager.show();

        } else if (menuButton == buttonCancel) {

            pop();

            TrackingManager.instance.prepareEvent(T.Event.LOGIN_CANCEL)
                           .add(T.Param.ORIGIN, originActivityName)
                           .track();

        } else if (menuButton == buttonContinue) {

            tryLogin();

            TrackingManager.instance.prepareEvent(T.Event.LOGIN_ACCEPT)
                           .add(T.Param.ORIGIN, originActivityName)
                           .track();

        } else if (menuButton == buttonCancelSync) {

            if (isSyncing) {

                LoginManager.Instance.cancelFacebookLogin();

                setSyncing(false);

                TrackingManager.instance.prepareEvent(T.Event.LOGIN_CANCEL)
                               .add(T.Param.ORIGIN, originActivityName)
                               .track();
            }

        } else {

            base.onButtonClick(menuButton);
        }

    }

    private void tryLogin() {

        setSyncing(true);

        LoginManager.Instance.logInToFacebook(originActivityName, () => {

            setSyncing(false);

            //when an advanced player logs in before onboarding, disable the onboarding without tracking the onbording ending
            if (!gameManager.hasPassedOnboarding && gameManager.maxArcadeLevel > 1) {
                gameManager.setOnboardingAsPassed(false);
            }

            //disable sync whithout showing the login popup (no flicking), to select the correct pop anim
            isSyncing = false;
            pop();

        }, () => {

            setSyncing(false);

            if (!LoginManager.Instance.isLoggedInFacebook()) {

                GameHelper.Instance.getNativePopupManager().show(
                    Tr.get("P1a.Title"),
                    Tr.get("P1a.Message"),
                    null,
                    null
                );

            } else if (!LoginManager.Instance.hasSyncDataOnce) {

                GameHelper.Instance.getNativePopupManager().show(
                    Tr.get("P1b.Title"),
                    Tr.get("P1b.Message"),
                    Tr.get("P1b.Later"),
                    pop,
                    Tr.get("P1b.Sync"),
                    () => {
                        //retry
                        onButtonClick(buttonContinue);
                    }
                );
            }

        });
    }

    private void setSyncing(bool syncing) {

        getLoadedGameObject("Activity31").SetActive(!syncing);
        getLoadedGameObject("Activity31Sync").SetActive(syncing);

        if (syncing) {

            //assign first so that playAnimationDialog will select the correct anim
            isSyncing = true;
            playAnimationDialog(false);

        } else {

            //stop before assign so that the sync anim is selected to stop
            stopAnimationDialog();
            isSyncing = false;
        }
    }

}

public class BundlePush31 : BaseBundle {

    public string originActivityName;

}
