/**
 * Hexa Snap
 * © Aurélien Lubecki 2019
 * All Rights Reserved
 */

using UnityEngine;
using UnityEngine.UI;


public class BadgeBehavior : MonoBehaviour {


    private void Awake() {
        setText(null);
    }


    public void setValue(int value) {

        if (value <= 0) {
            setText(null);
        } else {
            setText(value.ToString());
        }

    }

    public void setText(string text) {

        if (string.IsNullOrEmpty(text)) {

            gameObject.SetActive(false);

        } else {
            
            gameObject.SetActive(true);

            transform.Find("TextBadge").GetComponent<Text>().text = text;
        }
    }

}
