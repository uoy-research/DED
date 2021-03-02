//npcWorker.h

#ifndef NPC_WORKER_H
#define NPC_WORKER_H

namespace operations {
	namespace entities {
		namespace NPCWorker {

			class NPC : public ActorEntity {
				mutable shared_mutex process_mutex;

				void process( CoreUtility& utility ) {
					unique_lock<shared_mutex> lock( process_mutex );
					switch ( GetCurrentState( utility ) ) {
					case State::SEND_IS_CREATED:
						EstablishDirector( utility );
						break;
					case State::CREATE_INFO_MAP:
						if ( CreateMap( utility ) )
							UpdateState( State::ACT, utility );
						break;
					case State::BUILD_KNOWLEDGE_BASE: BuildKnowledgeBase( utility ); break;
					case State::ACT: Act( utility ); break;
					default:
						break;
					}
				}

				void EstablishDirector( CoreUtility & utility ) {
					switch ( GetCurrentAction( utility ) ) {
					case State::FIRST:
						if ( SendIsCreated( utility ) )
							UpdateAction( State::SECOND, utility );
						break;
					default:
						break; 
					}
				}

				void BuildKnowledgeBase( CoreUtility & utility ) {
					SendLogInfo( "Actor building knowledge base ", utility );
					UpdateState( State::ACT, utility );
				}

				void Act( CoreUtility & utility ) { }

			protected:
				virtual Option<EntityId> GetSpawnerEntityId( CoreUtility & utility ) {
					Option<ActorInfo::Data&> info = GetComponentData<ActorInfo>( );
					if ( info.empty( ) ) {
						SendLogInfo( LogLevel::kError, "Missing ActorInfo", utility );
						return Option<EntityId>( );
					}

					return Option<EntityId>( info.data( )->director_id( ) );
				}

				virtual ded::EntityType GetEntityType( ) { return ded::EntityType::NPC; }

			public:
				NPC( const string& _loggerName, const EntityId& _entityId,
					EntityPtr _pEntity, shared_ptr <EntityCreateMap> _pEntCreateMap ) :
					ActorEntity( _loggerName, _entityId, _pEntity, _pEntCreateMap ) { }

				virtual void OnCreate( CoreUtility& utility ) { }

				virtual void OnCommandResponse(
					const CommandResponseOp<EntitySpawning::Commands::EntitySpawningCommand>& op, CoreUtility& utility ) {
					if ( op.Response.data( )->success( ) ) UpdateState( State::CREATE_INFO_MAP, utility );
				}				
			};
		}
	}
}

#endif // !NPC_WORKER_H