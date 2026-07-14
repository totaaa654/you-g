import 'package:freezed_annotation/freezed_annotation.dart';

part 'group_member_dto.freezed.dart';
part 'group_member_dto.g.dart';

/// Mirrors backend `GroupMemberDto` exactly. `role` stays a raw wire string ("Member"/"Admin").
@freezed
abstract class GroupMemberDto with _$GroupMemberDto {
  const factory GroupMemberDto({
    required String userId,
    required String username,
    required String displayName,
    String? profilePictureUrl,
    required String role,
    required DateTime joinedAt,
  }) = _GroupMemberDto;

  factory GroupMemberDto.fromJson(Map<String, dynamic> json) => _$GroupMemberDtoFromJson(json);
}
