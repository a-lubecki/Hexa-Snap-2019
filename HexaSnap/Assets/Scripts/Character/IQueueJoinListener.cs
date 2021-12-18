/**
 * Hexa Snap
 * © Aurélien Lubecki 2019
 * All Rights Reserved
 */


public interface IQueueJoinListener  {
    
    void onJoinEnqueue(CharacterAnimatorQueue queue);

    /**
     * Called when a join is dequeuing and waiting for others to join too.
     * If the others are joined too, return true.
     */
    bool onJoinWait(CharacterAnimatorQueue queue);

}
