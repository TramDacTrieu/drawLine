using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class BeeHive : MonoBehaviour
{
    int beeNumber = 7;
    float spawnRange = 2f;

    public void SpawnBees() {
        for (int i = 0; i < beeNumber; i++) {
            Vector3 randomPos = new Vector3(Random.Range(-spawnRange, spawnRange), Random.Range(-spawnRange, spawnRange));
            ObjectPool.Instance.GetGameObjectFromPool("bee", transform.position + randomPos);
        }
    }

    public void Replay() {
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }
}
