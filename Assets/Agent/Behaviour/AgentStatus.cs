using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AgentStatus : MonoBehaviour
{
    // Animation curves are used here to enable more interesting changes to agent status
    // For example, hunger might not change much when the agent is not very hungry
    // But the agent's hunger could start to increase more rapidly after a certain point
    // This allows for the agent to not care so much about eating after it has just eaten
    public AnimationCurve HungerIncreasePerSecond;
    public AnimationCurve ThirstIncreasePerSecond;

    public float IncreasePerSecond = 0.01f;

    public float Hunger;
    public float Thirst;

    [SerializeField]
    float MaxHunger = 5f;
    [SerializeField]
    float MaxThirst = 5f;

    // Update is called once per frame
    void Update()
    {
        // Increase hunger over time
        if (Hunger < MaxHunger)
        {
            //Hunger += HungerIncreasePerSecond.Evaluate(Hunger) * Time.deltaTime;
            Hunger += IncreasePerSecond * Time.deltaTime;
        }
        else
        {
            Hunger = MaxHunger;
        }

        // Increase thirst over time
        if (Thirst < MaxThirst)
        {
            Thirst += IncreasePerSecond * Time.deltaTime;
        }
        else
        {
            Thirst = MaxThirst;
        }
    }

    public void Eat(float FoodValue)
    {
        if (Hunger - FoodValue <= 0)
        {
            Hunger = 0;
        }
        else
        {
            Hunger -= FoodValue;
        }
    }

    public void Drink(float DrinkValue)
    {
        if (Thirst - DrinkValue <= 0)
        {
            Thirst = 0;
        }
        else
        {
            Thirst -= DrinkValue;
        }
    }
}
