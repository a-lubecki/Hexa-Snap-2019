/**
 * Hexa Snap
 * © Aurélien Lubecki 2019
 * All Rights Reserved
 */


public interface IQueueDelayListener  {
    
    /**
     * Called when a delay is dequeuing and waiting
     */
    void onDelayWait(CharacterAnimatorQueue queue);

}
