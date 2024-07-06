using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemySpawn : MonoBehaviour
{
    [SerializeField] private GameObject _enemyPrefab;
    [SerializeField] private List<Vector3> _enemyPositions = new List<Vector3>();

    public void Start()
    {
        StartCoroutine(SpawnEnemy());
    }

    IEnumerator SpawnEnemy()
    {
        while (true)
        {
            yield return new WaitForSeconds(1f);
            GameObject enemy = Instantiate(_enemyPrefab);
            enemy.transform.position = _enemyPositions[Random.Range(0, _enemyPositions.Count)];
        }
    }
}
