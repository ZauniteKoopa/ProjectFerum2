using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TimedEffectQueue : MonoBehaviour
{
    //Stats array consisting of values that entity will use when accessing stats
    //  [(first 5 stats), POISON, PARALYSIS, BURN]
    //  The first 5 stats represent how much the associated stats changed
    //  The rest represent the number of stat effects associated with the status in the queue
    private float[] statEffectArr;

    //Sorted Queue to put TimedEffects in with associated timer
    private LinkedList<TimedEffect> queue;
    private float timer;

    //Constants concerning status effects on stats
    private const float PARALYSIS_SPEED_REDUCTION = 0.5f;
    private const float BURN_PWR_REDUCTION = 0.5f;

    //Constants concerning health decay on poison
    private const float HEALTH_POISON_PERCENT = 0.05f;

    //Default constructor
    public TimedEffectQueue() {
        statEffectArr = new float[(int)GeneralConstants.NUM_EFFECTS];
        queue = new LinkedList<TimedEffect>();

        for (int i = 0; i < (int)GeneralConstants.statIDs.NUM_STATS; i++) {
            statEffectArr[i] = 1f;
        }

        timer = 0f;
    }

    //Update method for queue
    public void updateQueue() {
        if (queue.Count > 0) {
            timer += Time.deltaTime;

            //Check the front of the queue: if its finished, reverse the effect and remove it
            TimedEffect front = queue.First.Value;
            while (front != null && front.finished(timer)) {
                int statusType = front.getEffectType();
                statEffectArr[statusType] = front.reverseEffect(statEffectArr[statusType]);
                queue.RemoveFirst();

                if (queue.Count == 0) {
                    timer = 0f;
                }

                front = queue.First.Value;
            }
        }
    }

    //Add method for the queue
    public void addEffect(TimedEffect effect) {
        //Apply status effect
        int statusType = effect.getEffectType();
        statEffectArr[statusType] = effect.applyEffect(statEffectArr[statusType]);

        //Find appropriate spot for effect (stop when you find another effect that stops later than this effect)
        effect.setEndTime(timer);
        LinkedListNode<TimedEffect> curNode = queue.First;

        while (curNode != null && !effect.lessThan(curNode.Value)) {
            curNode = curNode.Next;
        }

        //Add to queue
        if (curNode == null) {
            queue.AddLast(effect);
        } else {
            queue.AddBefore(curNode, effect);
        }
    }

    //Accessor methods for stat factors
    public float getStatFactor(int statID) {
        if (statID >= (int)GeneralConstants.statIDs.NUM_STATS)
            throw new System.Exception("Error: invalid stat ID");

        float factor = statEffectArr[statID];

        //Consider Paralysis (reduces speed by a half)
        if (statEffectArr[GeneralConstants.PARALYSIS_ID] > 0) {
            if (statID == (int)GeneralConstants.statIDs.SPEED) {
                return factor * PARALYSIS_SPEED_REDUCTION;
            }
        }

        //Consider burning (reduces special attack and attack by half)
        if (statEffectArr[GeneralConstants.BURN_ID] > 0) {
            if (statID == (int)GeneralConstants.statIDs.ATTACK || statID == (int)GeneralConstants.statIDs.SP_ATTACK)
                return factor * BURN_PWR_REDUCTION;
        }

        return factor;
    }

    //Method that states whether or not the entity can regenerate health or armor
    public bool canRegenHealth() {
        return statEffectArr[GeneralConstants.BURN_ID] > 0 || statEffectArr[GeneralConstants.POISON_ID] > 0;
    }

    public bool canRegenArmor() {
        return statEffectArr[GeneralConstants.BURN_ID] > 0;
    }

    //Method that updates health if poisoned: POISON DOESN'T KILL
    public float updateHealth(float curHealth, float maxHealth) {
        //If poisoned
        if (statEffectArr[GeneralConstants.POISON_ID] > 0) {
            float newHealth = curHealth - (maxHealth * HEALTH_POISON_PERCENT);
            if (newHealth <= 0f)
                newHealth = 1f;
            return newHealth;
        }

        return curHealth;
    }
}
