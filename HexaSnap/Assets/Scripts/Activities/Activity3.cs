/**
 * Hexa Snap
 * © Aurélien Lubecki 2019
 * All Rights Reserved
 */

using UnityEngine;
using UnityEngine.UI;


public class Activity3 : BaseUIActivity {


    private Text textLevel;
    private RawImage imageRequiredHexacoins;
    private Text textRequiredHexacoins;
    private Text textHexacoins;

    private MenuButtonBehavior buttonMinusMinus;
    private MenuButtonBehavior buttonMinus;
    private MenuButtonBehavior buttonPlus;
    private MenuButtonBehavior buttonPlusPlus;

    private MenuButtonBehavior buttonStart;
    private MenuButtonBehavior buttonShop;

    private int requiredHexacoins = 0;
    private int chosenLevel = 1;


    protected override MarkerBehavior getCurrentMarkerForInit(MarkerManager markerManager) {
        return markerManager.markerCLevelChoice;
    }

    protected override string[] getPrefabNamesToLoad() {
        return new string[] { "Activity3" };
    }

    protected override Line newPushLine(BaseActivity next) {

        if (next is Activity4) {

            return new Line(
                clickedMenuButton.transform.position,
                (next as Activity4).getTitleLinePos(),
                SegmentThickness.LARGE,
                1,
                true);
        }

        if (next is Activity23) {

            return new Line(
                clickedMenuButton.transform.position,
                next.markerRef.posSafeAreaBottomLeft,
                SegmentThickness.LARGE,
                0.5f,
                true);
        }

        return null;
    }

    protected override string getTitleForInit() {
        return Tr.get("Activity3.Title");
    }

    protected override bool hasDefaultHexacoinWallet() {
        return true;
    }

    protected override CharacterSituation getFirstCharacterSituation(bool isFirstResume) {

        //when the player has unlocked the hardcore mode, display specific speech once only
        if (gameManager.isArcadeHarcoreModeUnlocked()) {

            if (!GameHelper.Instance.getUniqueDisplaySpeechesManager().hasTag("3.HardcoreNew")) {

                return new CharacterSituation()
                    .enqueueTr("3.HardcoreNew")
                    .enqueueUniqueDisplay("3.HardcoreNew");
            }

            return new CharacterSituation()
                .enqueueTr("3.Hardcore")
                .enqueueUniqueDisplay("3.Hardcore");
        }

        return new CharacterSituation()
            .enqueueTr("3.Default");
    }

    protected override void onCreate() {
        base.onCreate();

        updateText("TextStartLevel", Tr.get("Activity3.Text.Level"));

        textLevel = findChildTransform("TextLevel").GetComponent<Text>();
        imageRequiredHexacoins = findChildTransform("ImageRequiredHexacoins").GetComponent<RawImage>();
        textRequiredHexacoins = updateText("TextRequiredHexacoins", Tr.get("Activity3.Text.RequiredHexacoins"));
        textHexacoins = findChildTransform("TextHexacoins").GetComponent<Text>();

        buttonMinusMinus = createButtonGameObject(
            this,
            "PosMinusMinus",
            MenuButtonSmall.newButtonDefault(
                "MenuButton.MinusMinus"
            )
        );

        buttonMinus = createButtonGameObject(
            this,
            "PosMinus",
            MenuButtonSmall.newButtonDefault(
                "MenuButton.Minus"
            )
        );

        buttonPlus = createButtonGameObject(
            this,
            "PosPlus",
            MenuButtonSmall.newButtonDefault(
                "MenuButton.Plus"
            )
        );

        buttonPlusPlus = createButtonGameObject(
            this,
            "PosPlusPlus",
            MenuButtonSmall.newButtonDefault(
                "MenuButton.PlusPlus"
            )
        );

        buttonStart = createButtonGameObject(
            this,
            "PosStart",
            MenuButtonIcon.newButtonDefault(
                Tr.get("Activity3.Button.Start"),
                "MenuButton.Play"
            )
        );

        buttonShop = createButtonGameObject(
            this,
            "PosShop",
            MenuButtonIcon.newButtonDefault(
                Tr.get("Activity3.Button.Shop"),
                "MenuButton.Shop"
            )
        );
    }

    protected override void onPreResume() {
        base.onPreResume();

        chosenLevel = 1;

        //udpdate max level selectable
        updateLevel(chosenLevel);

        //show badge for advanced players
        if (gameManager.maxArcadeLevel >= 3) {
            buttonShop.setBadgeValue(ShopItem.getNbFreeAvailableItems());
        } else {
            buttonShop.setBadgeText(null);
        }
    }


    private void updateLevel(int level) {

        int maxReachedLevel = gameManager.maxArcadeLevel;
        //cap level to avoid infinite playing with pay to win
        if (maxReachedLevel > 100) {
            maxReachedLevel = 100;
        }

		if (level < 1) {
			chosenLevel = 1;
        } else if (level > maxReachedLevel) {
			chosenLevel = maxReachedLevel;
		} else {
			chosenLevel = level;
		}

        requiredHexacoins = getNbHexacoinsForLevel(chosenLevel);

        //limit the level if the player can't afford it
        int maxLevelForHexacoins = getMaxLevelForHexacoins(requiredHexacoins);

		if (chosenLevel > maxLevelForHexacoins) {

            chosenLevel = maxLevelForHexacoins;

            //track button click
            TrackingManager.instance.prepareEvent(T.Event.HEXACOIN_PAY_START)
                           .add(T.Param.NB_HEXACOINS, requiredHexacoins)
                           .add(T.Param.ORIGIN, getActivityName())
                           .add(T.Param.REASON, T.Value.PAY_REASON_BOOST_LEVEL)
                           .track();
        
            //propose to buy hexacoins if the player has not enough coins
            push(new Activity22());
		}

		textLevel.text = chosenLevel.ToString();
		textHexacoins.text = requiredHexacoins.ToString();

		bool visible = (requiredHexacoins > 0);
        imageRequiredHexacoins.enabled = visible;
        textRequiredHexacoins.enabled = visible;
        textHexacoins.enabled = visible;

        buttonMinusMinus.menuButton.setEnabled(chosenLevel > 1);
        buttonMinus.menuButton.setEnabled(chosenLevel > 1);

        if (chosenLevel >= maxReachedLevel) {
            buttonPlus.menuButton.setEnabled(false);
            buttonPlusPlus.menuButton.setEnabled(false);
        } else {
            buttonPlus.menuButton.setEnabled(true);
            buttonPlusPlus.menuButton.setEnabled(true);
        }
	}

    private int getMaxLevelForHexacoins(int hexacoins) {

        int res = 0;

        if (!gameManager.isArcadeHarcoreModeUnlocked()) {
            // 1 => 20
            res = hexacoins + 1;

        } else {
            // 21 => 100+
            res = (hexacoins + 1) * 10;

            int maxReachedLevel = gameManager.maxArcadeLevel;
            if (res > maxReachedLevel) {
                res = maxReachedLevel;
            }
        }

        return res;
    }

    private int getNbHexacoinsForLevel(int level) {

        int res = 0;
        
        bool hardcoreUnlocked = gameManager.isArcadeHarcoreModeUnlocked();

        if (!hardcoreUnlocked) {
            // level 1 => 20 : 0 => 19 hexacoins
            res = level - 1;

        } else {
            // level 21 => 100+ : 0 => 9+ hexacoins
            res = (int) (Mathf.Floor(level - 1) / 10);
        }

        //limit the level if the player can't afford it
        int playerHexacoins = GameHelper.Instance.getHexacoinsWallet().nbHexacoins;

        if (res > playerHexacoins) {
            return playerHexacoins;
        }

        return res;
    }

    protected override void onButtonClick(MenuButtonBehavior menuButton) {

        if (menuButton == buttonMinusMinus) {

            updateLevel(1);

        } else if (menuButton == buttonMinus) {

			updateLevel(chosenLevel - 1);

		} else if (menuButton == buttonPlus) {
            
			updateLevel(chosenLevel + 1);

		} else if (menuButton == buttonPlusPlus) {
            
			updateLevel(int.MaxValue);

		} else if (menuButton == buttonStart) {
            
			if (requiredHexacoins > 0) {
                gameManager.payHexacoins(requiredHexacoins, getActivityName(), T.Value.PAY_REASON_BOOST_LEVEL);
			}

            BundlePush4a b = new BundlePush4a {
                nextLevel = chosenLevel
            };

            push(new Activity4a().setBundlePush(b));

		} else if (menuButton == buttonShop) {

			push(new Activity23());

		} else {

			base.onButtonClick(menuButton);
		}
    }

}

