using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WeaponController : MonoBehaviour {
    [SerializeField] private Weapon bowPrefab;
    [SerializeField] private Weapon hammerPrefab;

    private List<Weapon> worldWeapons = new List<Weapon>();
    private Dictionary<Weapon, GameObject> reservedWeapons = new Dictionary<Weapon, GameObject>();

    /// Public -- 

    public void drop(WeaponType type, Vector3 position) {
        Weapon weapon = Instantiate(prefabByType(type));
        position.x += Random.Range(-0.1f, 0.1f);
        position.y += Random.Range(-0.1f, 0.1f);
        weapon.drop(position);
        worldWeapons.Add(weapon);
    }

    public void pickup(Weapon weapon, System.Action onComplete = null) {
        weapon.pickup(onComplete: () => {
            onComplete?.Invoke();
            Destroy(weapon.gameObject);
        });
        worldWeapons.Remove(weapon);
    }

    public Weapon lookForWeapon(Vector3 pos, float distance) {
        foreach (Weapon weapon in worldWeapons) {
            if (Vector2.Distance(pos, weapon.transform.position) < distance) {
                GameObject reserver = null;
                if (!reservedWeapons.TryGetValue(weapon, out reserver))
                    return weapon;
            }
        }
        return null;
    }

    public bool reserveWeaponForPickup(Weapon weapon, GameObject reserver) {
        GameObject owner = null;
        if (reservedWeapons.TryGetValue(weapon, out owner)) {
            if (owner == reserver) {
                // already reserved by this reserver
                return true;
            } else {
                // reserved by someone else
                return false;
            }
        }
        reservedWeapons.Add(weapon, reserver);
        return true;
    }

    public void removeReservation(Weapon weapon) {
        reservedWeapons.Remove(weapon);
    }

    public static WeaponType weaponTypeForBuilding(BuildingType buildingType) {
        switch (buildingType) {
            case BuildingType.BowShop: return WeaponType.Bow;
            case BuildingType.HammerShop: return WeaponType.Hammer;
            default:
                throw new System.Exception($"Weapon type not set to drop by {buildingType}");
        }
    }

    /// Private -- 

    private Weapon prefabByType(WeaponType type) {
        switch (type) {
            case WeaponType.Bow:
                return bowPrefab;
            case WeaponType.Hammer:
                return hammerPrefab;
            default:
                throw new System.Exception($"Unknown weapon {type}");
        }
    }
}