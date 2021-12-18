/**
 * Hexa Snap
 * © Aurélien Lubecki 2019
 * All Rights Reserved
 */

using System;
using System.Collections.Generic;
using UnityEngine;


public sealed class Tr {

    public static string get(string key) {
        return Tr.Instance.getTranslation(key);
    }

    public static string get(string key, int pos) {
        return Tr.Instance.getTranslation(key, pos);
    }

    public static string[] arr(string key) {
        return Tr.Instance.getTranslationArray(key);
    }

    public static string[] arr(string key, int pos, int nb) {
        return Tr.Instance.getTranslationArray(key, pos, nb);
    }


	//singleton
	public static readonly Tr Instance = new Tr();


    private SystemLanguage currentLanguage;

    private Dictionary<string, string[]> translations = new Dictionary<string, string[]>();


	private Tr() {
        updateCurrentLanguage();
	}

    public void updateCurrentLanguage() {

        updateLanguage(TrLanguageManager.Instance.currentLanguage);
    }

    public void updateLanguage(SystemLanguage newLanguage) {
        
        if (newLanguage == currentLanguage) {
            //same
            return;
        }

        currentLanguage = newLanguage;

        translations.Clear();

        loadFile("Translations - ");
        loadFile("Translations.Speeches - ");
    }

    private void loadFile(string filePrefix) {

        foreach (KeyValuePair<string, string[]> pair in TrLanguageManager.Instance.loadTranslations(filePrefix)) {
            translations.Add(pair.Key, pair.Value);
        }
    }

    public string[] getTranslationArray(string key) {

        updateCurrentLanguage();

        if (!translations.ContainsKey(key)) {
            throw new ArgumentException("Translation \"" + key + "\" not found for language " + currentLanguage);
        }

        return translations[key];
    }

    public string[] getTranslationArray(string key, int pos, int nb) {

        string[] res = new string[nb];
        Array.Copy(getTranslationArray(key), pos, res, 0, nb);

        return res;
    }

    public string getTranslation(string key, int pos) {
        
        return getTranslationArray(key)[pos];
    }

    public string getTranslation(string key) {

        return getTranslationArray(key)[0];
    }

}

