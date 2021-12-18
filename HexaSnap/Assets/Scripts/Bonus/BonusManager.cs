/**
 * Hexa Snap
 * © Aurélien Lubecki 2019
 * All Rights Reserved
 */

using System;
using System.Collections.Generic;
using UnityEngine;


public class BonusManager {

    //arcade (zone 1)
	public BonusType bonusTypeAdjacentWipeout { get; private set; }
	public BonusType bonusTypeRowWipeout { get; private set; }
	public BonusType bonusTypeSimilarWipeout { get; private set; }
    public BonusType bonusTypeProliferation { get; private set; }
    public BonusType bonusTypeInversion { get; private set; }
    public BonusType bonusTypeErosion { get; private set; }

    //arcade (zone 2) + time attack (zone 3)
    public BonusType bonusTypeExtension { get; private set; }
	public BonusType bonusTypeMultiplier { get; private set; }
    public BonusType bonusTypeLimitation { get; private set; }
    public BonusType bonusTypeDivider { get; private set; }

    //arcade (zone 3)
    public BonusType bonusTypeRandomBonus { get; private set; }
    public BonusType bonusTypeRandomMalus { get; private set; }

    //arcade (zone 4) + time attack (zone 2)
	public BonusType bonusTypeShortage { get; private set; }
	public BonusType bonusTypeSlowDown { get; private set; }
    public BonusType bonusTypeProfusion { get; private set; }
    public BonusType bonusTypeSpeedUp { get; private set; }

    //arcade (zone 5)
    public BonusType bonusTypeChoiceBonus { get; private set; }
    public BonusType bonusTypeChoiceMalus { get; private set; }

    //arcade (zone 6)
    public BonusType bonusTypeProgression { get; private set; }
    public BonusType bonusTypeRegression { get; private set; }

    //time attack (zone 1)
    public BonusType bonusType5SecondsMore { get; private set; }
    public BonusType bonusType10SecondsMore { get; private set; }
    public BonusType bonusType5SecondsLess { get; private set; }
    public BonusType bonusType10SecondsLess { get; private set; }

    //time attack (zone 4)
    public BonusType bonusType20SecondsMore { get; private set; }
    public BonusType bonusType30SecondsMore { get; private set; }
    public BonusType bonusType20SecondsLess { get; private set; }
    public BonusType bonusType30SecondsLess { get; private set; }

    
	private Dictionary<string, Sprite> spritesBonusByTag = new Dictionary<string, Sprite>();


    public BonusManager() {

		bonusTypeAdjacentWipeout = new BonusType(
			"ADJACENT_WIPEOUT",
			false,
			true,
            true,
			false,
			new BonusCommandAdjacentWipeout()
		);

		bonusTypeRowWipeout = new BonusType(
			"ROW_WIPEOUT",
			false,
			true,
            true,
			true,
			new BonusCommandRowWipeout()
		);

		bonusTypeSimilarWipeout = new BonusType(
			"SIMILAR_WIPEOUT",
			false,
			true,
            true,
			false,
			new BonusCommandSimilarWipeout()
		);

        bonusTypeProliferation = new BonusType(
            "PROLIFERATION",
            true,
            true,
            true,
            true,
            new BonusCommandProliferation()
        );

        bonusTypeInversion = new BonusType(
            "INVERSION",
            true,
            false,
            true,
            false,
            new BonusCommandInversion()
        );

        bonusTypeErosion = new BonusType(
            "EROSION",
            true,
            false,
            true,
            false,
            null
        );

        bonusTypeExtension = new BonusType(
            "EXTENSION",
            false,
            true,
            true,
            false,
            new BonusCommandExtension()
        );

        bonusTypeMultiplier = new BonusType(
            "MULTIPLIER",
            false,
            false,
            true,
            false,
            new BonusCommandMultiplier()
        );

        bonusTypeLimitation = new BonusType(
			"LIMITATION",
			true,
			true,
            true,
			false,
			new BonusCommandLimitation()
		);

		bonusTypeDivider = new BonusType(
			"DIVIDER",
			true,
			false,
            true,
            false,
            new BonusCommandDivider()
		);

        bonusTypeRandomBonus = new BonusType(
            "RANDOM_BONUS",
            false,
            true,
            true,
            false,
            new BonusCommandRandomBonus()
        );

        bonusTypeRandomMalus = new BonusType(
			"RANDOM_MALUS",
			true,
			true,
            true,
			false,
			new BonusCommandRandomMalus()
		);

		bonusTypeShortage = new BonusType(
			"SHORTAGE",
			false,
			false,
            true,
			false,
			null
		);

		bonusTypeSlowDown = new BonusType(
			"SLOW_DOWN",
			false,
			false,
            true,
			false,
            new BonusCommandSlowDown()
		);

        bonusTypeProfusion = new BonusType(
            "PROFUSION",
            true,
            false,
            true,
            false,
            null
        );

        bonusTypeSpeedUp = new BonusType(
            "SPEED_UP",
            true,
            false,
            true,
            false,
            new BonusCommandSpeedUp()
        );

        bonusTypeChoiceBonus = new BonusType(
            "CHOICE_BONUS",
            false,
            true,
            false,
            false,
            new BonusCommandChoiceBonus()
        );

        bonusTypeChoiceMalus = new BonusType(
			"CHOICE_MALUS",
			true,
			true,
            false,
			false,
			new BonusCommandChoiceMalus()
		);

		bonusTypeProgression = new BonusType(
			"PROGRESSION",
			false,
			true,
            false,
			false,
			new BonusCommandProgression()
		);

        bonusTypeRegression = new BonusType(
            "REGRESSION",
            true,
            true,
            false,
            false,
            new BonusCommandRegression()
        );

        bonusType5SecondsMore = new BonusType(
            "+5_SEC",
            false,
            true,
            true,
            false,
            new BonusCommandAddSecondsToTimer(5)
        );

        bonusType10SecondsMore = new BonusType(
            "+10_SEC",
            false,
            true,
            true,
            false,
            new BonusCommandAddSecondsToTimer(10)
        );

        bonusType5SecondsLess = new BonusType(
            "-5_SEC",
            true,
            true,
            true,
            false,
            new BonusCommandAddSecondsToTimer(-5)
        );

        bonusType10SecondsLess = new BonusType(
            "-10_SEC",
            true,
            true,
            true,
            false,
            new BonusCommandAddSecondsToTimer(-10)
        );

        bonusType20SecondsMore = new BonusType(
            "+20_SEC",
            false,
            true,
            true,
            false,
            new BonusCommandAddSecondsToTimer(20)
        );

        bonusType30SecondsMore = new BonusType(
            "+30_SEC",
            false,
            true,
            true,
            false,
            new BonusCommandAddSecondsToTimer(30)
        );

        bonusType20SecondsLess = new BonusType(
            "-20_SEC",
            true,
            true,
            true,
            false,
            new BonusCommandAddSecondsToTimer(-20)
        );

        bonusType30SecondsLess = new BonusType(
            "-30_SEC",
            true,
            true,
            true,
            false,
            new BonusCommandAddSecondsToTimer(-30)
        );


        BonusType[] allBonusTypes = new BonusType[] {
            bonusTypeAdjacentWipeout,
            bonusTypeRowWipeout,
            bonusTypeSimilarWipeout,
            bonusTypeProliferation,
            bonusTypeInversion,
            bonusTypeErosion,
            bonusTypeExtension,
            bonusTypeMultiplier,
            bonusTypeLimitation,
            bonusTypeDivider,
            bonusTypeRandomBonus,
            bonusTypeRandomMalus,
            bonusTypeShortage,
            bonusTypeSlowDown,
            bonusTypeProfusion,
            bonusTypeSpeedUp,
            bonusTypeChoiceBonus,
            bonusTypeChoiceMalus,
            bonusTypeProgression,
            bonusTypeRegression,
            bonusType5SecondsMore,
            bonusType10SecondsMore,
            bonusType5SecondsLess,
            bonusType10SecondsLess,
            bonusType20SecondsMore,
            bonusType30SecondsMore,
            bonusType20SecondsLess,
            bonusType30SecondsLess
        };

		foreach (BonusType type in allBonusTypes) {

			foreach (string tag in type.getAllTags()) {
				spritesBonusByTag.Add(tag, GameHelper.Instance.loadSpriteAsset(Constants.PATH_DESIGNS_BONUS + "Item.Bonus." + tag));
			}
		}

	}

	public Sprite getSpriteItemBonus(string tag) {

		if (tag == null) {
			throw new ArgumentException();
		}
		if (!spritesBonusByTag.ContainsKey(tag)) {
            throw new InvalidOperationException("Unknown bonus tag : " + tag);
		}

		return spritesBonusByTag[tag];
	}
    

}

