using MediatR;

namespace YouG.Application.Features.Friends.Commands.SetFavorite;

public record SetFavoriteCommand(Guid FriendUserId, bool IsFavorite) : IRequest;
