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

    public int Ammo;
    public int Coin;
    public int Health;

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
    bool rDown;

    bool sDown1;
    bool sDown2;
    bool sDown3;
    bool isSwap;
    bool isFireReady = true;
    
    Vector3 moveVec;
    Vector3 dodgeVec;
    Animator anim;
    Rigidbody rigid;
    GameObject nearObject;
    Weapon equipWeapon;
    int equipWeaponIndex=-1;
    float fireDelay;

    void Start()
    {
        rigid = GetComponent<Rigidbody>();
        anim = GetComponentInChildren<Animator>();
    }

    // Update is called once per frame
    void Update()
    {
        GetInput();
        Move();
        Turn();
        Jump();
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
        if (isSwap || !isFireReady|| isReload)
            moveVec = Vector3.zero;
        if (wDown)
            transform.position += moveVec * speed * 0.3f * Time.deltaTime;
        else
            transform.position += moveVec * speed * Time.deltaTime;
        anim.SetBool("isRun", moveVec != Vector3.zero);
        anim.SetBool("isWalk", wDown);
    }
    void Turn()
    {
        transform.LookAt(transform.position + moveVec);

    }
    void Jump()
    {
        if (jDown && !isJump&&moveVec == Vector3.zero&&!isDodge&& !isSwap)
        {
            rigid.AddForce(Vector3.up * 15, ForceMode.Impulse);
            anim.SetBool("isJump", true);
            anim.SetTrigger("doJump");
            isJump = true;
        }
    }
    void Attack()
    {
        if (equipWeapon == null)
            return;

        fireDelay += Time.deltaTime;
        isFireReady = equipWeapon.rate < fireDelay;
        if(fDown && isFireReady && !isDodge && !isSwap)
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
        if(rDown &&!isJump && !isDodge &&!isSwap && isFireReady)
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
        if (jDown &&moveVec!=Vector3.zero&& !isJump&&!isDodge&& !isSwap)
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
        if ((sDown1 || sDown2 || sDown3)&&!isJump &&!isDodge)
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
        if (iDown && nearObject != null && !isJump && !isDodge)
        {
            if (nearObject.tag == "Weapon")
            {
                Item item = nearObject.GetComponent<Item>();
                int weaponIndex = item.value;
                hasWeapons[weaponIndex] = true;

                Destroy(nearObject);
            }
        }
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
        if (other.tag == "Weapon")
            nearObject = other.gameObject;
    }
    private void OnTriggerExit(Collider other)
    {
        if (other.tag == "Weapon")
            nearObject =null;
    }

    private void OnTriggerEnter(Collider other)
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
    }


}
