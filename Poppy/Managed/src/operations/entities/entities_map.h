// entities_map.h

#ifndef ENTITIES_MAP_H
#define ENTITIES_MAP_H

namespace operations {
namespace entities {

struct EntMap {
    mutable shared_mutex entMap_mutex;
    map<EntityId, EntityPtr> eMap;

    Option<EntityPtr> GetEntity(const EntityId& entityId) {
        unique_lock<shared_mutex> lock(entMap_mutex, try_to_lock);
        map<EntityId, EntityPtr>::iterator it = eMap.find(entityId);
        if (it != eMap.end())
            return it->second;
        return Option<EntityPtr>();
    }

    double GetMapSize() {
        unique_lock<shared_mutex> lock(entMap_mutex, try_to_lock);
        return (double)eMap.size();
    }

    void AddEntity(EntityId entityId) {
        unique_lock<shared_mutex> lock(entMap_mutex, try_to_lock);
        eMap[entityId] = make_shared<EntityManager>();
    }

    void AddEntity(EntityId entityId, EntityPtr pEntity) {
        unique_lock<shared_mutex> lock(entMap_mutex, try_to_lock);
        eMap[entityId] = pEntity;
    }

    void RemoveEntity(const EntityId& entityId) {
        unique_lock<shared_mutex> lock(entMap_mutex, try_to_lock);
        map<EntityId, EntityPtr>::iterator it = eMap.find(entityId);
        if (it != eMap.end())
            eMap.erase(it);
    }
};

class EntitiesMap {
    const string loggerName = "operations/entities/entity_map.h";
    mutable shared_mutex entitiesMap_mutex;
    void PrintError(string msg) { cerr << loggerName << " - " << msg << endl; }

    typedef shared_ptr<EntMap> MapPtr;
    MapPtr pEntityMap = make_shared<EntMap>();
    MapPtr pTreeMap = make_shared<EntMap>();
    MapPtr pPlayerMap = make_shared<EntMap>();
    MapPtr pNpcMap = make_shared<EntMap>();
    MapPtr pDirectorMap = make_shared<EntMap>();
    MapPtr pPlotMap = make_shared<EntMap>();
    map<EntityId, MapPtr> allEntitiesMap;

    template <typename T> MapPtr GetEntityMap(const EntityId& entityId) {
        if (entityId < 0)
            return pEntityMap;
        map<EntityId, MapPtr>::iterator it = allEntitiesMap.find(entityId);
        if (it != allEntitiesMap.end())
            return it->second;

        return pEntityMap;
    }

    template <> MapPtr GetEntityMap<Tree>(const EntityId& entityId) {
        return pTreeMap;
    }

    template <> MapPtr GetEntityMap<PlayerInfo>(const EntityId& entityId) {
        return pPlayerMap;
    }

    template <> MapPtr GetEntityMap<NPCInfo>(const EntityId& entityId) {
        return pNpcMap;
    }

    template <> MapPtr GetEntityMap<DirectorInfo>(const EntityId& entityId) {
        return pDirectorMap;
    }

    template <> MapPtr GetEntityMap<PlotInfo>(const EntityId& entityId) {
        return pDirectorMap;
    }

  public:
    Option<EntityPtr> GetEntity(const EntityId& entityId) {
        unique_lock<shared_mutex> lock(entitiesMap_mutex, try_to_lock);
        return GetEntityMap<MapPtr>(entityId)->GetEntity(entityId);
    }

    void AddEntity(const AddEntityOp& op) {
        unique_lock<shared_mutex> lock(entitiesMap_mutex, try_to_lock);
        MapPtr ptr = GetEntityMap<MapPtr>(op.EntityId);
        ptr->AddEntity(op.EntityId);
        allEntitiesMap[op.EntityId] = ptr;
    }

    void AddEntity(const EntityId& entityId, EntityManager entity) {
        unique_lock<shared_mutex> lock(entitiesMap_mutex, try_to_lock);
        MapPtr ptr = GetEntityMap<MapPtr>(entityId);
        ptr->AddEntity(entityId, make_shared<EntityManager>(entity));
        allEntitiesMap[entityId] = ptr;
    }

    void RemoveEntity(const RemoveEntityOp& op) {
        unique_lock<shared_mutex> lock(entitiesMap_mutex, try_to_lock);
        GetEntityMap<MapPtr>(op.EntityId)->RemoveEntity(op.EntityId);
        allEntitiesMap.erase(op.EntityId);
    }

    void RemoveEntity(const DeleteEntityResponseOp& op) {
        unique_lock<shared_mutex> lock(entitiesMap_mutex, try_to_lock);
        GetEntityMap<MapPtr>(op.EntityId)->RemoveEntity(op.EntityId);
        allEntitiesMap.erase(op.EntityId);
    }

    template <typename T> void AddComponent(const AddComponentOp<T>& op) {
        unique_lock<shared_mutex> lock(entitiesMap_mutex, try_to_lock);
        EntityPtr pEntity = make_shared<EntityManager>();
        MapPtr mapPtr = GetEntityMap<MapPtr>(op.EntityId);
        Option<EntityPtr> oEntity = mapPtr->GetEntity(op.EntityId);
        if (!oEntity.empty())
            pEntity = *oEntity.data();
        // remove it because we may want it somewhere else
        mapPtr->RemoveEntity(op.EntityId);

        pEntity->Add<T>(op.Data);
        MapPtr ptr = GetEntityMap<T>(op.EntityId);
        ptr->AddEntity(op.EntityId, pEntity);
        allEntitiesMap[op.EntityId] = ptr;
    }

    template <typename T> void UpdateComponent(const ComponentUpdateOp<T>& op) {
        unique_lock<shared_mutex> lock(entitiesMap_mutex, try_to_lock);
        Option<EntityPtr> oEntity = GetEntity(op.EntityId);
        if (oEntity.empty())
            return;

        EntityPtr pEntity = *oEntity.data();
        pEntity->UpdateComponent<T>(op);
    }

    template <typename T> void RemoveComponent(const RemoveComponentOp& op) {
        unique_lock<shared_mutex> lock(entitiesMap_mutex, try_to_lock);
        Option<EntityPtr> oEntity = GetEntity(op.EntityId);
        if (oEntity.empty())
            return;

        EntityPtr pEntity = *oEntity.data();
        pEntity->Remove<T>();
    }

    template <typename T> double GetMapSize() {
        unique_lock<shared_mutex> lock(entitiesMap_mutex, try_to_lock);
        return GetEntityMap<T>(-1)->GetMapSize();
    }

    double GetNumAllEntities() {
        return static_cast<double>(allEntitiesMap.size());
    }
};

class EntityCreateMap {
    shared_ptr<EntitiesMap> pEntitiesMap;

    mutable shared_mutex _mutex;
    // adding entities
    map<uint32_t, EntityRequestPair> entityIdsRequestMap;
    map<EntityId, EntityManager> createEntityMap;
    // Commands
    map<uint32_t, EntityId> commandIdsRequestMap;

  private:
    void addReserveEntityIdsRequest(
        const EntityId& entityId,
        const RequestId<ReserveEntityIdsRequest>& requestId,
        list<EntityManager>& entities) {
        Print("Adding " + to_string(entities.size()) +
              " reserve entities to Reserve id " + to_string(requestId.Id));
        entityIdsRequestMap[requestId.Id] = make_pair(entityId, entities);
    }

    void addReserveCommandRequest(const uint32_t& requestId,
                                  EntityId entityId) {
        commandIdsRequestMap[requestId] = entityId;
    }

    void addNewEntity(const EntityId& entityId, EntityManager& entity,
                      CoreUtility& utility) {
        createEntityMap[entityId] = entity;
    }

    void removeEntityIdsReservation(const uint32_t& requestId) {
        Print("removeEntityIdsReservation " + to_string(requestId));
        map<uint32_t, EntityRequestPair>::iterator it =
            entityIdsRequestMap.find(requestId);
        if (it != entityIdsRequestMap.end())
            entityIdsRequestMap.erase(it);
    }

    Option<list<EntityManager>>
    getRequestEntities(const uint32_t& requestId, bool suppressErrors = false) {
        map<uint32_t, EntityRequestPair>::iterator it =
            entityIdsRequestMap.find(requestId);
        if (it != entityIdsRequestMap.end())
            return Option<list<EntityManager>>(it->second.second);
        if (!suppressErrors)
            Print("Cannot find reserve entities to Reserve id " +
                  to_string(requestId));
        return Option<list<EntityManager>>();
    }

    Option<EntityId> getRequestEntityId(const uint32_t& requestId) {
        map<uint32_t, EntityRequestPair>::iterator it =
            entityIdsRequestMap.find(requestId);
        if (it != entityIdsRequestMap.end())
            return Option<EntityId>(it->second.first);
        Print("Cannot find RequestEntityId " + to_string(requestId));
        return Option<EntityId>();
    }

    Option<EntityManager> getNewEntity(const EntityId& entityId) {
        map<EntityId, EntityManager>::iterator it =
            createEntityMap.find(entityId);
        if (it != createEntityMap.end())
            return Option<EntityManager>(it->second);
        return Option<EntityManager>();
    }

    Option<RequestId<ReserveEntityIdsRequest>>
    spawnEntities(const EntityId& entityId, list<EntityManager>& _entities,
                  CoreUtility& utility) {
        RequestId<ReserveEntityIdsRequest> requestId;
        if (_entities.size() == 0 || _entities.empty()) {
            Print("Missing entities when spawning entities");
            return Option<RequestId<ReserveEntityIdsRequest>>();
        }
        requestId =
            utility.SendReserveEntityIdsRequest((uint32_t)_entities.size());
        addReserveEntityIdsRequest(entityId, requestId, _entities);
        return Option<RequestId<ReserveEntityIdsRequest>>(requestId);
    }

    void removeNewEntity(const EntityId& entityId) {
        map<EntityId, EntityManager>::iterator it =
            createEntityMap.find(entityId);
        if (it != createEntityMap.end())
            createEntityMap.erase(it);
    }

  public:
    EntityCreateMap(shared_ptr<EntitiesMap> _pEntitiesMap)
        : pEntitiesMap(_pEntitiesMap) {}

    void Print(const string& msg) {
        cerr << "EntityCreateMap"
             << " - " << msg << endl;
    }

    bool IsRequestEntities(const uint32_t& requestId) {
        unique_lock<shared_mutex> lock(_mutex);
        Option<list<EntityManager>> entities =
            getRequestEntities(requestId, true);
        return ((!entities.empty() && !(*entities.data()).empty() &&
                 (*entities.data()).size() != 0));
    }

    void AddReserveCommandRequest(const uint32_t& requestId,
                                  EntityId entityId) {
        unique_lock<shared_mutex> lock(_mutex);
        addReserveCommandRequest(requestId, entityId);
    }

    void CreateEntitySuccess(const CreateEntityResponseOp& op) {
        unique_lock<shared_mutex> lock(_mutex);
        EntityId entityId = *op.EntityId.data();
        Option<EntityManager> optEntity = getNewEntity(entityId);
        if (!optEntity.empty()) {
            // add the newly created entity
            pEntitiesMap->AddEntity(entityId, *optEntity.data());
            // clean up the request id map
            removeNewEntity(entityId);
        }
    }

    void RemoveCommandRequest(const uint32_t& requestId) {
        unique_lock<shared_mutex> lock(_mutex);
        map<uint32_t, EntityId>::iterator it =
            commandIdsRequestMap.find(requestId);
        if (it != commandIdsRequestMap.end())
            commandIdsRequestMap.erase(it);
    }

    Option<EntityId>
    GetRequestEntityId(const RequestId<ReserveEntityIdsRequest>& requestId) {
        unique_lock<shared_mutex> lock(_mutex);
        return getRequestEntityId(requestId.Id);
    }

    Option<EntityId> GetCommandRequest(const uint32_t& requestId) {
        unique_lock<shared_mutex> lock(_mutex);
        map<uint32_t, EntityId>::iterator it =
            commandIdsRequestMap.find(requestId);
        if (it != commandIdsRequestMap.end())
            return Option<EntityId>(it->second);
        return Option<EntityId>();
    }

    Option<RequestId<ReserveEntityIdsRequest>>
    SpawnEntities(const EntityId& entityId, list<EntityManager>& _entities,
                  CoreUtility& utility) {
        unique_lock<shared_mutex> lock(_mutex);
        return spawnEntities(entityId, _entities, utility);
    }

    Option<RequestId<ReserveEntityIdsRequest>>
    ReSpawnEntities(const EntityId& entityId, const uint32_t& requestId,
                    CoreUtility& utility) {
        unique_lock<shared_mutex> lock(_mutex);
        Option<list<EntityManager>> entities = getRequestEntities(requestId);
        return spawnEntities(entityId, *entities.data(), utility);
    }

    void CreateEntity(const ReserveEntityIdsResponseOp& op,
                      CoreUtility& utility) {
        Print("CreateEntity " + to_string(op.RequestId.Id));

        unique_lock<shared_mutex> lock(_mutex);
        Option<list<EntityManager>> newEntities =
            getRequestEntities(op.RequestId.Id);

        if ((*newEntities.data()).size() == 0 ||
            (*newEntities.data()).empty()) {
            Print("Missing entities when sending Create entities");
            return;
        }

        EntityId entityId = *op.FirstEntityId.data();
        for (list<EntityManager>::iterator it = (*newEntities.data()).begin();
             it != (*newEntities.data()).end(); ++it) {
            Print("SendCreateEntityRequest " + to_string(op.RequestId.Id));
            RequestId<CreateEntityRequest> requestId =
                utility.SendCreateEntityRequest(*it,
                                                Option<EntityId>(entityId));
            addNewEntity(entityId, *it, utility);
            ++entityId;
        }
        removeEntityIdsReservation(op.RequestId.Id);
    }

    void ResendCreateEntity(const CreateEntityResponseOp& op,
                            CoreUtility& utility) {
        unique_lock<shared_mutex> lock(_mutex);
        if (op.EntityId.empty()) {
            Print(
                "Missing entityId when attempting to re send Create entities");
            return;
        }
        Option<EntityManager> entity = getNewEntity(*op.EntityId.data());
        if (!entity.empty()) {
            utility.SendCreateEntityRequest(*entity.data(), op.EntityId);
            return;
        }
        Print("Couldn't find Entity when attempting to resend CreateEntity for "
              "RequestID: " +
              to_string(op.RequestId.Id));
    }
};
} // namespace entities
} // namespace operations

#endif // !ENTITIES_MAP_H