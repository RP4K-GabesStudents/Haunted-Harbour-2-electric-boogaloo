using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TurretA : Turrets
{
    [SerializeField] private int shootCount;
    // Start is called before the first frame update
    void Start()
    {
        //test he he he eh can you see this?
    }

    // Update is called once per frame
    void Update()
    {
        if (isShooting)
        {
            CheckFireAmount();
        }
    }

    private void CheckFireAmount()
    {
        if (shootCount++ >= 8)
        {
            stopShoot -= 0.1f;

            shootCount = 0;
        }
    }
}
