//thread_pool.h

#ifndef THREAD_POOL_H
#define THREAD_POOL_H

namespace operations {
	namespace entities {

		typedef pair <shared_ptr <EntityBase>, thread> EntityPair;
		typedef pair <EntityId, list<EntityManager>> EntityRequestPair;

		struct ActiveEntities {

			map <EntityId, EntityPair> entityPairs;

			mutable shared_mutex _mutex;
			void PrintError( string msg ) { cerr << "Entities" << " - " << msg << endl; }

			void Add( const EntityId& entityId, EntityPair _pair ) {
				entityPairs[entityId] = std::move( _pair );
			};

			void Remove( const EntityId& entityId ) {
				map <EntityId, EntityPair>::iterator it = entityPairs.find( entityId );
				if ( it != entityPairs.end( ) ) { entityPairs.erase( it ); }
			};

			void RemoveExpiredThread( const EntityId& entityId ) {
				entityPairs.erase( entityId );
			};

			Option<shared_ptr <EntityBase>> GetActiveEntity( const EntityId& entityId ) {
				map <EntityId, EntityPair>::iterator it = entityPairs.find( entityId );
				if ( it != entityPairs.end( ) ) return Option<shared_ptr <EntityBase>>( entityPairs[entityId].first );
				return Option<shared_ptr <EntityBase>>( );
			}

		public:

			template <typename T>
			void AddComponent( const AddComponentOp<T>& op, CoreUtility& utility ) {
				unique_lock<shared_mutex> lock( _mutex );
				Option<shared_ptr <EntityBase>> oPair = GetActiveEntity( op.EntityId );
				if ( !oPair.empty( ) ) {
					shared_ptr <EntityBase> ptr = *oPair.data( );
					ptr->OnAddComponent( op, utility );
					ptr->SetAuthority<T>( );
				}
			}

			template <typename T>
			void UpdateComponent( const ComponentUpdateOp<T>& op, CoreUtility& utility ) {
				unique_lock<shared_mutex> lock( _mutex );
				Option<shared_ptr <EntityBase>> oPair = GetActiveEntity( op.EntityId );
				if ( !oPair.empty( ) ) {
					shared_ptr <EntityBase> ptr = *oPair.data( );
					ptr->OnComponentUpdate( op, utility );
				}
			}

			template <typename T>
			void OnCommandRequest( const CommandRequestOp<T>& op, CoreUtility& utility ) {
				unique_lock<shared_mutex> lock( _mutex );
				Option<shared_ptr <EntityBase>> oPair = GetActiveEntity( op.EntityId );
				if ( !oPair.empty( ) ) {
					shared_ptr <EntityBase> ptr = *oPair.data( );
					ptr->OnCommandRequest( op, utility );
				}
			}

			template <typename T>
			void OnCommandResponse( const EntityId& entityId, const CommandResponseOp<T>& op, CoreUtility& utility ) {
				unique_lock<shared_mutex> lock( _mutex );
				Option<shared_ptr <EntityBase>> oPair = GetActiveEntity( entityId );
				if ( !oPair.empty( ) ) {
					shared_ptr <EntityBase> ptr = *oPair.data( );
					ptr->OnCommandResponse( op, utility );
				}
			}

			template <typename T>
			void OnCommandTimeout( const EntityId& entityId, const CommandResponseOp<T>& op, CoreUtility& utility ) {
				unique_lock<shared_mutex> lock( _mutex );
				Option<shared_ptr <EntityBase>> oPair = GetActiveEntity( entityId );
				if ( !oPair.empty( ) ) {
					shared_ptr <EntityBase> ptr = *oPair.data( );
					ptr->OnCommandTimeout( op, utility );
				}
			}

			template <typename T>
			void OnCommandAuthorityLost( const EntityId& entityId, const CommandResponseOp<T>& op, CoreUtility& utility ) {
				unique_lock<shared_mutex> lock( _mutex );
				Option<shared_ptr <EntityBase>> oPair = GetActiveEntity( entityId );
				if ( !oPair.empty( )  ) {
					shared_ptr <EntityBase> ptr = *oPair.data( );
					ptr->OnCommandAuthorityLost( op, utility );
				}
			}

			template <typename T>
			void RemoveComponent( const RemoveComponentOp& op, CoreUtility& utility ) {
				unique_lock<shared_mutex> lock( _mutex );
				Option<shared_ptr <EntityBase>> oPair = GetActiveEntity( op.EntityId );
				if ( !oPair.empty( ) ) {
					shared_ptr <EntityBase> ptr = *oPair.data( );
					ptr->RevokeAuthority<T>( );
					ptr->OnRemoveComponent<T>( op, utility );
				}
			}

			void ResendEntityIdsReservation( const EntityId& entityId,
				const ReserveEntityIdsResponseOp& op,
				CoreUtility& utility ) {
				unique_lock<shared_mutex> lock( _mutex );
				Option<shared_ptr <EntityBase>> oPair = GetActiveEntity( entityId );
				if ( !oPair.empty( ) ) {
					shared_ptr <EntityBase> ptr = *oPair.data( );
					ptr->ResendEntityIdsReservation( op, utility );
				}
			}

			void CreateEntity( const EntityId& entityId,
				const ReserveEntityIdsResponseOp& op,
				CoreUtility& utility ) {
				unique_lock<shared_mutex> lock( _mutex );
				Option<shared_ptr <EntityBase>> oPair = GetActiveEntity( entityId );
				if ( !oPair.empty( ) ) {
					shared_ptr <EntityBase> ptr = *oPair.data( );
					ptr->CreateEntity( op, utility );
				}
			}
			
			void StopThread( const EntityId& entityId ) {
				unique_lock<shared_mutex> lock( _mutex );
				Option<shared_ptr <EntityBase>> oPair = GetActiveEntity( entityId );
				if ( !oPair.empty( ) ) {
					shared_ptr <EntityBase> ptr = *oPair.data( );
					ptr->Stop( );
					while ( ptr->IsRunning( ) )
						this_thread::sleep_for( (kMainLoopFrequency/5) );
					RemoveExpiredThread( entityId );					
				}
			}

			void RemoveThread( const EntityId& entityId ) {
				unique_lock<shared_mutex> lock( _mutex );
				Option<shared_ptr <EntityBase>> oPair = GetActiveEntity( entityId );
				if ( !oPair.empty( ) ) {
					shared_ptr <EntityBase> ptr = *oPair.data( );
					if ( !ptr->IsRunning() )
						RemoveExpiredThread( entityId );
				} 
			}

			bool IsRunning( const EntityId& entityId ) {
				unique_lock<shared_mutex> lock( _mutex );
				Option<shared_ptr <EntityBase>> oPair = GetActiveEntity( entityId );
				if ( !oPair.empty( ) ) {
					shared_ptr <EntityBase> ptr = *oPair.data( );
					return ptr->IsRunning( );
				} 
				return false;
			}

			bool ShouldRun( const EntityId& entityId ) {
				unique_lock<shared_mutex> lock( _mutex );
				Option<shared_ptr <EntityBase>> oPair = GetActiveEntity( entityId );
				if ( !oPair.empty( ) ) {
					shared_ptr <EntityBase> ptr = *oPair.data( );
					return ptr->ShouldRun( );
				}
				return false;
			}

			size_t Size( ) {
				return entityPairs.size( );
			}

			template <typename T>
			void StartThread( const EntityId& entityId, CoreUtility& utility,
				shared_ptr<T> pGameObj ) {
				Remove( entityId );
				thread _thread( &T::run, &*pGameObj, ref( utility ) );
				_thread.detach( );
				Add( entityId, make_pair( pGameObj, std::move( _thread ) ) );
			}
		};

		typedef shared_ptr <ActiveEntities> ActiveEntPtr;

	}
}

#endif // !THREAD_POOL_H