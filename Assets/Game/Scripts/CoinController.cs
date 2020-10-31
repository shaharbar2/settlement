﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CoinController : MonoBehaviour {

    public int amountBank;

    [SerializeField] private Coin coinPrefab;
    private GameUI ui;
    private List<Coin> worldCoins = new List<Coin>();
    private Dictionary<Coin, GameObject> reservedCoins = new Dictionary<Coin, GameObject>();

    void Start() {
        ui = FindObjectOfType<GameUI>();
        updateUI();
    }

    /// Public -- 

    public void dropCoin(Vector3 playerPos, float direction) {
        if (amountBank > 0) {
            amountBank--;
            updateUI();
            Coin coin = Instantiate(coinPrefab);
            Vector3 distance = randomCoinDistance();
            worldCoins.Add(coin);

            if (direction > 0) {
                distance.x *= -1;
            }
            coin.gameObject.transform.position = playerPos + new Vector3(distance.x, distance.y);
            coin.drop(distance.x, 0.5f - distance.y);
        }
    }

    public void pickup(Coin coin, Vector3 playerPos) {
        coin.pickup(playerPos + new Vector3(0, 0.5f), () => {
            amountBank++;
            updateUI();
            Destroy(coin);
            worldCoins.Remove(coin);
        });
    }

    public Coin lookForCoin(Vector3 pos, float distance) {
        foreach (Coin coin in worldCoins) {
            if (Vector2.Distance(pos, coin.transform.position) > distance) {
                GameObject reserver = null;
                if (!reservedCoins.TryGetValue(coin, out reserver))
                    return coin;
            }
        }
        return null;
    }

    public bool reserveCoinForPickup(Coin coin, GameObject reserver) {
        GameObject owner = null;
        if (reservedCoins.TryGetValue(coin, out owner)) {
            if (owner == reserver) {
                // already reserved by this reserver
                return true;
            } else {
                // reserved by someone else
                return false;
            }
        } 
        reservedCoins.Add(coin, reserver);
        return true;
    }

    /// Private -- 

    private Vector3 randomCoinDistance() {
        return new Vector3(Random.Range(0.2f, 0.6f), Random.Range(-0.3f, 0.3f));
    }

    private void updateUI() {
        ui.setCoinsAmount(amountBank);
    }
}