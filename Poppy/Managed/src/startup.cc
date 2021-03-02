
#include <atomic>
#include <stdlib.h>
#include <map>
#include <list>
#include <shared_mutex>
#include <iostream>
#include <sstream>
#include <vector>
#include <iterator>
#include <algorithm>
#include <thread>
#include <utility> 
#include <string>
#include <cmath>
#include <ctime>
#include <random>
#include <exception>
#include <stdexcept>
#include <ratio>
#include <stdint.h>
#include <smile.h>
#include <improbable/worker.h>
#include <improbable/standard_library.cc>
#include <improbable/core/UnityWorkerAuthorityCheck.cc>
#include <improbable/core/ClientAuthorityCheck.cc>
#include <improbable/core/ConnectionHeartbeat.cc>
#include <improbable/core/Inventory.cc>
#include <improbable/core/Transform.cc>
#include <improbable/core/Nothing.cc>
#include <improbable/vector3.cc>

#include <ded/flora/Tree.cc>
#include <ded/director/DirectorInfo.cc>
#include <ded/player/PlayerInfo.cc>
#include <ded/npc/NPCInfo.cc>
#include <ded/InfoEnum.cc> 
#include <ded/ActorInfo.cc> 
#include <ded/EntitySpawning.cc> 
#include <ded/CurrentState.cc> 

#include <dpge/PlotMap.cc>
#include <dpge/PlotEnum.cc>

#include <rde/KnowledgeBase.cc>

#include "ai/AI_node.h"
#include "ai/bayes.h"

#include "dpge/dpge_enum_maps.h"
#include "dpge/settings.h"
#include "dpge/basic_net.h"
#include "dpge/murder_plot.h"

#include "rde/knowledge_builder.h"

#include "operations/constants.h"
#include "operations/core_utility.h"
#include "operations/thread_builder.h"
#include "ded/ded_enum_maps.h"

#include "operations/entities/entity_template_factory.h"
#include "operations/entities/entity_manager.h"
#include "operations/entities/entity_builder.h"
#include "operations/entities/base_entity_game_object.h"

#include "operations/entities/thread_pool.h"
#include "operations/entities/entities_map.h"
#include "operations/entities/entity_game_object.h"
#include "operations/entities/actor_entity.h"


#include "ded/playerWorker/player.h"
#include "ded/directorWorker/director_worker.h"
#include "ded/npcWorker/npc_worker.h"
#include "dpge/plot_worker.h"
#include "operations/entities/entities.h" 

#include "operations/entity_handling.h"
#include "operations/operator.h"

using namespace operations;

// Constants and parameters
const int ErrorExitStatus = 1;
const string kLoggerName = "startup.cc";

const worker::ComponentRegistry& DirectorComponents() { 

	static const worker::Components<
		DirectorInfo,
		ActorInfo,
		PlotMap,
		EntitySpawning,
		EntityAcl,
		Position,
		Metadata,
		CurrentState,
		Persistence> components;
	return components;
}

const worker::ComponentRegistry& AllComponents( ) {

	static const worker::Components<
		TransformComponent,
		Inventory,
		ConnectionHeartbeat,
		ded::flora::Tree,
		PlayerInfo,
		ActorInfo,
		PlotInfo,
		PlotMap,
		EntitySpawning,
		EntityAcl,
		Position,
		Metadata,
		CurrentState,
		Persistence> components;
	return components;
}


worker::Connection ConnectWithReceptionist( const string hostname,
	const uint16_t port,
	const string& worker_id,
	const worker::ConnectionParameters& connection_parameters ) {
	auto future = worker::Connection::ConnectAsync( NPCComponents{ }, hostname,
		port, worker_id, connection_parameters );

	return future.Get( );
}

template<typename Out>
void split( const string &s, char delim, Out result ) {
	stringstream ss( s );
	string item;
	while ( getline( ss, item, delim ) ) {
		*( result++ ) = item;
	}
}

vector<string> split( const string &s, char delim ) {
	vector<string> elems;
	split( s, delim, back_inserter( elems ) );
	return elems;
}

bool isWorkerSettingsValid( int argc, char** argv ) {
	auto print_usage = [&] ( ) {
		cerr << "argc: " << argc << endl;
		cerr << "argv 0: " << argv[0] << endl;
		cerr << "argv 1: " << argv[1] << endl;
		cerr << "argv 5: " << argv[5] << endl;
		cerr << "argv 6: " << argv[6] << endl;
		cerr << "argv 7: " << argv[7] << endl;
		cerr << "Usage: Managed receptionist <hostname> <port> <worker_id>" << endl;
		cerr << endl;
		cerr << "Connects to SpatialOS" << endl;
		cerr << argv[2] << "    <hostname>      - hostname of the receptionist or locator to connect to.";
		cerr << endl;
		cerr << atoi( argv[3] ) << "    <port>          - port to use if connecting through the receptionist.";
		cerr << endl;
		cerr << argv[4] << "    <worker_id>     - name of the worker assigned by SpatialOS." << endl;
		cerr << endl;
	};
	if ( argc != 9 ) {
		print_usage( );
		return false;
	}
	return true;
}

// Entry point
int main( int argc, char** argv ) {

	if ( !isWorkerSettingsValid( argc, argv ) ) {
		return ErrorExitStatus;
	}
	worker::ConnectionParameters parameters;
	
	parameters.WorkerType = argv[6]; // split( split( argv[0], '\\' ).back( ), '.' ).front( );
	parameters.Network.ConnectionType = worker::NetworkConnectionType::kTcp;
	parameters.Network.UseExternalIp = false;
	// Connect with receptionist
	worker::Connection connection = ConnectWithReceptionist( argv[2], atoi( argv[3] ), argv[4], parameters );

	CoreUtility utility( connection );

	//start threads
	Operator ops;
	thread operations_thread( &Operator::run, &ops, ref( utility ) );
	operations_thread.join( );

	return ErrorExitStatus;
}
