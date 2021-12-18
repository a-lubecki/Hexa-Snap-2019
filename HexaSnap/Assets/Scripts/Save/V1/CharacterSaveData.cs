/**
 * Hexa Snap
 * © Aurélien Lubecki 2019
 * All Rights Reserved
 */

using System;

[Serializable]
public class CharacterSaveData {


    protected string[] uniqueDisplayTags;

    public CharacterSaveData(GameManager gameManager) {

		if(gameManager == null) {
			throw new ArgumentException();
		}

        uniqueDisplayTags = GameHelper.Instance.getUniqueDisplaySpeechesManager().getTags();
    }

    public string[] getUniqueDisplayTags() {
        return uniqueDisplayTags;
    }

}

