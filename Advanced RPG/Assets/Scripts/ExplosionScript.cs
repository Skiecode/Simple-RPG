using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ExplosionScript : MonoBehaviour {

    private float _c = 0f;

	// Update is called once per frame
	void Update () {
        _c += Time.deltaTime;

        if(_c >= 2f)
        {
            Destroy(gameObject);
        }
	}
}
