using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Collisions : MonoBehaviour
{
    public bool CheckCollision(GameObject obj1, GameObject obj2)
    {
        if (obj1.GetComponent<SpriteRenderer>().bounds.min.x < obj2.GetComponent<SpriteRenderer>().bounds.max.x &&
            obj1.GetComponent<SpriteRenderer>().bounds.max.x > obj2.GetComponent<SpriteRenderer>().bounds.min.x &&
            obj1.GetComponent<SpriteRenderer>().bounds.min.y < obj2.GetComponent<SpriteRenderer>().bounds.max.y &&
            obj1.GetComponent<SpriteRenderer>().bounds.max.y > obj2.GetComponent<SpriteRenderer>().bounds.min.y)
        {
            return true;
        }
        return false;
    }
}
