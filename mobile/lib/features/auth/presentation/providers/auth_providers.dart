import 'package:flutter_riverpod/flutter_riverpod.dart';

import '../../../../core/network/network_providers.dart';
import '../../data/datasources/auth_remote_data_source.dart';
import '../../data/datasources/cached_user_storage.dart';
import '../../data/repositories/auth_repository_impl.dart';
import '../../domain/repositories/auth_repository.dart';

final authRemoteDataSourceProvider =
    Provider<AuthRemoteDataSource>((ref) => AuthRemoteDataSource(ref.watch(dioProvider)));

final cachedUserStorageProvider =
    Provider<CachedUserStorage>((ref) => CachedUserStorage(ref.watch(secureStorageProvider)));

final authRepositoryProvider = Provider<AuthRepository>(
  (ref) => AuthRepositoryImpl(
    ref.watch(authRemoteDataSourceProvider),
    ref.watch(tokenStorageProvider),
    ref.watch(cachedUserStorageProvider),
  ),
);
