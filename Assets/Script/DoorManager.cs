using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;
using UnityEngine.Rendering.Universal;

public class DoorManager : MonoBehaviour
{
    [SerializeField] GameObject fightScene;
    [SerializeField] RoomSetUp room;
    [SerializeField] Player player;

    [Header("Door")]
    [SerializeField] GameObject door;
    [SerializeField] float doorOpeningTime;
    [SerializeField] float doorZoom;

    [Header("Timer")]
    [SerializeField] float timer;
    float timerMultiplier = 1;
    [SerializeField] Slider timerSlider;
    [SerializeField] Slider timerSliderBis;

    [Header("Flash")]
    [SerializeField] Light flash;
    [SerializeField] float flashIntensity;
    [SerializeField] float flashTime;
    [SerializeField] AnimationCurve flashCurve;

    [Header("BehindDoor")]
    bool ennemyBehindDoor;
    bool pnjBehindDoor;
    [SerializeField] DecalProjector silhouette;
    [SerializeField] Material defaultPnjSilhouette;

    bool actionChose;
    void Start()
    {
        timerSlider.maxValue = timer;
        timerSliderBis.maxValue = timer;
        Init();
    }

    // Update is called once per frame
    void Update()
    {
        if (timer > 0)
        {
            timer -= Time.deltaTime*timerMultiplier;
            timerSlider.value = timer;
            timerSliderBis.value = timer;
        }
        if(timer <= 0)
        {
            actionChose = true;
            timerSlider.gameObject.SetActive(false);
            timerSliderBis.gameObject.SetActive(false);
            StartCoroutine(OpenDoor());
            StartCoroutine(FlashTransi());
        }
    }

    void Init()
    {
        switch (room.roomSpaceList[0])
        {
            case RoomSetUp.RoomSpaces.Ennemy:
                ennemyBehindDoor = true;
                silhouette.material = room.ennemyList[0].silhouetteDecal;
                break;
            case RoomSetUp.RoomSpaces.PNJ:
                pnjBehindDoor = true;
                silhouette.material = defaultPnjSilhouette;
                break;
            default:
                break;
        }
    }
    public void Open(InputAction.CallbackContext context)
    {
        if (context.performed  && !actionChose)
        {
            actionChose = true;
            if (ennemyBehindDoor)
            {
                Player.health -= room.ennemyList[0].damage;
            }

            StartCoroutine(OpenDoor());
            StartCoroutine(FlashTransi());
            timerSlider.gameObject.SetActive(false);
            timerSliderBis.gameObject.SetActive(false);

        }
    }
    IEnumerator OpenDoor()
    {
        Vector3 Gotoposition = new Vector3(door.transform.position.x+10, door.transform.position.y, door.transform.position.z);
        float elapsedTime = 0;
        Vector3 currentPos = door.transform.position;

        while (elapsedTime < doorOpeningTime)
        {
            door.transform.position = Vector3.Lerp(currentPos, Gotoposition, (elapsedTime / doorOpeningTime));
            elapsedTime += Time.deltaTime;
            yield return null;
        }

        yield return null;
    }

    IEnumerator FlashTransi()
    {
        float elapsedTime = 0;
        float currentPos = flash.intensity;

        while (elapsedTime < flashTime)
        {
            float curvePercent = flashCurve.Evaluate(elapsedTime / flashTime);
            flash.intensity = Mathf.Lerp(currentPos, flashIntensity, curvePercent);
            elapsedTime += Time.deltaTime;
            yield return null;
        }

        fightScene.SetActive(true);
        yield return null;
    }

    public void Attack(InputAction.CallbackContext context)
    {
        if (context.performed && !actionChose)
        {
            actionChose = true;
            if (ennemyBehindDoor)
            {
                room.ennemyList.Remove(room.ennemyList[0]);
            }
            door.SetActive(false);
            timerSlider.gameObject.SetActive(false);
            timerSliderBis.gameObject.SetActive(false);
            StartCoroutine(FlashTransi());
        }
    }

    bool listening;
    public void Listen(InputAction.CallbackContext context)
    {
        if (!listening && context.performed && !actionChose)
        {
            listening = true;
            timerMultiplier = 2;
            transform.position -= Vector3.forward * doorZoom;
        }
    }
}
