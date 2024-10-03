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
        /*Change tile positions
        foreach (GameObject tile in tiles)
        {
            if (Mathf.Abs(player.transform.position.x - tile.transform.position.x) > 15.2f)
            {
                if (player.transform.position.x > lastPlayerPos.x) //If the player is moving right, shift tiles to the right
                {
                    tile.transform.position = new Vector3(player.transform.position.x + (sizeValue * 4) - (player.transform.position.x % sizeValue), tile.transform.position.y);
                }
                else //If the player is moving left, shift tiles to the left
                {
                    tile.transform.position = new Vector3(player.transform.position.x - (sizeValue * 4) - (player.transform.position.x % sizeValue), tile.transform.position.y);
                }
            }
        }

        lastPlayerPos = player.transform.position;*/

        //You're on the right track... kind of... I think
    }
}
