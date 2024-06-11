/*
 * This Source Code Form is subject to the terms of the Mozilla Public
 * License, v. 2.0. If a copy of the MPL was not distributed with this
 * file, You can obtain one at https://mozilla.org/MPL/2.0/.
 * This Source Code Form is "Incompatible With Secondary Licenses", as
 * defined by the Mozilla Public License, v. 2.0.
 */

using System.Collections.Generic;
using Robust.Shared.Analyzers;
using Robust.Shared.GameObjects;
using Robust.Shared.GameStates;
using Robust.Shared.Serialization.Manager.Attributes;

namespace Content.KZlevels.Shared.Components;

/// <summary>
///     This is used for tracking a "stack" of maps, to form a cube (with z levels)
/// </summary>
/// <remarks>
///     The system tries to ensure the tracker is always "in view" for any entity within a tracked map.
/// </remarks>
[RegisterComponent, NetworkedComponent, AutoGenerateComponentState]
public sealed partial class ZStackTrackerComponent : Component
{
    [DataField, AutoNetworkedField]
    public List<EntityUid> Maps = new();
}
