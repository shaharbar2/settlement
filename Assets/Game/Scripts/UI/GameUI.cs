using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
public class GameUI : MonoBehaviour {

    [SerializeField] TMP_Text hintText;

    /// Public -- 

    public void setHintVisible(bool visible) {
        hintText.gameObject.SetActive(visible);
    }
}