/*
 * This Source Code Form is subject to the terms of the Mozilla Public
 * License, v. 2.0. If a copy of the MPL was not distributed with this
 * file, You can obtain one at https://mozilla.org/MPL/2.0/.
 * This Source Code Form is "Incompatible With Secondary Licenses", as
 * defined by the Mozilla Public License, v. 2.0.
 */

using Robust.Shared.Configuration;

namespace Content.KZlevels.Shared;

[CVarDefs]
public static class KZLevelsCVars
{
    public static readonly CVarDef<bool> ProcessAllPhysicsObjects = CVarDef.Create("kzlevels.process_all_physics_objects",
        true,
        CVar.SERVER | CVar.REPLICATED,
        "Whether or not KZLevels should process all object movement instead of only KZPhysicsComponent marked objects. Only settable during early startup, midgame changes have no effect.");
}
