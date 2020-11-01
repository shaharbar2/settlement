using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CoinController : MonoBehaviour {

    [HideInInspector] public int amountBank;

    [SerializeField] private Coin coinPrefab;
    private GameUI ui;
    private List<Coin> worldCoins = new List<Coin>();
    private Dictionary<Coin, GameObject> reservedCoins = new Dictionary<Coin, GameObject>();

    void Start() {
        amountBank = Constants.instance.INITIAL_COINS_AMOUNT;
        ui = FindObjectOfType<GameUI>();
        updateUI();
    }

    /// Public -- 

    public Coin dropCoin(Vector3 pos, float direction, CoinDropType dropType) {
        if (dropType == CoinDropType.ByPlayer) {
            amountBank--;
            if (amountBank <= 0) {
                return null;
            }
        }

        updateUI();
        Coin coin = Instantiate(coinPrefab);
        coin.dropType = dropType;
        Vector3 distance = randomCoinDistance();
        worldCoins.Add(coin);

        if (direction > 0) {
            distance.x *= -1;
        }
        coin.gameObject.transform.position = pos + new Vector3(distance.x, distance.y);
        coin.drop(distance.x, 0.5f - distance.y);
        return coin;
    }

    public void pickup(Coin coin, Vector3 pos, bool byPlayer) {
        coin.pickup(pos + new Vector3(0, 0.5f), () => {
            if (byPlayer) {
                amountBank++;
            }
            updateUI();
            removeReservation(coin);
            Destroy(coin.gameObject);
            worldCoins.Remove(coin);
        });
    }

    public void spend(int amount, Vector3 playerPos, Vector3 destPos, System.Action onComplete) {
        if (amountBank >= amount) {
            amountBank -= amount;
            updateUI();
            for (int i = 0; i < amount; i++) {
                float idx = i;
                LeanTween.delayedCall(i * 0.2f, () => {
                    Coin coin = Instantiate(coinPrefab);
                    Vector3 distance = randomCoinDistance();
                    float spacing = 0.3f;
                    Vector3 dest = destPos;
                    dest.x -= (spacing * (float)(amount - 1)) / 2f;
                    dest.x += spacing * (float)idx;
                    coin.gameObject.transform.position = dest;
                    // call complete after last coin only
                    System.Action callback = idx == amount - 1 ? onComplete : null;
                    coin.spend(dest.x - playerPos.x, playerPos.y - dest.y + 0.5f, callback);
                });
            }
        }
    }

    public Coin lookForCoin(Vector3 pos, float distance, CoinDropType dropType) {
        foreach (Coin coin in worldCoins) {
            if (coin.dropType != dropType)continue;
            if (Vector2.Distance(pos, coin.transform.position) < distance) {
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

    public void removeReservation(Coin coin) {
        reservedCoins.Remove(coin);
    }

    /// Private -- 

    private Vector3 randomCoinDistance() {
        return new Vector3(Random.Range(0.4f, 0.6f), Random.Range(-0.2f, 0.2f));
    }

    private void updateUI() {
        ui.setCoinsAmount(amountBank);
    }
}