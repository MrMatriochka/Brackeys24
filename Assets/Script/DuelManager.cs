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
    [SerializeField] Player playerStats;
    [SerializeField] RoomSetUp room;
    [SerializeField] float timeBetweenEnnemies;


    EnnemyAction[] sequence;
    [SerializeField] float blockWindow;
    EnnemyMoves currentState;

    [Header("Ennemy Feedback")]
    [SerializeField] float flashTime;
    [SerializeField] GameObject whiteFlash;
    [SerializeField] GameObject redFlash;
    [SerializeField] GameObject ennemyKatana;
    [SerializeField] GameObject ennemyKatana_attack;
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
    [SerializeField] GameObject playerKatana;
    [SerializeField] GameObject playerKatana_attack;
    [SerializeField] GameObject playerKatana_block;
    [SerializeField] float dodgeDistance;

    [Header("UI")]
    [SerializeField] TMP_Text playerHealthUI;
    [SerializeField] TMP_Text ennemyHealthUI;
    void Start()
    {
        ennemyStats = room.ennemyList[ennemyId];
        doorScene.SetActive(false);
        audioSource = GetComponent<AudioSource>();
        sequence = ennemyStats.sequences[Random.Range(0, ennemyStats.sequences.Length)].sequence;
        playerInitialPos = player.transform.position;
        ennemyHealth = ennemyStats.health;
        UpdateUI(playerHealthUI, playerStats.health.ToString());
        UpdateUI(ennemyHealthUI, ennemyHealth.ToString());

        StartCoroutine(EnnemySequence());
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
                Destroy(ennemyKatana.transform.parent.gameObject);
                StartCoroutine(NextRoom());
            }
        }
    }
    IEnumerator EnnemySequence()
    {
        for (int i = 0; i < sequence.Length-1; i++)
        {
            audioSource.PlayOneShot(tick);
            yield return new WaitForSeconds(sequence[i].timing / 2);
            switch (sequence[i].move)
            {
                case EnnemyMoves.Pause:
                    break;
                case EnnemyMoves.LightAttack:
                    StartCoroutine(Flash(whiteFlash, 1));
                    break;
                case EnnemyMoves.HeavyAttack:
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
            playerKatana.SetActive(true);
            playerKatana_block.SetActive(false);
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
            playerStats.health -= enemyAttack * ennemyStats.damage;
            UpdateUI(playerHealthUI, playerStats.health.ToString());
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
            ennemyKatana.SetActive(false);
            ennemyKatana_attack.SetActive(true);
            yield return new WaitForSeconds(flashTime);
            ennemyKatana.SetActive(true);
            ennemyKatana_attack.SetActive(false);
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
                ennemyHealth -= playerStats.damage;
                UpdateUI(ennemyHealthUI, ennemyHealth.ToString());
            }
            else
            {
                playerStats.health -= enemyAttack * ennemyStats.damage;
                UpdateUI(playerHealthUI, playerStats.health.ToString());
                audioSource.PlayOneShot(swordHit);
                blood.Play();
                enemyAttack = 0;
            }
            actionPerformed = true;
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
            playerKatana.SetActive(false);
            playerKatana_block.SetActive(true);
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
                    print("dodge");
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
        ennemyKatana.transform.parent.gameObject.SetActive(false);
        yield return new WaitForSeconds(timeBetweenEnnemies);  
        ennemyStats = room.ennemyList[ennemyId];
        sequence = ennemyStats.sequences[Random.Range(0, ennemyStats.sequences.Length)].sequence;
        ennemyHealth = ennemyStats.health;
        UpdateUI(ennemyHealthUI, ennemyHealth.ToString());
        ennemyDead = false;
        ennemyKatana.transform.parent.gameObject.SetActive(true);

        StartCoroutine(EnnemySequence());
        yield return null;
    }

    IEnumerator NextRoom()
    {
        yield return new WaitForSeconds(2);
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
        
        yield return null;
    }
}
