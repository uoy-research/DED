//ENTITY_TEMPLATE_FACTORY.h

#ifndef ENTITY_TEMPLATE_FACTORY_H
#define ENTITY_TEMPLATE_FACTORY_H

namespace operations {
	namespace entities {

		class EntityTemplateFactory {

			const WorkerAttributeSet physicsWorkerAttributeSet{
				List<string>{ "physics" } };
			const WorkerAttributeSet clientWorkerAttributeSet{
				List<string>{ "client" } };
			const WorkerAttributeSet playerWorkerAttributeSet{
				List<string>{ "player_ai" } };
			const WorkerAttributeSet directorWorkerAttributeSet{
				List<string>{ "director_ai" } };
			const WorkerAttributeSet plotWorkerAttributeSet{
				List<string>{ "plot_ai" } };
			const WorkerAttributeSet npcWorkerAttributeSet{
				List<string>{ "npc_ai" } };
			const WorkerAttributeSet defaultWorkerAttributeSet{
				List<string>{ "default_worker" } };

		public:


			EntityAclData DirectorAcl( ) {

				WorkerRequirementSet writeRequirementSet = makeDirectorRequirementSet( );

				WorkerRequirementSet readRequirementSet = makeAllRequirementSet( );

				Map<ComponentId, WorkerRequirementSet> writeAcl( BasicPersistentAcl( writeRequirementSet ) );
				writeAcl[DirectorInfo::ComponentId] = writeRequirementSet;
				writeAcl[EntitySpawning::ComponentId] = writeRequirementSet;
				writeAcl[PlotMap::ComponentId] = writeRequirementSet;
				writeAcl[CurrentState::ComponentId] = writeRequirementSet;

				return EntityAcl::Data{/* read */ makeAllRequirementSet( ), /* write */ writeAcl };
			}

			EntityAclData NPCAcl( ) {

				WorkerRequirementSet writeRequirementSet = makeNPCRequirementSet( );

				Map<ComponentId, WorkerRequirementSet> writeAcl( BasicPersistentAcl( writeRequirementSet ) );
				writeAcl[NPCInfo::ComponentId] = writeRequirementSet;
				writeAcl[ActorInfo::ComponentId] = writeRequirementSet;
				writeAcl[EntitySpawning::ComponentId] = writeRequirementSet;
				writeAcl[PlotMap::ComponentId] = writeRequirementSet;
				writeAcl[CurrentState::ComponentId] = writeRequirementSet;

				return EntityAcl::Data{/* read */ makeAllRequirementSet( ), /* write */ writeAcl };
			}

			EntityAclData PlotAcl( ) {

				WorkerRequirementSet writeRequirementSet = makePlotRequirementSet( );

				Map<ComponentId, WorkerRequirementSet> writeAcl( BasicPersistentAcl( writeRequirementSet ) );
				writeAcl[dpge::PlotInfo::ComponentId] = writeRequirementSet;
				
				return EntityAcl::Data{/* read */ makeAllRequirementSet( ), /* write */ writeAcl };
			}

		private:

			WorkerRequirementSet makeDirectorRequirementSet( ) {
				return WorkerRequirementSet{
					{ directorWorkerAttributeSet }
				};
			}

			WorkerRequirementSet makePlotRequirementSet( ) {
				return WorkerRequirementSet{
					{ plotWorkerAttributeSet }
				};
			}

			WorkerRequirementSet makeNPCRequirementSet( ) {
				return WorkerRequirementSet{
					{ npcWorkerAttributeSet }
				};
			}

			WorkerRequirementSet makeAllRequirementSet( ) {
				return WorkerRequirementSet{
					List<WorkerAttributeSet>{
						physicsWorkerAttributeSet,
						directorWorkerAttributeSet,
						defaultWorkerAttributeSet,
						playerWorkerAttributeSet,
						plotWorkerAttributeSet,
						npcWorkerAttributeSet
					}
				};
			}


			template <typename C>
			WorkerRequirementSet makeRequirementSet( const CommandRequestOp<C>& op ) {
				// This requirement set matches only the command caller, i.e. the worker that issued the command,
				// since attribute set includes the caller's unique attribute.
				auto callerWorkerAttributeSet = WorkerAttributeSet{ op.CallerAttributeSet };
				return WorkerRequirementSet{ List<WorkerAttributeSet>{ callerWorkerAttributeSet } };
			}

			Map<ComponentId, WorkerRequirementSet> BasicPersistentAcl( const WorkerRequirementSet& requirementSet ) {
				return { { Position::ComponentId, requirementSet },
						{ EntityAcl::ComponentId, requirementSet },
						{ Persistence::ComponentId, requirementSet },
						{ Metadata::ComponentId, requirementSet }
				};
			}
		};
	}
}


#endif // !ENTITY_TEMPLATE_FACTORY_H