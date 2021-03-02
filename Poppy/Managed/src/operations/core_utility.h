// CORE_UTILITY.h

#ifndef CORE_UTILITY_H
#define CORE_UTILITY_H

namespace operations {

class CoreUtility {
    std::unique_ptr<Connection> pConnection;
    // Constants and parameters
    const string loggerName = "operations/core_utility.h";
    atomic_bool is_connected;
    mutable shared_mutex connection_mutex;

  public:
    CoreUtility(Connection& bar) : pConnection(new Connection(std::move(bar))) {
        is_connected.store(true);
    }

    void Print(const string& logger, const string& msg) {
        cerr << logger << " - " << msg << endl;
    }

    void setConnected() { is_connected.store(true); }

    bool IsConnected() { return is_connected.load(); }

    void SendLogMessage(const string& logger_name, const string& message) {
        SendLogMessage(logger_name, message, Option<EntityId>());
    }

    void SendLogMessage(const string& logger_name, const string& message,
                        const Option<EntityId> entity_id) {
        unique_lock<shared_mutex> lock(connection_mutex, try_to_lock);
        pConnection->SendLogMessage(LogLevel::kInfo, logger_name, message,
                                    entity_id);
    }

    void SendLogMessage(const LogLevel level, const string& logger_name,
                        const string& message) {
        SendLogMessage(level, logger_name, message, Option<EntityId>());
    }

    void SendLogMessage(const LogLevel level, const string& logger_name,
                        const string& message,
                        const Option<EntityId> entity_id) {
        unique_lock<shared_mutex> lock(connection_mutex, try_to_lock);
        pConnection->SendLogMessage(level, logger_name, message, entity_id);
    }

    OpList GetOpList() {
        unique_lock<shared_mutex> lock(connection_mutex, try_to_lock);
        return pConnection->GetOpList(kGetOpListTimeoutInMilliseconds);
    }

    template <typename T>
    void SendComponentUpdate(const EntityId& entityId,
                             const typename T::Update& update) {
        unique_lock<shared_mutex> lock(connection_mutex, try_to_lock);
        pConnection->SendComponentUpdate<T>(entityId, update);
    }

    void SendMetrics(Metrics& metrics) {
        unique_lock<shared_mutex> lock(connection_mutex, try_to_lock);
        pConnection->SendMetrics(metrics);
    }

    RequestId<ReserveEntityIdsRequest>
    SendReserveEntityIdsRequest(std::uint32_t number_of_entity_ids) {
        unique_lock<shared_mutex> lock(connection_mutex, try_to_lock);

        Print(loggerName,
              "[ReserveEntityIdsRequest] " + to_string(number_of_entity_ids));
        return pConnection->SendReserveEntityIdsRequest(number_of_entity_ids,
                                                        timeoutInMilliseconds);
    }

    RequestId<CreateEntityRequest>
    SendCreateEntityRequest(Entity entity, const Option<EntityId>& entity_id) {
        unique_lock<shared_mutex> lock(connection_mutex, try_to_lock);
        Print(loggerName,
              "[SendCreateEntityRequest] " + to_string(*entity_id.data()));
        return pConnection->SendCreateEntityRequest(entity, entity_id,
                                                    timeoutInMilliseconds);
    }

    void Disconnected(const DisconnectOp& op) {
        is_connected.store(false);
        Print(loggerName, "[disconnect] " + op.Reason);
    }

    RequestId<DeleteEntityRequest>
    SendDeleteEntityRequest(const EntityId& entity_id) {
        unique_lock<shared_mutex> lock(connection_mutex, try_to_lock);
        return pConnection->SendDeleteEntityRequest(entity_id,
                                                    timeoutInMilliseconds);
    }

    RequestId<EntityQueryRequest>
    SendEntityQueryRequest(const query::EntityQuery& entity_query) {
        unique_lock<shared_mutex> lock(connection_mutex, try_to_lock);
        return pConnection->SendEntityQueryRequest(entity_query,
                                                   timeoutInMilliseconds);
    }

    void SendComponentInterest(
        const EntityId& entity_id,
        const Map<ComponentId, InterestOverride>& interest_overrides) {
        unique_lock<shared_mutex> lock(connection_mutex, try_to_lock);
        pConnection->SendComponentInterest(entity_id, interest_overrides);
    }

    void SendAuthorityLossImminentAcknowledgement(const EntityId& entity_id,
                                                  ComponentId component_id) {
        unique_lock<shared_mutex> lock(connection_mutex, try_to_lock);
        pConnection->SendAuthorityLossImminentAcknowledgement(entity_id,
                                                              component_id);
    }

    template <typename T>
    RequestId<OutgoingCommandRequest<T>>
    SendCommandRequest(const EntityId& entity_id,
                       const typename T::Request& request) {
        unique_lock<shared_mutex> lock(connection_mutex, try_to_lock);
        return pConnection->SendCommandRequest<T>(
            entity_id, request, timeoutInMilliseconds, {false});
    }

    template <typename T>
    RequestId<OutgoingCommandRequest<T>>
    SendCommandRequest(const EntityId& entity_id,
                       typename T::Request&& request) {
        unique_lock<shared_mutex> lock(connection_mutex, try_to_lock);
        return pConnection->SendCommandRequest<T>(
            entity_id, request, timeoutInMilliseconds, {false});
    }

    template <typename T>
    void SendCommandResponse(RequestId<IncomingCommandRequest<T>> request_id,
                             const typename T::Response& response) {
        unique_lock<shared_mutex> lock(connection_mutex, try_to_lock);
        pConnection->SendCommandResponse<T>(request_id, response);
    }

    template <typename T>
    void SendCommandResponse(RequestId<IncomingCommandRequest<T>> request_id,
                             typename T::Response&& response) {
        unique_lock<shared_mutex> lock(connection_mutex, try_to_lock);
        pConnection->SendCommandResponse<T>(request_id, response);
    }

    template <typename T>
    void SendCommandFailure(RequestId<IncomingCommandRequest<T>> request_id,
                            const string& message) {
        unique_lock<shared_mutex> lock(connection_mutex, try_to_lock);
        pConnection->SendCommandFailure<T>(request_id, message);
    }

    void SetProtocolLoggingEnabled(bool enabled) {
        unique_lock<shared_mutex> lock(connection_mutex, try_to_lock);
        pConnection->SetProtocolLoggingEnabled(enabled);
    }

    Option<string> GetWorkerFlag(const string& flag_name) {
        unique_lock<shared_mutex> lock(connection_mutex, try_to_lock);
        return pConnection->GetWorkerFlag(flag_name);
    }

    worker::List<string> GetWorkerAttributes() {
        unique_lock<shared_mutex> lock(connection_mutex, try_to_lock);
        return pConnection->GetWorkerAttributes();
    }

    string GetWorkerId() { return pConnection->GetWorkerId(); };
};
} // namespace operations

#endif // !CORE_UTILITY_H