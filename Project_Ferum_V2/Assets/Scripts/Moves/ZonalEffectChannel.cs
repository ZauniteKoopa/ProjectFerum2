using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ZonalEffectChannel : ChannelMove
{
    //Data on the zonal stat effect applied
    private int statType;
    private float statFactor;
    private float zoneDuration;

    //Refernce variables on move
    private Transform effectZone;
    private EntityStatus status;

    //Data structure to keep track of who's already effected
    private HashSet<EntityStatus> effected;

    
    //Constructor
    public ZonalEffectChannel(EntityStatus es, float cd, float channel, int type, float factor, float duration, string hitboxSrc) : base(es, cd, channel) {
        status = es;
        statType = type;
        statFactor = factor;
        effectZone = Resources.Load<Transform>(hitboxSrc);
        effected = new HashSet<EntityStatus>();
        zoneDuration = duration;
    }

    //Method to call when channel is done
    public override void executeFinishedChannel() {
        Transform zone = Object.Instantiate(effectZone, status.transform);
        zone.GetComponent<EffectZone>().setProperties(this, zoneDuration);
        zone.tag = assignHitboxTag(status.tag);
        zone.parent = null;
    }

    //Method to enact effects: if they entered the zone, add them to effected. if they exit, remove
    public override void enactEffects(EntityStatus tgt) {
        if (effected.Contains(tgt)) {
            effected.Remove(tgt);
            tgt.applyZonalEffect(statType, 1f / statFactor);
            Debug.Log("reverse effect");
        } else {
            effected.Add(tgt);
            tgt.applyZonalEffect(statType, statFactor);
            Debug.Log("apply effect");
        }
    }
}
