// ded_enum_maps.h
#ifndef DED_ENUM_MAPS_H
#define DED_ENUM_MAPS_H


namespace operations {

	namespace entities {
		
		map<State, string> States = {
			{ State::MISSING, "Missing" },
			{ State::FIRST, "First" },
			{ State::SECOND, "Second" },
			{ State::THIRD, "Third" },
			{ State::FOURTH, "Fourth" },
			{ State::FIFTH, "Fifth" },
			{ State::DIRECT, "Direct" },
			{ State::ACT, "Act" },
			{ State::DRAMA, "Drama" },
			{ State::NOT_AUTHORITATIVE, "Not Authoritative!" },
			{ State::BUILD_KNOWLEDGE_BASE, "Build Knowledge Base" },
			{ State::CREATE_INFO_MAP, "Create Info Map" },
			{ State::CREATE_ACTORS, "Create actors" },
			{ State::CREATE_PLOT, "Create plot" },
			{ State::CREATE_DIRECTOR, "Create director" },
			{ State::CHECK_ACTORS_CREATED, "Check actor created" },
			{ State::CHECK_DIRECTOR_CREATED, "Check director created" },
			{ State::CHECK_PLOT_CREATED, "Check plot created" },
			{ State::ASSIGN_PLOT, "Assign Plot" },
			{ State::SEND_IS_CREATED, "Send is Created" },
			{ State::WAIT_FOR_ACK, "Waiting for acknowledgment" }
		};

		string ToString( const State& state ) {
			return States[state];
		}
	}
}


#endif // !DED_ENUM_MAPS_H
