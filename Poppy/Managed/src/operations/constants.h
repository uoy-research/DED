//constants.h

#ifndef CORE_CONSTANTS_H
#define CORE_CONSTANTS_H

using namespace std;
using namespace chrono_literals;
using namespace worker;
using namespace improbable;
//using namespace dpge;
using namespace chrono;
using namespace improbable::core;
using ::ded::player::PlayerInfo;
using ::ded::director::DirectorInfo;
using ::ded::npc::NPCInfo;
using ::ded::ActorInfo;
using ::ded::EntitySpawning;
using ::ded::CurrentState;
using ::ded::Info;
using ::ded::Gender;
using ::ded::State;
using ::ded::EntityType;
using ::ded::flora::Tree;
using ::dpge::DirectorPlotMap;
using ::dpge::PlotMap;
using ::dpge::PlotEnum;
using ::dpge::Submodels;
using ::dpge::PlotInfo;
using ::dpge::DirectorPlotMapData;
using ::dpge::PlotMapData;
using ::rde::Knowledge;
using ::rde::KnowledgeItem;

namespace operations {

	typedef duration<int, ratio<1>> sec_type;
	const uint32_t kGetOpListTimeoutInMilliseconds = 100;
	const Option<uint32_t> timeoutInMilliseconds = 5000;
	const auto kMainLoopFrequency = 20ms;

	//Converting enum to string

	inline const char* ToString( worker::LogLevel v ) {
		switch ( v ) {
		case LogLevel::kDebug: return "Debug";
		case LogLevel::kInfo:  return "Info";
		case LogLevel::kWarn:  return "Warn";
		case LogLevel::kError: return "Error";
		case LogLevel::kFatal: return "Fatal";
		default:      return "[Unknown worker::LogLevel]";
		}
	}

	inline const char* ToString( worker::Authority v ) {
		switch ( v ) {
		case worker::Authority::kNotAuthoritative: return "Not Authoritative";
		case worker::Authority::kAuthoritative:  return "Authoritative";
		case worker::Authority::kAuthorityLossImminent:  return "Authority Loss Imminent";
		default:      return "[Unknown worker::Authority]";
		}
	}
}


#endif // !CORE_CONSTANTS_H