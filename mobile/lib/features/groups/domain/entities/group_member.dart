import 'package:freezed_annotation/freezed_annotation.dart';

import 'group_role.dart';

part 'group_member.freezed.dart';

@freezed
abstract class GroupMember with _$GroupMember {
  const factory GroupMember({
    required String userId,
    required String username,
    required String displayName,
    String? profilePictureUrl,
    required GroupRole role,
    required DateTime joinedAt,
  }) = _GroupMember;
}
