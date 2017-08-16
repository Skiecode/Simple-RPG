using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MoveToPointScript : MonoBehaviour {

    [SerializeField] float timeToDestroy = 1f;


    private float _counter = 0f;
	// Update is called once per frame
	void Update () {
        _counter += Time.deltaTime;

        if(_counter >= timeToDestroy)
        {
            Destroy(gameObject);
        }
	}
}
