// DIRECTOR.h
#ifndef DIRECTOR_H
#define DIRECTOR_H

namespace operations {
	namespace entities {
		namespace DirectorWorker {

			class Director : public EntityGameObject {
				mutable shared_mutex process_mutex;

				void process( CoreUtility& utility ) {
					unique_lock<shared_mutex> lock( process_mutex );

					switch ( GetCurrentState( utility ) ) {
					case State::SEND_IS_CREATED:
						if ( SendIsCreated( utility ) )
							UpdateState( State::WAIT_FOR_ACK, utility );
						break;
					case State::CREATE_ACTORS: CreateActors( utility ); break;
					case State::CREATE_PLOT: CreatePlot( utility ); break;
					case State::DRAMA: InitDrama( utility ); break;
					case State::DIRECT: ActiveDramaCycle( utility ); break;
					default:
						break;
					}
				}

				void CreateActors( CoreUtility& utility ) {
					switch ( GetCurrentAction( utility ) ) {
					case State::CREATE_ACTORS:
						if ( AddActors( utility ) )
							UpdateAction( State::CHECK_ACTORS_CREATED, utility );
						break;
					case State::CHECK_ACTORS_CREATED:
						if ( IsEntitySpawningFailing( utility ) ) {
							UpdateAction( State::CREATE_ACTORS, utility );
							break;
						}
						if ( AreActorsReady( utility ) )
							UpdateState( State::CREATE_PLOT, State::CREATE_PLOT, utility );
						break;
					default:
						break;
					}
				}

				void CreatePlot( CoreUtility& utility ) {
					switch ( GetCurrentAction( utility ) ) {
					case State::CREATE_PLOT:
						if ( BuildPlot( utility ) )
							UpdateState( State::DIRECT, utility );
						break;
					default:
						break;
					}
				}

				bool AddActors( CoreUtility& utility ) {
					srand( GetRandomSeed( ) );
					list<EntityManager> entities;
					Option<Position::Data&> opt = GetPosition( );

					if ( opt.empty( ) ) {
						SendLogInfo( LogLevel::kError, "Missing position", utility );
						return false;
					}

					for ( int i = 0; i < dpge::Villagers; i++ ) {
						EntityBuilder builder( "NPC " + to_string( i ) + "_" + to_string( GetEntityId( ) ),
							positionNearby( opt.data( )->coords( ), utility ) );
						entities.push_back( builder.BuildNPCEntity( GetMap( ), GetEntityId( ) ) );
					}

					if ( entities.size( ) == 0 || entities.empty( ) ) return false;
					ConsoleLogErr( "Spawning NPC, Entities - Array size: " + to_string( entities.size( ) ) );

					return SpawnEntities( entities, EntityType::NPC, utility );
				}

				bool AreActorsReady( CoreUtility& utility ) {
					Option<DirectorInfo::Data&> info = GetComponentData<DirectorInfo>( );
					if ( info.empty( ) ) {
						SendLogInfo( LogLevel::kError, "Missing DirectorInfo", utility );
						return false;
					}

					return ( info.data( )->npcs( ).size( ) >= dpge::Villagers );
				}

				Coordinates positionNearby( const Coordinates& pos, CoreUtility& utility ) {
					Coordinates coords( pos.x( ) + Rand<double>( 50 ), 0.0,
						pos.z( ) + Rand<double>( 30 ) );
					return coords;
				}

				bool BuildPlot( CoreUtility& utility ) {
					ConsoleLogErr( "Building plot ..." );
					dpge::MurderPlot plot;
					plot.GeneratePlot( );
					if ( !plot.IsPlot( ) ) {
						SendLogInfo( "Building plot failed ...", utility );
						return false;
					}

					ConsoleLogErr( "Plot built" );
					Option<DirectorInfo::Data&> info = GetComponentData<DirectorInfo>( );
					if ( info.empty( ) ) {
						SendLogInfo( LogLevel::kError, "Missing DirectorInfo", utility );
						return false;
					}
					EntityId playerId = info.data( )->player_id( );
					List<EntityId> npcs = info.data( )->npcs( );

					ConsoleLogErr( "Found NPCs, Array size: " + to_string( npcs.size( ) ) );
					Map<EntityId, PlotEnum> actors;
					actors[npcs[0]] = PlotEnum::MURDERER;
					actors[npcs[1]] = PlotEnum::VICTIM;
					for ( int i = 2; i <= dpge::NumberOfInstances[dpge::PlotEnum::SUSPECT] + 1; i++ ) {
						actors[npcs[i]] = PlotEnum::SUSPECT;
					}

					Option<Position::Data&> opt = GetPosition( );

					if ( opt.empty( ) ) {
						SendLogInfo( LogLevel::kError, "Missing position", utility );
						return false;
					}
					list<EntityManager> entities;
					EntityBuilder builder( "Plot", positionNearby( opt.data( )->coords( ), utility ) );
					entities.push_back( builder.BuildPlotEntity( GetEntityId( ),
						playerId, plot.plotView( ), actors ) );

					if ( entities.size( ) == 0 || entities.empty( ) ) return false;
					ConsoleLogErr( "Spawning plot, Entities - Array size: " + to_string( entities.size( ) ) );

					return SpawnEntities( entities, EntityType::PLOT, utility );
				}

				Map<EntityId, PlotEnum> getSimpleNPCMap( Map<EntityId, PlotEnum> npc_ids ) {
					Map<EntityId, PlotEnum> npc_map;
					for ( Map<EntityId, PlotEnum>::iterator it = npc_ids.begin( );
						it != npc_ids.end( ); ++it ) {
						npc_map[it->first] = PlotEnum::SUSPECT;
					}
					return npc_map;
				}

				bool IsUnassignedPlot( CoreUtility& utility ) {
					Option<PlotMap::Data&> info = GetComponentData<PlotMap>( );
					if ( info.empty( ) ) {
						SendLogInfo( LogLevel::kError, "Missing PlotMap", utility );
						return false;
					}
					if ( info.data( )->plot_map( ).size( ) == 0 || info.data( )->plot_map( ).empty( ) )
						return false;
					return true;
				}

				bool AssignPlotToNPCs( CoreUtility& utility ) {
					Option<PlotMap::Data&> info = GetComponentData<PlotMap>( );
					if ( info.empty( ) ) {
						SendLogInfo( LogLevel::kError, "Missing PlotMap", utility );
						return false;
					}

					dpge::Plot plot;
					EntityId plotId;
					Map<EntityId, dpge::Plot> plot_map = info.data( )->plot_map( );
					if ( plot_map.size( ) == 0 || plot_map.empty( ) ) return false;

					for ( Map<EntityId, dpge::Plot>::iterator plotIt = plot_map.begin( );
						plotIt != plot_map.end( ); ++plotIt ) {
						plotId = plotIt->first;
						plot = plotIt->second;
						break;
					}

					ConsoleLogErr( "found a plot map" );
					ConsoleLogErr( "plot EntityId " + to_string( plotId ) );

					Map<EntityId, PlotEnum> npc_map = getSimpleNPCMap( plot.npc_ids( ) );

					for ( Map<EntityId, PlotEnum>::iterator npcIt = plot.npc_ids( ).begin( );
						npcIt != plot.npc_ids( ).end( ); ++npcIt ) {
						list<PlotEnum> submodelIds = dpge::RoleSubnets[npcIt->second];
						Map<PlotEnum, Submodels> submodels;
						for ( list<PlotEnum>::iterator subIt = submodelIds.begin( );
							subIt != submodelIds.end( ); ++subIt ) {
							submodels[*subIt] = plot.submodels( )[*subIt];
						}

						SendCommandRequest<PlotMap::Commands::PlotCommand>( npcIt->first,
							{ plotId, { plot.director_id( ), plot.player_id( ),
							submodels, npc_map, npcIt->second } }, utility );
						ConsoleLogErr( "NPCsView plotMap Size: " +
							to_string( submodels.size( ) ) +
							"Sent to NPC ID: " + to_string( npcIt->first ) );
					}

					list<PlotEnum> submodelIds = dpge::RoleSubnets[PlotEnum::SUSPECT];

					Map<PlotEnum, Submodels> submodels;
					for ( list<PlotEnum>::iterator subIt = submodelIds.begin( );
						subIt != submodelIds.end( ); ++subIt ) {
						submodels[*subIt] = plot.submodels( )[*subIt];
					}

					SendCommandRequest<PlotMap::Commands::PlotCommand>( plot.player_id( ),
						{ plotId, { plot.director_id( ), plot.player_id( ),
						submodels, npc_map, PlotEnum::DETECTIVE } }, utility );
					ConsoleLogErr( "PlayersView plotMap Size: " + to_string( submodels.size( ) ) );
					return true;
				}

				Map<Info, uint32_t> GetMap( ) {
					Map<Info, uint32_t> infoMap;
					infoMap[ded::Info::FIRST_NAME] = Rand<uint32_t>( 200 );
					infoMap[ded::Info::SURNAME] = Rand<uint32_t>( 50 );
					infoMap[ded::Info::GENDER] = Rand<uint32_t>( 2 );
					infoMap[ded::Info::OCCUPATION] = Rand<uint32_t>( 8 );
					return infoMap;
				}

				void InitDrama( CoreUtility& utility ) { }
				void ActiveDramaCycle( CoreUtility& utility ) {
					if ( IsUnassignedPlot( utility ) ) {
						AssignPlotToNPCs( utility );
					}
					if ( IsEntitySpawningFailing( utility ) ) {
						BuildPlot( utility );
					}
				}

				virtual Option<EntityId> GetSpawnerEntityId( CoreUtility& utility ) {
					Option<DirectorInfo::Data&> info = GetComponentData<DirectorInfo>( );
					if ( info.empty( ) ) {
						SendLogInfo( LogLevel::kError, "Missing DirectorInfo", utility );
						return Option<EntityId>( );
					}

					return Option<EntityId>( info.data( )->player_id( ) );
				}

				virtual ded::EntityType GetEntityType( ) { return ded::EntityType::DIRECTOR; }

			public:
				Director( const string& _loggerName, const EntityId& _entityId,
					EntityPtr _pEntity, shared_ptr <EntityCreateMap> _pEntCreateMap ) :
					EntityGameObject( _loggerName, _entityId, _pEntity, _pEntCreateMap ) { }

				virtual void OnCreate( CoreUtility& utility ) { }

				virtual void OnCommandResponse(
					const CommandResponseOp<PlotMap::Commands::PlotCommand>& op,
					CoreUtility& utility ) {
					if ( op.Response.data( )->success( ) )
						ConsoleLogErr( "Director assigned plot to actor id: " +
							to_string( op.EntityId ) );
				}

				virtual void OnCommandRequest(
					const CommandRequestOp<EntitySpawning::Commands::EntitySpawningCommand>& op, CoreUtility& utility ) {
					switch ( op.Request.entity_type( ) ) {
					case EntityType::NPC:
						NPCEntityRequest( op, utility );
						break;
					case EntityType::PLOT:
						PlotEntityRequest( op, utility );
						break;
					default:
						break;
					}
				}

				void NPCEntityRequest( CommandRequestOp<EntitySpawning::Commands::EntitySpawningCommand> op, 
					CoreUtility & utility ) {
					SendCommandResponse<EntitySpawning::Commands::EntitySpawningCommand>( op, { true }, utility );
					EntityId entityId = op.Request.entity_id( );

					DirectorInfo::Update update = GetUpdate<DirectorInfo>( );
					DirectorInfo::Update directorUpdate = GetUpdate<DirectorInfo>( );
					List<EntityId> npcs = *directorUpdate.npcs( ).data( );

					List<EntityId>::iterator directorIt;
					directorIt = find( npcs.begin( ), npcs.end( ), entityId );
					if ( directorIt == npcs.end( ) ) {
						npcs.emplace_back( entityId );
						directorUpdate.set_npcs( npcs );
						SendUpdate<DirectorInfo>( directorUpdate, utility );
					}
				}

				virtual void PlotEntityRequest( CommandRequestOp<EntitySpawning::Commands::EntitySpawningCommand> op, 
					CoreUtility& utility ) {
					SendCommandResponse<EntitySpawning::Commands::EntitySpawningCommand>( op, { true }, utility );
				}

				virtual void OnCommandResponse(
					const CommandResponseOp<EntitySpawning::Commands::EntitySpawningCommand>& op, CoreUtility& utility ) {
					if ( op.Response.data( )->success( ) && GetCurrentState( utility ) == State::WAIT_FOR_ACK )
						UpdateState( State::CREATE_ACTORS, State::CREATE_ACTORS, utility );
				}

				virtual void OnCommandRequest(
					const CommandRequestOp<DirectorPlotMap::Commands::PlotCommand>& op, CoreUtility& utility ) {
					ConsoleLogErr( "OnCommandRequest (PlotMap::Commands::PlotCommand), Entity Id:" + 
						to_string( op.EntityId ) + ", Request Id:" + to_string( op.RequestId.Id ) );
					SendCommandResponse<DirectorPlotMap::Commands::PlotCommand>( op, { true }, utility );
					ConsoleLogErr( "receives plot: " + to_string( op.Request.plot_id( ) ) );
					DirectorPlotMap::Update update = GetUpdate<DirectorPlotMap>( );
					Map<EntityId, dpge::DirectorPlot> plot_map = *update.plot_map( ).data( );

					Map<EntityId, dpge::DirectorPlot>::iterator it = plot_map.find( op.Request.plot_id( ) );
					if ( it != plot_map.end( ) ) return; //alreaddy added 
					dpge::Plot plot = op.Request.plot( );
					Map< EntityId, uint32_t > last_assigned_time;
					Map< EntityId, ::dpge::PlotEnum > npc_ids;
					plot_map[op.Request.plot_id( )] = { plot, last_assigned_time, npc_ids };
					update.set_plot_map( plot_map );
					SendUpdate<DirectorPlotMap>( update, utility );
					ConsoleLogErr( "has added plot id: " + to_string( op.Request.plot_id( ) ) );
				};
			};
		}
	}
}

#endif // !DIRECTOR_H
