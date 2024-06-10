﻿using System.Collections.Generic;
using Robust.Shared.Analyzers;
using Robust.Shared.GameObjects;
using Robust.Shared.Serialization.Manager.Attributes;

namespace Content.KZlevels.Shared.Components;

/// <summary>
///     This is used for tracking a "stack" of maps, to form a cube (with z levels)
/// </summary>
/// <remarks>
///     The system tries to ensure the tracker is always "in view" for any entity within a tracked map.
/// </remarks>
[RegisterComponent, AutoGenerateComponentState]
public sealed partial class ZStackTrackerComponent : Component
{
    [DataField, AutoNetworkedField]
    public List<EntityUid> Set;
}