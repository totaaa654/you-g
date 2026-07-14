import 'package:freezed_annotation/freezed_annotation.dart';

part 'invite_link_dto.freezed.dart';
part 'invite_link_dto.g.dart';

/// Mirrors backend `InviteLinkDto` exactly.
@freezed
abstract class InviteLinkDto with _$InviteLinkDto {
  const factory InviteLinkDto({
    required String code,
    required DateTime expiresAt,
  }) = _InviteLinkDto;

  factory InviteLinkDto.fromJson(Map<String, dynamic> json) => _$InviteLinkDtoFromJson(json);
}
