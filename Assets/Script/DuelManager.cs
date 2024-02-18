using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using TMPro;
using EnnemyStruct;
using UnityEngine.SceneManagement;

public class DuelManager : MonoBehaviour
{
    [SerializeField] GameObject doorScene;
    EnnemiesStats ennemyStats;
    int ennemyId = 0;
    Player playerStats;
    [SerializeField] RoomSetUp room;
    [SerializeField] Transform roomSpawnPoint;
    [SerializeField] float timeBetweenEnnemies;


    EnnemyAction[] sequence;
    [SerializeField] float blockWindow;
    EnnemyMoves currentState;

    [Header("Ennemy Feedback")]
    [SerializeField] float flashTime;
    [SerializeField] GameObject whiteFlash;
    [SerializeField] GameObject redFlash;
    [SerializeField] SpriteRenderer ennemy;
    //[SerializeField] Sprite ennemyKatana;
    //[SerializeField] Sprite ennemyKatana_attack;
    [SerializeField] AudioClip swordHit;
    [SerializeField] AudioClip swordBlock;
    [SerializeField] ParticleSystem blood;
    [SerializeField] ParticleSystem sparks;
    AudioSource audioSource;
    [SerializeField] AudioClip tick;
    [SerializeField] AudioClip carillon;

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
    [SerializeField] TMP_Text playerHealthUI;
    [SerializeField] TMP_Text ennemyHealthUI;
    void Start()
    {
        doorScene.SetActive(false);
        audioSource = GetComponent<AudioSource>();
        Instantiate(room.room.decor, roomSpawnPoint.position, room.room.decor.transform.rotation);
        playerStats = Player.instance;
        blockWindow += Player.paryBonus;
        UpdateUI(playerHealthUI, Player.health.ToString());

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
            
            UpdateUI(ennemyHealthUI, ennemyHealth.ToString());
            StartCoroutine(EnnemySequence());
        }
        
    }

    private void Update()
    {
        if(ennemyHealth <= 0 && !ennemyDead)
        {
            ennemyDead = true;
            StopAllCoroutines();
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
    }
    IEnumerator EnnemySequence()
    {
        for (int i = 0; i < sequence.Length-1; i++)
        {
            
            yield return new WaitForSeconds(sequence[i].timing / 2);
            switch (sequence[i].move)
            {
                case EnnemyMoves.Pause:
                    audioSource.PlayOneShot(tick);
                    break;
                case EnnemyMoves.LightAttack:
                    audioSource.PlayOneShot(tick);
                    StartCoroutine(Flash(whiteFlash, 1));
                    break;
                case EnnemyMoves.HeavyAttack:
                    audioSource.PlayOneShot(tick);
                    StartCoroutine(Flash(redFlash, 1));
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

    IEnumerator Flash(GameObject flash, int num)
    {
        for (int i = 0; i < num; i++)
        {
            flash.SetActive(flash);
            yield return new WaitForSeconds(flashTime);
            flash.SetActive(false);
            yield return new WaitForSeconds(flashTime);
        }
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
                    StartCoroutine(EnemyKatanaStance(1));
                    break;
                case EnnemyMoves.HeavyAttack:
                    //currentState = EnnemyMoves.HeavyAttack;
                    enemyAttack = 3;
                    yield return new WaitForSeconds(action.timing / 2);
                    StartCoroutine(EnemyKatanaStance(1));
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
            UpdateUI(playerHealthUI, Player.health.ToString());
            audioSource.PlayOneShot(swordHit);
            blood.Play();
            enemyAttack = 0;
        }
        if (dodgingCD > 0 && currentState != EnnemyMoves.ResetAction)
        {
            dodgingCD--;
        }
        yield return new WaitForSeconds((actionTime - blockWindow) / 2);
        
        yield return null;
    }
    IEnumerator EnemyKatanaStance(int num)
    {
        for (int i = 0; i < num; i++)
        {
            ennemy.sprite = ennemyStats.attackStance;
            yield return new WaitForSeconds(flashTime);
            ennemy.sprite = ennemyStats.baseStance;
            yield return new WaitForSeconds(flashTime);
        }
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
                UpdateUI(ennemyHealthUI, ennemyHealth.ToString());
            }
            else
            {
                Player.health -= enemyAttack * ennemyStats.damage;
                UpdateUI(playerHealthUI, Player.health.ToString());
                audioSource.PlayOneShot(swordHit);
                blood.Play();
                enemyAttack = 0;
            }
            actionPerformed = true;
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

    void UpdateUI(TMP_Text ui, string newText)
    {
        ui.text = newText;
    }

    IEnumerator NextEnnemy()
    {
        ennemy.transform.parent.gameObject.SetActive(false);
        yield return new WaitForSeconds(timeBetweenEnnemies);  
        ennemyStats = room.ennemyList[ennemyId];
        sequence = ennemyStats.sequences[Random.Range(0, ennemyStats.sequences.Length)].sequence;
        ennemyHealth = ennemyStats.health;
        UpdateUI(ennemyHealthUI, ennemyHealth.ToString());
        ennemyDead = false;
        ennemy.transform.parent.gameObject.SetActive(true);

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
    public void NextRoom()
    {
        WorldGeneration.playerProgression++;
        if(WorldGeneration.playerProgression == WorldGeneration.roomList.Length)
        {
            print("gg");
            //Fin du jeu
        }
        else
        {
            SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
        }
    }
}
