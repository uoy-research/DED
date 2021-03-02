// base_entity_game_object.h

#ifndef BASE_ENTITY_GAME_OBJECT_H
#define BASE_ENTITY_GAME_OBJECT_H

namespace operations {

namespace entities {
typedef shared_ptr<EntityManager> EntityPtr;

class EntityBase : public ThreadBuilder {
    mutable shared_mutex authority_mutex;

    Authority authorityState = Authority::kAuthoritative;
    map<ComponentId, bool> authority;
    EntityId entityId;
    EntityPtr pEntity;

    void start(CoreUtility& utility) {
        unique_lock<shared_mutex> lock(authority_mutex, try_to_lock);

        List<ComponentId> compIds = pEntity->GetComponentIds();
        for (List<ComponentId>::iterator it = compIds.begin();
             it != compIds.end(); ++it) {
            authority[*it] = true;
        }

        OnCreate(utility);
    }

  protected:
    EntityId GetEntityId() { return entityId; }

    uint32_t GetRandomSeed() {
        return GetTimeInMilliseconds() + (uint32_t)GetEntityId();
    }

    void ConsoleLogErr(const string& msg) {
        ThreadBuilder::ConsoleLogErr(to_string(GetEntityId()) + " - " + msg);
    }

    virtual void SendLogInfo(const string& msg, CoreUtility& utility) {
        ThreadBuilder::SendLogInfo("[EntityId " + to_string(GetEntityId()) +
                                       "][" + GetTimeInMillisecondsString() +
                                       "]" + msg,
                                   GetEntityId(), utility);
    }

    virtual void SendLogInfo(const LogLevel level, const string& msg,
                             CoreUtility& utility) {
        ThreadBuilder::SendLogInfo(level,
                                   "[EntityId " + to_string(GetEntityId()) +
                                       "][" + GetTimeInMillisecondsString() +
                                       "]" + msg,
                                   GetEntityId(), utility);
    }

    virtual void OnCreate(CoreUtility& utility) = 0;

    template <typename T> Option<typename T::Data&> GetComponentData() {
        return pEntity->GetComponentData<T>();
    }

    template <typename T>
    bool SendUpdate(const typename T::Update& update, CoreUtility& utility) {
        if (!IsComponentAuthority<T>(utility)) {
            SendLogInfo("Cannot update, I don't have Authority", utility);
            return false;
        }
        utility.SendComponentUpdate<T>(GetEntityId(), update);
        pEntity->Update<T>(update);
        return true;
    }

    template <typename T> typename T::Update GetUpdate() {
        Option<typename T::Data&> info = GetComponentData<T>();
        if (info.empty()) {
            typename T::Update update;
            return update;
        }
        return typename T::Update::FromInitialData(*info.data());
    }

    Option<Position::Data&> GetPosition() {
        return pEntity->GetComponentData<Position>();
    }

    template <typename T> T Rand(int mod, int div = 1) {
        return (rand() % mod) / div;
    }

    template <typename T> bool IsComponentAuthority(CoreUtility& utility) {
        unique_lock<shared_mutex> lock(authority_mutex, try_to_lock);
        if (authorityState == Authority::kNotAuthoritative)
            return false;
        map<ComponentId, bool>::iterator it =
            authority.find(typename T::ComponentId);
        if (it != authority.end()) {
            return it->second;
        }
        return false;
    }

    State GetCurrentState(CoreUtility& utility) {
        if (!IsComponentAuthority<CurrentState>(utility))
            return State::NOT_AUTHORITATIVE;
        Option<typename CurrentState::Data&> info =
            GetComponentData<CurrentState>();
        if (info.empty()) {
            SendLogInfo(LogLevel::kError, "Missing State Component", utility);
            return State::MISSING;
        }
        return info.data()->state();
    }

    State GetCurrentAction(CoreUtility& utility) {
        if (!IsComponentAuthority<CurrentState>(utility))
            return State::NOT_AUTHORITATIVE;
        Option<typename CurrentState::Data&> info =
            GetComponentData<CurrentState>();
        if (info.empty()) {
            SendLogInfo(LogLevel::kError, "Missing action component", utility);
            return State::MISSING;
        }
        return info.data()->action();
    }

    void UpdateState(const State& value, CoreUtility& utility) {
        UpdateState(value, State::FIRST, utility);
    }

    void UpdateState(const State& state, const State& action,
                     CoreUtility& utility) {
        typename CurrentState::Update update = GetUpdate<CurrentState>();
        update.set_state(state);
        update.set_action(action);
        if (SendUpdate<CurrentState>(update, utility))
            ConsoleLogErr("Updates State: " +
                          ToString(GetCurrentState(utility)));
    }

    void UpdateAction(const State& value, CoreUtility& utility) {
        typename CurrentState::Update update = GetUpdate<CurrentState>();
        update.set_action(value);
        if (SendUpdate<CurrentState>(update, utility))
            ConsoleLogErr("Updates Action: " +
                          ToString(GetCurrentAction(utility)));
    }

  public:
    EntityBase(const string& _loggerName, const EntityId& _entityId,
               EntityPtr _pEntity)
        : ThreadBuilder(_loggerName), entityId(_entityId), pEntity(_pEntity) {}

    template <typename T> void SetAuthority() {
        unique_lock<shared_mutex> lock(authority_mutex, try_to_lock);
        authority[typename T::ComponentId] = true;
    }

    template <typename T> void RevokeAuthority() {
        unique_lock<shared_mutex> lock(authority_mutex, try_to_lock);
        authority[typename T::ComponentId] = false;
    }

    template <typename T>
    void OnAuthorityChange(const AuthorityChangeOp& op, CoreUtility& utility) {
        unique_lock<shared_mutex> lock(authority_mutex, try_to_lock);
    }

    // Entity spawning
    virtual void
    ResendEntityIdsReservation(const ReserveEntityIdsResponseOp& op,
                               CoreUtility& utility){};

    virtual void CreateEntity(const ReserveEntityIdsResponseOp& op,
                              CoreUtility& utility) {}

    // callbacks that are registerd by EntityGameObjects

    virtual void
    OnCommandRequest(const CommandRequestOp<PlotMap::Commands::PlotCommand>& op,
                     CoreUtility& utility) {
        ConsoleLogErr("Uncaught - OnCommandRequest "
                      "(PlotMap::Commands::PlotCommand), Entity Id:" +
                      to_string(op.EntityId) +
                      ", Request Id:" + to_string(op.RequestId.Id));
    };

    virtual void OnCommandResponse(
        const CommandResponseOp<PlotMap::Commands::PlotCommand>& op,
        CoreUtility& utility) {
        ConsoleLogErr("Uncaught - OnCommandResponse "
                      "(PlotMap::Commands::PlotCommand), Entity Id:" +
                      to_string(op.EntityId) +
                      ", Request Id:" + to_string(op.RequestId.Id));
    };

    virtual void OnCommandRequest(
        const CommandRequestOp<EntitySpawning::Commands::EntitySpawningCommand>&
            op,
        CoreUtility& utility) {
        ConsoleLogErr(
            "Uncaught - OnCommandRequest "
            "EntitySpawning::Commands::EntitySpawningCommand), Entity Id:" +
            to_string(op.EntityId) +
            ", Request Id:" + to_string(op.RequestId.Id));
    };

    virtual void
    OnCommandResponse(const CommandResponseOp<
                          EntitySpawning::Commands::EntitySpawningCommand>& op,
                      CoreUtility& utility) {
        ConsoleLogErr(
            "Uncaught - OnCommandResponse "
            "(EntitySpawning::Commands::EntitySpawningCommand), Entity Id:" +
            to_string(op.EntityId) +
            ", Request Id:" + to_string(op.RequestId.Id));
    };

    virtual void
    OnCommandTimeout(const CommandResponseOp<
                         EntitySpawning::Commands::EntitySpawningCommand>& op,
                     CoreUtility& utility) {
        ConsoleLogErr(
            "Uncaught - OnCommandTimeout "
            "(EntitySpawning::Commands::EntitySpawningCommand), Entity Id:" +
            to_string(op.EntityId) +
            ", Request Id:" + to_string(op.RequestId.Id));
    };

    virtual void OnCommandAuthorityLost(
        const CommandResponseOp<
            EntitySpawning::Commands::EntitySpawningCommand>& op,
        CoreUtility& utility) {
        ConsoleLogErr(
            "Uncaught - OnCommandAuthorityLost "
            "(EntitySpawning::Commands::EntitySpawningCommand), Entity Id:" +
            to_string(op.EntityId) +
            ", Request Id:" + to_string(op.RequestId.Id));
    };

    virtual void OnComponentUpdate(const ComponentUpdateOp<Position>& op,
                                   CoreUtility& utility){};
    virtual void OnAddComponent(const AddComponentOp<Position>& op,
                                CoreUtility& utility){};
    virtual void OnAddComponent(const AddComponentOp<ActorInfo>& op,
                                CoreUtility& utility){};

    // Generic template callback functions
    template <typename T>
    void OnComponentUpdate(const ComponentUpdateOp<T>& op,
                           CoreUtility& utility){};
    template <typename T>
    void OnAddComponent(const AddComponentOp<T>& op, CoreUtility& utility){};
    template <typename T>
    void OnCommandRequest(const CommandRequestOp<T>& op, CoreUtility& utility) {
        ConsoleLogErr(
            "OnCommandRequest (T), Entity Id:" + to_string(op.EntityId) +
            ", Request Id:" + to_string(op.RequestId.Id));
    };
    template <typename T>
    void OnCommandResponse(const CommandResponseOp<T>& op,
                           CoreUtility& utility) {
        ConsoleLogErr(
            "OnCommandResponse (T), Entity Id:" + to_string(op.EntityId) +
            ", Request Id:" + to_string(op.RequestId.Id));
    };
    template <typename T>
    void OnCommandTimeout(const CommandResponseOp<T>& op,
                          CoreUtility& utility) {
        ConsoleLogErr(
            "OnCommandTimeout (T), Entity Id:" + to_string(op.EntityId) +
            ", Request Id:" + to_string(op.RequestId.Id));
    };
    template <typename T>
    void OnCommandAuthorityLost(const CommandResponseOp<T>& op,
                                CoreUtility& utility) {
        ConsoleLogErr(
            "OnCommandAuthorityLost (T), Entity Id:" + to_string(op.EntityId) +
            ", Request Id:" + to_string(op.RequestId.Id));
    };
    template <typename T>
    void OnRemoveComponent(const RemoveComponentOp& op, CoreUtility& utility){};

    virtual ~EntityBase() {}
};
} // namespace entities
} // namespace operations

#endif // !BASE_ENTITY_GAME_OBJECT_H