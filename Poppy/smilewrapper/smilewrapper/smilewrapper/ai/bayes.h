// bayes.h : Defines the interface for Bayesian Network with smile.lib.  


#ifndef BAYES_H
#define BAYES_H

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
#include "bayes/include/smile.h"
#include "AI_node.h"

using namespace std;
namespace AI {

	class Bayes {
		DSL_network net;

	public:

		Bayes( ) { };
		static Bayes Create( ) { return { }; }

		// Copyable and moveable.
		Bayes( Bayes&& ) = default;
		Bayes( const Bayes& ) = default;
		Bayes& operator=( Bayes&& ) = default;
		Bayes& operator=( const Bayes& ) = default;
		~Bayes( ) = default;

		bool operator==( const Bayes& ) const;
		bool operator!=( const Bayes& ) const;

		void AddNode( AI::Node& node );
		int GetNetSize( );
		int GetNumEvidence( );
		bool IsNodeInNet( const int& nodeID );
		int SetOutcomeIDs( const int& nodeID, list<string>& outcomes );
		bool SetNodeDefinition( const string& name, list<double>& states );
		int AddArc( const string& _child, const string& _parent );
		void UpdateAllBeliefs( );
		int Calculate( const int& targetID );
		int SetEvidence( const string& _name, const int& outcomeIndex );
		int SetEvidence( const int& nodeID, const int& outcomeIndex );
		bool ActivateRelevance( );
		void WriteNet( char const* name );
		int FindNode( const string& name );
		list<int> Parents( const string& name );
		int NumberOfOutcomes( const int& nodeID );
		int NumberOfOutcomes( const string& name );
		void SetSubmodel( const int& nodeID, const string& sub );
		void SetAllEvidence( );

		string GetSubmodelNameById( const int& id );
		int GetFirstSubmodel( );
		int GetNextSubmodelById( const int& id );
		void EvidenceVariablesBySubnet( map<string, int>& variables, const int& id );
		void EvidenceVariablesBySubnet( map<string, int>& variables, const string& id );
	};

	bool Bayes::operator==( const Bayes& value ) const {
		return net.GetNumberOfNodes( ) == value.net.GetNumberOfNodes( ) &&
			net.GetNumberOfTargets( ) == value.net.GetNumberOfTargets( );
	}

	bool Bayes::operator!=( const Bayes& value ) const {
		return !operator==( value );
	}

	void Bayes::AddNode( AI::Node& node ) {
		int id = net.AddNode( DSL_CPT, node.Name( ).c_str( ) );
		if ( id < 0 ) throw invalid_argument( "NODE EXISTS : " + node.Name( ) );
		//SetOutcomeIDs( id, node.Outcomes( ) );
		SetSubmodel( id, node.SubnetName( ) );
	}

	int Bayes::GetNetSize( ) {
		return net.GetNumberOfNodes( );
	}

	int Bayes::GetNumEvidence( ) {
		DSL_intArray nodes;
		net.GetAllEvidenceVertexs( nodes );
		return nodes.NumItems( );
	}

	bool Bayes::IsNodeInNet( const int& nodeID ) {
		DSL_node *node = net.GetNode( nodeID );
		if ( node != NULL )
			return true;
		return false;
	}

	int Bayes::SetOutcomeIDs( const int& nodeID, list<string>& outcomes ) {
		DSL_stringArray ar;

		for ( list<string>::iterator it = outcomes.begin( ); it != outcomes.end( ); ++it ) {
			string label = *it;
			ar.Add( label.c_str( ) );
		}

		net.GetNode( nodeID )->Definition( )->SetNumberOfOutcomes( ar );
		return net.GetNode( nodeID )->Definition( )->GetSize( );
	}

	bool Bayes::SetNodeDefinition( const string& _name, list<double>& states ) {

		int nodeID = FindNode( _name );
		DSL_sysCoordinates theCoordinates( *net.GetNode( nodeID )->Definition( ) );

		for ( list<double>::iterator it = states.begin( ); it != states.end( ); ++it ) {
			double value = *it;
			theCoordinates.UncheckedValue( ) = value;
			theCoordinates.Next( );
		}
		net.GetNode( nodeID )->Definition( )->CheckConsistency( );
		net.GetNode( nodeID )->Definition( )->CheckReadiness( );
		bool consistent = net.GetNode( nodeID )->Definition( )->ObjectConsistent( ) == DSL_OBJECT_CONSISTENT;
		bool ready = net.GetNode( nodeID )->Definition( )->ObjectReady( ) == DSL_OBJECT_READY;
		return consistent && ready;
	}

	int Bayes::FindNode( const string& name ) {
		int id = net.FindNode( name.c_str( ) );

		if ( id == NULL ) {
			throw invalid_argument( "Node " + name + " not found in size of net: " + to_string( GetNetSize( ) ) );
		}

		return id;
	}

	int Bayes::AddArc( const string& _child, const string& _parent ) {
		int child = FindNode( _child );
		int parent = FindNode( _parent );
		return net.AddArc( parent, child );
	}

	list<int> Bayes::Parents( const string& name ) {
		int nodeID = FindNode( name );
		DSL_intArray results = net.GetParents( nodeID );
		list<int> parents;
		for ( int i = 0; i < results.NumItems( ); ++i )
			parents.push_back( results[i] );
		return parents;
	}

	int Bayes::NumberOfOutcomes( const int& nodeID ) {
		return net.GetNode( nodeID )->Definition( )->GetNumberOfOutcomes( );
	}

	int Bayes::NumberOfOutcomes( const string& _name ) {
		int nodeID = FindNode( _name );
		return net.GetNode( nodeID )->Definition( )->GetNumberOfOutcomes( );
	}

	void Bayes::SetSubmodel( const int& nodeID, const string& sub ) {
		int subId = 0;
		if ( net.GetSubmodelHandler( ).IsThisIdentifierInUse( sub.c_str( ) ) ) {
			subId = net.GetSubmodelHandler( ).FindSubmodel( (char*) sub.c_str( ) );
		} else {
			subId = net.GetSubmodelHandler( ).CreateSubmodel( DSL_MAIN_SUBMODEL, (char*) sub.c_str( ) );
		}

		net.GetNode( nodeID )->SetSubmodel( subId );
	}

	double randValue( ) {
		random_device rd; // obtain a random number from hardware
		mt19937 eng( rd( ) ); // seed the generator
		uniform_int_distribution<> distr( 0, 100 ); // define the range
		double rand = distr( eng ) / 100.0;
		return rand;
	}

	void Bayes::UpdateAllBeliefs( ) {
		net.ClearAllTargets( );
		net.UpdateBeliefs( );
	}

	int Bayes::Calculate( const int& targetID ) {
		net.ClearAllTargets( );
		net.SetTarget( targetID );
		if ( !net.IsRelevanceActive( ) ) net.ActivateRelevance( );
		net.UpdateBeliefs( );

		DSL_node *node = net.GetNode( targetID );
		DSL_nodeValue *nodeValue = node->Value( );

		if ( !net.IsTarget( targetID ) ) {
			return -1;
		}

		if ( !nodeValue->IsValueValid( ) ) {
			return -2;
		}

		DSL_Dmatrix *m = NULL;
		int res = nodeValue->GetValue( &m );
		if ( DSL_OKAY != res ) {
			return -3;
		}

		DSL_doubleArray values = m->GetItems( );
		const int imax = values.GetSize( );
		double rand = randValue( );
		double totalValue = 0;
		for ( int i = 0; i < imax; i++ ) {
			double value = values[i];
			totalValue += value;
			if ( rand <= totalValue ) return i;
		}

		return imax - 1;
	}

	int Bayes::SetEvidence( const string& _name, const int& outcomeIndex ) {
		return SetEvidence( FindNode( _name ), outcomeIndex );
	}

	int Bayes::SetEvidence( const int& nodeID, const int& outcomeIndex ) {
		if ( net.GetNode( nodeID )->Value( )->IsEvidence( ) ) return 1;
		int res = net.GetNode( nodeID )->Value( )->SetEvidence( outcomeIndex );
		return net.GetNode( nodeID )->Value( )->IsEvidence( );
	}

	bool Bayes::ActivateRelevance( ) {
		net.ActivateRelevance( );
		return net.IsRelevanceActive( );
	}

	void Bayes::SetAllEvidence( ) {
		UpdateAllBeliefs( );
		int nodeID = net.GetFirstNode( );
		int lastNodeID = net.GetLastNode( );
		while ( true ) {
			int idx = Calculate( nodeID );
			int valid = SetEvidence( nodeID, idx );
			if ( !valid )
				cerr << "Failed to set evidence too " << nodeID << " valid " << valid << endl;
			if ( nodeID == lastNodeID ) break;
			nodeID = net.GetNextNode( nodeID );
		}
	}

	void Bayes::EvidenceVariablesBySubnet( map<string, int>& variables, const string& name ) {
		if ( net.GetSubmodelHandler( ).IsThisIdentifierInUse( name.c_str( ) ) ) {
			int id = net.GetSubmodelHandler( ).FindSubmodel( (char*) name.c_str( ) );
			EvidenceVariablesBySubnet( variables, id );
		} else
			cerr << "Failed to find submodel " << name << endl;
	}

	void Bayes::EvidenceVariablesBySubnet( map<string, int>& variables, const int& id ) {

		DSL_intArray nodeIds;
		net.GetSubmodelHandler( ).GetIncludedNodes( id, nodeIds );
		if ( nodeIds.NumItems( ) == 0 ) return;

		for ( int i = 0; i < nodeIds.NumItems( ); i++ ) {
			int nodeId = nodeIds[i];
			DSL_node *node = net.GetNode( nodeId );
			if ( !node->Value( )->IsEvidence( ) ) continue;
			const char *name = node->Info( ).Header( ).GetName( );
			int evidenceId = node->Value( )->GetEvidence( );
			variables[name] = evidenceId;
		}
	}

	int Bayes::GetFirstSubmodel( ) {
		return net.GetSubmodelHandler( ).GetFirstSubmodel( );
	}

	int Bayes::GetNextSubmodelById( const int& id ) {
		return net.GetSubmodelHandler( ).GetNextSubmodel( id );
	}

	string Bayes::GetSubmodelNameById( const int& id ) {
		return net.GetSubmodelHandler( ).GetSubmodel( id )->header.GetName( );
	}

	void Bayes::WriteNet( char const* name ) {
		net.WriteFile( name );
	}
}

#endif // !BAYES_H
