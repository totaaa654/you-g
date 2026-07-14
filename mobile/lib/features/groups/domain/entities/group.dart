import 'package:freezed_annotation/freezed_annotation.dart';

part 'group.freezed.dart';

@freezed
abstract class Group with _$Group {
  const factory Group({
    required String id,
    required String name,
    String? description,
    String? pictureUrl,
    required String createdByUserId,
    required int memberCount,
    required DateTime createdAt,
  }) = _Group;
}
