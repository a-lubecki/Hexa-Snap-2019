/**
 * Hexa Snap
 * © Aurélien Lubecki 2019
 * All Rights Reserved
 */

using System;
using UnityEngine;
using UnityEngine.UI;


public class Activity10c : BaseDialogActivity {
		

	public static readonly int NB_ITEMS_TO_CHOOSE = 3;

	public static readonly int POP_CODE_CHOSEN_BONUS = 4587;


    private Text textTitle;
    private Text textSelectedItem;
    private MenuButtonBehavior buttonValidate;

    private MenuButtonItemBonus selectedButton;


	protected override string[] getPrefabNamesToLoad() {
		return new string[] { "Activity10c" };
	}

	protected override string getDialogAnimationName() {
		return "ActivityDialog.BonusChoice";
	}


	protected override void onCreate() {

		BundlePush10c b = (BundlePush10c) bundlePush;

		if (b.itemsToChoose == null) {
			throw new InvalidOperationException();
		}
		if (b.itemsToChoose.Length != NB_ITEMS_TO_CHOOSE) {
			throw new InvalidOperationException();
		}

		base.onCreate();

        textTitle = updateText("TextDialogTitle", Tr.get("Activity10c.Title"));

		createButtonGameObject(
            this,
			"PosItem0",
			new MenuButtonItemBonus(
				GameHelper.Instance.getBonusManager(),
				b.itemsToChoose[0]
			)
		);

        createButtonGameObject(
            this,
			"PosItem1",
			new MenuButtonItemBonus(
				GameHelper.Instance.getBonusManager(),
				b.itemsToChoose[1]
			)
		);

        createButtonGameObject(
            this,
			"PosItem2",
			new MenuButtonItemBonus(
				GameHelper.Instance.getBonusManager(),
				b.itemsToChoose[2]
			)
        );

        textSelectedItem = updateText("TextSelectedItem", "");

        buttonValidate = createButtonGameObject(
            this,
            "PosValidate",
            MenuButtonIcon.newButtonDialog(
                Tr.get("Activity10c.Validate"),
                "MenuButton.Play"
            )
        );

        buttonValidate.menuButton.setVisible(false);
	}

	protected override void onButtonClick(MenuButtonBehavior menuButton) {

        if (menuButton != buttonValidate) {

            if (selectedButton == null) {

                //change the UI
                textTitle.text = "";
                buttonValidate.menuButton.setVisible(true);

            } else {

                //clear last selected
                selectedButton.setHighlighted(false);
                BaseModelBehavior.findTransform(selectedButton).localScale = Vector3.one;
            }

            //select the clicked bonus
            selectedButton = (menuButton.menuButton as MenuButtonItemBonus);
            selectedButton.setHighlighted(true);
            textSelectedItem.text = selectedButton.itemBonus.bonusType.description;

            BaseModelBehavior.findTransform(selectedButton).localScale = new Vector3(1.3f, 1.3f, 1.3f);

            return;
        }

        //validate the bonus :

        //show FX item select
        GameObjectPoolBehavior pool = GameHelper.Instance.getPool();
        GameObject goFx = pool.pickFXBonusSelectGameObject(selectedButton, menuButton.transform.position);

        Async.call(goFx.GetComponent<Animation>().clip.length, () => {
            pool.storeFXBonusSelectGameObject(goFx);
        });

        //pop with the selected item
        BundlePop10c b = new BundlePop10c {
            selectedItem = selectedButton.itemBonus
        };

		pop(POP_CODE_CHOSEN_BONUS, b);
	}

}

public struct BundlePush10c : BaseBundle {

	public ItemBonus[] itemsToChoose;

}

public struct BundlePop10c : BaseBundle {

	public ItemBonus selectedItem;

}
