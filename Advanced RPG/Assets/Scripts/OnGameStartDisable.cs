using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class OnGameStartDisable : MonoBehaviour {

    [SerializeField] Text BtnYes;
    [SerializeField] Text BtnNo;

	// Use this for initialization
	void Start () {
        BtnYes.gameObject.SetActive     (false);
        BtnNo.gameObject.SetActive      (false);
	}
	
}
