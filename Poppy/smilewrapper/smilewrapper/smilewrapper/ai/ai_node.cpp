// AI_node.cpp

#include "ai_node.h"

namespace AI {

	bool Arc::operator==( const Arc& value ) const {
		return child == value.child &&
			parent == value.parent &&
			childSubnet == value.childSubnet &&
			parentSubnet == value.parentSubnet;
	}

	bool Arc::operator!=( const Arc& value ) const {
		return !operator==( value );
	}

	class Node {
		list<Node> parents;
		list<Node> children;

		list<double> states;
		list<string> outcomes;
		string name, subnetName;
		string subnet;

		int id = -1;

	public:

		Node( ) { };
		static Node Create( ) { return { }; }

		// Copyable and moveable.
		Node( Node&& ) = default;
		Node( const Node& ) = default;
		Node& operator=( Node&& ) = default;
		Node& operator=( const Node& ) = default;
		~Node( ) = default;

		bool operator==( const Node& ) const;
		bool operator!=( const Node& ) const;

		Node( string _name, list<string> _outcomes, string _subnet ) {
			name = _name;
			outcomes = _outcomes;
			subnet = _subnet;
			subnetName = _subnet;
		}

		void SetName( string v ) { name = v; }
		void SetSubnetName( string v ) { subnetName = v; }
		void SetSubnet( string v ) { subnet = v; }
		string Name( ) { return name; }
		string SubnetName( ) { return subnetName; }
		string Subnet( ) { return subnet; }
		int ID( ) { return id; }
		void SetID( int _id ) { id = _id; }
		list<string> Outcomes( ) { return outcomes; }
		list<Node> Parents( ) { return parents; }
		list<Node> Children( ) { return children; }
		list<double> States( ) { return states; }
	};

	bool Node::operator==( const Node& value ) const {
		return parents == value.parents && 
			children == value.children &&
			states == value.states &&
			outcomes == value.outcomes &&
			name == value.name &&
			subnetName == value.subnetName &&
			subnet == value.subnet;
	}

	bool Node::operator!=( const Node& value ) const {
		return !operator==( value );
	}
}

#endif // !AI_NODE_H