using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Environment : MonoBehaviour
{
    //==== FIELDS ====
    [SerializeField] GameObject oceanTile; //Ocean Tile Prefab
    List<GameObject> tiles; //List of tile prefabs
    GameObject player;
    Vector3 lastPlayerPos;
    [SerializeField] GameObject waterHolder;

    float x, y;
    float sizeValue;
    
    //==== START ====
    void Start()
    {
        tiles = new List<GameObject>();
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

        lastPlayerPos = player.transform.position;
    }
}
