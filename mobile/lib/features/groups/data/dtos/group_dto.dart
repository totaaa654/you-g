import 'package:freezed_annotation/freezed_annotation.dart';

part 'group_dto.freezed.dart';
part 'group_dto.g.dart';

/// Mirrors backend `GroupDto` exactly.
@freezed
abstract class GroupDto with _$GroupDto {
  const factory GroupDto({
    required String id,
    required String name,
    String? description,
    String? pictureUrl,
    required String createdByUserId,
    required int memberCount,
    required DateTime createdAt,
  }) = _GroupDto;

  factory GroupDto.fromJson(Map<String, dynamic> json) => _$GroupDtoFromJson(json);
}
