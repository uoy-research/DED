// plot.h
#ifndef PLOT_H
#define PLOT_H

namespace dpge {

	class MurderPlot : public BasicNet {
		bool isPlot = false;
		const string loggerName = "dpge/plot.h";
		void ConsoleLogErr( const string& msg ) { cerr << loggerName << " - " << msg << endl; }
	public:

		void GeneratePlot( ) {
			Settings settings;
			AddNodes( settings.Nodes( ) );
			PositionSubmodels( );
			WriteNet( "plotNodes" );
			AddArcs( settings.InitialPlotArcs( ) );
			WriteNet( "plotArcs" );
			AddDefinitions( settings.TotalogyDefinitions( ) );
			VerifyBasicStructure( );
			WriteNet( "plotDef" );
			SetThePlot( settings.TotalogyDefinitions( ) );
			isPlot = true;
		}

		bool IsPlot( ) { return isPlot; }

		Map<PlotEnum, Submodels> plotView( ) {
			Map<PlotEnum, Submodels> plotMap;
			AllEvidence( plotMap );
			return plotMap;
		}

		Map<PlotEnum, Submodels> VictimsView( ) {
			list<string> subnets = { PLOT_TYPE_NAME[PlotEnum::VICTIM] };
			Map<PlotEnum, Submodels> plotMap;
			EvidenceBySubnets( plotMap, subnets );
			return plotMap;
		}

		Map<PlotEnum, Submodels> MurdererView( ) {
			list<string> subnets = {
				PLOT_TYPE_NAME[PlotEnum::VICTIM],
				PLOT_TYPE_NAME[PlotEnum::MURDER_WEAPON],
				PLOT_TYPE_NAME[PlotEnum::MURDER_SCENE],
				PLOT_TYPE_NAME[PlotEnum::MURDERER],
				PLOT_TYPE_NAME[PlotEnum::OPPORTUNITY]
			};
			Map<PlotEnum, Submodels> plotMap;
			EvidenceBySubnets( plotMap, subnets );
			return plotMap;
		}

		Map<PlotEnum, Submodels> SuspectsView( int num ) {
			list<string> subnets = { PLOT_TYPE_NAME[PlotEnum::SUSPECT] + "_" + to_string( num ) };
			Map<PlotEnum, Submodels> plotMap;
			EvidenceBySubnets( plotMap, subnets );
			return plotMap;
		}

		Map<PlotEnum, Submodels> PlayersView( ) {
			list<string> subnets = { PLOT_TYPE_NAME[PlotEnum::SUSPECT] + "_" + to_string( 0 ) };
			Map<PlotEnum, Submodels> plotMap;
			EvidenceBySubnets( plotMap, subnets );
			return plotMap;
		}

	private:

		void SetThePlot( list<pair<string, PlotEnum>> totologies ) {
			SetTotologyDefinitions( totologies );
			SetAllEvidence( );
		}
	};
}

#endif // !PLOT_H