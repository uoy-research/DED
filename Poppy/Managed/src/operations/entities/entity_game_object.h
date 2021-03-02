// entity_game_object.h

#ifndef ENTITY_GAME_OBJECT_H
#define ENTITY_GAME_OBJECT_H

namespace operations {

namespace entities {

class EntityGameObject : public EntityBase {
    shared_ptr<EntityCreateMap> pEntCreateMap;
    mutable shared_mutex _mutex;

  protected:
    //################ Commands #####################

    template <typename T>
    void SendCommandRequest(const EntityId& entityId,
                            const typename T::Request request,
                            CoreUtility& utility) {
        if (!ShouldRun()) {
            ConsoleLogErr("SendCommandRequest lost authority Entity Id:" +
                          to_string(entityId));
            return;
        }
        RequestId<OutgoingCommandRequest<T>> requestId =
            utility.SendCommandRequest<T>(entityId, request);

        pEntCreateMap->AddReserveCommandRequest(requestId.Id, GetEntityId());
        SendLogInfo("Sending command Request to " + to_string(entityId),
                    utility);
    }

    template <typename T>
    void SendCommandResponse(CommandRequestOp<T> op,
                             const typename T::Response& response,
                             CoreUtility& utility) {

        if (!ShouldRun()) {
            ConsoleLogErr("SendCommandResponse lost authority Entity Id:" +
                          to_string(op.EntityId) +
                          ", Request Id:" + to_string(op.RequestId.Id));
            return;
        }
        utility.SendCommandResponse<T>(op.RequestId, response);

        pEntCreateMap->RemoveCommandRequest(op.RequestId.Id);
        SendLogInfo("Sending command Response to RequestId " +
                        to_string(op.RequestId.Id),
                    utility);
    }

    //################ Spawn Entities #####################
    bool SpawnEntities(list<EntityManager>& entities,
                       const EntityType& entityType, CoreUtility& utility) {
        unique_lock<shared_mutex> lock(_mutex);
        Option<RequestId<ReserveEntityIdsRequest>> requestId =
            pEntCreateMap->SpawnEntities(GetEntityId(), entities, utility);
        if (requestId.empty())
            return false;
        EntitySpawning::Update update = GetUpdate<EntitySpawning>();
        UpdateEntityIdsRequest(update, (*requestId).Id, entityType, utility);
        SendUpdate<EntitySpawning>(update, utility);
        return true;
    }

    virtual void CreateEntitySuccess(const CreateEntityResponseOp& op,
                                     CoreUtility& utility) {
        ConsoleLogErr("Create Entity Success");
        unique_lock<shared_mutex> lock(_mutex);
        EntitySpawning::Update entityUpdate = GetUpdate<EntitySpawning>();
        Map<EntityId, EntityType> entityIds = *entityUpdate.entity_ids().data();
        EntityId entityId = *op.EntityId.data();

        ConsoleLogErr("number of entity ids" + to_string(entityIds.size()));
        Map<EntityId, EntityType>::iterator entityIt = entityIds.find(entityId);
        if (entityIt == entityIds.end())
            return;
        ConsoleLogErr("Found the entity type");

        entityIds.erase(entityIt);
        entityUpdate.set_entity_ids(entityIds);
        SendUpdate<EntitySpawning>(entityUpdate, utility);
    }

    virtual void
    ResendEntityIdsReservation(const ReserveEntityIdsResponseOp& op,
                               CoreUtility& utility) {
        unique_lock<shared_mutex> lock(_mutex);

        EntitySpawning::Update update = GetUpdate<EntitySpawning>();
        Map<uint32_t, EntityType> requestIds =
            *update.request_entity_ids().data();
        Map<uint32_t, EntityType>::iterator it =
            requestIds.find(op.RequestId.Id);
        if (it == requestIds.end())
            return;

        EntityType entityType = it->second;
        RemoveEntityIdsRequestId(update, op.RequestId.Id, utility);

        Option<RequestId<ReserveEntityIdsRequest>> requestId =
            pEntCreateMap->ReSpawnEntities(GetEntityId(), op.RequestId.Id,
                                           utility);
        if (requestId.empty())
            return;
        UpdateEntityIdsRequest(update, (*requestId).Id, entityType, utility);
        SendUpdate<EntitySpawning>(update, utility);
    }

    virtual void CreateEntity(const ReserveEntityIdsResponseOp& op,
                              CoreUtility& utility) {
        ConsoleLogErr("CreateEntity Request Id: " + to_string(op.RequestId.Id));
        unique_lock<shared_mutex> lock(_mutex);
        pEntCreateMap->CreateEntity(op, utility);
        EntitySpawning::Update update = GetUpdate<EntitySpawning>();
        RemoveEntityIdsRequestId(update, op.RequestId.Id, utility);
        SendUpdate<EntitySpawning>(update, utility);
    }

    void RemoveEntityIdsRequestId(EntitySpawning::Update& update,
                                  uint32_t requestId, CoreUtility& utility) {
        ConsoleLogErr("RemoveEntityIdsRequestId Request Id:" +
                      to_string(requestId));

        Map<uint32_t, EntityType> requestIds =
            *update.request_entity_ids().data();
        Map<uint32_t, EntityType>::iterator it = requestIds.find(requestId);
        if (it != requestIds.end())
            requestIds.erase(it);
        update.set_request_entity_ids(requestIds);
    }

    void UpdateEntityIdsRequest(EntitySpawning::Update& update,
                                uint32_t requestId,
                                const EntityType& entityType,
                                CoreUtility& utility) {
        Map<uint32_t, EntityType> requestIds =
            *update.request_entity_ids().data();
        requestIds[requestId] = entityType;
        update.set_request_entity_ids(requestIds);
    }

    bool IsEntitySpawningFailing(CoreUtility& utility) {
        unique_lock<shared_mutex> lock(_mutex);

        EntitySpawning::Update update = GetUpdate<EntitySpawning>();
        Map<uint32_t, EntityType> requestIds =
            *update.request_entity_ids().data();
        if (requestIds.size() == 0)
            return false;
        list<uint32_t> ids;

        for (Map<uint32_t, EntityType>::iterator it = requestIds.begin();
             it != requestIds.end(); ++it) {
            if (pEntCreateMap->IsRequestEntities(it->first))
                continue;
            ids.push_back(it->first);
        }

        if (ids.size() == 0)
            return false;

        for (list<uint32_t>::iterator itIds = ids.begin(); itIds != ids.end();
             ++itIds) {
            RemoveEntityIdsRequestId(update, *itIds, utility);
        }

        SendUpdate<EntitySpawning>(update, utility);
        return true;
    }

    void DeleteEntity(const EntityId& _entity_id, CoreUtility& utility) {
        utility.SendDeleteEntityRequest(_entity_id);
        SendLogInfo("Deleting entity Id " + to_string(_entity_id), utility);
    }

  protected:
    virtual Option<EntityId> GetSpawnerEntityId(CoreUtility& utility) {
        return Option<EntityId>();
    };

    virtual ded::EntityType GetEntityType() = 0;

    bool SendIsCreated(CoreUtility& utility) {
        Option<EntityId> entityId = GetSpawnerEntityId(utility);
        if (entityId.empty())
            return false;
        // Send My Id to Player
        SendCommandRequest<EntitySpawning::Commands::EntitySpawningCommand>(
            *entityId.data(), {GetEntityId(), GetEntityType()}, utility);

        return true;
    }

  public:
    EntityGameObject(const string& _loggerName, const EntityId& _entityId,
                     EntityPtr _pEntity,
                     shared_ptr<EntityCreateMap> _pEntCreateMap)
        : EntityBase(_loggerName, _entityId, _pEntity),
          pEntCreateMap(_pEntCreateMap) {}

    virtual void
    OnCommandTimeout(const CommandResponseOp<
                         EntitySpawning::Commands::EntitySpawningCommand>& op,
                     CoreUtility& utility) {
        ConsoleLogErr(
            "OnCommandTimeout, resending, "
            "(EntitySpawning::Commands::EntitySpawningCommand), Entity Id:" +
            to_string(op.EntityId) +
            ", Request Id:" + to_string(op.RequestId.Id));
        SendIsCreated(utility);
    };

    virtual void OnCommandAuthorityLost(
        const CommandResponseOp<
            EntitySpawning::Commands::EntitySpawningCommand>& op,
        CoreUtility& utility) {
        ConsoleLogErr(
            "OnCommandTimeout, resending, "
            "(EntitySpawning::Commands::EntitySpawningCommand), Entity Id:" +
            to_string(op.EntityId) +
            ", Request Id:" + to_string(op.RequestId.Id));
        SendIsCreated(utility);
    };
};
} // namespace entities
} // namespace operations

#endif // !BASE_ENTITY_GAME_OBJECT_H