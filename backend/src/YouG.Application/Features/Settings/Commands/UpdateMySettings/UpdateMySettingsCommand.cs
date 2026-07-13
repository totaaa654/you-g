using MediatR;
using YouG.Application.Features.Settings.Dtos;
using YouG.Domain.Enums;

namespace YouG.Application.Features.Settings.Commands.UpdateMySettings;

public record UpdateMySettingsCommand(
    ThemeMode ThemePreference,
    bool IsSearchable,
    bool NotifyOnFriendRequest,
    bool NotifyOnGroupInvite,
    bool NotifyOnEventReminder,
    bool NotifyOnScheduleUpdate) : IRequest<SettingsDto>;
