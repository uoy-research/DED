//ENTITIES.h

#ifndef ENTITIES_H
#define ENTITIES_H

namespace operations {

	namespace entities {

		using PlayerWorker::Player;
		using DirectorWorker::Director;
		using NPCWorker::NPC;
		using PlotWorker::PlotInstance;

		class Entities {
			mutable shared_mutex process_mutex;

			shared_ptr <EntitiesMap> pEntitiesMap = make_shared<EntitiesMap>( );
			// eMap(pEntitiesMap);
			shared_ptr <EntityCreateMap> pEntCreateMap = make_shared<EntityCreateMap>( pEntitiesMap );

			ActiveEntPtr pActiveEntities = make_shared<ActiveEntities>( );
			void PrintError( string msg ) { cerr << "Entities" << " - " << msg << endl; }

		public:
			Option<EntityPtr> GetEntity( const EntityId& entityId ) {
				return pEntitiesMap->GetEntity( entityId );
			}

			void AddEntity( const AddEntityOp& op ) {
				pEntitiesMap->AddEntity( op );
			}

			void AddEntity( const EntityId& entityId, EntityManager entity ) {
				pEntitiesMap->AddEntity( entityId, entity );
			}

			void RemoveEntity( const RemoveEntityOp& op ) {
				pEntitiesMap->RemoveEntity( op );
			}

			void RemoveEntity( const DeleteEntityResponseOp& op ) {
				pEntitiesMap->RemoveEntity( op );
			}

			template <typename T>
			void AddComponent( const AddComponentOp<T>& op, CoreUtility& utility ) {
				pEntitiesMap->AddComponent<T>( op );
				pActiveEntities->AddComponent<T>( op, utility );
			}

			template <typename T>
			void UpdateComponent( const ComponentUpdateOp<T>& op, CoreUtility& utility ) {
				pEntitiesMap->UpdateComponent<T>( op );
				pActiveEntities->UpdateComponent<T>( op, utility );
			}

			template <typename T>
			void RemoveComponent( const RemoveComponentOp& op, CoreUtility& utility ) {
				pEntitiesMap->RemoveComponent<T>( op );
				pActiveEntities->RemoveComponent<T>( op, utility );
			}

			template <typename T>
			void OnCommandResponse( const CommandResponseOp<T>& op, CoreUtility& utility ) {
				Option<EntityId> oEntityId = pEntCreateMap->GetCommandRequest( op.RequestId.Id );
				if ( !oEntityId.empty( ) ) 
					pActiveEntities->OnCommandResponse<T>( *oEntityId.data( ), op, utility );
				else PrintError( "Missing entity ID in OnCommandResponse" + to_string( op.RequestId.Id ));
			}

			template <typename T>
			void OnCommandTimeout( const CommandResponseOp<T>& op, CoreUtility& utility ) {
				Option<EntityId> oEntityId = pEntCreateMap->GetCommandRequest( op.RequestId.Id );
				if ( !oEntityId.empty( ) )
					pActiveEntities->OnCommandTimeout<T>( *oEntityId.data( ), op, utility );
				else PrintError( "Missing entity ID in OnCommandResponse" + to_string( op.RequestId.Id ) );
			}

			template <typename T>
			void OnCommandAuthorityLost( const CommandResponseOp<T>& op, CoreUtility& utility ) {
				Option<EntityId> oEntityId = pEntCreateMap->GetCommandRequest( op.RequestId.Id );
				if ( !oEntityId.empty( ) )
					pActiveEntities->OnCommandAuthorityLost<T>( *oEntityId.data( ), op, utility );
				else PrintError( "Missing entity ID in OnCommandResponse" + to_string( op.RequestId.Id ) );
			}

			template <typename T>
			void OnCommandRequest( const CommandRequestOp<T>& op, CoreUtility& utility ) {
				pActiveEntities->OnCommandRequest<T>( op, utility );
			}

			template <typename T>
			double GetMapSize( ) {
				return pEntitiesMap->GetMapSize<T>( );
			}

			double GetNumAllEntities( ) { return pEntitiesMap->GetNumAllEntities( ); }
			
			void CreateEntitySuccess( const CreateEntityResponseOp& op ) {
				pEntCreateMap->CreateEntitySuccess( op );
			}

			void CreateEntity( const ReserveEntityIdsResponseOp& op, CoreUtility& utility ) { 
				PrintError( "CreateEntity for request id " + to_string( op.RequestId.Id ) );
				Option<EntityId> optEntityId = pEntCreateMap->GetRequestEntityId( op.RequestId );
				if ( optEntityId.empty( ) ) return;
				PrintError( "CreateEntity found entity id " + to_string(*optEntityId.data( ))
					+ " for request id " + to_string( op.RequestId.Id ) );
				pActiveEntities->CreateEntity( *optEntityId.data( ), op, utility );				
			}

			void ResendCreateEntity( const CreateEntityResponseOp& op, CoreUtility& utility ) { 
				pEntCreateMap->ResendCreateEntity( op, utility );
			}

			void ResendEntityIdsReservation( const ReserveEntityIdsResponseOp& op,
				CoreUtility& utility ) {
				Option<EntityId> optEntityId = pEntCreateMap->GetRequestEntityId( op.RequestId );
				if ( optEntityId.empty( ) ) return;
				pActiveEntities->ResendEntityIdsReservation( *optEntityId.data( ), op, utility );
			}
			
			template <typename T>
			void StartThread( const string& name, const EntityId& entityId, CoreUtility& utility ) {
				Option<EntityPtr> entity = GetEntity( entityId );
				if ( entity.empty( ) ) return;

				EntityPtr pEntity = *entity.data( );
				shared_ptr<T> pGameObj = make_shared<T>( name, entityId, pEntity, pEntCreateMap );
				pActiveEntities->StartThread<T>( entityId, utility, pGameObj );
			}

			template <typename T>
			void StartEntityProcess( const EntityId& entityId, CoreUtility& utility ) { };

			template<>
			void StartEntityProcess<PlayerInfo>( const EntityId& entityId,
				CoreUtility& utility ) {
				PrintError( "StartEntityProcess, Player, Entity ID: " + to_string( entityId ) );
				StartThread<Player>( "Player", entityId, utility );
			}

			template<>
			void StartEntityProcess<DirectorInfo>( const EntityId& entityId,
				CoreUtility& utility ) {
				PrintError( "StartEntityProcess, Director, Entity ID: " + to_string( entityId ) );
				StartThread<Director>( "Director", entityId, utility );
			}

			template<>
			void StartEntityProcess<NPCInfo>( const EntityId& entityId,
				CoreUtility& utility ) {
				PrintError( "StartEntityProcess, NPC, Entity ID: " + to_string( entityId ) );
				StartThread<NPC>( "NPC", entityId, utility );
			}

			template<>
			void StartEntityProcess<PlotInfo>( const EntityId& entityId,
				CoreUtility& utility ) {
				PrintError( "StartEntityProcess, Plot, Entity ID: " + to_string( entityId ) );
				StartThread<PlotInstance>( "Plot", entityId, utility );
			}

			template <typename T>
			void StopEntityProcess( const EntityId& entityId, CoreUtility& utility ) { }

			template<>
			void StopEntityProcess<PlayerInfo>( const EntityId& entityId,
				CoreUtility& utility ) {
				PrintError( "StopEntityProcess Entity ID: " + to_string( entityId ) );
				pActiveEntities->StopThread( entityId );
			}

			template<>
			void StopEntityProcess<DirectorInfo>( const EntityId& entityId,
				CoreUtility& utility ) {
				PrintError( "StopEntityProcess Entity ID: " + to_string( entityId ) );
				pActiveEntities->StopThread( entityId );
			}

			template<>
			void StopEntityProcess<NPCInfo>( const EntityId& entityId,
				CoreUtility& utility ) {
				PrintError( "StopEntityProcess Entity ID: " + to_string( entityId ) );
				pActiveEntities->StopThread( entityId );
			}

			template<>
			void StopEntityProcess<PlotInfo>( const EntityId& entityId,
				CoreUtility& utility ) {
				PrintError( "StopEntityProcess Entity ID: " + to_string( entityId ) );
				pActiveEntities->StopThread( entityId );
			}

			template <typename T>
			void RemoveEntityProcess( const EntityId& entityId, CoreUtility& utility ) { }

			template<>
			void RemoveEntityProcess<PlayerInfo>( const EntityId& entityId,
				CoreUtility& utility ) {
				PrintError( "RemoveEntityProcess Entity ID: " + to_string( entityId ) );
				pActiveEntities->RemoveThread( entityId );
			}

			template<>
			void RemoveEntityProcess<DirectorInfo>( const EntityId& entityId,
				CoreUtility& utility ) {
				PrintError( "RemoveEntityProcess Entity ID: " + to_string( entityId ) );
				pActiveEntities->RemoveThread( entityId );
			}

			template<>
			void RemoveEntityProcess<NPCInfo>( const EntityId& entityId,
				CoreUtility& utility ) {
				PrintError( "RemoveEntityProcess Entity ID: " + to_string( entityId ) );
				pActiveEntities->RemoveThread( entityId );
			}

			template<>
			void RemoveEntityProcess<PlotInfo>( const EntityId& entityId,
				CoreUtility& utility ) {
				PrintError( "RemoveEntityProcess Entity ID: " + to_string( entityId ) );
				pActiveEntities->RemoveThread( entityId );
			}

			double GetNumActiveEntities( ) {
				return (double) pActiveEntities->Size( );
			}

			void GougeMetrics( Map<string, double>& gougeMetrics ) {
				gougeMetrics["Entities - Unclassified"] = GetMapSize<EntitiesMap>( );
				gougeMetrics["Entities - Tree entities"] = GetMapSize<Tree>( );
				gougeMetrics["Entities - Player entities"] = GetMapSize<PlayerInfo>( );
				gougeMetrics["Entities - Plot entities"] = GetMapSize<PlotInfo>( );
				gougeMetrics["Entities - Director entities"] = GetMapSize<DirectorInfo>( );
				gougeMetrics["Entities - NPC entities"] = GetMapSize<NPCInfo>( );
				gougeMetrics["Entities - All entities"] = GetNumAllEntities( );
				gougeMetrics["Entities - Active Entities"] = GetNumActiveEntities( );
			}
		};
	}
}


#endif // !ENTITIES_H