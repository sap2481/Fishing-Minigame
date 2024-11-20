using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Quest : MonoBehaviour
{
    //==== FIELDS ====
    int type; //Indicates the type of quest that it will be.
    /*
     1 = Fish-Finding Quest: Catch a specific fish, and Fork will pay double
     2 = Timer Quest: Fill your cargo hold with fish in a certain amount of time (requires time pop-up)
    (more will be implemented when the game is developed (delivery quests between outposts, etc.)
     */
    string description; //Description of Quest
    string dialogue; //Fork's quest dialogue
    Fish goalFish; //IF TYPE 1, which fish is the player trying to catch?
    int seconds; //IF TYPE 2, how many seconds a player has to fill their inventory
    int reward; //IF TYPE 2, the reward for completing the quest

    //==== PROPERTIES ====
    public int Type { get { return type; } }
    public string Description { get { return description; } }
    public string Dialogue { get { return dialogue; } }
    public Fish GoalFish { get { return goalFish; } }
    public int Seconds { get { return seconds; } }
    public int Reward { get { return reward; } }

    //==== CONSTRUCTOR ====
    public Quest(int type)
    {
        this.type = type;
        switch (type)
        {
            case 1: //Type 1 Quest - Fetch Fish Quest
                goalFish = new Fish(Random.Range(1, 4)); //Randomly selects one of the 9 fish to be the goal fish
                description = $"Catch one <b>{goalFish.Name}</b> & sell it to Fork for double its value.";
                int randomDesc = Random.Range(1, 4);
                switch (randomDesc)
                {
                    case 1:
                        dialogue = $"I'm looking for a <b>{goalFish.Name}</b> and willing to pay double. Wanna go find one for me? Can't exactly do it myself.";
                        break;
                    case 2:
                        dialogue = $"I've got a hankering for a <b>{goalFish.Name}</b>. Don't ask me why, that's private. Just go get one for me, and I'll pay double what I usually do.";
                        break;
                    case 3:
                        dialogue = $"I wish I could go find a <b>{goalFish.Name}</b>, but I can't, 'cause I'm a video game NPC and I'm stuck here. If you can nab one for me, I'll pay double.";
                        break;
                    default:
                        break;
                }
                break;

            case 2: //Type 2 Quest - Timer Quest
                seconds = Random.Range(120, 241);
                reward = seconds + Random.Range(100, 251);
                description = $"Fill your cargo hold with fish and return to Fork before time runs out for <b>${reward}</b>";
                int randomDescription = Random.Range(1, 4);
                switch (randomDescription)
                {
                    case 1:
                        dialogue = $"I've got a challenge for you - get your cargo hold from empty to full in {seconds} seconds. Oh, that seems specific, does it? That's 'cause the number was randomly generated, dingus. This whole game is. Get with the program. Don't you want an extra ${reward}?";
                        break;
                    case 2:
                        dialogue = $"Wanna keep playing the game? Bored of just plain ol' fishing? Try filling your cargo hold in {seconds} seconds. If you do, I'll pay you ${reward}, just for doin' it! Doesn't that seem like a fun way to stave off the boredom?";
                        break;
                    case 3:
                        dialogue = $"Timer quest! If you can fill your cargo hold with fish, any fish, in {seconds} seconds, I'll pay you ${reward}. Why, you ask? It's cute that you think I have any answers for you. These prompts are pre-written - I'm just as confused as you are. Seems like a fun challenge though!";
                        break;
                    default:
                        break;
                }
                break;
        }
    }
    
    //==== START ====
    void Start()
    {
        
    }

    //==== UPDATE ====
    void Update()
    {
        
    }
}
