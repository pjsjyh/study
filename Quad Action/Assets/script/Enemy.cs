using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
public class Enemy : MonoBehaviour
{
    public enum Type { A,B,C, D};
    public Type enemyType;

    public int maxHealth;
    public int curHealth;
    public Transform Target;
    public BoxCollider meleeArea;
    public bool isChase;
    public bool isAttack;
    public bool isDead;
    public GameObject bullet;

    public Rigidbody rigid;
    public BoxCollider boxColloder;
    public MeshRenderer[] meshs;
    public NavMeshAgent nav;
    public Animator anim;
    void Awake()
    {
        rigid = GetComponent<Rigidbody>();
        boxColloder = GetComponent<BoxCollider>();
        meshs = GetComponentsInChildren<MeshRenderer>();
        nav = GetComponent<NavMeshAgent>();
        anim=GetComponentInChildren<Animator>();
        if(enemyType!=Type.D)
            Invoke("ChaseStart", 2);
    }
    void ChaseStart()
    {
        isChase = true;
        anim.SetBool("isWalk",true);
    }
    void Update()
    {
        if (nav.enabled&& enemyType != Type.D)
        {
            nav.SetDestination(Target.position);
            nav.isStopped = !isChase;
        }

    }
    void Targeting()
    {
        if(!isDead&& enemyType != Type.D)
        {
            float targetRadius = 1.5f;
            float targetRange = 3f;

            switch (enemyType)
            {
                case Type.A:
                    targetRadius = 1.5f;
                    targetRange = 3f;
                    break;
                case Type.B:
                    targetRadius = 1f;
                    targetRange = 3f;
                    break;
                case Type.C:
                    targetRadius = 0.5f;
                    targetRange = 25f;
                    break;



            }
            RaycastHit[] rayHits = Physics.SphereCastAll(transform.position, targetRadius, transform.forward, targetRange, LayerMask.GetMask("player"));
            if (rayHits.Length > 0 && !isAttack)
            {
                StartCoroutine(Attack());
            }
        }
        
    }
    IEnumerator Attack()
    {
        isChase = false;
        isAttack = true;
        anim.SetBool("isAttack", true);


        switch (enemyType)
        {
            case Type.A:
                yield return new WaitForSeconds(0.2f);
                meleeArea.enabled = true;
                yield return new WaitForSeconds(1f);
                meleeArea.enabled = false;
                yield return new WaitForSeconds(1f);
                break;
            case Type.B:
                yield return new WaitForSeconds(0.1f);
                rigid.AddForce(transform.forward * 20, ForceMode.Impulse);
                meleeArea.enabled = true;
                yield return new WaitForSeconds(0.5f);
                rigid.velocity = Vector3.zero;
                meleeArea.enabled = false;
                yield return new WaitForSeconds(2f);
                break;
            case Type.C:
                yield return new WaitForSeconds(0.5f);
                GameObject instantBullet = Instantiate(bullet, transform.position, transform.rotation);
                Rigidbody rigidBullet = instantBullet.GetComponent<Rigidbody>();
                rigidBullet.velocity = transform.forward * 20;
                yield return new WaitForSeconds(2f);
                break;
        }


        
        isChase = true;
        isAttack = false;
        anim.SetBool("isAttack", false);
    }
    void FixedUpdate()
    {
        Targeting();
        FixedVelocity();
    }
    void FixedVelocity()
    {
        if (isChase)
        {
            rigid.velocity = Vector3.zero;  //항상 회전0 만들어줌
            rigid.angularVelocity = Vector3.zero;
        }
        
    }
    private void OnTriggerEnter(Collider other)
    {
        if (other.tag == "Melee")
        {
            Weapon weapon = other.GetComponent<Weapon>();
            curHealth -= weapon.damage;
            Vector3 reactVec = transform.position - other.transform.position;
            StartCoroutine(onDamage(reactVec,false));
        }
        else if (other.tag == "bullet")
        {
            Bullet bulelt = other.GetComponent<Bullet>();
            curHealth -= bulelt.damage;
            Vector3 reactVec = transform.position - other.transform.position;
            Destroy(other.gameObject);
            StartCoroutine(onDamage(reactVec,false));
        }
    }

    public void HitByGreanade(Vector3 explosionPos)
    {
        curHealth -= 100;
        Vector3 reactVec = transform.position - explosionPos;
        StartCoroutine(onDamage(reactVec, true));
    }

    IEnumerator onDamage(Vector3 reactVec, bool isGrenade)
    {
        foreach(MeshRenderer mesh in meshs)
            mesh.material.color = Color.red;

        yield return new WaitForSeconds(0.1f);
        if (curHealth > 0)
        {
            foreach (MeshRenderer mesh in meshs)
                mesh.material.color = Color.white;
        }
        else
        {
            isDead = true;
            gameObject.layer = 14;
            foreach (MeshRenderer mesh in meshs)
                mesh.material.color = Color.gray;
            isChase = false;
            nav.enabled = false;
            anim.SetTrigger("doDie");

            if (isGrenade)
            {
                reactVec = reactVec.normalized;
                reactVec += Vector3.up*3;
                rigid.freezeRotation = false;
                rigid.AddForce(reactVec * 5, ForceMode.Impulse);
                rigid.AddTorque(reactVec * 15, ForceMode.Impulse);
            }
            else
            {
                reactVec = reactVec.normalized;
                reactVec += Vector3.up;
                rigid.AddForce(reactVec * 5, ForceMode.Impulse);
            }
            if(enemyType != Type.D)
                Destroy(gameObject, 4);
        }
    }
}
