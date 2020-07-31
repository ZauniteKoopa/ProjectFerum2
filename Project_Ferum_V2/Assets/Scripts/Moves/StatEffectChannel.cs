using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StatEffectChannel : ChannelMove
{
    //Data on the statEffect
    private int statType;
    private float effectDuration;
    private float statFactor;

    //Reference variable
    private EntityStatus status;

    //Constructor method
    public StatEffectChannel(EntityStatus es, float cd, float channel, int type, float dur, float factor) : base(es, cd, channel) {
        status = es;
        statType = type;
        effectDuration = dur;
        statFactor = factor;
    }

    //Method to call when channel is done
    public override void executeFinishedChannel() {
        status.applyTimedEffect(new StatEffect(statType, effectDuration, statFactor));
    }

    //Enact effects method: does nothing
    public override void enactEffects(EntityStatus tgt) {}

}
