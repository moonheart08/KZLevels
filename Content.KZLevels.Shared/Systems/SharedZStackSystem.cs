﻿using System;
using System.Diagnostics.CodeAnalysis;
using Content.KZlevels.Shared.Components;
using Robust.Shared.GameObjects;

namespace Content.KZlevels.Shared.Systems;

/// <summary>
/// This handles Z stacks, including their construction and destruction.
/// </summary>
public abstract class SharedZStackSystem : EntitySystem
{
    /// <inheritdoc/>
    public override void Initialize()
    {
        SubscribeLocalEvent<ZStackMemberComponent, ComponentShutdown>(OnMemberShutdown);
        SubscribeLocalEvent<ZStackTrackerComponent, ComponentShutdown>(OnStackShutdown);
    }

    private void OnStackShutdown(Entity<ZStackTrackerComponent> ent, ref ComponentShutdown args)
    {
        foreach (var map in ent.Comp.Maps.ToArray()) // Defensive copy, as map shutdown will modify the list.
        {
            RemComp<ZStackMemberComponent>(map);
        }
    }

    private void OnMemberShutdown(Entity<ZStackMemberComponent> ent, ref ComponentShutdown args)
    {
        if (Deleted(ent.Comp.Tracker) || !TryComp(ent.Comp.Tracker, out ZStackTrackerComponent? tracker))
            return;

        if (!tracker.Maps.Remove(ent))
            Log.Error($"BUG: {ent} (a stack member) is being removed, and claims to have a tracker, but the tracker {tracker} did not claim to own it!");
    }

    public bool TryGetZStack(EntityUid ent, [NotNullWhen(true)] out Entity<ZStackTrackerComponent>? stack)
    {
        if (Deleted(ent))
        {
            stack = null;
            return false;
        }

        var xform = Transform(ent);
        if (TryComp(xform.MapUid, out ZStackMemberComponent? comp))
        {
            stack = new(comp.Tracker, Comp<ZStackTrackerComponent>(comp.Tracker));
            return true;
        }

        stack = null;
        return false;
    }
}
