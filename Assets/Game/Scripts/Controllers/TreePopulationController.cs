using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TreePopulationController : MonoBehaviour {
    [SerializeField] private Tree treePrefab;

    private TilemapController tilemapController;
    private List<Tree> worldTrees = new List<Tree>();

    private float loopInterval = 0.1f;
    private float loopElapsed = 0;
    private float spawnInterval = 0;

    [SerializeField] private GameObject biomeContainer;
    [SerializeField] private Sprite[] mosses;
    [SerializeField] private Sprite[] dirts;
    [SerializeField] private Sprite[] grasses;
    [SerializeField] private Sprite[] rocks;
    [SerializeField] private int mossDensity;
    [SerializeField] private int dirtDensity;
    [SerializeField] private int grassDensity;
    [SerializeField] private int rocksDensity;
    
    void Awake() {
        tilemapController = FindObjectOfType<TilemapController>();
        spawnInterval = float.MaxValue;
        spawnBiomeObjects();
    }

    void Start() {
        spawnTreeAt(new Vector3(2.54f, 0.01f, 0f));
    }

    void Update() {
        spawnInterval += Time.deltaTime;
        loopElapsed += Time.deltaTime;
        if (loopElapsed >= loopInterval) {
            loopElapsed -= loopInterval;
            controlPopulation();
        }
    }

    private void spawnTreeAt(Vector3 pos) {
        Tree tree = Instantiate(treePrefab);
        tree.transform.position = pos;
        worldTrees.Add(tree);
        tree.onChoppedDown += onTreeChoppedDown;
        tilemapController.markUnwalkable(tree.transform.position, TilemapAreaType.S_1x1);
    }

    /// Private -- 

    private void controlPopulation() {
        if (worldTrees.Count < Constants.instance.MAX_TREES) {
            if (spawnInterval > Constants.instance.TREE_SPAWN_INTERVAL) {
                while (worldTrees.Count < Constants.instance.MIN_TREES) {
                    spawnRandomTree();
                    spawnInterval = 0;
                }
            }
        }
    }

    private void spawnRandomTree() {
        Tree tree = Instantiate(treePrefab);
        tree.transform.position = randomTreePosition();
        worldTrees.Add(tree);
        tree.onChoppedDown += onTreeChoppedDown;
        tilemapController.markUnwalkable(tree.transform.position, TilemapAreaType.S_1x1);
    }

    private void onTreeChoppedDown(Tree tree) {
        tilemapController.markWalkable(tree.transform.position, TilemapAreaType.S_1x1);
        worldTrees.Remove(tree);
        spawnInterval = 0;
    }

    private Vector3 randomTreePosition() {
        Vector3 pos = Vector3.zero;
        float R = BabyUtils.chance(0.1f) ? 1f : 2.6f; // center radius
        bool isOutsideCenter = false;
        int iterations = 0;
        int maxIterations = 100;
        while (!isOutsideCenter) {
            if (iterations++ > maxIterations) {
                return pos;
            }
            pos = tilemapController.randomPosition(walkable: true);
            isOutsideCenter = Mathf.Abs(pos.x/2) > R || Mathf.Abs(pos.y) > R;
        }
        return pos;
    }

    private void spawnBiomeObjects() {
        for (int i = 0; i < mossDensity; i++) {
            randomSpriteWithTexture(BabyUtils.arrayRandom(mosses), layer: "UnderCover");
        }
        for (int i = 0; i < dirtDensity; i++) {
            randomSpriteWithTexture(BabyUtils.arrayRandom(dirts), layer: "OverCover");
        }
        for (int i = 0; i < grassDensity; i++) {
            randomSpriteWithTexture(BabyUtils.arrayRandom(grasses), layer: "Objects");
        }
        for (int i = 0; i < rocksDensity; i++) {
            randomSpriteWithTexture(BabyUtils.arrayRandom(rocks), layer: "Objects");
        }
    }

    private void randomSpriteWithTexture(Sprite sprite, string layer) {
        Vector3 pos = tilemapController.randomPosition(walkable: true);
        var go = new GameObject();
        var spriteRenderer = go.AddComponent<SpriteRenderer>();
        spriteRenderer.sprite = sprite;
        spriteRenderer.sortingLayerName = layer;
        go.transform.position = pos;
        go.transform.localScale = new Vector3(BabyUtils.randomBool ? -1 : 1, 1, 1);
        spriteRenderer.spriteSortPoint = SpriteSortPoint.Pivot;
        go.transform.parent = biomeContainer.transform;
    }
}