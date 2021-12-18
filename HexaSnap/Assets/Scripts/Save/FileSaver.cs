/**
 * Hexa Snap
 * © Aurélien Lubecki 2019
 * All Rights Reserved
 */

using System;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using System.Security.Cryptography;
using System.Text;
using UnityEngine;


public class FileSaver {


    public static readonly bool HAS_ENCRYPTION = false;//disabled due to a bug

    private static readonly byte[] ENCRYPTION_KEY = ASCIIEncoding.ASCII.GetBytes("XXXXXXXX");
    private static readonly byte[] ENCRYPTION_IV = ASCIIEncoding.ASCII.GetBytes("XXXXXXXX" + SystemInfo.deviceUniqueIdentifier.Substring(0, 8));


    private string getFilePath(int version) {
        return Application.persistentDataPath + "/save_v" + version;
    }

    public object loadAllFromFile(int version) {

        string filePath = getFilePath(version);

        Debug.Log("LOAD begin : " + DateTime.Now + " => " + filePath);

        bool exists = File.Exists(filePath);
        if (!exists) {
            return null;
        }

        BinaryFormatter bf = new BinaryFormatter();
        FileStream fs = File.OpenRead(filePath);

        Stream s = null;

        object data = null;

        try {
            if (HAS_ENCRYPTION) {
                s = new CryptoStream(
                    fs,
                    new DESCryptoServiceProvider().CreateDecryptor(ENCRYPTION_KEY, ENCRYPTION_IV),
                    CryptoStreamMode.Read
                );
            } else {
                s = fs;
            }

            int unserializedVersion = (int)bf.Deserialize(s);

            if (unserializedVersion != version) {
                //log to crashlytics
                Debug.LogError("The version of the game save is incorrect : current=v" + GameSaverVersionsHandler.CURRENT_VERSION + " / loaded=v" + version + " / unserialized=v" + unserializedVersion);
                return null;
            }

            data = bf.Deserialize(s);

        } catch (Exception e) {

            //log to crashlytics
            Debug.LogError("The loaded file has a problem : current=v" + GameSaverVersionsHandler.CURRENT_VERSION + " / loaded=v" + version + "\n" + e);

        } finally {

            s?.Close();
            fs.Close();
        }

        Debug.Log("LOAD end : " + DateTime.Now);

        return data;
    }

    public void saveAllToFile(int version, object data) {

        Debug.Log("SAVE begin : " + DateTime.Now);

        string filePath = getFilePath(version);

        if (Debug.isDebugBuild) {

            //copy the save before saving
            if (File.Exists(filePath)) {
                File.Copy(filePath, filePath + "_bak", true);
            }
        }

        BinaryFormatter bf = new BinaryFormatter();
        FileStream fs = File.Open(filePath, FileMode.OpenOrCreate);

        Stream s = null;

        try {
            if (HAS_ENCRYPTION) {
                s = new CryptoStream(
                    fs,
                    new DESCryptoServiceProvider().CreateEncryptor(ENCRYPTION_KEY, ENCRYPTION_IV),
                    CryptoStreamMode.Write
                );
            } else {
                s = fs;
            }

            bf.Serialize(s, version);
            bf.Serialize(s, data);

        } catch (Exception e) {

            //log to crashlytics
            Debug.LogError("The saved file has a problem : current=v" + GameSaverVersionsHandler.CURRENT_VERSION + " / loaded=v" + version + "\n" + e);

        } finally {

            s?.Close();
            fs.Close();
        }

        Debug.Log("SAVE end : " + DateTime.Now);
    }

    public void deleteSave(int version) {

        string path = getFilePath(version);
        if (!File.Exists(path)) {
            return;
        }

        File.Delete(path);
    }

}
