// smilewrapper.cpp : Defines the exported functions for the DLL application.

#include "stdafx.h"
#include "smilewrapper.h"
#include "ai/bayes.h"


AI::Bayes net;

void network_init() {}




//void AddNode(AI::Node& node);
int GetNetSize() { return net.GetNetSize();  }

int GetNumEvidence() { return net.GetNumEvidence(); }
bool IsNodeInNet(const int& nodeID) { return net.IsNodeInNet(nodeID); }
//int SetOutcomeIDs(const int& nodeID, list<string>& outcomes);
//bool SetNodeDefinition(const string& name, list<double>& states);
//int AddArc(const string& _child, const string& _parent);
void UpdateAllBeliefs() { net.UpdateAllBeliefs(); }
int Calculate(const int& targetID) { return net.Calculate(targetID); }
//int SetEvidence(const string& _name, const int& outcomeIndex);
int SetEvidence(const int& nodeID, const int& outcomeIndex) { return net.SetEvidence(nodeID, outcomeIndex); }
bool ActivateRelevance() { return net.ActivateRelevance(); }
//void WriteNet(char const* name);
//int FindNode(const string& name);
//list<int> Parents(const string& name);
int NumberOfOutcomes(const int& nodeID) { return net.NumberOfOutcomes(nodeID); }
//int NumberOfOutcomes(const string& name);
//void SetSubmodel(const int& nodeID, const string& sub);
void SetAllEvidence() { net.SetAllEvidence(); }

//string GetSubmodelNameById(const int& id);
int GetFirstSubmodel() { return net.GetFirstSubmodel(); }
int GetNextSubmodelById(const int& id) { return net.GetNextSubmodelById(id); }
//void EvidenceVariablesBySubnet(map<string, int>& variables, const int& id);
//void EvidenceVariablesBySubnet(map<string, int>& variables, const string& id);
