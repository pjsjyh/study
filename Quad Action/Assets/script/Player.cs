using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour
{
    // Start is called before the first frame update
    float hAxis;
    float vAxis;
    public float speed;
    public GameObject[] weapons;
    public bool[] hasWeapons;
    public GameObject[] grenades;
    public int HasGrenade;
    public GameObject grenadeObj;
    public Camera followCamera;
    public GameManager manager;

    public AudioSource jumpsound;

    public int Ammo;
    public int Coin;
    public int Health;
    public int score;

    public int maxAmmo;
    public int maxCoin;
    public int maxHealth;
    public int maxHasGrenade;

    bool wDown;
    bool jDown;
    bool isJump;
    bool isDodge;
    bool isReload;

    bool iDown;
    bool fDown;
    bool gDown;
    bool rDown;

    bool sDown1;
    bool sDown2;
    bool sDown3;
    bool isSwap;
    bool isFireReady = true;
    bool isBoarder;
    bool isDamage=false;
    bool isDead;

    Vector3 moveVec;
    Vector3 dodgeVec;
    Animator anim;
    Rigidbody rigid;
    MeshRenderer[] meshs;

    GameObject nearObject;
    public Weapon equipWeapon;
    int equipWeaponIndex=-1;
    float fireDelay;

    void Awake()
    {
        rigid = GetComponent<Rigidbody>();
        anim = GetComponentInChildren<Animator>();
        meshs = GetComponentsInChildren<MeshRenderer>();

       // PlayerPrefs.SetInt("MaxScore", 0);
    }

    // Update is called once per frame
    void Update()
    {
        GetInput();
        Move();
        Turn();
        Jump();
        Grenade();
        Attack();
        Reload();
        Dodge();
        Swap();
        Interation();
    }

    void GetInput()
    {
        hAxis = Input.GetAxisRaw("Horizontal");
        vAxis = Input.GetAxisRaw("Vertical");
        wDown = Input.GetButton("Walk");
        jDown = Input.GetButtonDown("Jump");
        fDown = Input.GetButton("Fire1");
        rDown = Input.GetButtonDown("Reload");
        gDown = Input.GetButtonDown("Fire2");
        iDown = Input.GetButtonDown("Interation");
        sDown1 = Input.GetButtonDown("Swap1");
        sDown2 = Input.GetButtonDown("Swap2");
        sDown3 = Input.GetButtonDown("Swap3");
    }
    void Move()
    {
        moveVec = new Vector3(hAxis, 0, vAxis).normalized;
        if (isDodge)
            moveVec = dodgeVec;
        if (isSwap || !isFireReady|| isReload||isDead)
            moveVec = Vector3.zero;
        if(!isBoarder)
            transform.position+=moveVec*speed*(wDown ? 0.3f:1f)*Time.deltaTime;



        anim.SetBool("isRun", moveVec != Vector3.zero);
        anim.SetBool("isWalk", wDown);
    }
    void Turn()
    {
        transform.LookAt(transform.position + moveVec);

        if (fDown&&!isDead)
        {
            Ray ray = followCamera.ScreenPointToRay(Input.mousePosition);
            RaycastHit rayHit;
            if (Physics.Raycast(ray, out rayHit, 100))
            {
                Vector3 nextVec = rayHit.point - transform.position;
                nextVec.y = 0;
                transform.LookAt(transform.position + nextVec);
            }
        }
        
    }
    void Jump()
    {
        if (jDown && !isJump&&moveVec == Vector3.zero&&!isDodge&& !isSwap&&!isDead)
        {
            rigid.AddForce(Vector3.up * 15, ForceMode.Impulse);
            anim.SetBool("isJump", true);
            anim.SetTrigger("doJump");
            isJump = true;

            //jumpsound.Play();
        }
    }
    void Grenade()
    {
        if (HasGrenade == 0)
            return;
        if (gDown && !isReload && !isSwap && !isDead)
        {
            Ray ray = followCamera.ScreenPointToRay(Input.mousePosition);
            RaycastHit rayHit;
            if (Physics.Raycast(ray, out rayHit, 100))
            {
                Vector3 nextVec = rayHit.point - transform.position;
                nextVec.y = 10;

                GameObject instantGrenade = Instantiate(grenadeObj, transform.position, transform.rotation);
                Rigidbody rigidGrenade = instantGrenade.GetComponent<Rigidbody>();
                rigidGrenade.AddForce(nextVec, ForceMode.Impulse);
                rigidGrenade.AddTorque(Vector3.back * 10, ForceMode.Impulse);

                HasGrenade -= 1;
                grenades[HasGrenade].SetActive(false);
            }
        }
    }
    void Attack()
    {
        if (equipWeapon == null)
            return;

        fireDelay += Time.deltaTime;
        isFireReady = equipWeapon.rate < fireDelay;
        if(fDown && isFireReady && !isDodge && !isSwap && !isDead)
        {
            equipWeapon.Use();
            anim.SetTrigger(equipWeapon.type==Weapon.Type.Melee ? "doSwing":"doShot");
            fireDelay = 0;
        }
    }
    void Reload()
    {
        if (equipWeapon == null)
            return;
        if (equipWeapon.type == Weapon.Type.Melee)
            return;
        if (Ammo == 0)
            return;
        if(rDown &&!isJump && !isDodge &&!isSwap && isFireReady && !isDead)
        {
            anim.SetTrigger("doReload");
            isReload = true;
            Invoke("ReloadOut", 2f);
        }
    }
    void ReloadOut()
    {
        int reAmmo = Ammo<equipWeapon.maxAmmo ? Ammo : equipWeapon.maxAmmo;
        equipWeapon.curAmmo = reAmmo;
        Ammo -= reAmmo;
        isReload = false;
    }
    void Dodge()
    {
        if (jDown &&moveVec!=Vector3.zero&& !isJump&&!isDodge&& !isSwap && !isDead)
        {
            dodgeVec = moveVec;
            speed *= 2;
            anim.SetTrigger("doDodge");
            isDodge = true;

            Invoke("DodgeOut", 0.5f);
        }
    }
    void DodgeOut()
    {
        isDodge = false;
        speed *= 0.5f;
    }


    void Swap()
    {
        if (sDown1 && (!hasWeapons[0] || equipWeaponIndex == 0))
            return;
        if (sDown2 && (!hasWeapons[1] || equipWeaponIndex == 1))
            return;
        if (sDown3 && (!hasWeapons[2] || equipWeaponIndex == 2))
            return;
        int weaponindex = -1;
        if (sDown1) weaponindex = 0;
        if (sDown2) weaponindex = 1;
        if (sDown3) weaponindex = 2;
        if ((sDown1 || sDown2 || sDown3)&&!isJump &&!isDodge && !isDead)
        {
            if(equipWeapon!=null)
                equipWeapon.gameObject.SetActive(false);
            equipWeaponIndex = weaponindex;
            equipWeapon = weapons[weaponindex].GetComponent<Weapon>();
            equipWeapon.gameObject.SetActive(true);

            anim.SetTrigger("doSwap");

            isSwap = true;
            Invoke("SwapOut", 0.4f);
        }
    }
    void SwapOut()
    {
        isSwap = false;
    }
    void Interation()
    {
        if (iDown && nearObject != null && !isJump && !isDodge && !isDead)
        {
            if (nearObject.tag == "Weapon")
            {
                Item item = nearObject.GetComponent<Item>();
                int weaponIndex = item.value;
                hasWeapons[weaponIndex] = true;

                Destroy(nearObject);
            }
            else if (nearObject.tag == "Shop")
            {
                Shop shop = nearObject.GetComponent<Shop>();
                shop.Enter(this);
            }
        }
    }
    void FreezeRotation()
    {
        rigid.angularVelocity = Vector3.zero;  //항상 회전0 만들어줌
    }
    void StopToWall()
    {
        Debug.DrawRay(transform.position, transform.forward * 5, Color.green);
        isBoarder = Physics.Raycast(transform.position, transform.forward, 5, LayerMask.GetMask("Wall"));
    }
    void FixedUpdate()
    {
        FreezeRotation();
        StopToWall();
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.tag == "Floor")
        {
            anim.SetBool("isJump", false);
            isJump = false;
        }
    }
    private void OnTriggerStay(Collider other)
    {
        if (other.tag == "Weapon"||other.tag=="Shop")
            nearObject = other.gameObject;
    }
    void OnTriggerExit(Collider other)
    {
        if (other.tag == "Weapon")
            nearObject =null;
        else if (other.tag == "Shop")
        {
            Shop shop = nearObject.GetComponent<Shop>();
            //shop.Exit();
            nearObject = null;
        }
    }

    void OnTriggerEnter(Collider other)
    {
        if (other.tag == "Item")
        {
            Item item = other.GetComponent<Item>();
            switch (item.type)
            {
                case Item.Type.Ammo:
                    Ammo += item.value;
                    if (Ammo >= maxAmmo)
                        Ammo = maxAmmo;
                    break;
                case Item.Type.Coin:
                    Coin += item.value;
                    if (Coin >= maxCoin)
                        Coin = maxCoin;
                    break;
                case Item.Type.Grenade:
                    grenades[HasGrenade].SetActive(true);
                    HasGrenade += item.value;
                    if (HasGrenade >= maxHasGrenade)
                        HasGrenade = maxHasGrenade;
                    break;
                case Item.Type.Heart:
                    Health += item.value;
                    if (Health >= maxHealth)
                        Health = maxHealth;
                    break;

            }
            Destroy(other.gameObject);
        }
        else if (other.tag == "EnemyBullet")
        {
            if (!isDamage)
            {
                Bullet enemyBullet = other.GetComponent<Bullet>();
                Health -= enemyBullet.damage;
                bool isBossAtk = other.name == "Boss Melee Area";
                StartCoroutine(OnDamage(isBossAtk));
            }
            if (other.GetComponent<Rigidbody>() != null)
                Destroy(other.gameObject);

        }
    }
    IEnumerator OnDamage(bool isBossAtk)
    {
        isDamage = true;
        foreach(MeshRenderer mesh in meshs)
        {
            mesh.material.color = Color.yellow;
        }
        if (isBossAtk)
            rigid.AddForce(transform.forward * -25, ForceMode.Impulse);
        if (Health <= 0 && !isDead)
            OnDie();
        yield return new WaitForSeconds(1f);
        isDamage = false;
        foreach (MeshRenderer mesh in meshs)
        {
            mesh.material.color = Color.white;
        }
        if(isBossAtk)
            rigid.velocity = Vector3.zero;
        

    }
    void OnDie()
    {
        anim.SetTrigger("doDie");
        isDead = true;
        manager.GameOver();
    }

}
