using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SummonEnemys : MonoBehaviour {

    [SerializeField] LayerMask mask;
    [SerializeField] GameObject prefab;
    [SerializeField] Transform player;

    private float _counter = 0f;

    private float minX = -18.5f;
    private float maxX =  17.5f;

    private float minZ = -17.5f;
    private float maxZ =  18.0f;
	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
        _counter += Time.deltaTime;
        if(_counter >= 10f)
        {
            _counter = 0f;
            float posX = Random.Range(minX, maxX);
            float posZ = Random.Range(minZ, maxZ);

            RaycastHit hit;
            if(Physics.Raycast(new Vector3(posX, 9999f, posZ), Vector3.down, out hit, Mathf.Infinity, mask))
            {
                Vector3 pos = new Vector3(hit.point.x, hit.point.y, hit.point.z);
                var newPrefab = Instantiate(prefab, pos, Quaternion.identity);
                newPrefab.GetComponent<SimpleEnemyAI>().target = player;
            }
        }
	}
}
