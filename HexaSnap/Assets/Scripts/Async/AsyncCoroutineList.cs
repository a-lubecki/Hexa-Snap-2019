/**
 * Hexa Snap
 * © Aurélien Lubecki 2019
 * All Rights Reserved
 */

using System.Linq;
using System.Collections.Generic;


public class AsyncCoroutineList : List<AsyncCoroutineHandler> {


    public IEnumerable<AsyncCoroutineHandler> extractRunningCoroutines() {
        return this.Where(h => !h.isFinished());
    }

    public void removeFinishedCoroutines() {
        RemoveAll(h => h.isFinished());
    }

}
