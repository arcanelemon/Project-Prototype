using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyTestCube : Enemy
{
    private Weapon[] weapons;

    private Enemy enemyComponent;

    // Start is called before the first frame update
    void Start()
    {
        enemyComponent = GetComponent<Enemy>();
        weapons = GetComponentsInChildren<Weapon>();
        foreach (Weapon weapon in weapons)
        {
            weapon.Initialize();
        }

        StartCoroutine(TestFireCoroutine());
    }

    // Update is called once per frame
    void Update()
    {

    }

    private IEnumerator TestFireCoroutine()
    {
        while (gameObject.activeInHierarchy)
        {
            yield return new WaitForSeconds(1);
            foreach (Weapon weapon in weapons)
            {
                weapon.Shoot();
            }
        }
    }
}