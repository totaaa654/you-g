namespace YouG.API.Contracts.Friends;

public record SendFriendRequestRequest(Guid? AddresseeId, string? FriendCode);
