using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class GameManager : MonoBehaviour
{
    private static GameManager _instance;    public static GameManager Instance => _instance;    [Header("Layers")]    [SerializeField] private LayerMask enemyLayer;  //can be edited in Unity
    [SerializeField] private LayerMask playerLayer;    [SerializeField] private LayerMask worldLayer;    [Header("Player")]    [SerializeField] private Player player;    [SerializeField] private Transform bulletParent;    [SerializeField] private AudioSource audioManager;    public AudioSource AudioManager => audioManager;    [SerializeField] private Slider healthBar;    [SerializeField] private TextMeshProUGUI healthText;    public LayerMask EnemyLayer => enemyLayer;     //acessible as read only. => is a getter
    public LayerMask PlayerLayer => playerLayer;
    public LayerMask WorldLayer => worldLayer;    public Player Player => player;    public Transform BulletParent => bulletParent;    public Action OnGameStart;    private void Awake()    {        if (_instance != null && _instance != this) //if there's an instance which is not this, destroy it
        {            Destroy(this.gameObject);        }
        else
        {            _instance = this;        }        audioManager = GetComponent<AudioSource>();    }

    private void Start()
    {
        OnGameStart?.Invoke();
        //by the way ? means we're reffering to null
        //??= means if null
        //?. means if not null then do this
    }

    public void UpdatePlayerHealth(int curHP)
    {
        healthText.text = $"{curHP} / {player.maxHealth}";
        healthBar.value = (float)curHP / player.maxHealth;
    }
}
