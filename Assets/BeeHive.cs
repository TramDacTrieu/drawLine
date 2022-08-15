using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using TMPro;

public class BeeHive : MonoBehaviour
{
    public static BeeHive Instance;

    int beeNumber = 7;
    float spawnRange = 0.5f;

    public GameObject winObj;
    public GameObject gameOverObj;
    public TextMeshProUGUI txtCountDown;

    int countDownTime = 5;

    private void Awake() {
        Instance = this;

        txtCountDown.text = "";
        winObj.SetActive(false);
        gameOverObj.SetActive(false);
    }

    public void SpawnBees() {
        for (int i = 0; i < beeNumber; i++) {
            Vector3 randomPos = new Vector3(Random.Range(-spawnRange, spawnRange), Random.Range(-spawnRange, spawnRange));
            ObjectPool.Instance.GetGameObjectFromPool("bee", transform.position + randomPos);
        }

        StartCoroutine(IECountDownWin());
    }

    public void Replay() {
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }

    IEnumerator IECountDownWin() {
        for (int i = countDownTime; i >= 0; i--) {
            txtCountDown.text = i.ToString();
            yield return new WaitForSeconds(1f);
        }

        txtCountDown.text = "";

        if (!gameOverObj.activeInHierarchy) {
            winObj.SetActive(true);
        }
    }

    public void GameOver() {
        gameOverObj.SetActive(true);
    }
}
