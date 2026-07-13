using MediatR;
using YouG.Application.Features.Friends.Dtos;

namespace YouG.Application.Features.Friends.Commands.SendFriendRequest;

public record SendFriendRequestCommand(Guid? AddresseeId, string? FriendCode) : IRequest<FriendRequestDto>;
