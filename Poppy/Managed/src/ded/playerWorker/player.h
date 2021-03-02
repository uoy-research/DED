// player_dummy.h
#ifndef PLAYER_DUMMY_H
#define PLAYER_DUMMY_H

namespace operations {
namespace entities {
namespace PlayerWorker {

class Player : public ActorEntity {
    mutable shared_mutex process_mutex;

    void process(CoreUtility& utility) {
        unique_lock<shared_mutex> lock(process_mutex, try_to_lock);
        switch (GetCurrentState(utility)) {
        case State::CREATE_INFO_MAP:
            if (CreateMap(utility))
                UpdateState(State::CREATE_DIRECTOR, State::CREATE_DIRECTOR,
                            utility);
            break;
        case State::CREATE_DIRECTOR:
            CreateDirector(utility);
            break;
        case State::BUILD_KNOWLEDGE_BASE:
            BuildKnowledgeBase(utility);
            break;
        case State::ACT:
            Act(utility);
            break;

        default:
            break;
        }
    }

    void BuildKnowledgeBase(CoreUtility& utility) {
        CreatePersonalKnowledge();
        UpdateState(State::ACT, utility);
    };

    void CreatePersonalKnowledge() {
        dpge::MurderPlot plot;
        Map<dpge::PlotEnum, dpge::Submodels> view = plot.PlayersView();
    };

    void CreateNPCKnowledge(){};
    void CreateMurderKnowledge(){};

    void Act(CoreUtility& utility){};

    void CreateDirector(CoreUtility& utility) {
        switch (GetCurrentAction(utility)) {
        case State::CREATE_DIRECTOR:
            if (SpawnDirector(utility))
                UpdateAction(State::CHECK_DIRECTOR_CREATED, utility);
            break;
        case State::CHECK_DIRECTOR_CREATED:
            if (IsEntitySpawningFailing(utility))
                UpdateAction(State::CREATE_DIRECTOR, utility);
        default:
            break;
        }
    }

    bool SpawnDirector(CoreUtility& utility) {
        if (IsDirectorAllocated())
            return true; // director exists
        Option<PlayerInfo::Data&> optPlayer = GetComponentData<PlayerInfo>();
        if (optPlayer.empty())
            return false;
        Option<Position::Data&> oPos = GetPosition();
        if (oPos.empty())
            return false;

        EntityBuilder builder("Director " + to_string(GetEntityId()),
                              oPos.data()->coords());
        EntityManager entity = builder.BuildDirectorEntity(GetEntityId());

        list<EntityManager> entities = list<EntityManager>{entity};
        return SpawnEntities(list<EntityManager>{entity}, EntityType::DIRECTOR,
                             utility);
    }

    virtual ded::EntityType GetEntityType() { return ded::EntityType::PLAYER; }

  public:
    Player(const string& _loggerName, const EntityId& _entityId,
           EntityPtr _pEntity, shared_ptr<EntityCreateMap> _pEntCreateMap)
        : ActorEntity(_loggerName, _entityId, _pEntity, _pEntCreateMap) {}

    virtual void OnCreate(CoreUtility& utility) {}

    virtual void OnCommandRequest(
        const CommandRequestOp<EntitySpawning::Commands::EntitySpawningCommand>&
            op,
        CoreUtility& utility) {

        EntityId entityId = op.Request.entity_id();
        ActorInfo::Update update = GetUpdate<ActorInfo>();
        ActorInfo::Data data = update.ToInitialData();
        EntityId directorId = data.director_id();
        if (directorId > 0) {
            if (directorId != entityId) {
                utility.SendDeleteEntityRequest(entityId);
                SendCommandResponse(op, {false}, utility);
                return;
            }

            SendCommandResponse(op, {true}, utility);
            return;
        }
        SendCommandResponse(op, {true}, utility);
        update.set_director_id(entityId);
        SendUpdate<ActorInfo>(update, utility);
        UpdateState(State::BUILD_KNOWLEDGE_BASE, utility);
    }
};
} // namespace PlayerWorker
} // namespace entities
} // namespace operations
#endif // !PLAYER_DUMMY_H