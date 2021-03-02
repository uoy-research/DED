//plot_worker.h

#ifndef PLOT_WORKER_H
#define PLOT_WORKER_H

using namespace std;
using namespace worker;

namespace operations {
	namespace entities {
		namespace PlotWorker {

			class PlotInstance : public EntityGameObject {
				mutable shared_mutex process_mutex;

				void process( CoreUtility& utility ) {
					unique_lock<shared_mutex> lock( process_mutex );
					switch ( GetCurrentState( utility ) ) {
					case State::SEND_IS_CREATED:
						if ( SendIsCreated( utility ) )
							UpdateState( State::WAIT_FOR_ACK, utility );
						break;
					case State::ASSIGN_PLOT:
						if ( SendPlot( utility ) )
							UpdateState( State::WAIT_FOR_ACK, utility );
						break;
					default:
						break;
					}
				}

				bool SendPlot( CoreUtility & utility ) {
					Option<PlotInfo::Data&> info = GetComponentData<PlotInfo>( );
					if ( info.empty( ) ) {
						SendLogInfo( LogLevel::kError, "Missing PlotInfo", utility );
						return false;
					}

					ConsoleLogErr( "Plot sending plot to director: " + to_string( info.data( )->plot( ).director_id( ) ) );

					SendCommandRequest<PlotMap::Commands::PlotCommand>( info.data( )->plot( ).director_id( ), { GetEntityId( ),  info.data( )->plot( ) }, utility );
					return true;
				}

			protected:
				virtual Option<EntityId> GetSpawnerEntityId( CoreUtility& utility ) {
					Option<PlotInfo::Data&> info = GetComponentData<PlotInfo>( );
					if ( info.empty( ) ) {
						SendLogInfo( LogLevel::kError, "Missing PlotInfo", utility );
						return Option<EntityId>( );
					}

					return Option<EntityId>( info.data( )->plot( ).director_id( ) );
				}

				virtual ded::EntityType GetEntityType( ) { return ded::EntityType::PLOT; }

			public:
				PlotInstance( const string& _loggerName, const EntityId& _entityId,
					EntityPtr _pEntity, shared_ptr <EntityCreateMap> _pEntCreateMap ) :
					EntityGameObject( _loggerName, _entityId, _pEntity, _pEntCreateMap ) { }

				virtual void OnCreate( CoreUtility& utility ) { }

				virtual void OnCommandResponse(
					const CommandResponseOp<PlotMap::Commands::PlotCommand>& op, CoreUtility& utility ) {
					ConsoleLogErr( "PlotMap OnCommandResponse from director: " + to_string( op.EntityId ) );

					if ( op.Response.data( )->success( ) ) UpdateState( State::DRAMA, utility );
				}

				virtual void OnCommandResponse(
					const CommandResponseOp<EntitySpawning::Commands::EntitySpawningCommand>& op, CoreUtility& utility ) {
					ConsoleLogErr( "EntitySpawning OnCommandResponse from director: " + to_string( op.EntityId ) );

					if ( op.Response.data( )->success( ) && GetCurrentState( utility ) != State::DRAMA )
						UpdateState( State::ASSIGN_PLOT, State::ASSIGN_PLOT, utility );
				}
			};
		}
	}
}


#endif // !PLOT_WORKER_H