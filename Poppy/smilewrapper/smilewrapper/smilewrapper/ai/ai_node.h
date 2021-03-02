// AI_node.h
#ifndef AI_NODE_H
#define AI_NODE_H

using namespace std;

namespace AI {

	class Arc {
		string child, parent, childSubnet, parentSubnet;
	public:

		Arc( ) { };
		static Arc Create( ) { return { }; }

		// Copyable and moveable.
		Arc( Arc&& ) = default;
		Arc( const Arc& ) = default;
		Arc& operator=( Arc&& ) = default;
		Arc& operator=( const Arc& ) = default;
		~Arc( ) = default;

		bool operator==( const Arc& ) const;
		bool operator!=( const Arc& ) const;

		Arc( string _child, string _childSubnet, string _parent, string _parentSubnet ) :
			child( _child ), childSubnet( _childSubnet ), parent( _parent ), parentSubnet( _parentSubnet ) { };

		string Child( ) { return child; };
		string ChildSubnet( ) { return childSubnet; };
		string Parent( ) { return parent; };
		string ParentSubnet( ) { return parentSubnet; };
	};

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

		void Name( string v ) { name = v; }
		void SubnetName( string v ) { subnetName = v; }
		void Subnet( string v ) { subnet = v; }
		string Name( ) { return name; }
		string SubnetName( ) { return subnetName; }
		string Subnet( ) { return subnet; }
		int ID( ) { return id; }
		void ID( int _id ) { id = _id; }
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