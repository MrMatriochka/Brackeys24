using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.InputSystem;
using TMPro;
using EnnemyStruct;
using UnityEngine.SceneManagement;
using UnityEngine.VFX;

public class DuelManager : MonoBehaviour
{
    [SerializeField] GameObject doorScene;
    EnnemiesStats ennemyStats;
    int ennemyId = 0;

    [SerializeField] RoomSetUp room;
    [SerializeField] Transform roomSpawnPoint;
    [SerializeField] float timeBetweenEnnemies;


    EnnemyAction[] sequence;
    [SerializeField] float blockWindow;
    EnnemyMoves currentState;

    [Header("Ennemy Feedback")]
    [SerializeField] float flashTime;
    [SerializeField] VisualEffect whiteFlash;
    [SerializeField] VisualEffect redFlash;
    [SerializeField] SpriteRenderer ennemy;
    [SerializeField] AudioClip swordHit;
    [SerializeField] AudioClip swordBlock;
    [SerializeField] Material bloodMatPlayer;
    [SerializeField] Material bloodMatEnemy;
    [SerializeField] VisualEffect sparks;
    AudioSource audioSource;
    [SerializeField] AudioClip tick;
    [SerializeField] AudioClip tickHeavy;
    [SerializeField] AudioClip carillon;
    [SerializeField] AudioClip slashMiss;
    [SerializeField] ScreenShake shake;

    bool canBlock;
    int enemyAttack;
    int ennemyHealth;
    bool actionPerformed;
    bool dodging;
    int dodgingCD;
    bool ennemyDead;

    [Header("Player Stances")]
    [SerializeField] GameObject player;
    Vector3 playerInitialPos;
    [SerializeField] Sprite playerKatana;
    [SerializeField] Sprite playerKatana_attack;
    [SerializeField] Sprite playerKatana_block;
    [SerializeField] float dodgeDistance;

    [Header("UI")]
    [SerializeField] Slider playerHealthUILeft;
    [SerializeField] Slider playerHealthUIRight;

    void Start()
    {
        doorScene.SetActive(false);
        audioSource = GetComponent<AudioSource>();
        Instantiate(room.room.decor, roomSpawnPoint.position, room.room.decor.transform.rotation);
        blockWindow += Player.paryBonus;
        UpdateHP();

        if (ennemyId>= room.ennemyList.Count)
        {
            ennemy.sprite = null;
            ennemyDead = true;
            CombatOver();
        }
        else if (room.ennemyList[ennemyId] == null)
        {
            ennemy.sprite = null;
            ennemyDead = true;
            CombatOver();
        }
        else
        {
            ennemyStats = room.ennemyList[ennemyId];
            sequence = ennemyStats.sequences[Random.Range(0, ennemyStats.sequences.Length)].sequence;
            playerInitialPos = player.transform.position;
            ennemyHealth = ennemyStats.health;
            
            StartCoroutine(EnnemySequence());
        }
        
    }

    [SerializeField] GameObject gameOver;
    private void Update()
    {
        if(ennemyHealth <= 0 && !ennemyDead)
        {
            ennemyDead = true;
            StopAllCoroutines();
            StartCoroutine(Blood(bloodMatEnemy));
            ennemyId++;
            if (ennemyId< room.ennemyList.Count)
            {
                StartCoroutine(NextEnnemy());
            }
            else
            {
                ennemy.sprite = null;
                CombatOver();
            }
        }

        if (Player.health <= 0 && !ennemyDead)
        {
            player.SetActive(false);
            StopAllCoroutines();
            StartCoroutine(Blood(bloodMatPlayer));
            ennemyDead = true;
            gameOver.SetActive(true);
            WorldGeneration.playerProgression = 0;
            Player.instance.Init();
        }
    }
    IEnumerator EnnemySequence()
    {
        for (int i = 0; i < sequence.Length-1; i++)
        {
            
            yield return new WaitForSeconds(sequence[i].timing / 2);
            switch (sequence[i].move)
            {
                case EnnemyMoves.LightAttack:
                    audioSource.PlayOneShot(tick);
                    whiteFlash.Play();
                    break;
                case EnnemyMoves.HeavyAttack:
                    audioSource.PlayOneShot(tickHeavy);
                    redFlash.Play();
                    break;
                default:
                    break;
            }
            yield return new WaitForSeconds(sequence[i].timing / 2);
        }
        audioSource.PlayOneShot(carillon);
        StartCoroutine(PlayerSequence());
        yield return null;
    }

    IEnumerator PlayerSequence()
    {
        foreach (EnnemyAction action in sequence)
        {
            //audioSource.PlayOneShot(tick);

            //init action
            currentState = action.move;
            if (dodgingCD <= 0)
            {
                dodging = false;
            }
            actionPerformed = false;
            player.GetComponent<SpriteRenderer>().sprite = playerKatana;
            player.transform.position = playerInitialPos;
            enemyAttack = 0;
            StartCoroutine(InputActionWindow(action.timing));
            
            switch (action.move)
            {
                case EnnemyMoves.LightAttack:
                    enemyAttack = 1;
                    yield return new WaitForSeconds(action.timing / 2);
                    StartCoroutine(EnemyKatanaStance());
                    break;
                case EnnemyMoves.HeavyAttack:
                    //currentState = EnnemyMoves.HeavyAttack;
                    enemyAttack = 3;
                    yield return new WaitForSeconds(action.timing / 2);
                    StartCoroutine(EnemyKatanaHeavyStance());
                    break;
                default:
                    yield return new WaitForSeconds(action.timing / 2);
                    break;
            }

            yield return new WaitForSeconds(action.timing / 2);
        }

        sequence = ennemyStats.sequences[Random.Range(0, ennemyStats.sequences.Length - 1)].sequence;
        StartCoroutine(EnnemySequence());
        yield return null;
    }

    IEnumerator InputActionWindow(float actionTime)
    {
        canBlock = false;
        yield return new WaitForSeconds((actionTime - blockWindow) / 2);
        canBlock = true;
        yield return new WaitForSeconds(blockWindow);
        canBlock = false;
        if (enemyAttack > 0)
        {
            Player.health -= enemyAttack * ennemyStats.damage;
            UpdateHP();
            audioSource.PlayOneShot(swordHit);
            StartCoroutine(Blood(bloodMatPlayer));
            enemyAttack = 0;
        }
        if (dodgingCD > 0 && currentState != EnnemyMoves.ResetAction)
        {
            audioSource.PlayOneShot(slashMiss);
            dodgingCD--;
        }
        yield return new WaitForSeconds((actionTime - blockWindow) / 2);
        
        yield return null;
    }
    IEnumerator Blood(Material bloodMat)
    {
        float Gotoposition = -1;
        float elapsedTime = 0;
        float waitTime = 0.2f;
        float currentPos = 1;

        while (elapsedTime < waitTime)
        {
            bloodMat.SetFloat("_Scroll" ,Mathf.Lerp(currentPos, Gotoposition, (elapsedTime / waitTime)));
            elapsedTime += Time.deltaTime;
            yield return null;
        }
        bloodMat.SetFloat("_Scroll", -1);
        yield return null;
    }
    IEnumerator EnemyKatanaStance()
    {
            ennemy.sprite = ennemyStats.attackStance;
            shake.shake = 0.2f;
            yield return new WaitForSeconds(flashTime);
            ennemy.sprite = ennemyStats.baseStance;
            yield return new WaitForSeconds(flashTime);

        yield return null;
    }
    IEnumerator EnemyKatanaHeavyStance()
    {
        ennemy.sprite = ennemyStats.heavyStance;
        yield return new WaitForSeconds(flashTime);
        ennemy.sprite = ennemyStats.attackStance;
        yield return new WaitForSeconds(flashTime);
        shake.shake = 0.2f;
        ennemy.sprite = ennemyStats.baseStance;
        yield return new WaitForSeconds(flashTime);
        yield return null;
    }
    //player actions
    public void Attack(InputAction.CallbackContext context)
    {
        if (context.performed && !actionPerformed)
        {
            if (currentState == EnnemyMoves.Open)
            {
                ennemyHealth -= Player.damage;
                audioSource.PlayOneShot(swordHit);
                StartCoroutine(Blood(bloodMatEnemy));
            }
            else
            {
                Player.health -= enemyAttack * ennemyStats.damage;
                UpdateHP();
                audioSource.PlayOneShot(swordHit);
                StartCoroutine(Blood(bloodMatPlayer));
                enemyAttack = 0;
            }
            actionPerformed = true;
            shake.shake = 0.2f;
            player.GetComponent<SpriteRenderer>().sprite = playerKatana_attack;
        }
    }
    public void Block(InputAction.CallbackContext context)
    {
        if (context.performed && !dodging && !actionPerformed)
        {

            if (currentState == EnnemyMoves.LightAttack && canBlock)
            {
                enemyAttack --;
                audioSource.PlayOneShot(swordBlock);
                sparks.Play();
            }
            actionPerformed = true;
            player.GetComponent<SpriteRenderer>().sprite = playerKatana_block;
        }        
    }
    public void Dodge(InputAction.CallbackContext context)
    {
        if (context.performed && !dodging && !actionPerformed)
        {
            if (currentState == EnnemyMoves.LightAttack || currentState == EnnemyMoves.HeavyAttack)
            {
                if (canBlock)
                {
                    enemyAttack = 0;
                }
            }
            dodging = true;
            dodgingCD = 2;
            actionPerformed = true;
            player.transform.position -= Vector3.right * dodgeDistance;
        }
    }

    public void UpdateHP()
    {
        playerHealthUILeft.maxValue = Player.maxhealth;
        playerHealthUILeft.value = Player.health;
        playerHealthUIRight.maxValue = Player.maxhealth;
        playerHealthUIRight.value = Player.health;
    }
    IEnumerator NextEnnemy()
    {
        ennemy.transform.gameObject.SetActive(false);
        yield return new WaitForSeconds(timeBetweenEnnemies);  
        ennemyStats = room.ennemyList[ennemyId];
        sequence = ennemyStats.sequences[Random.Range(0, ennemyStats.sequences.Length)].sequence;
        ennemyHealth = ennemyStats.health;
        ennemyDead = false;
        ennemy.transform.gameObject.SetActive(true);

        StartCoroutine(EnnemySequence());
        yield return null;
    }


    public GameObject nextRoomButton;
    public GameObject shop;
    void CombatOver()
    {
        if(room.pnj != RoomStats.Pnj.None)
        {
            shop.SetActive(true);
        }
        nextRoomButton.SetActive(true);
    }
    [SerializeField] Transition transi;
    [SerializeField] GameObject end;
    public void NextRoom()
    {
        WorldGeneration.playerProgression++;
        if(WorldGeneration.playerProgression == WorldGeneration.roomList.Length)
        {
            end.SetActive(true);
        }
        else
        {
            transi.Out(1);
        }
    }
}
