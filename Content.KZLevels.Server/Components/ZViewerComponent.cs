/*
 * This Source Code Form is subject to the terms of the Mozilla Public
 * License, v. 2.0. If a copy of the MPL was not distributed with this
 * file, You can obtain one at https://mozilla.org/MPL/2.0/.
 */

using System.Collections.Generic;
using Robust.Shared.GameObjects;

namespace Content.KZlevels.Server.Components;

/// <summary>
/// This is used for tracking Z Level viewers, which handle ensuring a client can view everything above and below it.
/// </summary>
[RegisterComponent]
public sealed partial class ZViewerComponent : Component
{
    public HashSet<EntityUid>
}
