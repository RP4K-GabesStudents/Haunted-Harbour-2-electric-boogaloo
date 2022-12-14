using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(CircleCollider2D))]

public class Coin : MonoBehaviour
{
    int value;
    [SerializeField] protected AudioClip coinCollectSound;

    // Start is called before the first frame update
    void Start()
    {
        value = 5;
    }

    // Update is called once per frame
    void Update()
    {

    }


    private void OnTriggerEnter2D(Collider2D col)
    {
        if (col.gameObject.TryGetComponent(out Player ply))
        {
            ply.coinCount += value;
            GameManager.Instance.AudioManager.PlayOneShot(coinCollectSound);
            Destroy(gameObject); //1) refers to the time before its destroyed
        }
    }
}
