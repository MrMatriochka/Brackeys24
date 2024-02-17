using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FurnitureRandomizer : MonoBehaviour
{
    [SerializeField] GameObject[] possibleObject;
    void Start()
    {
        int id = Random.Range(0,possibleObject.Length);
        Instantiate(possibleObject[id], transform.position, possibleObject[id].transform.rotation);
    }

}
