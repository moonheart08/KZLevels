/*
 * This Source Code Form is subject to the terms of the Mozilla Public
 * License, v. 2.0. If a copy of the MPL was not distributed with this
 * file, You can obtain one at https://mozilla.org/MPL/2.0/.
 * This Source Code Form is "Incompatible With Secondary Licenses", as
 * defined by the Mozilla Public License, v. 2.0.
 * /

using System.Collections.Generic;
using Content.KZlevels.Shared.Components;
using Robust.Shared.Configuration;
using Robust.Shared.GameObjects;
using Robust.Shared.IoC;
using Robust.Shared.Map;
using Robust.Shared.Physics.Systems;

namespace Content.KZlevels.Shared.Systems;

/// <summary>
///     This handles breaking your legs.
///     The general process for gravity follows:<br/>
///     - Starting from the level below the current, search for a gravity source.<br/>
///     -   - First, check each lower map for gravity. If any of them have gravity, take the one furthest from the player as the source.<br/>
///     -   - Otherwise, check for a grid in the fall location on each map, if one is present, check it for gravity.<br/>
///     - If no gravity source was found, the object does not fall (return).<br/>
///     - Otherwise, construct and fire ZLevelDroppingEvent.<br/>
///     - If the event is marked handled, return.<br/>
///     - Move the object to the fall location.<br/>
///     - Construct and fire ZLevelDroppedEvent.<br/>
/// </summary>
/// <remarks>
///     This does not handle making grids fall.
/// </remarks>
public sealed class ZPhysicsSystem : EntitySystem
{
    [Dependency] private readonly SharedZStackSystem _zStack = default!;
    [Dependency] private readonly SharedTransformSystem _xform = default!;
    [Dependency] private readonly SharedPhysicsSystem _physics = default!;
    [Dependency] private readonly IMapManager _mapManager = default!;
    [Dependency] private readonly IConfigurationManager _cfg = default!;

    /// <inheritdoc/>
    public override void Initialize()
    {
        SubscribeLocalEvent((Entity<KZPhysicsComponent> ent, ref MoveEvent args) => OnPossiblyFalling(ent, ref args));
        if (_cfg.GetCVar(KZLevelsCVars.ProcessAllPhysicsObjects))
            _xform.OnGlobalMoveEvent += XformOnOnGlobalMoveEvent;
    }

    public override void Shutdown()
    {
        if (_cfg.GetCVar(KZLevelsCVars.ProcessAllPhysicsObjects))
            _xform.OnGlobalMoveEvent -= XformOnOnGlobalMoveEvent;
    }

    private void XformOnOnGlobalMoveEvent(ref MoveEvent ev)
    {
        OnPossiblyFalling(ev.Entity, ref ev);
    }

    private void OnPossiblyFalling(EntityUid ent, ref MoveEvent args)
    {
        if (!_zStack.TryGetZStack(ent, out var zStack))
            return; // Not in a Z level containing space.

        if (args.Entity.Comp1.ParentUid != args.Entity.Comp1.MapUid || args.Entity.Comp1.MapUid is null)
            return; // No falling through grids, no nullspace.

        var coords = _xform.GetWorldPosition(args.Entity.Comp1);

        var maps = zStack.Value.Comp.Maps;
        var mapIdx = maps.IndexOf(args.Entity.Comp1.MapUid.Value);
        if (mapIdx <= 0)
            return; // Bottommost map can't be fallen through.

        var ev = new IsGravityAffectedEvent(ent, false);
        RaiseLocalEvent(ent, ref ev, broadcast: true);
        if (!ev.Affected)
            return; // We're not affected by gravity to begin with, so no falling.

        EntityUid? gravitySource = null;
        var distance = 1;
        var levels = new List<EntityUid>(); // TODO: Remove allocation.

        for (var i = mapIdx - 1; i >= 0; i--)
        {
            levels.Add(maps[i]);
            var gravityEv = new IsGravitySource(maps[i], false);
            RaiseLocalEvent(maps[i], ref gravityEv, true);
            if (gravityEv.Handled)
            {
                gravitySource = maps[i];
                break;
            }

            // Try for grid.
            if (_mapManager.TryFindGridAt(maps[i], coords, out var gridId, out _))
            {
                gravityEv = new IsGravitySource(gridId, false);
                RaiseLocalEvent(gridId, ref gravityEv, broadcast: true);
                if (gravityEv.Handled)
                    gravitySource = gridId;
                // Well.. we can't fall through grids.
                // So break regardless of if we found gravity.
                break;
            }

            distance++;
        }

        if (gravitySource is null)
            return; // Well, nothing to pull us down.

        var fallingEv = new ZLevelDroppingEvent(ent, distance, levels, gravitySource.Value, false);
        RaiseLocalEvent(ent, ref fallingEv, broadcast: true);

        if (fallingEv.Handled) // Someone else did the obliteration, probably.
            return;

        var gSourceXform = Transform(gravitySource.Value);
        var fallTarget = gSourceXform.MapUid ?? gravitySource.Value;

        // splat.
        _xform.SetCoordinates(ent, new EntityCoordinates(fallTarget, coords));

        var fellEv = new ZLevelDroppedEvent(ent, distance, levels, gravitySource.Value, false);
        RaiseLocalEvent(ent, ref fellEv, broadcast: true);
    }
}

[ByRefEvent]
public record struct IsGravityAffectedEvent(EntityUid Target, bool Affected)
{
    public void Set() => Affected = true;
}

[ByRefEvent]
public record struct IsGravitySource(EntityUid Target, bool Handled)
{
    public void Handle() => Handled = true;
}

/// <summary>
///     Indicates an entity fell between Z levels.
/// </summary>
/// <param name="Distance">An approximation (based on level count) of drop distance.</param>
/// <param name="Levels">The levels dropped through, in order.</param>
/// <param name="GravitySource">The source of the gravitational pull (a grid or map.)</param>
/// <remarks>
///     If your Z levels are not strictly the same height, you'll want to calculate distance yourself.
///     It is strongly encouraged to play a cartoon splat effect if they fall far enough.
/// </remarks>
[ByRefEvent]
public record struct ZLevelDroppingEvent(EntityUid Target, int Distance, IReadOnlyList<EntityUid> Levels, EntityUid GravitySource, bool Handled);


/// <summary>
///     Indicates an entity fell between Z levels.
/// </summary>
/// <param name="Distance">An approximation (based on level count) of drop distance.</param>
/// <param name="Levels">The levels dropped through, in order.</param>
/// <param name="GravitySource">The source of the gravitational pull (a grid or map.)</param>
/// <remarks>
///     If your Z levels are not strictly the same height, you'll want to calculate distance yourself.
///     It is strongly encouraged to play a cartoon splat effect if they fall far enough.
/// </remarks>
[ByRefEvent]
public record struct ZLevelDroppedEvent(EntityUid Target, int Distance, IReadOnlyList<EntityUid> Levels, EntityUid GravitySource, bool Handled);
