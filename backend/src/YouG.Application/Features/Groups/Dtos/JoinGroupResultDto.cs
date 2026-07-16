namespace YouG.Application.Features.Groups.Dtos;

/// <summary>
/// `Joined` is true and `Group` populated when the invite led to instant membership (admin-
/// created link, or the user was already a member). When the invite was created by a non-admin
/// member, `Joined` is false and a `GroupJoinRequest` was created/reused instead, pending admin approval.
/// </summary>
public record JoinGroupResultDto(bool Joined, GroupDto? Group);
