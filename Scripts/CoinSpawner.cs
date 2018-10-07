using UnityEngine;

public class CoinSpawner : MonoBehaviour {
    public int maxCoin = 5;
    public float spawnChance = 0.5f;
    public bool forceSpawnAll = false;

    private GameObject[] coins;

    private void Awake() {
        coins = new GameObject[transform.childCount];
        for (int i = 0; i < transform.childCount; i++) {
            coins[i] = transform.GetChild(i).gameObject;
        }

        OnDisable();
    }

    private void OnEnable() {
        if (Random.Range(0f, 1f) > spawnChance)
            return;

        if (forceSpawnAll == true) {
            for (int i = 0; i < maxCoin; i++) {
                coins[i].SetActive(true);
            }            
        }
        else {
            int coinCount = Random.Range(0, maxCoin);
            for (int i = 0; i < coinCount; i++) {
                coins[i].SetActive(true);
            }
        }
    }

    private void OnDisable() {
        foreach (GameObject go in coins)
            go.SetActive(false);
    }

}
