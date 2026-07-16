import 'package:freezed_annotation/freezed_annotation.dart';

import '../../../friends/domain/entities/public_profile.dart';

part 'group_join_request.freezed.dart';

@freezed
abstract class GroupJoinRequest with _$GroupJoinRequest {
  const factory GroupJoinRequest({
    required String id,
    required PublicProfile profile,
    required DateTime createdAt,
  }) = _GroupJoinRequest;
}
