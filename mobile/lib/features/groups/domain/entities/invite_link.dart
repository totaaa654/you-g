import 'package:freezed_annotation/freezed_annotation.dart';

part 'invite_link.freezed.dart';

@freezed
abstract class InviteLink with _$InviteLink {
  const factory InviteLink({required String code, required DateTime expiresAt}) = _InviteLink;
}
