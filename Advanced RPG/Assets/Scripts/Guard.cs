using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.AI;

public class Guard : MonoBehaviour
{

    [SerializeField] Transform player;
    [SerializeField] Transform lookAt;

    [SerializeField] RawImage speechBubble;
    [SerializeField] RawImage HealthBar;
    [SerializeField] RawImage HealthBackground;

    [SerializeField] Canvas canvas;

    [SerializeField] Text text;
    [SerializeField] Text btnYes;
    [SerializeField] Text btnNo;
    [SerializeField] Text HealthRatio;

    [SerializeField] GameObject lights;

    [SerializeField] SpawnItems spawner;

    [SerializeField] float MaxHealth = 400f;
    [SerializeField] float appearTime = 3f;
    [SerializeField] float interval = 0.15f;

    [SerializeField] string[] messagesNoItems;

    [HideInInspector] private int hasItemsState = 1;
    [HideInInspector] private int index = 0;

    [HideInInspector] public bool isRage;
    [HideInInspector] public bool hasKey ;
    [HideInInspector] private bool madeDamage = false;
    [HideInInspector] private bool isDead = false;

    [HideInInspector] private float _cCanAttack;
    [HideInInspector] private float _cAttack;

    [HideInInspector] private float CurrentHealth;

    [HideInInspector] private NavMeshAgent agent;
    [HideInInspector] private Animator anim;

    void Start()
    {
        anim = GetComponent<Animator>();
        agent = GetComponent<NavMeshAgent>();
        CurrentHealth = MaxHealth;
        lights.SetActive(false);
        HealthBar.gameObject.SetActive(false);
        HealthBackground.gameObject.SetActive(false);
        HealthRatio.gameObject.SetActive(false);
        speechBubble.enabled = false;
    }

    void Update()
    {
        if (isDead)
        {
            return;
        }
        if (isRage)
        {
            _cCanAttack += Time.deltaTime;
            Vector3 direction = player.transform.position - this.transform.position;
            direction.y = 0;
            this.transform.rotation = Quaternion.Slerp(this.transform.rotation, Quaternion.LookRotation(direction), 0.1f);
            if (agent.velocity.magnitude <= 0 && !anim.GetBool("isAttacking") && !anim.GetBool("isSpecialAttack")) 
            {
                anim.SetBool("isIdle", true);
                anim.SetBool("isWalking", false);
                anim.SetBool("isAttacking", false);
                anim.SetBool("isSpecialAttack", false);
            }
            if (anim.GetBool("isAttacking") || anim.GetBool("isSpecialAttack"))
            {
                _cAttack += Time.deltaTime;
                agent.isStopped = true;
                if(_cAttack >= 0.5f && !madeDamage)
                {
                    if (anim.GetBool("isAttacking"))
                    {
                        player.GetComponent<PlayerController>().ChangeHealth(UnityEngine.Random.Range(-10, -40));
                    }
                    else if (anim.GetBool("isSpecialAttack"))
                    {
                        player.GetComponent<PlayerController>().ChangeHealth(UnityEngine.Random.Range(-20, -75));
                    }
                    madeDamage = true;
                }
                if (_cAttack >= 0.7f)
                {
                    anim.SetBool("isIdle", true);
                    anim.SetBool("isWalking", false);
                    anim.SetBool("isAttacking", false);
                    anim.SetBool("isSpecialAttack", false);
                    _cAttack = 0f;
                    madeDamage = false;
                }
            }
            if (Vector3.Distance(this.transform.position, player.transform.position) <= 2.5f)
            {
                if (_cCanAttack >= 2)
                {
                    float odd = UnityEngine.Random.Range(0.0f, 100f);
                    Debug.Log(odd);
                    agent.isStopped = true;
                    if (odd >= 80)
                    {
                        anim.SetBool("isSpecialAttack", true);
                        anim.SetBool("isAttacking", false);
                        anim.SetBool("isWalking", false);
                        anim.SetBool("isIdle", false);
                        _cCanAttack = 0f;
                    }
                    else if (odd < 80)
                    {
                        anim.SetBool("isAttacking", true);
                        anim.SetBool("isSpecialAttack", false);
                        anim.SetBool("isWalking", false);
                        anim.SetBool("isIdle", false);
                        _cCanAttack = 0f;
                    }
                }
            }
            else
            {
                if (!anim.GetBool("isAttacking") && !anim.GetBool("isSpecialAttack"))
                {
                    MoveToPoint(player.transform.position);
                }
            }
        }
    }

    public void NextToDo()
    {
        if (speechBubble.enabled) { return; }
        speechBubble.enabled = true;
        if (isRage)
        {
            return;
        }
        else
        {
            try
            {
                StartCoroutine("displayMessage", messagesNoItems[index]);
            }
            catch (Exception ex)
            {
                if (ex is IndexOutOfRangeException)
                {
                    index = 0;
                    StartCoroutine("displayMessage", messagesNoItems[index]);
                }
            }
        }

        index++;
    }
    public void MessageHasItem()
    {
        switch (hasItemsState)
        {
            case 1:
                btnYes.gameObject.SetActive(false);
                btnNo.gameObject.SetActive(false);
                StartCoroutine(dpMsg("Ich sehe, dass du Items gesammelt hast! Willst du mit mir handeln?"));
                hasItemsState++;
                break;
            case 2:
                btnYes.gameObject.SetActive(false);
                btnNo.gameObject.SetActive(false);
                StartCoroutine(dpMsg("Sehr gut! Gib mir die Goldstücke, den Diamant und das Fleisch, und ich verrate dir wie du den Kreis aktivieren kannst! Einverstanden?!!"));
                hasItemsState = 4;
                break;
            case 3:
                btnYes.gameObject.SetActive(false);
                btnNo.gameObject.SetActive(false);
                hasItemsState = 1;
                StartCoroutine(displayMessage("Schade! Komm einfach wieder vorbei wenn du es dir anders überlegt hast!"));
                break;
            case 4:
                btnYes.gameObject.SetActive(false);
                btnNo.gameObject.SetActive(false);
                StartCoroutine(dpMsg("Okay, Danke für die Sachen! Auf der Welt ist ein Schlüssel versteckt! Wenn du mir den bringst, dann kann ich den Kreis aktivieren! Hahahahahahaha"));
                hasKey = false;
                hasItemsState = 6;
                break;
            case 5:
                btnYes.gameObject.SetActive(false);
                btnNo.gameObject.SetActive(false);
                hasItemsState = 1;
                StartCoroutine(displayMessage("Schade! Komm einfach wieder vorbei wenn du es dir anders überlegt hast!"));
                break;
            case 6:
                if (hasKey)
                {
                    btnYes.gameObject.SetActive(false);
                    btnNo.gameObject.SetActive(false);
                    hasItemsState = 7;
                    StartCoroutine(dpMsg("Ich sehe du hast den Schlüssel gefunden hehehehehe, wollen wir tauschen?"));
                }
                else
                {
                    btnYes.gameObject.SetActive(false);
                    btnNo.gameObject.SetActive(false);
                    StartCoroutine(displayMessage("Du wolltest mir den Schlüssel bringen ehehehe"));
                }
                break;
            case 8:
                StartCoroutine(dpMsg("Okay! Dann musst du nur noch an mir vorbei, denn jetzt hab ich alles!"));
                hasItemsState = 10;
                break;
            case 9:
                btnYes.gameObject.SetActive(false);
                btnNo.gameObject.SetActive(false);
                StartCoroutine(displayMessage("Schade! Komm wieder wenn du es die anders überlegt hast, ich bin immer zum tauschen bereit!"));
                break;
        }
    }
    public void ChangeHealth(float Value)
    {
        Value = (Value / 2);
        if (Value > 0)
        {
            float newValue = CurrentHealth += Value;
            if (newValue > MaxHealth)
            {
                CurrentHealth = MaxHealth;
                UpdateBar();
            }
            else
            {
                CurrentHealth += Value;
                UpdateBar();
            }
        }
        else
        {
            float newValue = CurrentHealth += Value;
            if (newValue <= 0)
            {
                OnDeath();
                CurrentHealth = 0;
                UpdateBar();
            }
            else
            {
                CurrentHealth += Value;
                UpdateBar();
            }
        }
    }
    public void OnDeath()
    {
        anim.SetBool("isDead", true);
        anim.SetBool("isWalking", false);
        anim.SetBool("isIdle", false);
        anim.SetBool("isAttacking", false);
        anim.SetBool("isSpecialAttack", false);
        isDead = true;
    }
    public void UpdateBar()
    {
        float percentage = CurrentHealth / MaxHealth;
        HealthBar.rectTransform.localScale = new Vector3(percentage, 1, 1);
        HealthRatio.text = (percentage * 100).ToString("0") + '%';
    }
    public void MoveToPoint(Vector3 point)
    {
        anim.SetBool("isWalking", true);
        anim.SetBool("isAttacking", false);
        anim.SetBool("isSpecialAttack", false);
        anim.SetBool("isIdle", false);
        agent.isStopped = false;
        agent.SetDestination(point);
    }

    public void OnYes()
    {
        switch (hasItemsState)
        {
            case 2:
                hasItemsState = 2;
                MessageHasItem(); ;
                break;
            case 4:
                hasItemsState = 4;
                MessageHasItem();
                break;
            case 7:
                hasItemsState = 8;
                MessageHasItem();
                break;
        }
    }
    public void OnNo()
    {
        switch (hasItemsState)
        {
            case 2:
                hasItemsState = 3;
                MessageHasItem();
                break;
            case 4:
                hasItemsState = 5;
                MessageHasItem();
                break;
            case 7:
                hasItemsState = 9;
                MessageHasItem();
                break;
        }
    }

    IEnumerator dpMsg(string message)
    {
        int stringLenth = message.Length;
        int currentIndex = 0;

        text.text = "";
        speechBubble.enabled = true;

        while (currentIndex < stringLenth)
        {
            text.text += message[currentIndex];
            currentIndex++;

            if (currentIndex < stringLenth)
            {
                yield return new WaitForSeconds(interval);
            }
            else
            {
                switch (hasItemsState)
                {
                    case 2:
                        btnYes.gameObject.SetActive(true);
                        btnNo.gameObject.SetActive(true);
                        break;
                    case 4:
                        btnYes.gameObject.SetActive(true);
                        btnNo.gameObject.SetActive(true);
                        break;
                    case 6:
                        spawner.RandomKeySpawn();
                        yield return new WaitForSeconds(appearTime);
                        break;
                    case 7:
                        btnYes.gameObject.SetActive(true);
                        btnNo.gameObject.SetActive(true);
                        break;
                    case 10:
                        btnYes.gameObject.SetActive(false);
                        btnNo.gameObject.SetActive(false);
                        lights.SetActive(true);
                        HealthBar.gameObject.SetActive(true);
                        HealthBackground.gameObject.SetActive(true);
                        HealthRatio.gameObject.SetActive(true);
                        speechBubble.enabled = false;
                        text.text = "";
                        isRage = true;
                        break;
                }
            }
        }
    }
    IEnumerator displayMessage(string message)
    {
        int stringLenth = message.Length;
        int currentIndex = 0;

        text.text = "";
        speechBubble.enabled = true;

        while (currentIndex < stringLenth)
        {
            text.text += message[currentIndex];
            currentIndex++;

            if (currentIndex < stringLenth)
            {
                yield return new WaitForSeconds(interval);
            }
            else
            {
                StartCoroutine(disappear());
                break;
            }
        }
    }
    IEnumerator disappear()
    {
        yield return new WaitForSeconds(appearTime);
        speechBubble.enabled = false;
        text.text = "";
    }

}
