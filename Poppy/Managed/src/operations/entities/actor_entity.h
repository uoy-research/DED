//actor_entity.h

#ifndef ACTOR_ENTITY_H
#define ACTOR_ENTITY_H

namespace operations {

	namespace entities {

		class ActorEntity : public EntityGameObject {

			bool IsMap( CoreUtility& utility ) {
				SendLogInfo( "Actor checks for Map<Info, uint32_t>", utility );
				Option<ActorInfo::Data&> oInfMap = GetComponentData<ActorInfo>( );
				SendLogInfo( "Retrieving Map<Info, uint32_t> is succesful", utility );
				Map<Info, uint32_t> infoMap = oInfMap.data( )->info_map( );
				SendLogInfo( "Map<Info, uint32_t> size: " + to_string( infoMap.size( ) ), utility );
				LogInfo( infoMap, utility );
				return ( !infoMap.empty( ) && infoMap.size( ) >= 4 );
			}

		protected:

			bool IsDirectorAllocated( ) {
				return GetComponentData<ActorInfo>( )->director_id( ) > 0;
			}

		public:
			ActorEntity( const string& _loggerName, const EntityId& _entityId,
				EntityPtr _pEntity, shared_ptr <EntityCreateMap> _pEntCreateMap ) :
				EntityGameObject( _loggerName, _entityId, _pEntity, _pEntCreateMap ) { }

			string FirstName( Map<Info, uint32_t>& infoMap ) {
				uint32_t id = infoMap[Info::FIRST_NAME];
				return infoMap[Info::GENDER] == (uint32_t) Gender::MALE ? dpge::MALE_NAMES[id] : dpge::FEMALE_NAMES[id];
			}

			string Surname( Map<Info, uint32_t>& infoMap ) {
				return dpge::SURNAMES[infoMap[Info::SURNAME]];
			}

			string Gender( Map<Info, uint32_t>& infoMap ) {
				return infoMap[Info::GENDER] == (uint32_t) Gender::MALE ? "Male" : "Female";
			}

			string Occupation( Map<Info, uint32_t>& infoMap ) {
				return dpge::OCCUPATION[infoMap[Info::OCCUPATION]];
			}

			void LogInfo( Map<Info, uint32_t> infoMap, CoreUtility& utility ) {
				if ( infoMap.size( ) < 4 ) return;
				SendLogInfo( "Actor is " + Gender( infoMap ) + " " + FirstName( infoMap ) + " "
					+ Surname( infoMap ) + ", occupation: " + Occupation( infoMap ), utility );
			}

			bool CreateMap( CoreUtility & utility ) {
				Option<ActorInfo::Data&> oInfMap = GetComponentData<ActorInfo>( );
				if ( oInfMap.empty( ) ) {
					SendLogInfo( "Map<Info, uint32_t> is empty", utility );
					return false;
				}

				if ( IsMap( utility ) ) return true;

				srand( GetRandomSeed( ) );
				SendLogInfo( "Actor creates info map RAND: " + to_string( Rand<uint32_t>( 200 ) ), utility );
				Map<Info, uint32_t> infoMap;
				infoMap[Info::FIRST_NAME] = Rand<uint32_t>( 200 );
				infoMap[Info::SURNAME] = Rand<uint32_t>( 50 );
				infoMap[Info::GENDER] = Rand<uint32_t>( 2 );
				infoMap[Info::OCCUPATION] = Rand<uint32_t>( 8 );

				ActorInfo::Update update = GetUpdate<ActorInfo>( );
				update.set_info_map( infoMap );
				SendUpdate<ActorInfo>( update, utility );
				LogInfo( infoMap, utility );
				return true;
			}
			
			virtual void OnCommandRequest(
				const CommandRequestOp<PlotMap::Commands::PlotCommand>& op, CoreUtility& utility ) {
				ConsoleLogErr( "OnCommandRequest (PlotMap::Commands::PlotCommand), Entity Id:" + to_string( op.EntityId ) + ", Request Id:" + to_string( op.RequestId.Id ) );
				SendCommandResponse<PlotMap::Commands::PlotCommand>( op, { true }, utility );
				ConsoleLogErr( "receives plot: " + to_string( op.Request.plot_id( ) ) );
				PlotMap::Update update = GetUpdate<PlotMap>( );
				Map<EntityId, dpge::Plot> plot_map = *update.plot_map( ).data( );

				Map<EntityId, dpge::Plot>::iterator it = plot_map.find( op.Request.plot_id( ) );
				if ( it != plot_map.end( ) ) return; //alreaddy added 

				plot_map[op.Request.plot_id( )] = op.Request.plot( );
				update.set_plot_map( plot_map );
				SendUpdate<PlotMap>( update, utility );
				ConsoleLogErr( "has added plot id: " + to_string( op.Request.plot_id( ) ) );
			};
		};
	}
}

#endif // !ACTOR_ENTITY_H