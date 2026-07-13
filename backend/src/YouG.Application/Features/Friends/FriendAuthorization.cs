using YouG.Application.Common.Exceptions;
using YouG.Domain.Entities;

namespace YouG.Application.Features.Friends;

/// <summary>
/// Shared checks for Friends handlers — matches the authorization matrix in
/// docs/04-API-DESIGN.md Section 4 (404 for callers not party to the relationship).
/// </summary>
internal static class FriendAuthorization
{
    /// <summary>Throws unless the caller is the requester or addressee of this row.</summary>
    public static void RequireParticipant(FriendRequest friendRequest, Guid userId)
    {
        if (friendRequest.RequesterId != userId && friendRequest.AddresseeId != userId)
        {
            throw new NotFoundException("Friend request not found.");
        }
    }
}
