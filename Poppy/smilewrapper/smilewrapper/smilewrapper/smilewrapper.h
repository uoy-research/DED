#pragma once
// smilewrapper.h - wrapps the smile library
#pragma once

#ifdef SMILEWRAPPER_EXPORTS
#define SMILEWRAPPER_API __declspec(dllexport)
#else
#define SMILEWRAPPER_API __declspec(dllimport)
#endif


extern "C" SMILEWRAPPER_API void network_init();


//void AddNode(AI::Node& node);
extern "C" SMILEWRAPPER_API int GetNetSize();
extern "C" SMILEWRAPPER_API int GetNumEvidence();
extern "C" SMILEWRAPPER_API bool IsNodeInNet(const int& nodeID);
//int SetOutcomeIDs(const int& nodeID, list<string>& outcomes);
//bool SetNodeDefinition(const string& name, list<double>& states);
//int AddArc(const string& _child, const string& _parent);
extern "C" SMILEWRAPPER_API void UpdateAllBeliefs();
extern "C" SMILEWRAPPER_API int Calculate(const int& targetID);
//int SetEvidence(const string& _name, const int& outcomeIndex);
extern "C" SMILEWRAPPER_API int SetEvidence(const int& nodeID, const int& outcomeIndex);
extern "C" SMILEWRAPPER_API bool ActivateRelevance();
//void WriteNet(char const* name);
//int FindNode(const string& name);
//list<int> Parents(const string& name);
extern "C" SMILEWRAPPER_API int NumberOfOutcomes(const int& nodeID);
//int NumberOfOutcomes(const string& name);
//void SetSubmodel(const int& nodeID, const string& sub);
extern "C" SMILEWRAPPER_API void SetAllEvidence();

//string GetSubmodelNameById(const int& id);
extern "C" SMILEWRAPPER_API int GetFirstSubmodel();
extern "C" SMILEWRAPPER_API int GetNextSubmodelById(const int& id);
//void EvidenceVariablesBySubnet(map<string, int>& variables, const int& id);
//void EvidenceVariablesBySubnet(map<string, int>& variables, const string& id);
