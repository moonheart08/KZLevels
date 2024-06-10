using Robust.Shared.GameObjects;
using Robust.Shared.Serialization.Manager.Attributes;

namespace Content.KZlevels.Shared.Components;

/// <summary>
/// This is used for tracking members of a stack of maps.
/// </summary>
[RegisterComponent]
public sealed partial class ZStackMemberComponent : Component
{
    [DataField]
    public EntityUid Tracker;
}
