using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CoinController : MonoBehaviour {

    [SerializeField] private Coin coinPrefab;
    private GameUI ui;
    public int amount;

    void Start() {
        ui = FindObjectOfType<GameUI>();
        updateUI();
    }

    /// Public -- 

    public void dropCoin(Vector3 playerPos, float direction) {
        if (amount > 0) {
            amount--;
            updateUI();
            Coin coin = Instantiate(coinPrefab);
            Vector3 distance = randomCoinDistance();
            
            if (direction > 0) {
                distance.x *= -1;
            }
            coin.gameObject.transform.position = playerPos + new Vector3(distance.x, distance.y);
            coin.drop(distance.x, 0.5f - distance.y);
        }
    }

    public void pickup(Coin coin, Vector3 playerPos) {
        coin.pickup(playerPos + new Vector3(0, 0.5f), () => {
            amount++;
            updateUI();
            Destroy(coin);
        });
    }

    /// Private -- 

    private Vector3 randomCoinDistance() {
        return new Vector3(Random.Range(0.2f,0.6f), Random.Range(-0.3f, 0.3f));
    }

    private void updateUI() {
        ui.setCoinsAmount(amount);
    }
}