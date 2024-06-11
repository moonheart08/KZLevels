/*
 * This Source Code Form is subject to the terms of the Mozilla Public
 * License, v. 2.0. If a copy of the MPL was not distributed with this
 * file, You can obtain one at https://mozilla.org/MPL/2.0/.
 * This Source Code Form is "Incompatible With Secondary Licenses", as
 * defined by the Mozilla Public License, v. 2.0.
 */


using Robust.Shared.GameObjects;
using Robust.Shared.Player;

namespace Content.KZlevels.Server.Components;

/// <summary>
/// This is used for loading the world for the given client.
/// </summary>
[RegisterComponent]
public sealed partial class ZLoaderComponent : Component
{
    public ICommonSession? Target = default!;
}
