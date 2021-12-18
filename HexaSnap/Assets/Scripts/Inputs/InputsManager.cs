/**
 * Hexa Snap
 * © Aurélien Lubecki 2019
 * All Rights Reserved
 */
 
using System.Collections.Generic;
using UnityEngine;

public interface InputsManagerListener {

	void onActionDone(AbstractInputAction action);
}

// controller buttons mapping : http://www.gallantgames.com/pages/incontrol-standardized-controls
public class InputsManager : MonoBehaviour {
	
	public static InputsManager Instance {
		get {
			return GameHelper.Instance.getInputsManager();
		}
	}


	public bool isPaused { get; private set; }

	private HashSet<AbstractInputAction> possibleUIActions = new HashSet<AbstractInputAction>();
	private HashSet<AbstractInputAction> possiblePhysicsActions = new HashSet<AbstractInputAction>();

    void Start() {

        if (Debug.isDebugBuild) {
            possibleUIActions.Add(new InputActionDebugTriggerNextLevel());
            possibleUIActions.Add(new InputActionDebugTriggerPreviousLevel());
            possibleUIActions.Add(new InputActionDebugLevelIncrement());
            possibleUIActions.Add(new InputActionDebugLevelDecrement());
            possibleUIActions.Add(new InputActionDebugSwapGeneration());
        }

        possibleUIActions.Add(new InputActionSelectItem());

        if (Input.touchSupported) {
            updateDragControls();
        } else {
            possiblePhysicsActions.Add(new InputActionRotateClockwise());
            possiblePhysicsActions.Add(new InputActionRotateCounterClockwise());
        }
    }
		
	protected void OnApplicationFocus(bool focusStatus) {
		this.isPaused = !focusStatus;
	}

	void FixedUpdate() {

		if (isPaused) {
			return;
		}

	}

	void Update() {
		
		if (isPaused) {
			return;
		}

		foreach (AbstractInputAction action in possibleUIActions) {
			if(action.isActionDone()) {
				action.execute();
			}
		}

        foreach (AbstractInputAction action in possiblePhysicsActions) {
            if (action.isActionDone()) {
                action.execute();
            }
        }

	}

    public void updateDragControls() {

        if (!Input.touchSupported) {
            return;
        }

        //remove last drag actions
        possiblePhysicsActions.RemoveWhere((obj) => obj is BaseInputActionRotateDrag);

        //add chosen action
        if (GameHelper.Instance.getGameManager().isControlsOptionDragHorizontal) {
            possiblePhysicsActions.Add(new InputActionRotateDragHorizontally());
        } else {
            possiblePhysicsActions.Add(new InputActionRotateDragAroundAxis());
        }
    }

}

