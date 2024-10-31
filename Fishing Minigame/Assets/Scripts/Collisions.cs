using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class Collisions : MonoBehaviour
{
    /*public bool CheckCollision(GameObject obj1, GameObject obj2)
    {
        if (obj1.GetComponent<SpriteRenderer>().bounds.min.x < obj2.GetComponent<SpriteRenderer>().bounds.max.x &&
            obj1.GetComponent<SpriteRenderer>().bounds.max.x > obj2.GetComponent<SpriteRenderer>().bounds.min.x &&
            obj1.GetComponent<SpriteRenderer>().bounds.min.y < obj2.GetComponent<SpriteRenderer>().bounds.max.y &&
            obj1.GetComponent<SpriteRenderer>().bounds.max.y > obj2.GetComponent<SpriteRenderer>().bounds.min.y)
        {
            return true;
        }
        return false;
    }*/

    /*public bool CheckCollision(GameObject obj1, GameObject obj2)
    {
        //Testing border collisions instead of bounds collisions
        //X = left, Y = bottom, Z = right, W = top
        if (obj1.GetComponent<SpriteRenderer>().sprite.border.x < obj2.GetComponent<SpriteRenderer>().sprite.border.z &&
            obj1.GetComponent<SpriteRenderer>().sprite.border.z > obj2.GetComponent<SpriteRenderer>().sprite.border.x &&
            obj1.GetComponent<SpriteRenderer>().sprite.border.y < obj2.GetComponent<SpriteRenderer>().sprite.border.w &&
            obj1.GetComponent<SpriteRenderer>().sprite.border.w > obj1.GetComponent<SpriteRenderer>().sprite.border.y)
        {
            Debug.Log("Is Colliding");
            return true;
        }
        return false;
    }*/ //This did not work

    public bool CheckCollision(GameObject obj1, GameObject obj2) //THIS WORKS WAY BETTER YAY
    {
        BoxCollider2D collider1 = obj1.GetComponent<BoxCollider2D>();
        Vector2 colliderSize = new Vector2();
        colliderSize.x = (obj1.GetComponent<SpriteRenderer>().sprite.bounds.size.x - (obj1.GetComponent<SpriteRenderer>().sprite.border.x + obj1.GetComponent<SpriteRenderer>().sprite.border.z)
            / obj1.GetComponent<SpriteRenderer>().sprite.pixelsPerUnit);
        colliderSize.y = (obj1.GetComponent<SpriteRenderer>().sprite.bounds.size.y - (obj1.GetComponent<SpriteRenderer>().sprite.border.w + obj1.GetComponent<SpriteRenderer>().sprite.border.y)
            / obj1.GetComponent<SpriteRenderer>().sprite.pixelsPerUnit);
        collider1.size = colliderSize;

        BoxCollider2D collider2 = obj2.GetComponent<BoxCollider2D>();
        colliderSize.x = (obj2.GetComponent<SpriteRenderer>().sprite.bounds.size.x - (obj2.GetComponent<SpriteRenderer>().sprite.border.x + obj2.GetComponent<SpriteRenderer>().sprite.border.z)
            / obj2.GetComponent<SpriteRenderer>().sprite.pixelsPerUnit);
        colliderSize.y = (obj2.GetComponent<SpriteRenderer>().sprite.bounds.size.y - (obj2.GetComponent<SpriteRenderer>().sprite.border.w + obj2.GetComponent<SpriteRenderer>().sprite.border.y)
            / obj2.GetComponent<SpriteRenderer>().sprite.pixelsPerUnit);
        collider2.size = colliderSize;

        if (collider1.bounds.Intersects(collider2.bounds))
        {
            Debug.Log("Is colliding");
            return true;
        }
        return false;
    }

    public bool CheckMouseOverlap(Vector2 mousePos, GameObject obj)
    {
        if (mousePos.x < obj.GetComponent<SpriteRenderer>().bounds.max.x &&
            mousePos.x > obj.GetComponent<SpriteRenderer>().bounds.min.x &&
            mousePos.y < obj.GetComponent<SpriteRenderer>().bounds.max.y &&
            mousePos.y > obj.GetComponent<SpriteRenderer>().bounds.min.y)
        {
            return true;
        }
        return false;
    }
}
