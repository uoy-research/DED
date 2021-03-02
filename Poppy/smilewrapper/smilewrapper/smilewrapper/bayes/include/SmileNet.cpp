// SmileNet.cpp : Defines the exported functions for the DLL application.  
// Compile by using: cl /EHsc SmileNet.cpp /link smile.lib  

//#define _ITERATOR_DEBUG_LEVEL 2

#define __STDC_WANT_LIB_EXT1__ 1
#include <string.h>
#include <stdlib.h>
#include <iostream>
#include <random>
#include "smile.h"
#include <unordered_map>

#define DllExport __declspec(dllexport)

extern "C"
{
	int c = 0;
	DSL_network *nets[20];

	DllExport int CreateNet() {
		DSL_network *net = new DSL_network();
		nets[c] = net;
		int idx = c;
		++c;
		return idx;
	}
/*
	DllExport int AddNode(int netID, int idx) {
		std::string s = std::to_string(idx);
		char const* name = s.c_str();
		DSL_network *net = nets[netID];
		return net->AddNode(DSL_CPT, name);
	}

	DllExport int AddNode(int netID, char const* name) {
		return nets[netID]->AddNode(DSL_CPT, name);
	}

	DllExport int GetNetSize(int netID) {
		DSL_idArray nodes; 

		DSL_network *net = nets[netID];
		net->GetAllNodeIds(nodes);
		return nodes.NumItems();
	}
	
	DllExport bool IsNodeInNet(int netID, int nodeID) {
		DSL_network *net = nets[netID];
		DSL_node *node = net->GetNode(nodeID);
		if (node != NULL)
			return true;
		return false;
	}

	DllExport int SetOutcomeIDs(int netID, int nodeID, char *outcomes[], int size) {
		DSL_network *net = nets[netID];

		DSL_stringArray ar;
		for (int i = 0; i < size; i++) {
			char *label = outcomes[i];
			//strcpy_s(label, 4, outcomes[i]);
			ar.Add(label);
		}

		net->GetNode(nodeID)->Definition()->SetNumberOfOutcomes(ar);
		return net->GetNode(nodeID)->Definition()->GetSize();
	}

	DllExport bool SetNodeDefinition(int netID, int nodeID, double states[], int size) {
		DSL_network *net = nets[netID];
		DSL_sysCoordinates theCoordinates(*net->GetNode(nodeID)->Definition());
		for (int i = 0; i < size; i++) {
			theCoordinates.UncheckedValue() = states[i];
			theCoordinates.Next();
		}
		net->GetNode(nodeID)->Definition()->CheckConsistency();
		net->GetNode(nodeID)->Definition()->CheckReadiness();
		bool consistent = net->GetNode(nodeID)->Definition()->ObjectConsistent() == DSL_OBJECT_CONSISTENT;
		bool ready = net->GetNode(nodeID)->Definition()->ObjectReady() == DSL_OBJECT_READY;
		return consistent && ready;
	}

	DllExport int AddArc(int netID, int parent, int child) {
		DSL_network *net = nets[netID];
		return net->AddArc(parent, child);
	}
	
	double randValue() {
		std::random_device rd; // obtain a random number from hardware
		std::mt19937 eng(rd()); // seed the generator
		std::uniform_int_distribution<> distr(0, 100); // define the range
		double rand = distr(eng) / 100.0;
		//std::cout << "the rand value is " << rand << std::endl;
		return rand;
	}

	DllExport void UpdateBeliefs(int netID) {
		DSL_network *net = nets[netID];
		net->ClearAllTargets();
		net->UpdateBeliefs();
	}

	DllExport int Calculate(int netID, int targetID)
	{
		DSL_network *net = nets[netID];
		net->ClearAllTargets();
		net->SetTarget(targetID);
		if (!net->IsRelevanceActive()) net->ActivateRelevance();
		net->UpdateBeliefs();

		DSL_node *node = net->GetNode(targetID);
		DSL_nodeValue *nodeValue = node->Value();

		if (!net->IsTarget(targetID))
		{
			std::cout << "Value is invalid " << std::endl;
			return -1;
		}

		if (!nodeValue->IsValueValid())
		{
			std::cout << "Value is invalid " << std::endl;
			return -2;
		}

		DSL_Dmatrix *m = NULL;
		int res = nodeValue->GetValue(&m);
		if (DSL_OKAY != res)
		{
			std::cout << "Cant get result as array of doubles " << std::endl;
			return -3;
		}

		DSL_doubleArray values = m->GetItems();
		const int imax = values.GetSize();
		double rand = randValue();
		double totalValue = 0;
		for (int i = 0; i < imax; i++)
		{
			double value = values[i];
			totalValue += value;
			if (rand <= totalValue) return i;
		}

		return imax-1;
	}

	DllExport bool setEvidence(int netID, int nodeID, int outcomeIndex) {		
		int res = nets[netID]->GetNode(nodeID)->Value()->SetEvidence(outcomeIndex);
		return (DSL_OKAY != res);
	}
	
	DllExport bool ActivateRelevance(int netID) {
		nets[netID]->ActivateRelevance();
		return nets[netID]->IsRelevanceActive();
	}

	DllExport void WriteNet(int netID, char const* name) {
		nets[netID]->WriteFile(name);
	}
*/
	/*
	int main()
	{
		int idx = CreateNet();
		int size = GetNetSize(idx);
		std::cout << "size is: " << size << " and id is: " << idx << "\n";

		int nodeID = AddNode(idx, "first");
		size = GetNetSize(idx);
		std::cout << "size is: " << size << " and id is: " << idx << " nodeID: " << nodeID << "\n";

		bool isNode = IsNodeInNet(idx, nodeID);
		std::cout << "NodeID: " << nodeID << ", Exists: " << isNode << "\n";

		isNode = IsNodeInNet(idx, 10);
		std::cout << "NodeID: " << nodeID << ", Exists: " << isNode << "\n";

		char *ArrayOfCharPtrs[2];
		ArrayOfCharPtrs[0] = new char[4];
		ArrayOfCharPtrs[1] = new char[4];
		strcpy_s(ArrayOfCharPtrs[0], 4, "DEF");
		strcpy_s(ArrayOfCharPtrs[1], 4, "GHI");

		std::cout << ArrayOfCharPtrs[0] << std::endl;
		std::cout << ArrayOfCharPtrs[1] << std::endl;
		size = SetOutcomeIDs(idx, nodeID, ArrayOfCharPtrs, 2);

		std::cout << "size is: " << size << " and id is: " << idx << " nodeID: " << nodeID << "\n";

		double states[2];
		states[0] = 0.4;
		states[1] = 0.6;

		bool valid = SetNodeDefinition(idx, nodeID, states, 2);
		std::cout << "Net is valid: " << valid << " nodeID: " << nodeID << "\n";

		int parentID = nodeID;
		///===========================================


		nodeID = AddNode(idx, "second");
		size = GetNetSize(idx);
		std::cout << "Size: " << size << " and id is: " << idx << " nodeID: " << nodeID << "\n";

		char *outcomes[3];
		outcomes[0] = new char[4];
		outcomes[1] = new char[4];
		outcomes[2] = new char[4];
		strcpy_s(outcomes[0], 4, "DEF");
		strcpy_s(outcomes[1], 4, "GHI");
		strcpy_s(outcomes[2], 4, "ABF");

		std::cout << outcomes[0] << std::endl;
		std::cout << outcomes[1] << std::endl;
		std::cout << outcomes[2] << std::endl;
		size = SetOutcomeIDs(idx, nodeID, outcomes, 3);
		
		std::cout << "size is: " << size << " and id is: " << idx << " nodeID: " << nodeID << "\n";
		
		AddArc(idx, parentID, nodeID);

		double states2[2*3];
		states2[0] = 0.4;
		states2[1] = 0.4;
		states2[2] = 0.2;
		states2[3] = 0.1;
		states2[4] = 0.3;
		states2[5] = 0.6;

		valid = SetNodeDefinition(idx, nodeID, states2, 2*3); 

		std::cout << "Net is valid: " << valid << " nodeID: " << nodeID << "\n";

		setEvidence(idx, nodeID, 1);

		int res = Calculate(idx, nodeID);
		WriteNet(idx, "C:/improbable/DED/plot/generated/test.dsl");

		std::cout << " nodeID: " << nodeID << ", results " << res << "\n";
		//std::tr1::unordered_map<int, int> m; 
		//for (int n = 0; n<40; ++n)
		//	std::cout << randValue(1,5) << ' '; // generate numbers
	}		
*/	
}
