/**
 * Hexa Snap
 * © Aurélien Lubecki 2019
 * All Rights Reserved
 */

using System;
using System.Collections.Generic;
using UnityEngine;


public sealed class TrLanguageManager {

    private static SystemLanguage[] supportedLanguages = {
        SystemLanguage.English, //first is default language
        SystemLanguage.French
    };


    //singleton
    public static readonly TrLanguageManager Instance = new TrLanguageManager();


    public SystemLanguage currentLanguage { get; private set; }


    private TrLanguageManager() {

        //init default language
        var l = getDefaultLanguage();

        if (!hasLanguage(l)) {
            //ignore
            Debug.LogWarning("Language not managed : " + l);
            return;
        }

        currentLanguage = l;
    }

    public SystemLanguage getDefaultLanguage() {

#if DEBUG && UNITY_EDITOR
        var newLanguage = HexaSnapEditor.getLanguageToChange();
        if ("en".Equals(newLanguage)) {
            return SystemLanguage.English;
        }

        if ("fr".Equals(newLanguage)) {
            return SystemLanguage.French;
        }
#endif

        if (hasLanguage(Application.systemLanguage)) {
            return Application.systemLanguage;
        }

        //default language of the game is EN
        return supportedLanguages[0];
    }

    public bool hasLanguage(SystemLanguage language) {

        foreach (SystemLanguage l in supportedLanguages) {
            if (l == language) {
                return true;
            }
        }

        return false;
    }

    public Dictionary<string, string[]> loadTranslations(string filePrefix) {

        Dictionary<string, string[]> res = new Dictionary<string, string[]>();

        TextAsset textAsset = GameHelper.Instance.loadTextAsset(Constants.PATH_TRANSLATIONS + filePrefix + currentLanguage);
        if (textAsset == null) {
            throw new InvalidOperationException("Failed to load translation file : " + currentLanguage);
        }

        //clean previous translations
        res.Clear();

        string[] newTranslations = textAsset.text.Split(new string[] { Environment.NewLine }, StringSplitOptions.RemoveEmptyEntries);

        string key;
        List<string> noBlankTrList = new List<string>();

        int nbLines = newTranslations.Length;
        for (int i = 0; i < nbLines; i++) {

            //replace the \\n by a real line break
            string[] tr = newTranslations[i].Replace("\\n", "\n").Replace("\r", "").Split('\t');

            if (tr.Length < 2) {
                throw new InvalidOperationException("No value found for translation : " + tr.Length + " for " + currentLanguage);
            }

            key = tr[0];

            if (key.Length <= 0) {
                throw new InvalidOperationException("Empty key for " + currentLanguage);
            }

            if (res.ContainsKey(key)) {
                throw new InvalidOperationException("Duplicate key " + key + " for " + currentLanguage);
            }

            //remove the blank parts from tr
            noBlankTrList.Clear();

            int nbTr = tr.Length;
            for (int part = 1 ; part < nbTr ; part++) {

                string trPart = tr[part];
                if (trPart.Length <= 0) {
                    break;
                }

                noBlankTrList.Add(trPart);
            }

            int nbNoBlankTr = noBlankTrList.Count;
            if (nbNoBlankTr < 1) {
                throw new InvalidOperationException("No translation found : " + key + " for " + currentLanguage);
            }

            res.Add(key, noBlankTrList.ToArray());
        }

        return res;
    }
    
}

