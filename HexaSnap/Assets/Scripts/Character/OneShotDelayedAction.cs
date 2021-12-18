/**
 * Hexa Snap
 * © Aurélien Lubecki 2019
 * All Rights Reserved
 */

using System;


/**
 * Used to know if an action will be called called or not.
 * Pass a new OneShotDelayedAction object as a parameter in a method.
 * Then call anticipateCall(willBeCalled) before exiting the method.
 */
public class OneShotDelayedAction {


    public bool willBeCalled { get; private set; }

    private Action action;


    public OneShotDelayedAction(Action action) {

        if (action == null) {
            throw new ArgumentException();
        }

        this.action = action;
    }


    public void anticipateCall(bool willBeCalled) {
        this.willBeCalled = willBeCalled;
    }

    public void callAction() {
        
        if (!willBeCalled) {
            throw new InvalidOperationException("The action ws not marked as willBeCalled, it can't be called");
        }

        action.Invoke();
    }

}
