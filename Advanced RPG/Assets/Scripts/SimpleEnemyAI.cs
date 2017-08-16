using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.UI;
public class SimpleEnemyAI : MonoBehaviour {

    [SerializeField] float Range;
    [SerializeField] float MaxHealth;
    [SerializeField] RawImage bar;
    [SerializeField] Text ratio;

    [SerializeField] public Transform target;

    private float _c;
    private float _canAttack;
    private float _dead;
    private float CurrentHealth;

    private bool canWalk = true;
    private bool attacked = false;
    private bool isDead = false;

    private Animator anim;
    private NavMeshAgent agent;
    private Rigidbody rb;

    void Start()
    {
        anim = GetComponent<Animator>();
        agent = GetComponent<NavMeshAgent>();
        CurrentHealth = MaxHealth;
    }


	// Update is called once per frame
	void Update () {
        if (isDead)
        {
            _dead += Time.deltaTime;
            if(_dead >= 2)
            {
                Destroy(gameObject);
            }
            return;
        }
        _canAttack += Time.deltaTime;
        if(agent.velocity.magnitude <= 0 && !anim.GetBool("isIdle") && !anim.GetBool("isAttacking")) {
            anim.SetBool("isIdle", true);
            anim.SetBool("isWalking", false);
            anim.SetBool("isAttacking", false);
        }
        if (Vector3.Distance(target.position, this.transform.position) <= 2)
        {
            Vector3 direction = target.position - this.transform.position;
            direction.y = 0f;
            this.transform.rotation = Quaternion.Slerp(this.transform.rotation, Quaternion.LookRotation(direction), 0.1f);
            if (_canAttack >= 2.3 || anim.GetBool("isAttacking"))
            {
                if (!anim.GetBool("isAttacking"))
                {
                    agent.isStopped = true;
                    _canAttack = 0f;
                    anim.SetBool("isAttacking", true);
                    anim.SetBool("isIdle", false);
                    anim.SetBool("isWalking", false);
                }
                else
                {
                    _c += Time.deltaTime;
                    if (_c >= 1 && !attacked)
                    {
                        attacked = true;
                        target.GetComponent<PlayerController>().ChangeHealth(-10);
                    }
                    if (_c >= 2)
                    {
                        _c = 0f;
                        attacked = false;
                        anim.SetBool("isIdle", true);
                        anim.SetBool("isAttacking", false);
                        anim.SetBool("isWalking", false);
                    }
                }
            }
            return;
        }
		if(Vector3.Distance(target.position, this.transform.position) <= Range)
        {
            if(Vector3.Distance(target.position, this.transform.position) <= 2) { return; }
            if (agent.isStopped) { agent.isStopped = false; }
            agent.SetDestination(target.transform.position);
            if (!anim.GetBool("isWalking")) {
                anim.SetBool("isWalking", true);
                anim.SetBool("isIdle", false);
                anim.SetBool("isAttacking", false);
            }
        }
	}

    public void ChangeHealth(int Value)
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
                CurrentHealth = 0;
                OnDeath();
                UpdateBar();
            }
            else
            {
                CurrentHealth += Value;
                UpdateBar();
            }
        }

    }

    void OnDeath()
    {
        agent.isStopped = true;
        anim.SetBool("isDead", true);
        anim.SetBool("isWalking", false);
        anim.SetBool("isIdle", false);
        anim.SetBool("isAttacking", false);
        target.GetComponent<PlayerMotor>().ChangeUltiPercentage(10);
        isDead = true;
    }

    public void UpdateBar()
    {
        float percentage = CurrentHealth / MaxHealth;
        bar.rectTransform.localScale = new Vector3(percentage, 1, 1);
        ratio.text = (percentage * 100).ToString("0") + '%';
    }
}
