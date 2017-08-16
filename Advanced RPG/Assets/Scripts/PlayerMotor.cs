using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.AI;

[RequireComponent(typeof(NavMeshAgent))]
public class PlayerMotor : MonoBehaviour {

    [SerializeField] Animator anim;
    [SerializeField] LayerMask interactMask;
    [SerializeField] GameObject ultiExplosion;
    [SerializeField] GameObject R_middle1;
    [SerializeField] Text ultiText;
    [SerializeField] Text informationText;
    [SerializeField] Text newItemTextfield;

    [SerializeField] float TimeToGetUlti;

    NavMeshAgent agent;
    Camera cam;
    Collider focusing;

    GameObject guard;

    GameObject chest;
    GameObject diamond;
    GameObject meat;
    GameObject key;

    private float _canAttack = 0f;
    private float _c = 0f;
    private float _cUlti;
    private float ultiPercentPerSec;

    private bool playedDeathAni = false;
    private bool isUlti = false;
    private bool madeExplosion = false;

    private bool hasInteractableFocused;

    private bool hasChestFound;
    private bool hasDiamondFound;
    private bool hasMeatFound;
    private bool hasKeyFound;
    private bool hasChest;
    private bool hasDiamond;
    private bool hasMeat;
    private bool hasKey;

    [HideInInspector] public float ultiPercentage = 0;
    [HideInInspector] public bool isFocusig;
    [HideInInspector] public bool isDead = false;
	// Use this for initialization
	void Start () {
        cam = Camera.main;
        agent = GetComponent<NavMeshAgent>();
        ultiPercentPerSec = 100 / TimeToGetUlti;
        newItemTextfield.enabled = false;
	}

    void Update()
    {
        if (isDead)
        {
            if (!playedDeathAni)
            {
                anim.SetBool("isDead", true);
                anim.SetBool("isWalking", false);
                anim.SetBool("isIdle", false);
                anim.SetBool("isAttacking", false);
                playedDeathAni = true;
            }
            return;
        }
        if (isUlti)
        {
            _cUlti += Time.deltaTime;
            if(_cUlti >= 0.7f && !madeExplosion)
            {
                var exp = Instantiate(ultiExplosion, R_middle1.transform.position, Quaternion.identity);
                Collider[] cols = Physics.OverlapSphere(exp.transform.position, 5);
                foreach(Collider col in cols)
                {
                    if(col.CompareTag("Enemy"))
                    {
                        col.transform.GetComponent<SimpleEnemyAI>().ChangeHealth(-110);
                    }
                    if (col.CompareTag("Guard"))
                    {
                        if(col.GetComponent<Guard>().isRage)
                            col.GetComponent<Guard>().ChangeHealth(-110);
                    }
                }
                madeExplosion = true;
                return;
            }
            if(_cUlti < 1f)
            {
                return;
            }
            else
            {
                anim.SetBool("isIdle", true);
                anim.SetBool("ulti", false);
                anim.SetBool("isAttacking", false);
                anim.SetBool("isWalking", false);
                _cUlti = 0f;
                isUlti = false;
                madeExplosion = false;
                agent.isStopped = false;
            }
        }
        if (hasInteractableFocused)
        {
            if (hasChestFound && !hasChest)
            {
                if (!chest.GetComponent<Animator>().GetBool("opening"))
                {
                    if (Vector3.Distance(this.transform.position, chest.transform.position) <= 3)
                    {
                        agent.isStopped = true;
                        chest.GetComponent<Animator>().enabled = true;
                        chest.GetComponent<Animator>().SetBool("opening", true);
                    }
                }
                else
                {
                    if (Vector3.Distance(this.transform.position, chest.transform.position) <= 3)
                    {
                        informationText.text = "E Drücken um den Kisteninhalt zu entnehemen";
                        if (Input.GetKeyDown(KeyCode.E))
                        {
                            hasChest = true;
                            StartCoroutine(newItemText("Goldmünzen"));
                            chest.GetComponent<Animator>().SetBool("chestClosing", true);
                            informationText.text = "";
                            Destroy(chest);
                            hasInteractableFocused = false;
                        }
                    }
                }
            }
            else if (hasDiamondFound && !hasDiamond)
            {
                if (Vector3.Distance(this.transform.position, diamond.transform.position) <= 3)
                {
                    informationText.text = "E Drücken um den Diamanten aufzunehmen";
                    if (Input.GetKeyDown(KeyCode.E))
                    {
                        hasDiamond = true;
                        StartCoroutine(newItemText("Diamant"));
                        informationText.text = "";
                        Destroy(diamond);
                        hasInteractableFocused = false;
                    }
                }
            }
            else if (hasMeatFound && !hasMeat)
            {
                if(Vector3.Distance(this.transform.position, meat.transform.position) <= 3)
                {
                    informationText.text = "E Drücken um das Fleisch aufzunehmen";
                    if (Input.GetKeyDown(KeyCode.E))
                    {
                        hasMeat = true;
                        StartCoroutine(newItemText("Fleisch"));
                        informationText.text = "";
                        Destroy(meat);
                        hasInteractableFocused = false;
                    }
                }
            }
            else if(hasKeyFound && !hasKey)
            {
                if(Vector3.Distance(this.transform.position, key.transform.position) <= 3)
                {
                    informationText.text = "E Drücken um den Schlüssel aufzunehmen";
                    if (Input.GetKeyDown(KeyCode.E))
                    {
                        hasKey = true;
                        guard.GetComponent<Guard>().hasKey = true;
                        StartCoroutine(newItemText("Schlüssel"));
                        informationText.text = "";
                        Destroy(key);
                        hasInteractableFocused = false;
                    }
                }
            }
            else { hasInteractableFocused = false; }
        }
        if (isFocusig)
        {
            MoveToPoint(focusing.transform.position);
            if(Vector3.Distance(focusing.transform.position, this.transform.position) <= 3 && _canAttack >= 1)
            {
                if (isFocusig) { isFocusig = false; }
                agent.isStopped = true;
                anim.SetBool("isAttacking", true);
                anim.SetInteger("attackState", Random.Range(1, 3));
                anim.SetBool("isWalking", false);
                anim.SetBool("isIdle", false);
                _canAttack = 0f;
                this.transform.LookAt(focusing.transform);
                if (focusing.CompareTag("Guard"))
                    focusing.GetComponent<Guard>().ChangeHealth(-10);
                else focusing.transform.GetComponent<SimpleEnemyAI>().ChangeHealth(-10);
            }
        }
        _canAttack += Time.deltaTime;
        ChangeUltiPercentage(ultiPercentPerSec * Time.deltaTime);
        if (anim.GetBool("isAttacking"))
        {
            _c += Time.deltaTime;
            if(_c >= 0.5f)
            {
                _c = 0f;
                anim.SetBool("isIdle", true);
                anim.SetBool("isWalking", false);
                anim.SetBool("isAttacking", false);
            }
        }
        if (Input.GetKeyDown(KeyCode.Q))
        {
            if(ultiPercentage >= 100f)
            {
                useUlti();
            }
        }
        if (Input.GetMouseButtonDown(0))
        {
            RaycastHit hit;
            Ray ray = cam.ScreenPointToRay(Input.mousePosition);
            if(Physics.Raycast(ray, out hit, 100, interactMask))
            {
                if (hit.collider.CompareTag("Enemy"))
                {
                    if (Vector3.Distance(hit.collider.transform.position, this.transform.position) <= 3 && _canAttack >= 1)
                    {
                        if (isFocusig) { isFocusig = false; }
                        agent.isStopped = true;
                        anim.SetBool("isAttacking", true);
                        anim.SetInteger("attackState", Random.Range(1, 3));
                        anim.SetBool("isWalking", false);
                        anim.SetBool("isIdle", false);
                        _canAttack = 0f;
                        this.transform.LookAt(hit.collider.transform);
                        hit.collider.GetComponent<SimpleEnemyAI>().ChangeHealth(-10);
                    }
                    else
                    {
                        focusing = hit.collider;
                        isFocusig = true;
                    }
                }
                else if (hit.collider.CompareTag("Guard"))
                {
                    guard = hit.collider.gameObject;
                    if(Vector3.Distance(hit.collider.transform.position, this.transform.position) <= 3 && _canAttack >= 1)
                    {
                        if(guard.GetComponent<Guard>().isRage)
                        {
                            if (isFocusig) { isFocusig = false; }
                            agent.isStopped = true;
                            anim.SetBool("isAttacking", true);
                            anim.SetInteger("attackState", Random.Range(1, 3));
                            anim.SetBool("isWalking", false);
                            anim.SetBool("isIdle", false);
                            _canAttack = 0f;
                            this.transform.LookAt(hit.collider.transform);
                            hit.collider.GetComponent<Guard>().ChangeHealth(-10);
                        }
                        else  if (hasChest && hasDiamond && hasMeat)
                        {
                            hit.collider.GetComponent<Guard>().MessageHasItem();
                        }
                        else { hit.collider.GetComponent<Guard>().NextToDo(); }
                    }
                    else
                    {
                        focusing = hit.collider;
                        isFocusig = true; 
                    }
                }
                else if (hit.collider.CompareTag("Interact"))
                { 
                    if(hit.collider.name == "ChestCartoon")
                    {
                        MoveToPoint(hit.point);
                        hasChestFound = true;
                        hasInteractableFocused = true;
                        chest = hit.collider.gameObject;
                    }
                    else if (hit.collider.name == "Diamond")
                    {
                        MoveToPoint(hit.point);
                        hasDiamondFound = true;
                        hasInteractableFocused = true;
                        diamond = hit.collider.gameObject;
                    }
                    else if (hit.collider.name == "Meat")
                    {
                        MoveToPoint(hit.point);
                        hasMeatFound = true;
                        hasInteractableFocused = true;
                        meat = hit.collider.gameObject;
                    }
                    else if(hit.collider.name == "Key")
                    {
                        MoveToPoint(hit.point);
                        hasKeyFound = true;
                        hasInteractableFocused = true;
                        key = hit.collider.gameObject;
                    }
                    else { Debug.Log(hit.collider.name); }
                }
            }
        }

        float velocity = agent.velocity.magnitude;
        if(velocity > 0)
        {
            anim.SetBool("isWalking", true);
            anim.SetBool("isIdle", false);
        }
        else { anim.SetBool("isIdle", true); anim.SetBool("isWalking", false); }
    }

    public void MoveToPoint(Vector3 point)
    {
        agent.isStopped = false;
        agent.SetDestination(point);
    }

    public void useUlti()
    {
        ultiPercentage = 0f;
        anim.SetBool("ulti", true);
        agent.isStopped = true;
        anim.SetBool("isAttacking", false);
        anim.SetBool("isIdle", false);
        anim.SetBool("isWalking", false);
        isUlti = true;
    }

    public void ChangeUltiPercentage(float value)
    {
        float newValue = ultiPercentage += value;

        if(newValue > 100f)
        {
            ultiPercentage = 100;
        }
        else
        {
            ultiPercentage = newValue;
        }

        ultiText.text = ultiPercentage.ToString("0") + '%';
    }

    IEnumerator newItemText(string itemName)
    {
        newItemTextfield.enabled = true;
        string stringToDisplay = itemName += " erhalten!";
        int nameLength = stringToDisplay.Length;
        int currentIndex = 0;

        while(currentIndex < nameLength)
        {
            newItemTextfield.text += stringToDisplay[currentIndex];
            currentIndex++;
            if (currentIndex < nameLength)
            {
                yield return new WaitForSeconds(0.1f);
            }
            else
            {
                yield return new WaitForSeconds(3);
                newItemTextfield.text = "";
                newItemTextfield.enabled = false;
                break;
            }
        }
    }
}

