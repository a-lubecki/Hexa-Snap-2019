/**
 * Hexa Snap
 * © Aurélien Lubecki 2019
 * All Rights Reserved
 */

using UnityEngine;
using System;


public class GameSaverVersionsHandler {


    public static readonly int CURRENT_VERSION = 2;

    public static readonly Func<object, object>[] migrationsByVersion = {
        //v1 to v2
        dataV1 => new GameSaveDataV2((GameSaveDataV1)dataV1)
    };


    private FileSaver fileSaver;


    public GameSaverVersionsHandler(FileSaver fileSaver) {
        this.fileSaver = fileSaver;
    }

    /**
     * Find current save and if not found, find the previous saves if any
     */
    public object loadVersionsUntilCurrent() {

        //search from v{current} to v1
        for (var v = CURRENT_VERSION; v >= 1; v--) {
            
            var data = fileSaver.loadAllFromFile(v);

            if (data != null) {
                //convert data of the previous version to the current one
                return migrateFromOlderVersions(v, data);
            }

            //else find previous version
        }

        //no found save
        return null;
    }

    /**
     * Convert an old data to a new one by iterating through all the data versions to the current one
     */
    private object migrateFromOlderVersions(int oldVersion, object oldData) {

        if (oldVersion == CURRENT_VERSION) {
            return oldData;
        }

        var data = oldData;
            
        for (var v = oldVersion; v < CURRENT_VERSION; v++) {
            
            var func = getMigrationFunction(v);
            if (func == null) {
                break;
            }

            data = func.Invoke(data);

            Debug.Log("Migrated from save v" + v + " to v" + (v + 1) + " (current=v" + CURRENT_VERSION + ")");
        }
                       
        return data;
    }

    private Func<object, object> getMigrationFunction(int version) {

        //if version of the file is v1, the migration function is at pos 0 in the array (migration from v1 to v2)
        int pos = version - 1;

        if (pos < 0) {
            return null;
        }

        if (pos >= migrationsByVersion.Length) {
            return null;    
        }

        return migrationsByVersion[pos];
    }

}
