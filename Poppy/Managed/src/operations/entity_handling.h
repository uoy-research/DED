//entity_handling.h

#ifndef ENTITY_HANDLING_H
#define ENTITY_HANDLING_H

namespace operations {

	using entities::Entities;
	using entities::EntityManager;

	class EntityHandling {

		const string loggerName = "operations/entity_handling.h";

		mutable shared_mutex creatEntity_mutex;
		mutable shared_mutex creatEntities_mutex;

		void PrintError( string msg ) { cerr << loggerName << " - " << msg << endl; }

	protected:
		void RegisterEntityHandlingCallbacks( CoreUtility& utility, Dispatcher& dispatcher,
			shared_ptr <Entities>& pEntities ) {
			//OnAddEntity	AddEntityOp(EntityId EntityId)	
			//an entity is added to the worker’s view of the simulation.
			dispatcher.OnAddEntity( [&] ( const AddEntityOp& op ) {
				pEntities->AddEntity( op );
			} );

			//OnRemoveEntity	RemoveEntityOp(EntityId EntityId)	
			//an entity is removed from the worker’s view of the simulation.

			dispatcher.OnRemoveEntity( [&] ( const RemoveEntityOp& op ) {
				pEntities->RemoveEntity( op );
			} );

			//OnCreateEntityResponse CreateEntityResponseOp(RequestId<CreateEntityRequest> RequestId, 
			//StatusCode StatusCode, string Message, Option<EntityId> EntityId)	
			//the worker has received a response for an entity creation it had requested previously.
			dispatcher.OnCreateEntityResponse( [&] ( const CreateEntityResponseOp& op ) {
				string msg = "Create Entity ID(" + to_string( *op.EntityId.data( ) ) + "), ";
				switch ( op.StatusCode ) {
				case StatusCode::kSuccess:
					pEntities->CreateEntitySuccess( op );
					utility.SendLogMessage( LogLevel::kInfo, loggerName, msg + op.Message );
					return;
				case StatusCode::kTimeout:
					msg += " timed out. Resending";
					pEntities->ResendCreateEntity( op, utility );
					break;
				case StatusCode::kApplicationError:
					msg = " reservation may have expired. Resending";
					pEntities->ResendCreateEntity( op, utility );
					break;
				case StatusCode::kInternalError:
					msg += "I received an internal error, which is usually a Spatial OS error! ";
					msg += "Please advice Improbable off the details. Thanks Operator";
					pEntities->ResendCreateEntity( op, utility );
					break;
				case StatusCode::kAuthorityLost:
					msg += "I lost authority while trying to create entity.";
					break;
				case StatusCode::kPermissionDenied:
					msg = " worker does not heave permission to create entity";
					break;

				default:
					msg += "I received an unmapped status code, ";
					msg += "Please advice, thanks Operator";
				}

				msg += " - > " + op.Message;
				utility.SendLogMessage( LogLevel::kError, loggerName, msg );

			} );

			//OnDeleteEntityResponse	 DeleteEntityResponseOp(RequestId<DeleteEntityRequest> RequestId, 
			//EntityId EntityId, StatusCode StatusCode, string Message)	
			//the worker has received a response for an entity deletion it had requested previously.

			dispatcher.OnDeleteEntityResponse( [&] ( const DeleteEntityResponseOp& op ) {
				string msg = "Received a Delete Entity Response Callback, EntityId(" + to_string( op.EntityId ) + "), ";
				switch ( op.StatusCode ) {
				case StatusCode::kSuccess:
					pEntities->RemoveEntity( op );
					return;
				case StatusCode::kTimeout:
					msg += "timed out. Resending";
					utility.SendDeleteEntityRequest( op.EntityId );
					break;
				case StatusCode::kApplicationError:
					msg += "application error. Resending";
					utility.SendDeleteEntityRequest( op.EntityId );
					break;
				case StatusCode::kInternalError:
					msg += "I received an internal error, which is usually a Spatial OS error! ";
					msg += "Please advice Improbable off the details. Thanks Operator";
					utility.SendDeleteEntityRequest( op.EntityId );
					break;
				case StatusCode::kAuthorityLost:
					msg += "I lost authority while trying to create entity";
					break;
				case StatusCode::kPermissionDenied:
					msg += "this worker does not heave permission to delete the entity.";
					break;
				default:
					msg += "I received an unmapped status code and I don't know what to do! ";
					msg += "Please advice, thanks Operator. resending!";
					utility.SendDeleteEntityRequest( op.EntityId );
				}
				msg += " - > " + op.Message;
				utility.SendLogMessage( LogLevel::kError, loggerName, msg );

			} );
		}

		void RegisterEntityIdCallbacks( CoreUtility& utility, Dispatcher& dispatcher,
			shared_ptr <Entities>& pEntities ) {
			//OnReserveEntityIdsResponse	RequestId<ReserveEntityIdsRequest>(ReserveEntityIdsRequest RequestId, 
			//StatusCode StatusCode, string Message, Option<EntityId> FirstEntityId, size_t NumberOfEntityIds)	
			//the worker has received a response for a reservation of an entity ID range it had requested previously.
			dispatcher.OnReserveEntityIdsResponse( [&] ( const ReserveEntityIdsResponseOp& op ) {
				string msg = "Reserve EntityIds, Request ID (" + to_string( op.RequestId.Id ) + ") ";
				switch ( op.StatusCode ) {
				case StatusCode::kSuccess:
					pEntities->CreateEntity( op, utility );
					utility.SendLogMessage( LogLevel::kInfo, loggerName, msg + op.Message );
					return;
				case StatusCode::kTimeout:
					pEntities->ResendEntityIdsReservation( op, utility );
					msg += "Reserving Entity ID timed out, resending. ";
					break;
				case StatusCode::kAuthorityLost:
					msg += "I lost authority while trying to create entity with ";
					msg += "request ID (" + to_string( op.RequestId.Id ) + ") ";
					break;
				case StatusCode::kPermissionDenied:
					msg += "This worker does not heave permission to create entity with ";
					msg += "entity ID(" + to_string( *op.FirstEntityId.data( ) ) + ") no further action to be taken. ";
					msg += "request ID (" + to_string( op.RequestId.Id ) + ") ";
					break;
				case StatusCode::kApplicationError:
					pEntities->ResendEntityIdsReservation( op, utility );
					msg += "reservation may have expired. ";
					msg += "I have assigned the entity with entityID ( " + to_string( *op.FirstEntityId.data( ) ) + " ) to be resent.";
					break;
				case StatusCode::kInternalError:
					msg += "I received an internal error, which is usually a Spatial OS error! ";
					msg += "Please advice Improbable off the details. Thanks Operator";
					break;
				default:
					msg += "I received an unmapped status code, ";
					msg += "Please advice, thanks Operator";
				}

				msg += " - > " + op.Message;
				utility.SendLogMessage( LogLevel::kError, loggerName, msg );
			} );
		}
	};
}

#endif // !ENTITY_HANDLING_H
