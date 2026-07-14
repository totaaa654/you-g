import 'package:freezed_annotation/freezed_annotation.dart';

import 'public_profile.dart';

part 'friend.freezed.dart';

@freezed
abstract class Friend with _$Friend {
  const factory Friend({
    required PublicProfile profile,
    required bool isFavorite,
    required DateTime friendedSince,
  }) = _Friend;
}
