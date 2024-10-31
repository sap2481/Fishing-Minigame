using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Environment : MonoBehaviour
{
    //==== FIELDS ====
    [SerializeField] GameObject oceanTile; //Ocean Tile Prefab
    List<GameObject> tiles; //List of tile prefabs
    [SerializeField] GameObject waterHolder; //Parent to hold the water tiles

    GameObject player;
    Vector3 lastPlayerPos;

    [SerializeField] GameObject rockPrefab;
    List<GameObject> rocks; 
    [SerializeField] GameObject rockHolder;

    float x, y;
    float sizeValue;

    [SerializeField] Collisions collisions;
    
    //==== START ====
    void Start()
    {
        tiles = new List<GameObject>();
        rocks = new List<GameObject>();
        player = GameObject.FindGameObjectWithTag("Player");
        lastPlayerPos = Vector3.zero;
        sizeValue = 3.8f;

        x = sizeValue * 3;
        y = sizeValue * 3;

        //Create Initial Series of Tiles
        for (int i = 0; i < 7; i++)
        {
            for (int j = 0; j < 7; j++)
            {
                tiles.Add(Instantiate(oceanTile, new Vector3(x, y, 0), Quaternion.identity, waterHolder.transform));
                x -= sizeValue;
            }
            x = sizeValue * 3;
            y -= sizeValue;
        }

        //Create Rocks
        for (int i = 0; i < 10; i++)
        {
            rocks.Add(Instantiate(rockPrefab, new Vector3(Random.Range(-30f, 30f), Random.Range(-30f, 30f)), Quaternion.identity, rockHolder.transform));

            //Make sure rocks don't spawn over the player
            bool isNotColliding = false;
            while (!isNotColliding)
            {
                if (collisions.CheckColliderCollision(rocks[i], player))
                {
                    rocks[i].transform.position = new Vector3(Random.Range(-30f, 30f), Random.Range(-30f, 30f));
                }
                else
                {
                    isNotColliding = true;
                }
            }
        }
    }

    //==== UPDATE ====
    void Update()
    {
        //Change tile positions
        foreach (GameObject tile in tiles)
        {
            float xDistance = Mathf.Abs(player.transform.position.x - tile.transform.position.x);
            if (xDistance >= (sizeValue * 4))
            {
                if (player.transform.position.x > lastPlayerPos.x) //If the player is moving right, shift tiles to the right
                {
                    tile.transform.position = new Vector3(player.transform.position.x + (sizeValue * 3), tile.transform.position.y);
                }
                else //If the player is moving left, shift tiles to the left
                {
                    tile.transform.position = new Vector3(player.transform.position.x - (sizeValue * 3), tile.transform.position.y);
                }
            }

            float yDistance = Mathf.Abs(player.transform.position.y - tile.transform.position.y);
            if (yDistance >= (sizeValue * 4))
            {
                if (player.transform.position.y > lastPlayerPos.y) //If the player is moving up, shift tiles up
                {
                    tile.transform.position = new Vector3(tile.transform.position.x, player.transform.position.y + (sizeValue * 3));
                }
                else //If the player is moving down, shift tiles down
                {
                    tile.transform.position = new Vector3(tile.transform.position.x, player.transform.position.y - (sizeValue * 3));
                }
            }
        }

        //Check for Collisions with Rocks
        foreach (GameObject rock in rocks)
        {
            if (collisions.CheckColliderCollision(rock, player)) {
                player.GetComponent<Player>().StartBounceback(rock);
            }
        }

        if (player.GetComponent<Player>().ResetTiles) //Reset tile placement around player placement
        {
            foreach (GameObject tile in tiles) { Destroy(tile); }
            tiles.Clear();

            x = sizeValue * 3;
            y = sizeValue * 3;

            //Create Initial Series of Tiles
            for (int i = 0; i < 7; i++)
            {
                for (int j = 0; j < 7; j++)
                {
                    tiles.Add(Instantiate(oceanTile, new Vector3(x, y, 0), Quaternion.identity, waterHolder.transform));
                    x -= sizeValue;
                }
                x = sizeValue * 3;
                y -= sizeValue;
            }

            player.GetComponent<Player>().ResetTiles = false;
        }

        lastPlayerPos = player.transform.position;
    }
}
