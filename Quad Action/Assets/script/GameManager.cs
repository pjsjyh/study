using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GameManager : MonoBehaviour
{
    public GameObject menuCam;
    public GameObject GameCam;
    public Player player;
    public Boss boss;
    public int stage;
    public float playTime;
    public bool isBattle;
    public int enemyCntA;
    public int enemyCntB;
    public int enemyCntC;

    public GameObject menuPanel;
    public GameObject gamePanel;
    public Text maxScoreTxt;
    public Text scoreTxt;
    public Text stageTxt;
    public Text playTimeTxt;
    public Text playerHealthTxt;
    public Text playerAmmoTxt;
    public Text playerCoinTxt;
    public Image weapon1Img;
    public Image weapon2Img;
    public Image weapon3Img;
    public Image weaponRImg;
    public Text enemyATxt;
    public Text enemyBTxt;
    public Text enemyCTxt;
    public RectTransform bossHealthGroup;
    public RectTransform bossHealthBar;

    void Awake()
    {
        maxScoreTxt.text = string.Format("{0:n0}", PlayerPrefs.GetInt("MaxScore")); 
    }
    public void GameStart()
    {
        menuCam.SetActive(false);
        GameCam.SetActive(true);

        menuPanel.SetActive(false);
        gamePanel.SetActive(true);

        player.gameObject.SetActive(true);
    }
    void Update()
    {
        if (isBattle)
            playTime += Time.deltaTime;
    }
    void LateUpdate()
    {//상단 ui
        scoreTxt.text = string.Format("{0:n0}", player.score);

        stageTxt.text = "STAGE " + stage;
        int hour = (int)(playTime / 3600);
        int min = (int)((playTime-hour*3600 )/ 60);
        int second = (int)(playTime%60);
        playTimeTxt.text = string.Format("{0:00}", hour)+":"+string.Format("{0:00}", min) + ":"+string.Format("{0:00}", second);
        //플레이어 ui
        playerHealthTxt.text = player.Health + " / " + player.maxHealth;
        playerCoinTxt.text = string.Format("{0:n0}", player.Coin);
        if (player.equipWeapon == null)
            playerAmmoTxt.text = "- / " + player.Ammo;
        else
            playerAmmoTxt.text = player.equipWeapon.curAmmo+" / " + player.Ammo;
        //무기 ui
        weapon1Img.color = new Color(1, 1, 1, player.hasWeapons[0] ? 1 : 0);
        weapon2Img.color = new Color(1, 1, 1, player.hasWeapons[1] ? 1 : 0);
        weapon3Img.color = new Color(1, 1, 1, player.hasWeapons[2] ? 1 : 0);
        weaponRImg.color = new Color(1, 1, 1, player.HasGrenade>0 ? 1 : 0);
        //몬스터 숫자ui
        enemyATxt.text = enemyCntA.ToString();
        enemyBTxt.text = enemyCntB.ToString();
        enemyCTxt.text = enemyCntC.ToString();

        bossHealthBar.localScale = new Vector3((float)boss.curHealth / boss.maxHealth,1,1);
    }

}
