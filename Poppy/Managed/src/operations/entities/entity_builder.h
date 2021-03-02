//ENTITY_BUILDER.h

#ifndef ENTITY_BUILDER_H
#define ENTITY_BUILDER_H


namespace operations {

	namespace entities {


		class EntityBuilder {
			EntityManager entity;

		public:

			EntityBuilder( const string& _label, Coordinates& coords ) {
				entity.Add<Position>( { coords } );
				entity.Add<Metadata>( { _label } );
			}

			void AddPersistence( ) {
				entity.Add<improbable::Persistence>( {} );
			}

			EntityManager BuildPlotEntity( const EntityId& playerId, const EntityId& directorId,
				const Map<PlotEnum, Submodels>& plotMap, Map<EntityId, PlotEnum> npcs ) {
				EntityTemplateFactory* pFactory = new EntityTemplateFactory( );
				entity.Add<CurrentState>( { State::SEND_IS_CREATED, State::SEND_IS_CREATED } );
				entity.Add<PlotInfo>( { { playerId, directorId, plotMap, npcs, PlotEnum::DIRECTOR } } );
				entity.Add<EntityAcl>( pFactory->PlotAcl( ) );
				AddPersistence( );
				return entity;
			}

			EntityManager BuildDirectorEntity( const EntityId& playerId ) {
				EntityTemplateFactory* pFactory = new EntityTemplateFactory( );
				Map<EntityId, dpge::Plot> plotMap;
				Map<EntityId, dpge::DirectorPlot> directorPlotMap;
				entity.Add<PlotMap>( { plotMap } );
				entity.Add<DirectorPlotMap>( { directorPlotMap } );
				entity.Add<CurrentState>( { State::SEND_IS_CREATED, State::SEND_IS_CREATED } );
				entity.Add<DirectorInfo>( { playerId, {} } );
				entity.Add<EntitySpawning>( { {}, {} } );
				entity.Add<EntityAcl>( pFactory->DirectorAcl( ) );
				AddPersistence( );
				return entity;
			}

			EntityManager BuildNPCEntity( const Map<Info, uint32_t>& infoMap,
				EntityId directorId ) {
				EntityTemplateFactory* pFactory = new EntityTemplateFactory( );
				Map<EntityId, dpge::Plot> plotMap;
				Map<EntityId, KnowledgeItem> knowledgeItems;
				entity.Add<PlotMap>( { plotMap } );
				entity.Add<CurrentState>( { State::SEND_IS_CREATED, State::FIRST } );
				entity.Add<ActorInfo>( { infoMap, directorId } );
				entity.Add<Knowledge>( { knowledgeItems } );
				entity.Add<EntitySpawning>( { { }, { } } );
				entity.Add<NPCInfo>( { } );
				entity.Add<EntityAcl>( pFactory->NPCAcl( ) );
				AddPersistence( );
				return entity;
			}

			template <typename T>
			void Add( const typename T::Data& data ) {
				entity.Add<T>( data );
			}
		};
	}
}


#endif // !ENTITY_BUILDER_H