using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
public class Boss : Enemy
{
    public GameObject missile;
    public Transform missilePortA;
    public Transform missilePortB;

    Vector3 lookVec;
    Vector3 TauntVec;
    public bool isLook;

    void Awake()
    {
        rigid = GetComponent<Rigidbody>();
        boxColloder = GetComponent<BoxCollider>();
        meshs = GetComponentsInChildren<MeshRenderer>();
        nav = GetComponent<NavMeshAgent>();
        anim = GetComponentInChildren<Animator>();

        nav.isStopped = true;
        StartCoroutine(Think());
    }

    void Update()
    {
        if (isDead)
        {
            StopAllCoroutines();
            return;
        }
        if (isLook)
        {
            float h = Input.GetAxisRaw("Horizontal");
            float v = Input.GetAxisRaw("Vertical");
            lookVec = new Vector3(h, 0, v) * 5f;
            transform.LookAt(Target.position + lookVec);
        }
        else
        {
            nav.SetDestination(TauntVec);
        }
    }
    IEnumerator Think()
    {
        yield return new WaitForSeconds(0.1f);

        int ranAction = Random.Range(0, 5);
        switch (ranAction)
        {
            case 0:
            case 1:
                StartCoroutine(MissileShot());
                break;
            case 2:
            case 3:
                StartCoroutine(RockShot());
                break;
            case 4:
                StartCoroutine(Tount());
                break;
        }
    }

    IEnumerator MissileShot()
    {
        anim.SetTrigger("doShot");
        yield return new WaitForSeconds(0.2f);
        GameObject instanatMissileA = Instantiate(missile, missilePortA.position, missilePortA.rotation);
        BossMissile bossMissileA = instanatMissileA.GetComponent<BossMissile>();
        bossMissileA.target = Target;
        yield return new WaitForSeconds(0.3f);
        GameObject instanatMissileB = Instantiate(missile, missilePortB.position, missilePortB.rotation);
        BossMissile bossMissileB = instanatMissileB.GetComponent<BossMissile>();
        bossMissileB.target = Target;
        yield return new WaitForSeconds(2f);
        StartCoroutine(Think());

    }
    IEnumerator RockShot()
    {
        isLook = false;
        anim.SetTrigger("doBigShot");
        Instantiate(bullet, transform.position, transform.rotation);
        yield return new WaitForSeconds(3f);
        isLook = true;
        StartCoroutine(Think());
    }
    IEnumerator Tount()
    {
        TauntVec = Target.position + lookVec;
        isLook = false;
        nav.isStopped = false;
        boxColloder.enabled = false;

        anim.SetTrigger("doTaunt");
        yield return new WaitForSeconds(1.5f);
        meleeArea.enabled = true;
        yield return new WaitForSeconds(0.5f);
        meleeArea.enabled = false;

        yield return new WaitForSeconds(1f);
        isLook = true;
        nav.isStopped = true;
        boxColloder.enabled = true;
        StartCoroutine(Think());
    }

}
