// BASIC_NET.h
#ifndef BASIC_NET_H
#define BASIC_NET_H

namespace dpge {

	class BasicNet {
		const string loggerName = "dpge/basic_net.h";
		const string directory = "C:/PhdThesis/DED/logs/";
		AI::Bayes bayes;

	protected:
		void ConsoleLogErr( const string& msg ) { cerr << loggerName << " - " << msg << endl; }

		void PositionSubmodels( ) {
			//info.position.center_X = 100;
		}

		string FormatName( string name, PlotEnum subnet ) {
			if ( subnet == PlotEnum::VICTIM )
				return PrefixVictim + name;

			if ( IsMurderSubnet( subnet ) )
				return PrefixMurder + name;
			return name;
		}

		string FormatNumberName( string name, int number ) {
			return name + "_" + to_string( number );
		}

		void AddNodes( map<string, Node>& nodes ) {
			try {
				for ( map<string, Node>::iterator it = nodes.begin( ); it != nodes.end( ); ++it )
					CreateNodes( it->second );
			} catch ( exception &e ) {
				string msg = string( "Plot builder EXCEPTION: " ) + string( e.what( ) );
				ConsoleLogErr( msg );
				return;
			} catch ( ... ) {
				ConsoleLogErr( "Plot builder ADD NODE EXCEPTION: " );
				return;
			}
		}

		void CreateNodes( Node node ) {
			//Add a victim node for suspect template
			if ( PLOT_TYPE[node.Subnet( )] == PlotEnum::SUSPECT )
				AddNode( node, FormatName( node.Name( ), PlotEnum::VICTIM ), 
					PLOT_TYPE_NAME[PlotEnum::VICTIM] );

			//add murder node for murder template
			if ( IsMurderSubnet( PLOT_TYPE[node.Subnet( )] ) )
				AddNode( node, FormatName( node.Name( ), 
					PLOT_TYPE[node.Subnet( )] ), 
					PLOT_TYPE_NAME[MurderSubnets[PLOT_TYPE[node.Subnet( )]]] );

			for ( int i = 0; i < NumberOfInstances[PLOT_TYPE[node.Subnet( )]]; i++ )
				AddNode( node, FormatNumberName( node.Name( ), i ), 
					FormatNumberName( PLOT_TYPE_NAME[PLOT_TYPE[node.Subnet( )]], i ) );
		}

		void AddNode( Node node, string name, string subnet ) {
			NODE_NAME[name] = NODE_NAME[node.Name( )];
			PLOT_TYPE[subnet] = PLOT_TYPE[node.Subnet( )];
			node.SetName( name );
			node.SetSubnetName( subnet );
			bayes.AddNode( node );
		}

		void AddArcs( list<Arc>& arcs ) {
			try {
				for ( list<Arc>::iterator it = arcs.begin( ); it != arcs.end( ); ++it )
					AddArcs( *it );
			} catch ( exception &e ) {
				string msg = string( "Plot builder ADD ARC EXCEPTION: " ) 
					+ string( e.what( ) );
				ConsoleLogErr( msg );
				return;
			} catch ( ... ) {
				ConsoleLogErr( "Plot builder ADD ARC UNDEFINED EXCEPTION: " );
				return;
			}
		}

		void AddArcs( Arc arc ) {
			try {
				if ( IsMurderSubnet( PLOT_TYPE[arc.ChildSubnet( )] ) )
					bayes.AddArc( FormatName( arc.Child( ), 
						PLOT_TYPE[arc.ChildSubnet( )] ), 
						FormatName( arc.Parent( ), PLOT_TYPE[arc.ParentSubnet( )] ) );
			} catch ( exception &e ) {
				string msg = string( "Plot builder ADD ARC EXCEPTION: " ) 
					+ string( e.what( ) );
				ConsoleLogErr( msg );
			}

			for ( int i = 0; i < NumberOfInstances[PLOT_TYPE[arc.ChildSubnet( )]]; i++ )
				bayes.AddArc( FormatNumberName( arc.Child( ), i ), 
					FormatNumberName( arc.Parent( ), i ) );
		}

		void AddDefinitions( list<pair<string, PlotEnum>>& totologies ) {
			addTotologies( totologies );
		}

		void addTotologies( list<pair<string, PlotEnum>>& totologies ) {
			for ( list<pair<string, PlotEnum>>::iterator it = totologies.begin( ); it != totologies.end( ); ++it ) {
				pair<string, PlotEnum> pair = *it;

				if ( IsMurderSubnet( pair.second ) ) {
					string name = FormatName( pair.first, pair.second );
					bayes.SetNodeDefinition( name, createTotologyDefinition( name ) );
				}

				for ( int i = 0; i < NumberOfInstances[pair.second]; i++ ) {
					string name = FormatNumberName( pair.first, i );
					bayes.SetNodeDefinition( name, createTotologyDefinition( name ) );
				}
			}
		}

		void VerifyBasicStructure( ) { };

		list<double> createTotologyDefinition( string name ) {
			list<int> parents = bayes.Parents( name );
			list<double> deffinition;
			int numOutcomes = bayes.NumberOfOutcomes( name );
			for ( list<int>::iterator it = parents.begin( ); it != parents.end( ); ++it ) {

				if ( bayes.NumberOfOutcomes( *it ) != numOutcomes ) {
					string msg = "Can't create arc Totology definition for node " + name +
						", parent states are " + to_string( bayes.NumberOfOutcomes( *it ) ) 
						+ " != child states "
						+ to_string( numOutcomes ) + "!";
					ConsoleLogErr( msg );
					throw;
				}
			}

			double size = pow( numOutcomes, parents.size( ) );
			deffinition.push_back( 1 );
			deffinition.push_back( 0 );
			for ( int i = 0; i <= size - numOutcomes; ++i ) {
				for ( int x = 0; x < numOutcomes - 1; ++x )
					deffinition.push_back( 0 );
				deffinition.push_back( 1 );
			}
			if ( numOutcomes * size != deffinition.size( ) )
				ConsoleLogErr( "Size of expected matrix: "
					+ to_string( numOutcomes * size )
					+ ", size of deffinitions: " + to_string( deffinition.size( ) ) );

			return deffinition;
		}

		void SetTotologyDefinitions( list<pair<string, PlotEnum>> totologies ) {
			for ( list<pair<string, PlotEnum>>::iterator it = totologies.begin( );
				it != totologies.end( ); ++it ) {
				pair<string, PlotEnum> pair = *it;

				if ( IsMurderSubnet( pair.second ) ) {
					string name = FormatName( pair.first, pair.second );
					bayes.SetEvidence( name, 0 );
				}

				for ( int i = 0; i < NumberOfInstances[pair.second]; i++ ) {
					string name = FormatNumberName( pair.first, i );
					bayes.SetEvidence( name, 1 );
				}
			}
		}

		void SetAllEvidence( ) {
			bayes.SetAllEvidence( );
		}

		void AllEvidence( worker::Map<PlotEnum, Submodels>& plotMap ) {
			int id = bayes.GetFirstSubmodel( );
			while ( true ) {
				map<string, int> variables;
				bayes.EvidenceVariablesBySubnet( variables, id );
				AddToPlotMapp( plotMap, variables, bayes.GetSubmodelNameById( id ) );
				id = bayes.GetNextSubmodelById( id );
				if ( id < 0 ) return; //-1 indicates that there is no next submodel handler
			}
		}

		void EvidenceBySubnets( worker::Map<PlotEnum, Submodels>& plotMap,
			list<string>& subnets ) {
			for ( list<string>::iterator it = subnets.begin( ); it != subnets.end( ); ++it ) {
				map<string, int> variables;
				const string name = *it;
				bayes.EvidenceVariablesBySubnet( variables, name );
				AddToPlotMapp( plotMap, variables, name );
			}
		}

		void AddToPlotMapp( worker::Map<PlotEnum, Submodels>& plotMap,
			map<string, int>& variables, const string name ) {
			Map< PlotNodeEnum, uint32_t > mappVariables;
			MappVariables( mappVariables, variables );
			Submodel submodel( name, PLOT_TYPE[name], mappVariables );
			plotMap[PLOT_TYPE[name]].submodels( ).emplace_back( submodel );
		}

		void MappVariables( Map< PlotNodeEnum, uint32_t >& mappVariables, 
			map<string, int>& variables ) {
			for ( map<string, int>::iterator it = variables.begin( ); 
				it != variables.end( ); ++it ) 
				mappVariables[NODE_NAME[it->first]] = it->second;
		}		

		void WriteNet( const string& name ) {
			bayes.WriteNet( ( directory + name + ".dsl" ).c_str( ) );
		}
	};
}

#endif // !BASIC_NET_H