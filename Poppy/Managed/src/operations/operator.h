//operator.h

#ifndef OPERATOR_H
#define OPERATOR_H


namespace operations {

	using NPCComponents = Components<
		ConnectionHeartbeat,
		ded::flora::Tree,
		NPCInfo,
		PlayerInfo,
		DirectorInfo,
		ActorInfo,
		PlotInfo,
		PlotMap,
		EntitySpawning,
		EntityAcl,
		Position,
		Metadata,
		CurrentState,
		Persistence
	>;

	class Operator : public ThreadBuilder, public EntityHandling {
	public:
		Operator( ) : ThreadBuilder( "Operator" ) { };

	private:
		Dispatcher dispatcher{ NPCComponents{} };

		typedef std::shared_ptr <Entities> EntitiesPtr;
		EntitiesPtr pEntities = std::make_shared<Entities>( );

		void process( CoreUtility& utility ) {
			dispatcher.Process( utility.GetOpList( ) );
		}

		void start( CoreUtility& utility ) {
			RegisterConnectionCallbacks( utility );
			RegisterCriticalSectionCallbacks( utility );
			RegisterMetricsCallbacks( utility, pEntities );
			RegisterCommandCallbacks<PlotMap::Commands::PlotCommand>( utility );
			RegisterCommandCallbacks<EntitySpawning::Commands::EntitySpawningCommand>( utility );
			RegisterFlagCallbacks( utility );
			RegisterMessageCallbacks( utility );
			RegisterQueryCallbacks( utility );
			RegisterEntityHandlingCallbacks( utility, dispatcher, pEntities );
			RegisterEntityIdCallbacks( utility, dispatcher, pEntities );

			ForEachComponent( NPCComponents{}, TrackComponentHandler{ pEntities, utility, dispatcher } );
			SendLogInfo( "All Callbacks registered", utility );
		}
		const double kMaximumQueueSize = 200;  // An arbitrary maximum value.

		struct TrackComponentHandler {
			shared_ptr <Entities>& pEntities_ref;
			CoreUtility& utility_ref;
			Dispatcher& dispatcher_ref;

			template <typename T>
			void Accept( ) const {
				Dispatcher& dispatcher = this->dispatcher_ref;
				CoreUtility& utility = this->utility_ref;
				shared_ptr <Entities>& pEntities = this->pEntities_ref;

				dispatcher.OnAddComponent<T>( [&] ( const AddComponentOp<T>& op ) {
					pEntities->AddComponent( op, utility );
				} );

				dispatcher.OnRemoveComponent<T>( [&] ( const RemoveComponentOp& op ) {
					pEntities->RemoveComponent<T>( op, utility );
				} );

				dispatcher.OnComponentUpdate<T>( [&] ( const ComponentUpdateOp<T>& op ) {
					pEntities->UpdateComponent( op, utility );
				} );

				dispatcher.OnAuthorityChange<T>( [&] ( const AuthorityChangeOp& op ) {
					switch ( op.Authority ) {					
					case Authority::kAuthoritative:
						pEntities->StartEntityProcess<T>( op.EntityId, utility );
						break;
					case Authority::kNotAuthoritative:
						pEntities->RemoveEntityProcess<T>( op.EntityId, utility );
						break;
					case Authority::kAuthorityLossImminent:
						pEntities->StopEntityProcess<T>( op.EntityId, utility );
						break;
					}
				} );
			}
		};

		template <typename T>
		void RegisterCommandCallbacks( CoreUtility& utility ) {
			//OnCommandRequest<T>	CommandRequestOp<T>(RequestId<IncomingCommandRequest<T>> RequestId,
			//EntityId EntityId, uint32_t TimeoutMillis, string CallerWorkerId,
			//List<string> CallerAttributeSet, T::Request Request)
			//the worker has received a command request for a component on an entity over which it has authority.
			dispatcher.OnCommandRequest<T>( [&] (
				const CommandRequestOp<T>& op ) {
				pEntities->OnCommandRequest<T>( op, utility );
			} );

			//OnCommandResponse<T>	CommandResponseOp<T>(RequestId<OutgoingCommandRequest<T>> RequestId,
			//EntityId EntityId, StatusCode StatusCode, Option<T::Response> Response)
			//the worker has received a command response for a request it issued previously.
			dispatcher.OnCommandResponse<T>( [&] (
				const CommandResponseOp<T>& op ) {
				string msg = "The command request with request ID ( " + to_string( op.RequestId.Id ) + " )";
				switch ( op.StatusCode ) {
				case StatusCode::kSuccess:
					msg += " Is Successfull! ";
					pEntities->OnCommandResponse<T>( op, utility );
					utility.SendLogMessage( "Operator", msg, op.EntityId );
					return;
				case StatusCode::kTimeout:
					pEntities->OnCommandTimeout<T>( op, utility );
					return;
				case StatusCode::kAuthorityLost:
					pEntities->OnCommandAuthorityLost<T>( op, utility );
					return;
				case StatusCode::kNotFound:
					msg += " not found.";
					break;
				case StatusCode::kApplicationError:
					msg += " has application error.";
					break;
				case StatusCode::kPermissionDenied:
					msg += " This worker does not heave permission to send this command request";
					break;
				case StatusCode::kInternalError:
					msg += " I received an internal error, which is usually a Spatial OS error! ";
					msg += "Please advice Improbable off the details.";
					break;
				default:
					msg += " I received an unmapped status code, ";
					msg += "don't know what to do! ";
					msg += "Please advice";
					break;
				}
				utility.SendLogMessage( LogLevel::kError, "Operator", msg, op.EntityId );
			} );
		}

		//System handling
		void RegisterConnectionCallbacks( CoreUtility& utility ) {
			dispatcher.OnDisconnect( [&] ( const DisconnectOp& op ) {
				utility.Disconnected( op );
			} );
		}

		void RegisterCriticalSectionCallbacks(
			CoreUtility& utility ) {
			//OnCriticalSection	CriticalSectionOp(bool InCriticalSection)	
			//a critical section is about to be entered or has just been left.
			dispatcher.OnCriticalSection( [&] ( const CriticalSectionOp& op ) {
				//string msg = "In Critical session: " + to_string(op.InCriticalSection);
				//utility.SendLogMessage("Director", msg);
			} );
		}

		Metrics CollectMetrics( shared_ptr <Entities>& pEntities ) {
			Metrics metrics;
			metrics.Load = CurrentTimeLoad( );
			Map<string, double> gougeMetrics = metrics.GaugeMetrics;
			pEntities->GougeMetrics( gougeMetrics );
			metrics.GaugeMetrics = gougeMetrics;
			return metrics;
		}

		void RegisterMetricsCallbacks( CoreUtility& utility, shared_ptr <Entities>& pEntities ) {
			dispatcher.OnMetrics( [&] ( const MetricsOp& op ) {
				Metrics metrics = CollectMetrics( pEntities );
				// Fill in the SDK built-in metrics.
				metrics.Merge( op.Metrics );
				utility.SendMetrics( metrics );
			} );
		}

		void RegisterFlagCallbacks( CoreUtility& utility ) {
			//a worker flag has been created, deleted or when its value has changed.
			// (string Name, Option<string> Value)
			dispatcher.OnFlagUpdate( [&] ( const FlagUpdateOp& op ) {
				string msg = "Received unhandled Flag change Callback (" + op.Name + ")";
				SendLogInfo( msg, utility );
			} );
		}

		void RegisterMessageCallbacks( CoreUtility& utility ) {
			// Print messages received from SpatialOS
			dispatcher.OnLogMessage( [&] ( const LogMessageOp& op ) {
				if ( op.Level == LogLevel::kFatal ) {
					cerr << "SDK Log Message, Fatal error: " << op.Message << endl;
					terminate( );
				}

				string msg = "Received a SDK Message Callback (" + op.Message + ")";
				SendLogInfo( op.Level, msg, utility );
			} );
		}

		void RegisterQueryCallbacks( CoreUtility& utility ) {
			//OnEntityQueryResponse	EntityQueryResponseOp(RequestId<EntityQueryRequest> RequestId,
			//StatusCode StatusCode, string Message, size_t ResultCount, Map<EntityId,
			//Entity> Result>)	the worker has received a response for an entity query it had requested previously.
			dispatcher.OnEntityQueryResponse( [&] ( const EntityQueryResponseOp& op ) {
				string msg = "Received a Entity Query Response Callback, (" + op.Message + ")";
				SendLogInfo( msg, utility );
			} );
		}

	};
}
#endif // !OPERATOR_H
