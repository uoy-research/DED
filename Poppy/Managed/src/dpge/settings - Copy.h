// settings.h
#ifndef SETTINGS_H
#define SETTINGS_H

using namespace AI;

namespace dpge {

	//
	//map<EvidenceEnum, string> evidenceNameEnum;
	//map<string, EvidenceEnum> evidenceName;

	//evidenceNameEnum[OBJECT] = "";
	//evidenceName[""] = OBJECT;
	//evidenceNameEnum[CHARACTER] = "";
	//evidenceName[""] = CHARACTER;
	//evidenceNameEnum[WEAPON] = "";
	//evidenceName[""] = WEAPON;
	//evidenceNameEnum[REACTION] = "";
	//evidenceName[""] = REACTION;
	//evidenceNameEnum[OPPORTUNITY] = "";
	//evidenceName[""] = OPPORTUNITY;
	   
	
/*	nodeNameEnum[PlotNodeEnum::WEAPON_CONTAINER] = "Weapon_container";
	nodeName["Weapon_container"] = dpge::WEAPON_CONTAINER;
	nodeNameEnum[dpge::WEAPON_CONTAINED_IN_SUSPECT] = "Weapon_contained_in_suspect";
	nodeName["Weapon_contained_in_suspect"] = dpge::WEAPON_CONTAINED_IN_SUSPECT;
	nodeNameEnum[dpge::WEAPON_CONTAINED_IN_OBJECT] = "Weapon_contained_in_object";
	nodeName["Weapon_contained_in_object"] = dpge::WEAPON_CONTAINED_IN_OBJECT;
	nodeNameEnum[dpge::WEAPON_MARKS] = "Weapon_marks";
	nodeName["Weapon_marks"] = dpge::WEAPON_MARKS;
	nodeNameEnum[dpge::WEAPON_MEATHOD] = "Weapon_meathod";
	nodeName["Weapon_meathod"] = dpge::WEAPON_MEATHOD;
	nodeNameEnum[dpge::WEAPON_MATERIAL] = "Weapon_material";
	nodeName["Weapon_material"] = dpge::WEAPON_MATERIAL;
	nodeNameEnum[dpge::WEAPON_NEEDS_STRENGTH] = "Weapon_needs_strength";
	nodeName["Weapon_needs_strength"] = dpge::WEAPON_NEEDS_STRENGTH;
	nodeNameEnum[dpge::WEAPON_WOUND_TYPE] = "Weapon_wound_type";
	nodeName["Weapon_wound_type"] = dpge::WEAPON_WOUND_TYPE;
	nodeNameEnum[dpge::VICTIM_WOUND_TYPE] = "Victim_wound_type";
	nodeName["Victim_wound_type"] = dpge::VICTIM_WOUND_TYPE;
	nodeNameEnum[dpge::IS_MURDER_WEAPON] = "Is_murder_weapon";
	nodeName["Is_murder_weapon"] = dpge::IS_MURDER_WEAPON;
	nodeNameEnum[dpge::BLOOD_ON_WEAPON] = "blood_on_weapon";
	nodeName["blood_on_weapon"] = dpge::BLOOD_ON_WEAPON;
	nodeNameEnum[dpge::HAIR_ON_WEAPON] = "Hair_on_weapon";
	nodeName["Hair_on_weapon"] = dpge::HAIR_ON_WEAPON;
	nodeNameEnum[dpge::HAIR_ON_VICTIM] = "Hair_on_victim";
	nodeName["Hair_on_victim"] = dpge::HAIR_ON_VICTIM;
	nodeNameEnum[dpge::IS_STRENGTH] = "Is_strength";
	nodeName["Is_strength"] = dpge::IS_STRENGTH;
	nodeNameEnum[dpge::IS_MARKS] = "Is_marks";
	nodeName["Is_marks"] = dpge::IS_MARKS;
	nodeNameEnum[dpge::IS_MATERIAL] = "Is_material";
	nodeName["Is_material"] = dpge::IS_MATERIAL;
	nodeNameEnum[dpge::IS_WOUND] = "Is_wound";
	nodeName["Is_wound"] = dpge::IS_WOUND;
	nodeNameEnum[dpge::IS_WEAPONS_HAIR] = "Is_weapons_hair";
	nodeName["Is_weapons_hair"] = dpge::IS_WEAPONS_HAIR;
	nodeNameEnum[dpge::IS_BLOOD] = "Is_blood";
	nodeName["Is_blood"] = dpge::IS_BLOOD;
	nodeNameEnum[dpge::WEAPONS_HAIR_COLOUR] = "Weapons_hair_colour";
	nodeName["Weapons_hair_colour"] = dpge::WEAPONS_HAIR_COLOUR;
	nodeNameEnum[dpge::SCENE_ROOM] = "Scene_room";
	nodeName["Scene_room"] = dpge::SCENE_ROOM;
	nodeNameEnum[dpge::SCENE_SHOE_PRINT] = "Scene_shoe_print";
	nodeName["Scene_shoe_print"] = dpge::SCENE_SHOE_PRINT;
	nodeNameEnum[dpge::SCENE_GLASS] = "Scene_glass";
	nodeName["Scene_glass"] = dpge::SCENE_GLASS;
	nodeNameEnum[dpge::HAIR_ON_SCENE] = "Hair_on_scene";
	nodeName["Hair_on_scene"] = dpge::HAIR_ON_SCENE;
	nodeNameEnum[dpge::OBJECT_NAME] = "Object_name";
	nodeName["Object_name"] = dpge::OBJECT_NAME;
	nodeNameEnum[dpge::OBJECT_POSITION] = "Object_position";
	nodeName["Object_position"] = dpge::OBJECT_POSITION;
	nodeNameEnum[dpge::OBJECT_IS_CONTAINER] = "Object_is_container";
	nodeName["Object_is_container"] = dpge::OBJECT_IS_CONTAINER;
	nodeNameEnum[dpge::OBJECT_IS_BREAKABLE] = "Object_is_breakable";
	nodeName["Object_is_breakable"] = dpge::OBJECT_IS_BREAKABLE;
	nodeNameEnum[dpge::OBJECT_GLASS] = "Object_glass";
	nodeName["Object_glass"] = dpge::OBJECT_GLASS;
	nodeNameEnum[dpge::OBJECT_LOCKED] = "Object_locked";
	nodeName["Object_locked"] = dpge::OBJECT_LOCKED;
	nodeNameEnum[dpge::OBJECT_SIZE] = "Object_size";
	nodeName["Object_size"] = dpge::OBJECT_SIZE;
	nodeNameEnum[dpge::OBJECT_LOCK] = "Object_lock";
	nodeName["Object_lock"] = dpge::OBJECT_LOCK;
	nodeNameEnum[dpge::OBJECT_BROKEN] = "Object_broken";
	nodeName["Object_broken"] = dpge::OBJECT_BROKEN;
	nodeNameEnum[dpge::OBJECT_LIST_OF_SUSPECTS] = "Object_list_of_suspects";
	nodeName["Object_list_of_suspects"] = dpge::OBJECT_LIST_OF_SUSPECTS;
	nodeNameEnum[dpge::OBJECT_KEY_IN_LOCK] = "Object_key_in_lock";
	nodeName["Object_key_in_lock"] = dpge::OBJECT_KEY_IN_LOCK;
	nodeNameEnum[dpge::SUSPECT_SEX] = "Suspect_sex";
	nodeName["Suspect_sex"] = dpge::SUSPECT_SEX;
	nodeNameEnum[dpge::SUSPECT_OCCUPATION] = "Suspect_occupation";
	nodeName["Suspect_occupation"] = dpge::SUSPECT_OCCUPATION;
	nodeNameEnum[dpge::SUSPECT_STRENGTH] = "Suspect_strength";
	nodeName["Suspect_strength"] = dpge::SUSPECT_STRENGTH;
	nodeNameEnum[dpge::SUSPECT_SIZE] = "Suspect_size";
	nodeName["Suspect_size"] = dpge::SUSPECT_SIZE;
	nodeNameEnum[dpge::SUSPECT_SHOE_SIZE] = "Suspect_shoe_size";
	nodeName["Suspect_shoe_size"] = dpge::SUSPECT_SHOE_SIZE;
	nodeNameEnum[dpge::SUSPECT_IS_SUFFICIENT_STRENGTH] = "Suspect_is_sufficient_strength";
	nodeName["Suspect_is_sufficient_strength"] = dpge::SUSPECT_IS_SUFFICIENT_STRENGTH;
	nodeNameEnum[dpge::SUSPECT_IS_SHOE_PRINT_ON_SCENE] = "Suspect_is_shoe_print_on_scene";
	nodeName["Suspect_is_shoe_print_on_scene"] = dpge::SUSPECT_IS_SHOE_PRINT_ON_SCENE;
	nodeNameEnum[dpge::SUSPECT_HAIR_COLOUR] = "Suspect_hair_colour";
	nodeName["Suspect_hair_colour"] = dpge::SUSPECT_HAIR_COLOUR;
	nodeNameEnum[dpge::SUSPECT_IS_HAIR_FOUND_ON_SCENE] = "Suspect_is_hair_found_on_scene";
	nodeName["Suspect_is_hair_found_on_scene"] = dpge::SUSPECT_IS_HAIR_FOUND_ON_SCENE;
	nodeNameEnum[dpge::SUSPECT_IS_HAIR_FOUND_ON_VICTIM] = "Suspect_is_hair_found_on_victim";
	nodeName["Suspect_is_hair_found_on_victim"] = dpge::SUSPECT_IS_HAIR_FOUND_ON_VICTIM;
	nodeNameEnum[dpge::IS_SUSPECT_MURDERER] = "Is_suspect_murderer";
	nodeName["Is_suspect_murderer"] = dpge::IS_SUSPECT_MURDERER;
	nodeNameEnum[dpge::VICTIM_RELATION] = "Victim_relation";
	nodeName["Victim_relation"] = dpge::VICTIM_RELATION;
	nodeNameEnum[dpge::SUSPECT_HEIR] = "Suspect_heir";
	nodeName["Suspect_heir"] = dpge::SUSPECT_HEIR;
	nodeNameEnum[dpge::SUSPECT_RICH] = "Suspect_rich";
	nodeName["Suspect_rich"] = dpge::SUSPECT_RICH;
	nodeNameEnum[dpge::SUSPECT_IN_DEBT] = "Suspect_in_debt";
	nodeName["Suspect_in_debt"] = dpge::SUSPECT_IN_DEBT;
	nodeNameEnum[dpge::SUSPECT_HAS_DARK_SECRET] = "Suspect_has_dark_secret";
	nodeName["Suspect_has_dark_secret"] = dpge::SUSPECT_HAS_DARK_SECRET;
	nodeNameEnum[dpge::VICTIM_KNOWS_OF_DARK_SECRET] = "Victim_knows_of_dark_secret";
	nodeName["Victim_knows_of_dark_secret"] = dpge::VICTIM_KNOWS_OF_DARK_SECRET;
	nodeNameEnum[dpge::SUSPECT_KNOWS_OF_DARK_SECRET] = "Suspect_knows_of_dark_secret";
	nodeName["Suspect_knows_of_dark_secret"] = dpge::SUSPECT_KNOWS_OF_DARK_SECRET;
	nodeNameEnum[dpge::HARMED_SUSPECT_IN_PAST] = "Harmed_suspect_in_past";
	nodeName["Harmed_suspect_in_past"] = dpge::HARMED_SUSPECT_IN_PAST;
	nodeNameEnum[dpge::VICTIM_THREATENED_TO_EXPOSE_SWINDLE] = "Victim_threatened_to_expose_swindle";
	nodeName["Victim_threatened_to_expose_swindle"] = dpge::VICTIM_THREATENED_TO_EXPOSE_SWINDLE;
	nodeNameEnum[dpge::VICTIM_THREATENED_TO_EXPOSE_BLACKMAIL] = "Victim_threatened_to_expose_blackmail";
	nodeName["Victim_threatened_to_expose_blackmail"] = dpge::VICTIM_THREATENED_TO_EXPOSE_BLACKMAIL;
	nodeNameEnum[dpge::SUSPECT_LOST_A_FORTUNE] = "Suspect_lost_a_fortune";
	nodeName["Suspect_lost_a_fortune"] = dpge::SUSPECT_LOST_A_FORTUNE;
	nodeNameEnum[dpge::SUSPECT_INHERITANCE] = "Suspect_inheritance";
	nodeName["Suspect_inheritance"] = dpge::SUSPECT_INHERITANCE;
	nodeNameEnum[dpge::SUSPECT_IS_BLACKMAILER] = "Suspect_is_blackmailer";
	nodeName["Suspect_is_blackmailer"] = dpge::SUSPECT_IS_BLACKMAILER;
	nodeNameEnum[dpge::SUSPECT_IS_WEDLOCKED] = "Suspect_is_wedlocked";
	nodeName["Suspect_is_wedlocked"] = dpge::SUSPECT_IS_WEDLOCKED;
	nodeNameEnum[dpge::SUSPECT_IS_SWINDLER] = "Suspect_is_swindler";
	nodeName["Suspect_is_swindler"] = dpge::SUSPECT_IS_SWINDLER;
	nodeNameEnum[dpge::SUSPECT_IS_ADULTERER] = "Suspect_is_adulterer";
	nodeName["Suspect_is_adulterer"] = dpge::SUSPECT_IS_ADULTERER;
	nodeNameEnum[dpge::VICTIM_IS_ADULTERER] = "Victim_is_adulterer";
	nodeName["Victim_is_adulterer"] = dpge::VICTIM_IS_ADULTERER;
	nodeNameEnum[dpge::SUSPECT_IS_SWINDLED] = "Suspect_is_swindled";
	nodeName["Suspect_is_swindled"] = dpge::SUSPECT_IS_SWINDLED;
	nodeNameEnum[dpge::SUSPECT_WANTED_REVENGE] = "Suspect_wanted_revenge";
	nodeName["Suspect_wanted_revenge"] = dpge::SUSPECT_WANTED_REVENGE;
	nodeNameEnum[dpge::SUSPECT_WAS_BLACKMAILED] = "Suspect_was_blackmailed";
	nodeName["Suspect_was_blackmailed"] = dpge::SUSPECT_WAS_BLACKMAILED;
	nodeNameEnum[dpge::SUSPECT_HAS_MOTIVE] = "Suspect_has_motive";
	nodeName["Suspect_has_motive"] = dpge::SUSPECT_HAS_MOTIVE;
	nodeNameEnum[dpge::SUSPECT_HAS_OPPORTUNITY] = "Suspect_has_opportunity";
	nodeName["Suspect_has_opportunity"] = dpge::SUSPECT_HAS_OPPORTUNITY;
	nodeNameEnum[dpge::SUSPECT_HAS_MEANS] = "Suspect_has_means";
	nodeName["Suspect_has_means"] = dpge::SUSPECT_HAS_MEANS;
	nodeNameEnum[dpge::SUSPECT_IS_MURDERER] = "Suspect_is_murderer";
	nodeName["Suspect_is_murderer"] = dpge::SUSPECT_IS_MURDERER;
	nodeNameEnum[dpge::GREET] = "Greet";
	nodeName["Greet"] = dpge::GREET;
	nodeNameEnum[dpge::PARTY] = "Party";
	nodeName["Party"] = dpge::PARTY;
	nodeNameEnum[dpge::INQUIRE] = "Inquire";
	nodeName["Inquire"] = dpge::INQUIRE;
	nodeNameEnum[dpge::INTRODUCTION] = "Introduction";
	nodeName["Introduction"] = dpge::INTRODUCTION;
	nodeNameEnum[dpge::DRINK] = "Drink";
	nodeName["Drink"] = dpge::DRINK;
	nodeNameEnum[dpge::SEAT] = "Seat";
	nodeName["Seat"] = dpge::SEAT;
	nodeNameEnum[dpge::REACTION_LOOK_AT] = "Reaction_look_at";
	nodeName["Reaction_look_at"] = dpge::REACTION_LOOK_AT;
	nodeNameEnum[dpge::REACTION_REACT] = "Reaction_react";
	nodeName["Reaction_react"] = dpge::REACTION_REACT;

	map<PlotNodeLabelEnum, string> labelNameEnum;
	map<string, PlotNodeLabelEnum> labelName;
	labelNameEnum[dpge::SUSPECTS] = "Suspects";
	labelName["Suspects"] = dpge::SUSPECTS;
	labelNameEnum[dpge::MATCH] = "Match";
	labelName["Match"] = dpge::MATCH;
	labelNameEnum[dpge::BROWN] = "Brown";
	labelName["Brown"] = dpge::BROWN;
	labelNameEnum[dpge::BULLETHOLES] = "Bulletholes";
	labelName["Bulletholes"] = dpge::BULLETHOLES;
	labelNameEnum[dpge::PARENT] = "Parent";
	labelName["Parent"] = dpge::PARENT;
	labelNameEnum[dpge::SHELF] = "Shelf";
	labelName["Shelf"] = dpge::SHELF;
	labelNameEnum[dpge::OBJECT] = "Object";
	labelName["Object"] = dpge::OBJECT;
	labelNameEnum[dpge::WEAK] = "Weak";
	labelName["Weak"] = dpge::WEAK;
	labelNameEnum[dpge::ON_THE_FLOOR] = "On the floor";
	labelName["On the floor"] = dpge::ON_THE_FLOOR;
	labelNameEnum[dpge::NO_MATCH] = "No match";
	labelName["No match"] = dpge::NO_MATCH;
	labelNameEnum[dpge::SINK] = "Sink";
	labelName["Sink"] = dpge::SINK;
	labelNameEnum[dpge::HANDS] = "Hands";
	labelName["Hands"] = dpge::HANDS;
	labelNameEnum[dpge::SPANNER] = "Spanner";
	labelName["Spanner"] = dpge::SPANNER;
	labelNameEnum[dpge::STRONG] = "Strong";
	labelName["Strong"] = dpge::STRONG;
	labelNameEnum[dpge::KEY_LOCK] = "Key lock";
	labelName["Key lock"] = dpge::KEY_LOCK;
	labelNameEnum[dpge::GP] = "GP";
	labelName["GP"] = dpge::GP;
	labelNameEnum[dpge::BLUDGEONED] = "Bludgeoned";
	labelName["Bludgeoned"] = dpge::BLUDGEONED;
	labelNameEnum[dpge::LARGE] = "Large";
	labelName["Large"] = dpge::LARGE;
	labelNameEnum[dpge::TALL] = "Tall";
	labelName["Tall"] = dpge::TALL;
	labelNameEnum[dpge::STEAL] = "Steal";
	labelName["Steal"] = dpge::STEAL;
	labelNameEnum[dpge::ADULTERER] = "Adulterer";
	labelName["Adulterer"] = dpge::ADULTERER;
	labelNameEnum[dpge::LOUNGE] = "Lounge";
	labelName["Lounge"] = dpge::LOUNGE;
	labelNameEnum[dpge::FALSE] = "False";
	labelName["False"] = dpge::FALSE;
	labelNameEnum[dpge::ROBEMARKS] = "RobeMarks";
	labelName["RobeMarks"] = dpge::ROBEMARKS;
	labelNameEnum[dpge::TEACHER] = "Teacher";
	labelName["Teacher"] = dpge::TEACHER;
	labelNameEnum[dpge::BLOND] = "Blond";
	labelName["Blond"] = dpge::BLOND;
	labelNameEnum[dpge::HALL] = "Hall";
	labelName["Hall"] = dpge::HALL;
	labelNameEnum[dpge::SUSPECT] = "Suspect";
	labelName["Suspect"] = dpge::SUSPECT;
	labelNameEnum[dpge::FILINGCABINET] = "FilingCabinet";
	labelName["FilingCabinet"] = dpge::FILINGCABINET;
	labelNameEnum[dpge::ROBE] = "Robe";
	labelName["Robe"] = dpge::ROBE;
	labelNameEnum[dpge::SMELL] = "Smell";
	labelName["Smell"] = dpge::SMELL;
	labelNameEnum[dpge::RED] = "Red";
	labelName["Red"] = dpge::RED;
	labelNameEnum[dpge::MEDIUM] = "Medium";
	labelName["Medium"] = dpge::MEDIUM;
	labelNameEnum[dpge::DOOR] = "Door";
	labelName["Door"] = dpge::DOOR;
	labelNameEnum[dpge::STRING] = "String";
	labelName["String"] = dpge::STRING;
	labelNameEnum[dpge::WATER] = "Water";
	labelName["Water"] = dpge::WATER;
	labelNameEnum[dpge::NON_KEY_LOCK] = "Non key lock";
	labelName["Non key lock"] = dpge::NON_KEY_LOCK;
	labelNameEnum[dpge::ARSENIC] = "Arsenic";
	labelName["Arsenic"] = dpge::ARSENIC;
	labelNameEnum[dpge::STRANGLED] = "Strangled";
	labelName["Strangled"] = dpge::STRANGLED;
	labelNameEnum[dpge::TRUE] = "True";
	labelName["True"] = dpge::TRUE;
	labelNameEnum[dpge::KITCHEN] = "Kitchen";
	labelName["Kitchen"] = dpge::KITCHEN;
	labelNameEnum[dpge::NONE] = "None";
	labelName["None"] = dpge::NONE;
	labelNameEnum[dpge::FEMALE] = "Female";
	labelName["Female"] = dpge::FEMALE;
	labelNameEnum[dpge::INSIDE] = "Inside";
	labelName["Inside"] = dpge::INSIDE;
	labelNameEnum[dpge::MALE] = "Male";
	labelName["Male"] = dpge::MALE;
	labelNameEnum[dpge::MIDDLE_OF_THE_ROOM] = "Middle of the room";
	labelName["Middle of the room"] = dpge::MIDDLE_OF_THE_ROOM;
	labelNameEnum[dpge::SQUARE] = "Square";
	labelName["Square"] = dpge::SQUARE;
	labelNameEnum[dpge::STRINGMARKS] = "Stringmarks";
	labelName["Stringmarks"] = dpge::STRINGMARKS;
	labelNameEnum[dpge::OPEN_WOUND] = "Open wound";
	labelName["Open wound"] = dpge::OPEN_WOUND;
	labelNameEnum[dpge::DINING_ROOM] = "Dining room";
	labelName["Dining room"] = dpge::DINING_ROOM;
	labelNameEnum[dpge::LAWYER] = "Lawyer";
	labelName["Lawyer"] = dpge::LAWYER;
	labelNameEnum[dpge::CALENDAR] = "Calendar";
	labelName["Calendar"] = dpge::CALENDAR;
	labelNameEnum[dpge::BOTANIST] = "Botanist";
	labelName["Botanist"] = dpge::BOTANIST;
	labelNameEnum[dpge::CUT] = "Cut";
	labelName["Cut"] = dpge::CUT;
	labelNameEnum[dpge::REVOLVER] = "Revolver";
	labelName["Revolver"] = dpge::REVOLVER;
	labelNameEnum[dpge::RUBBER] = "Rubber";
	labelName["Rubber"] = dpge::RUBBER;
	labelNameEnum[dpge::BLACK] = "Black";
	labelName["Black"] = dpge::BLACK;
	labelNameEnum[dpge::LIVING_ROOM] = "Living room";
	labelName["Living room"] = dpge::LIVING_ROOM;
	labelNameEnum[dpge::BLUNT_WOUND] = "Blunt wound";
	labelName["Blunt wound"] = dpge::BLUNT_WOUND;
	labelNameEnum[dpge::IN_A_CORNER] = "In a corner";
	labelName["In a corner"] = dpge::IN_A_CORNER;
	labelNameEnum[dpge::SHOT] = "Shot";
	labelName["Shot"] = dpge::SHOT;
	labelNameEnum[dpge::DENTIST] = "Dentist";
	labelName["Dentist"] = dpge::DENTIST;
	labelNameEnum[dpge::NOTEBOOK] = "Notebook";
	labelName["Notebook"] = dpge::NOTEBOOK;
	labelNameEnum[dpge::CHILD] = "Child";
	labelName["Child"] = dpge::CHILD;
	labelNameEnum[dpge::STABBED] = "Stabbed";
	labelName["Stabbed"] = dpge::STABBED;
	labelNameEnum[dpge::RIFLE] = "Rifle";
	labelName["Rifle"] = dpge::RIFLE;
	labelNameEnum[dpge::JUDGE] = "Judge";
	labelName["Judge"] = dpge::JUDGE;
	labelNameEnum[dpge::SMALL] = "Small";
	labelName["Small"] = dpge::SMALL;
	labelNameEnum[dpge::OUTSIDE] = "Outside";
	labelName["Outside"] = dpge::OUTSIDE;
	labelNameEnum[dpge::LIBRARY] = "Library";
	labelName["Library"] = dpge::LIBRARY;
	labelNameEnum[dpge::OVAL] = "Oval";
	labelName["Oval"] = dpge::OVAL;
	labelNameEnum[dpge::SPOUCE] = "Spouce";
	labelName["Spouce"] = dpge::SPOUCE;
	labelNameEnum[dpge::WINDOW] = "Window";
	labelName["Window"] = dpge::WINDOW;
	labelNameEnum[dpge::WOOD] = "Wood";
	labelName["Wood"] = dpge::WOOD;
	labelNameEnum[dpge::BULLETHOLE] = "Bullethole";
	labelName["Bullethole"] = dpge::BULLETHOLE;
	labelNameEnum[dpge::CASEFILE] = "Casefile";
	labelName["Casefile"] = dpge::CASEFILE;
	labelNameEnum[dpge::NURSE] = "Nurse";
	labelName["Nurse"] = dpge::NURSE;
	labelNameEnum[dpge::CLUB] = "Club";
	labelName["Club"] = dpge::CLUB;
	labelNameEnum[dpge::SECRATERY] = "Secratery";
	labelName["Secratery"] = dpge::SECRATERY;
	labelNameEnum[dpge::UP_BY_A_WALL] = "Up by a wall";
	labelName["Up by a wall"] = dpge::UP_BY_A_WALL;
	labelNameEnum[dpge::DESK] = "Desk";
	labelName["Desk"] = dpge::DESK;
	labelNameEnum[dpge::FRIENDS] = "Friends";
	labelName["Friends"] = dpge::FRIENDS;
	labelNameEnum[dpge::WASTBASKET] = "Wastbasket";
	labelName["Wastbasket"] = dpge::WASTBASKET;
	labelNameEnum[dpge::REALESTATE_AGENT] = "Realestate agent";
	labelName["Realestate agent"] = dpge::REALESTATE_AGENT;
	labelNameEnum[dpge::SHOTGUN] = "Shotgun";
	labelName["Shotgun"] = dpge::SHOTGUN;
	labelNameEnum[dpge::IRON] = "Iron";
	labelName["Iron"] = dpge::IRON;
	labelNameEnum[dpge::CYANIDE] = "Cyanide";
	labelName["Cyanide"] = dpge::CYANIDE;
	labelNameEnum[dpge::POISONED] = "Poisoned";
	labelName["Poisoned"] = dpge::POISONED;
	labelNameEnum[dpge::CHAIR] = "Chair";
	labelName["Chair"] = dpge::CHAIR;
	labelNameEnum[dpge::WASTEBASKET] = "Wastebasket";
	labelName["Wastebasket"] = dpge::WASTEBASKET;
	labelNameEnum[dpge::DROWNED] = "Drowned";
	labelName["Drowned"] = dpge::DROWNED;
	labelNameEnum[dpge::TABLE] = "Table";
	labelName["Table"] = dpge::TABLE;
	*/

	class Settings{
	public:
		map<uint32_t, string> nodeNameEnum;
		map<string, uint32_t> nodeName;

		nodeNameEnum.[PlotNodeEnum::WEAPON] = "Weapon";
		nodeName["Weapon"] = PlotNodeEnum::WEAPON;
		map<string, Node> Nodes() {
			string name;
			map<string, Node> nodes;
			//Weapon
			name = "Weapon";
			nodes[name] = Node(name, list<string>{ "Revolver", "Rifle", "Shotgun", "Spanner", "Club", "Robe", "String", "Hands", "Arsenic", "Cyanide", "Water" }, SubnetWeapon);
			name = "Weapon_container";
			nodes[name] = Node(name, list<string>{ "Object", "Suspect" }, SubnetWeapon);
			name = "Weapon_contained_in_suspect";
			nodes[name] = Node(name, list<string>{ "Suspects", "None" }, SubnetWeapon);
			name = "Weapon_contained_in_object";
			nodes[name] = Node(name, list<string>{ "Desk", "Chair", "Sink", "FilingCabinet", "Wastbasket", "Table", "Shelf", "Door", "Window", "None" }, SubnetWeapon);
			name = "Weapon_marks";
			nodes[name] = Node(name, list<string>{ "Square", "Oval", "Cut", "RobeMarks", "Bullethole", "Bulletholes", "Stringmarks", "None" }, SubnetWeapon);
			name = "Weapon_meathod";
			nodes[name] = Node(name, list<string>{ "Strangled", "Stabbed", "Drowned", "Shot", "Bludgeoned", "Poisoned" }, SubnetWeapon);
			name = "Weapon_material";
			nodes[name] = Node(name, list<string>{ "Iron", "Wood", "Steal", "Rubber", "Smell" }, SubnetWeapon);
			name = "Weapon_needs_strength";
			nodes[name] = Node(name, list<string>{ "True", "False" }, SubnetWeapon);
			name = "Weapon_wound_type";
			nodes[name] = Node(name, list<string>{ "Blunt wound", "Open wound", "None" }, SubnetWeapon);
			name = "Victim_wound_type";
			nodes[name] = Node(name, list<string>{ "Blunt wound", "Open wound", "None" }, SubnetWeapon);
			name = "Is_murder_weapon";
			nodes[name] = Node(name, list<string>{ "True", "False" }, SubnetWeapon);
			name = "blood_on_weapon";
			nodes[name] = Node(name, list<string>{ "True", "False" }, SubnetWeapon);
			name = "Hair_on_weapon";
			nodes[name] = Node(name, list<string>{ "True", "False" }, SubnetWeapon);
			name = "Hair_on_victim";
			nodes[name] = Node(name, list<string>{ "True", "False" }, SubnetWeapon);
			name = "Is_strength";
			nodes[name] = Node(name, list<string>{ "True", "False" }, SubnetWeapon);
			name = "Is_marks";
			nodes[name] = Node(name, list<string>{ "True", "False" }, SubnetWeapon);
			name = "Is_material";
			nodes[name] = Node(name, list<string>{ "True", "False" }, SubnetWeapon);
			name = "Is_wound";
			nodes[name] = Node(name, list<string>{ "True", "False" }, SubnetWeapon);
			name = "Is_weapons_hair";
			nodes[name] = Node(name, list<string>{ "True", "False" }, SubnetWeapon);
			name = "Is_blood";
			nodes[name] = Node(name, list<string>{ "True", "False" }, SubnetWeapon);
			name = "Weapons_hair_colour";
			nodes[name] = Node(name, list<string>{ "Blond", "Brown", "Black", "Red", "None" }, SubnetWeapon);

			//murder scene
			name = "Scene_room";
			nodes[name] = Node(name, list<string>{ "Dining room", "Living room", "Kitchen", "Lounge", "Hall", "Library" }, SubnetScene); //todo supply rooms
			name = "Scene_shoe_print";
			nodes[name] = Node(name, list<string>{ "37", "38", "39", "40", "41", "42", "43", "44", "45", "None" }, SubnetScene);
			name = "Scene_glass";
			nodes[name] = Node(name, list<string>{ "Inside", "Outside", "None" }, SubnetScene);
			name = "Hair_on_scene";
			nodes[name] = Node(name, list<string>{ "Blond", "Brown", "Black", "Red", "None" }, SubnetScene);

			//object
			name = "Object_name";
			nodes[name] = Node(name, list<string>{ "Desk", "Chair", "Sink", "FilingCabinet", "Wastebasket", "Table", "Shelf", "Door", "Window" }, SubnetObject); //TODO supply objects
			name = "Object_position";
			nodes[name] = Node(name, list<string>{ "Middle of the room", "Up by a wall", "In a corner", "On the floor" }, SubnetObject);
			name = "Object_is_container";
			nodes[name] = Node(name, list<string>{ "True", "False" }, SubnetObject);
			name = "Object_is_breakable";
			nodes[name] = Node(name, list<string>{ "True", "False" }, SubnetObject);
			name = "Object_glass";
			nodes[name] = Node(name, list<string>{ "True", "False" }, SubnetObject);
			name = "Object_locked";
			nodes[name] = Node(name, list<string>{ "True", "False", "None" }, SubnetObject);
			name = "Object_size";
			nodes[name] = Node(name, list<string>{ "Large", "Tall", "Medium", "Small" }, SubnetObject);
			name = "Object_lock";
			nodes[name] = Node(name, list<string>{ "Key lock", "Non key lock", "None" }, SubnetObject);
			name = "Object_broken";
			nodes[name] = Node(name, list<string>{ "True", "False" }, SubnetObject);
			name = "Object_list_of_suspects";
			nodes[name] = Node(name, list<string>{ "Notebook", "Calendar", "Casefile" }, SubnetObject);
			name = "Object_key_in_lock";
			nodes[name] = Node(name, list<string>{ "True", "False" }, SubnetObject);

			//suspect
			name = "Suspect_sex";
			nodes[name] = Node(name, list<string>{ "Male", "Female" }, SubnetSuspect);
			name = "Suspect_occupation";
			nodes[name] = Node(name, list<string>{ "Teacher", "Nurse", "Botanist", "Dentist", "Realestate agent", "Secratery", "Judge", "Lawyer", "GP" }, SubnetSuspect);
			name = "Suspect_strength";
			nodes[name] = Node(name, list<string>{ "Strong", "Weak" }, SubnetSuspect);
			name = "Suspect_size";
			nodes[name] = Node(name, list<string>{ "Tall", "Medium", "Small" }, SubnetSuspect);
			name = "Suspect_shoe_size";
			nodes[name] = Node(name, list<string>{ "37", "38", "39", "40", "41", "42", "43", "44", "45" }, SubnetSuspect);
			name = "Suspect_is_sufficient_strength";
			nodes[name] = Node(name, list<string>{ "True", "False" }, SubnetSuspect);
			name = "Suspect_is_shoe_print_on_scene";
			nodes[name] = Node(name, list<string>{ "Match", "No match", "None" }, SubnetSuspect);
			name = "Suspect_hair_colour";
			nodes[name] = Node(name, list<string>{ "Blond", "Brown", "Black", "Red" }, SubnetSuspect);
			name = "Suspect_is_hair_found_on_scene";
			nodes[name] = Node(name, list<string>{ "Match", "No match", "None" }, SubnetSuspect);
			name = "Suspect_is_hair_found_on_victim";
			nodes[name] = Node(name, list<string>{ "Match", "No match", "None" }, SubnetSuspect);
			name = "Is_suspect_murderer";
			nodes[name] = Node(name, list<string>{ "True", "False" }, SubnetSuspect);
			name = "Victim_relation";
			nodes[name] = Node(name, list<string>{ "Child", "Parent", "Spouce", "Friends", "Adulterer", "None" }, SubnetSuspect);
			name = "Suspect_heir";
			nodes[name] = Node(name, list<string>{ "True", "False" }, SubnetSuspect);
			name = "Suspect_rich";
			nodes[name] = Node(name, list<string>{ "True", "False" }, SubnetSuspect);
			name = "Suspect_in_debt";
			nodes[name] = Node(name, list<string>{ "True", "False" }, SubnetSuspect);
			name = "Suspect_has_dark_secret";
			nodes[name] = Node(name, list<string>{ "True", "False" }, SubnetSuspect);
			name = "Victim_knows_of_dark_secret";
			nodes[name] = Node(name, list<string>{ "True", "False" }, SubnetSuspect);
			name = "Suspect_knows_of_dark_secret";
			nodes[name] = Node(name, list<string>{ "True", "False" }, SubnetSuspect);
			name = "Harmed_suspect_in_past";
			nodes[name] = Node(name, list<string>{ "True", "False" }, SubnetSuspect);
			name = "Victim_threatened_to_expose_swindle";
			nodes[name] = Node(name, list<string>{ "True", "False" }, SubnetSuspect);
			name = "Victim_threatened_to_expose_blackmail";
			nodes[name] = Node(name, list<string>{ "True", "False" }, SubnetSuspect);
			name = "Suspect_lost_a_fortune";
			nodes[name] = Node(name, list<string>{ "True", "False" }, SubnetSuspect);
			name = "Suspect_inheritance";
			nodes[name] = Node(name, list<string>{ "True", "False" }, SubnetSuspect);
			name = "Suspect_is_blackmailer";
			nodes[name] = Node(name, list<string>{ "True", "False" }, SubnetSuspect);
			name = "Suspect_is_wedlocked";
			nodes[name] = Node(name, list<string>{ "True", "False" }, SubnetSuspect);
			name = "Suspect_is_swindler";
			nodes[name] = Node(name, list<string>{ "True", "False" }, SubnetSuspect);
			name = "Suspect_is_adulterer";
			nodes[name] = Node(name, list<string>{ "True", "False" }, SubnetSuspect);
			name = "Victim_is_adulterer";
			nodes[name] = Node(name, list<string>{ "True", "False" }, SubnetSuspect);
			name = "Suspect_is_swindled";
			nodes[name] = Node(name, list<string>{ "True", "False" }, SubnetSuspect);
			name = "Suspect_wanted_revenge";
			nodes[name] = Node(name, list<string>{ "True", "False" }, SubnetSuspect);
			name = "Suspect_was_blackmailed";
			nodes[name] = Node(name, list<string>{ "True", "False" }, SubnetSuspect);
			name = "Suspect_has_motive";
			nodes[name] = Node(name, list<string>{ "True", "False" }, SubnetSuspect);
			name = "Suspect_has_opportunity";
			nodes[name] = Node(name, list<string>{ "True", "False" }, SubnetSuspect);
			name = "Suspect_has_means";
			nodes[name] = Node(name, list<string>{ "True", "False" }, SubnetSuspect);
			name = "Suspect_is_murderer";
			nodes[name] = Node(name, list<string>{ "True", "False" }, SubnetSuspect);

			//chat
			name = "Greet";
			nodes[name] = Node(name, list<string>{ "True", "False" }, SubnetChat);
			name = "Party";
			nodes[name] = Node(name, list<string>{ "True", "False" }, SubnetChat);
			name = "Inquire";
			nodes[name] = Node(name, list<string>{ "True", "False" }, SubnetChat);
			name = "Introduction";
			nodes[name] = Node(name, list<string>{ "True", "False" }, SubnetChat);
			name = "Drink";
			nodes[name] = Node(name, list<string>{ "True", "False" }, SubnetChat);
			name = "Seat";
			nodes[name] = Node(name, list<string>{ "True", "False" }, SubnetChat);

			//reaction;
			name = "Reaction_look_at";
			nodes[name] = Node(name, list<string>{ "True", "False" }, SubnetReaction);
			name = "Reaction_react";
			nodes[name] = Node(name, list<string>{ "True", "False" }, SubnetReaction); 

			return nodes;
		}

		list<Arc> InitialPlotArcs()
		{
			list<Arc> arcs;

			//Weapon

			arcs.push_back( Arc("Weapon_container", SubnetWeapon, "Weapon", SubnetWeapon));
			arcs.push_back( Arc("Weapon_contained_in_suspect", SubnetWeapon, "Weapon_container", SubnetWeapon));
			arcs.push_back( Arc("Weapon_contained_in_object", SubnetWeapon, "Weapon_container", SubnetWeapon));
			arcs.push_back( Arc("Weapon_marks", SubnetWeapon, "Weapon", SubnetWeapon));
			arcs.push_back( Arc("Weapon_meathod", SubnetWeapon, "Weapon", SubnetWeapon));
			arcs.push_back( Arc("Weapon_material", SubnetWeapon, "Weapon", SubnetWeapon));
			arcs.push_back( Arc("Weapon_needs_strength", SubnetWeapon, "Weapon_meathod", SubnetWeapon));
			arcs.push_back( Arc("Weapon_needs_strength", SubnetWeapon, "Victim_wound_type", SubnetWeapon));
			arcs.push_back( Arc("Victim_wound_type", SubnetWeapon, "Weapon_meathod", SubnetWeapon));

			arcs.push_back( Arc("Is_murder_weapon", SubnetWeapon, "Is_strength", SubnetWeapon));
			arcs.push_back( Arc("Is_murder_weapon", SubnetWeapon, "Is_marks", SubnetWeapon));
			arcs.push_back( Arc("Is_murder_weapon", SubnetWeapon, "Is_material", SubnetWeapon));
			arcs.push_back( Arc("Is_murder_weapon", SubnetWeapon, "Is_wound", SubnetWeapon));
			arcs.push_back( Arc("Is_murder_weapon", SubnetWeapon, "Is_weapons_hair", SubnetWeapon));
			arcs.push_back( Arc("Is_murder_weapon", SubnetWeapon, "Is_blood", SubnetWeapon));

			arcs.push_back( Arc("blood_on_weapon", SubnetWeapon, "Weapon_meathod", SubnetWeapon));
			arcs.push_back( Arc("Weapons_hair_colour", SubnetWeapon, "Hair_on_weapon", SubnetWeapon));
			arcs.push_back( Arc("Weapons_hair_colour", SubnetWeapon, "Suspect_hair_colour", SubnetMurderer));
			arcs.push_back( Arc("Weapons_hair_colour", SubnetWeapon, "Suspect_hair_colour", SubnetVictim));

			arcs.push_back( Arc("Is_strength", SubnetWeapon, "Weapon_needs_strength", SubnetWeapon));
			arcs.push_back( Arc("Is_strength", SubnetWeapon, "Weapon_needs_strength", SubnetMurderWeapon));
			arcs.push_back( Arc("Is_marks", SubnetWeapon, "Weapon_marks", SubnetWeapon));
			arcs.push_back( Arc("Is_marks", SubnetWeapon, "Weapon_marks", SubnetMurderWeapon));
			arcs.push_back( Arc("Is_material", SubnetWeapon, "Weapon_material", SubnetWeapon));
			arcs.push_back( Arc("Is_material", SubnetWeapon, "Weapon_material", SubnetMurderWeapon));
			arcs.push_back( Arc("Is_wound", SubnetWeapon, "Weapon_wound_type", SubnetWeapon));
			arcs.push_back( Arc("Is_wound", SubnetWeapon, "Weapon_wound_type", SubnetMurderWeapon));
			arcs.push_back( Arc("Is_wound", SubnetWeapon, "Victim_wound_type", SubnetWeapon));
			arcs.push_back( Arc("Is_weapons_hair", SubnetWeapon, "Weapons_hair_colour", SubnetWeapon));
			arcs.push_back( Arc("Is_weapons_hair", SubnetWeapon, "Weapons_hair_colour", SubnetMurderWeapon));
			arcs.push_back( Arc("Is_blood", SubnetWeapon, "blood_on_weapon", SubnetWeapon));
			arcs.push_back( Arc("Is_blood", SubnetWeapon, "blood_on_weapon", SubnetMurderWeapon));

			//Object 
			arcs.push_back( Arc("Object_name", SubnetObject, "Object_position", SubnetObject));
			arcs.push_back( Arc("Object_lock", SubnetObject, "Object_name", SubnetObject));
			arcs.push_back( Arc("Object_key_in_lock", SubnetObject, "Object_lock", SubnetObject));
			arcs.push_back( Arc("Object_is_breakable", SubnetObject, "Object_name", SubnetObject));
			arcs.push_back( Arc("Object_broken", SubnetObject, "Object_is_breakable", SubnetObject));
			arcs.push_back( Arc("Object_glass", SubnetObject, "Object_broken", SubnetObject));
			arcs.push_back( Arc("Object_glass", SubnetObject, "Object_name", SubnetObject));
			arcs.push_back( Arc("Object_size", SubnetObject, "Object_name", SubnetObject));
			arcs.push_back( Arc("Object_is_container", SubnetObject, "Object_name", SubnetObject));

			//Suspect
			arcs.push_back( Arc("Suspect_size", SubnetSuspect, "Suspect_sex", SubnetSuspect));
			arcs.push_back( Arc("Suspect_strength", SubnetSuspect, "Suspect_sex", SubnetSuspect));
			arcs.push_back( Arc("Suspect_shoe_size", SubnetSuspect, "Suspect_sex", SubnetSuspect));
			arcs.push_back( Arc("Suspect_shoe_size", SubnetSuspect, "Suspect_size", SubnetSuspect));

			arcs.push_back( Arc("Suspect_is_sufficient_strength", SubnetSuspect, "Suspect_strength", SubnetSuspect));
			arcs.push_back( Arc("Suspect_is_sufficient_strength", SubnetSuspect, "Weapon_needs_strength", SubnetMurderWeapon));
			arcs.push_back( Arc("Suspect_is_shoe_print_on_scene", SubnetSuspect, "Suspect_shoe_size", SubnetSuspect));
			arcs.push_back( Arc("Suspect_is_shoe_print_on_scene", SubnetSuspect, "Scene_shoe_print", SubnetMurderScene));
			arcs.push_back( Arc("Suspect_is_hair_found_on_victim", SubnetSuspect, "Suspect_hair_colour", SubnetSuspect));
			arcs.push_back( Arc("Suspect_is_hair_found_on_victim", SubnetSuspect, "Suspect_hair_colour", SubnetVictim));
			arcs.push_back( Arc("Suspect_is_blackmailer", SubnetSuspect, "Suspect_knows_of_dark_secret", SubnetSuspect));
			arcs.push_back( Arc("Suspect_is_blackmailer", SubnetSuspect, "Victim_threatened_to_expose_blackmail", SubnetSuspect));
			arcs.push_back( Arc("Suspect_was_blackmailed", SubnetSuspect, "Suspect_has_dark_secret", SubnetSuspect));
			arcs.push_back( Arc("Suspect_was_blackmailed", SubnetSuspect, "Suspect_knows_of_dark_secret", SubnetVictim));
			arcs.push_back( Arc("Suspect_knows_of_dark_secret", SubnetSuspect, "Suspect_has_dark_secret", SubnetVictim));
			arcs.push_back( Arc("Suspect_inheritance", SubnetSuspect, "Suspect_heir", SubnetSuspect));
			arcs.push_back( Arc("Suspect_inheritance", SubnetSuspect, "Suspect_rich", SubnetSuspect));
			arcs.push_back( Arc("Suspect_heir", SubnetSuspect, "Victim_relation", SubnetSuspect));
			arcs.push_back( Arc("Suspect_is_wedlocked", SubnetSuspect, "Victim_relation", SubnetSuspect));
			arcs.push_back( Arc("Suspect_is_adulterer", SubnetSuspect, "Victim_relation", SubnetSuspect));
			arcs.push_back( Arc("Victim_is_adulterer", SubnetSuspect, "Victim_relation", SubnetSuspect));
			arcs.push_back( Arc("Suspect_in_debt", SubnetSuspect, "Suspect_rich", SubnetSuspect));
			arcs.push_back( Arc("Suspect_is_swindled", SubnetSuspect, "Suspect_is_swindler", SubnetVictim));
			arcs.push_back( Arc("Suspect_is_swindled", SubnetSuspect, "Suspect_lost_a_fortune", SubnetVictim));
			arcs.push_back( Arc("Suspect_is_swindler", SubnetSuspect, "Victim_threatened_to_expose_swindle", SubnetSuspect));
			arcs.push_back( Arc("Suspect_wanted_revenge", SubnetSuspect, "Suspect_lost_a_fortune", SubnetSuspect));
			arcs.push_back( Arc("Suspect_wanted_revenge", SubnetSuspect, "Suspect_sex", SubnetSuspect));
			arcs.push_back( Arc("Suspect_has_motive", SubnetSuspect, "Suspect_is_blackmailer", SubnetSuspect));
			arcs.push_back( Arc("Suspect_has_motive", SubnetSuspect, "Suspect_was_blackmailed", SubnetSuspect));
			arcs.push_back( Arc("Suspect_has_motive", SubnetSuspect, "Suspect_inheritance", SubnetSuspect));
			arcs.push_back( Arc("Suspect_has_motive", SubnetSuspect, "Suspect_is_adulterer", SubnetSuspect));
			arcs.push_back( Arc("Suspect_has_motive", SubnetSuspect, "Victim_is_adulterer", SubnetSuspect));
			arcs.push_back( Arc("Suspect_has_motive", SubnetSuspect, "Suspect_in_debt", SubnetSuspect));
			arcs.push_back( Arc("Suspect_has_motive", SubnetSuspect, "Suspect_is_swindled", SubnetSuspect));
			arcs.push_back( Arc("Suspect_has_motive", SubnetSuspect, "Suspect_is_swindler", SubnetSuspect));
			arcs.push_back( Arc("Suspect_is_murderer", SubnetSuspect, "Suspect_has_motive", SubnetSuspect));
			arcs.push_back( Arc("Suspect_is_murderer", SubnetSuspect, "Suspect_has_opportunity", SubnetSuspect));
			arcs.push_back( Arc("Suspect_is_murderer", SubnetSuspect, "Suspect_has_means", SubnetSuspect));

			return arcs;
		}

		list<pair<string, string>> TotalogyDefinitions()
		{
			list<pair<string, string>> totologies;
			totologies.push_back(make_pair( "Suspect_is_murderer", SubnetSuspect));
			totologies.push_back(make_pair( "Suspect_has_motive", SubnetSuspect));
			totologies.push_back(make_pair( "Suspect_has_opportunity", SubnetSuspect));
			totologies.push_back(make_pair( "Suspect_has_means", SubnetSuspect));
			totologies.push_back(make_pair( "Is_murder_weapon", SubnetWeapon));
			return totologies;
		}

		/*
		;
		values:V_name:victim:normalized:1,1,1,1,1,1;

		parents:V_sex:victim:V_name;
		values:V_sex:victim:twoRows:1,1,1,0,0,1;

		parents:V_victims_size:victim:V_sex;
		values:V_victims_size:victim::0.4,0.4,0.2,0.2,0.5,0.3;

		parents:V_victims_shoe_size:victim:V_sex,V_victims_size;
		values:V_victims_shoe_size:victim:normalized:0,0,0,0,0,1,1,3,4,0,0,0,0,0,2,3,3,2,0,0,0,0,0,4,3,1,1,2,1,1,3,4,2,0,0,0,3,2,3,3,2,0,0,0,0,4,3,1,1,0,0,0,0,0;

		values:V_victims_dark_secret:victim:normalized:1,1;

		values:V_victim_rich:victim:normalized:1,1;

		values:V_hair_color:victim:normalized:1,1,1,1;

		sub:weapon;

		values:W_weapon_name:weapon:normalized:1,1,1,1,1,1,1,1,1,1,1;

		parents:W_contained:weapon:W_weapon_name;
		values:W_contained:weapon:twoColumns:0.5,1,1,1,0.8,1,0.2,0,0.7,0.7,1;

		parents:W_contained_in_suspect:weapon:W_contained;
		values:W_contained_in_suspect:weapon:normalized:0,0,0,0,0,0,1,1,1,1,1,1,1,0;

		parents:W_contained_in_object:weapon:W_contained;
		values:W_contained_in_object:weapon:normalized:1,1,1,1,1,1,1,1,1,0,0,0,0,0,0,0,0,0,0,1;

		parents:W_weapon_marks:weapon:W_weapon_name;
		values:W_weapon_marks:weapon::0,0,0,0,0,0.8,0.2,0,0,0,0,0,0,0.98,0.02,0,0,0,0,0,0,0,1,0,0.8,0.2,0,0,0,0,0,0,0,1,0,0,0,0,0,0,0,0,0,0,1,0,0,0,0,0,0,0,0,0,0,1,0,0,0,1,0,0,0,0,0,0,0,1,0,0,0,0,0,0,0,1,0,0,0,0,0,0,0,1,0,0,0,0;

		parents:W_meathod:weapon:W_weapon_name;
		values:W_meathod:weapon::0,0,0,1,0,0,0,0,0,1,0,0,0,0,0,1,0,0,0,0,0,0,1,0,0,0,0,0,1,0,1,0,0,0,0,0,1,0,0,0,0,0,1,0,0,0,0,0,0,0,0,0,0,1,0,0,0,0,0,1,0,0,1,0,0,0;

		parents:W_weapon_needs_strength:weapon:W_meathod;
		values:W_weapon_needs_strength:weapon:twoColumns:1,0.83,1,0,0.85,0;

		parents:W_wound_type:weapon:W_meathod;
		values:W_wound_type:weapon::0,0,1,0,1,0,0,0,1,0,1,0,1,0,0,0,0,1;

		parents:W_is_strength:weapon:W_weapon_needs_strength,MW_Is_murder_weapon_needs_strength$Is_murder_weapon;
		values:W_is_strength:weapon:twoColumns:1,0,0,1;

		parents:W_is_marks:weapon:W_weapon_marks,MW_Is_murder_weapon_marks$Is_murder_weapon;
		values:W_is_marks:weapon:twoColumns:1,0,0,0,0,0,0,0,0,1,0,0,0,0,0,0,0,0,1,0,0,0,0,0,0,0,0,1,0,0,0,0,0,0,0,0,1,0,0,0,0,0,0,0,0,1,0,0,0,0,0,0,0,0,1,0,0,0,0,0,0,0,0,1;

		parents:W_is_wound:weapon:MW_victim_wound_type$Is_murder_weapon,W_wound_type;
		values:W_is_wound:weapon:twoColumns:1,0,0,0,1,0,0,0,1;

		parents:W_Is_murder_weapon:weapon:W_is_face,W_is_neck,W_is_hands,W_is_marks,W_is_strength,W_is_wound;

		parents:W_blood_on_weapon:weapon:MW_blood_on_Is_murder_weapon$Is_murder_weapon,W_Is_murder_weapon;
		values:W_blood_on_weapon:weapon:twoColumns:1,0,0,0;

		parents:W_weapons_hair_color:weapon:MW_hair_on_Is_murder_weapon$Is_murder_weapon,V_hair_color$victim,W_Is_murder_weapon;
		values:W_weapons_hair_color:weapon::1,0,0,0,0,0,0,0,0,1,0,1,0,0,0,0,0,0,0,1,0,0,1,0,0,0,0,0,0,1,0,0,0,1,0,0,0,0,0,1,0,0,0,0,1,0,0,0,0,1,0,0,0,0,1,0,0,0,0,1,0,0,0,0,1,0,0,0,0,1,0,0,0,0,1,0,0,0,0,1;




		sub:murder_scene;

		values: MS_Scene_name: murder_scene: normalized: 1,1,1,1,1,1;

		values: MS_shoe_print: murder_scene::0.00700797,0.0140159,0.0420478,0.119696,0.106872,0.0389332,0.0967355,0.0967355,0.0424078,0.435549;

		values: MS_glass: murder_scene: normalized: 1,1,1;

		loop:object:6;
		sub:object;


		parents:P_object_name:object:MS_Scene_name$murder_scene;
		values:P_object_name:object:normalized:0,1,0,0,1,1,1,1,1,0,1,0,0,1,1,1,1,1,0,1,1,0,1,1,1,1,1,1,1,0,1,1,1,1,1,1,1,1,0,0,1,1,1,1,1,1,1,0,1,1,1,1,1,1;

		parents:P_position:object:P_object_name;
		values:P_position:object:normalized:1,1,1,0,1,1,1,1,0,1,1,0,0,1,1,0,1,1,1,1,1,1,1,1,0,1,1,0,0,1,1,0,0,1,1,0;

		parents:P_container:object:P_object_name;
		values:P_container:object:twoColumns:1,0,0,1,1,1,1,0,0;

		parents:P_breakable:object:P_object_name;
		values:P_breakable:object:twoColumns:0,0,0,0,0,0,0,0,1;

		parents:P_size:object:P_object_name;
		values:P_size:object:normalized:1,0,1,1,1,0,1,1,1,0,1,1,1,1,1,1,1,0,1,1,1,0,1,1,0,1,1,1,1,0,0,1,1,0,1,1;

		parents:P_lock:object:P_object_name;
		values:P_lock:object:normalized:1,1,1,0,0,1,0,0,1,1,1,1,0,0,1,0,0,1,0,0,1,1,1,1,0,1,1;

		parents:P_broken:object:P_breakable,MS_glass$murder_scene;
		values:P_broken:object:twoColumns:0.5,0.5,0,0,0,0;

		values:P_list_of_suspects:object:normalized:1,1,1;

		parents:P_key_in_lock:object:P_lock;
		values:P_key_in_lock:object:twoColumns:0.5,0,0;

		parents:P_locked:object:P_lock;
		values:P_locked:object:twoColumns:0.5,0.5,0;

		values:S_name:suspect:normalized:1,1,1,1,1,1;

		parents:S_sex:suspect:S_name;
		values:S_sex:suspect:twoRows:1,1,1,0,0,1;

		values:S_occupation:suspect:normalized:1,1,1,1,1,1,1,1,1;

		parents:S_strength:suspect:S_sex;
		values:S_strength:suspect:twoColumns:0.7,0.3;

		parents:S_size:suspect:S_sex;
		values:S_size:suspect::0.4,0.4,0.2,0.2,0.5,0.3;

		parents:S_shoe_size:suspect:S_sex,S_size;
		values:S_shoe_size:suspect:normalized:0,0,0,0,0,1,1,3,4,0,0,0,0,0,2,3,3,2,0,0,0,0,0,4,3,1,1,0.2,1,1,3,4,2,0,0,0,0.3,2,3,3,2,0,0,0,0,4,3,1,1,0.24,0,0,0,0;

		parents:S_sufficient_strength:suspect:S_strength,MW_Is_murder_weapon_needs_strength$Is_murder_weapon;
		values:S_sufficient_strength:suspect:twoColumns:1,1,0,1;

		parents:S_ShoePrintFoundOnScene:suspect:MS_shoe_print$murder_scene,S_shoe_size;
		values:S_ShoePrintFoundOnScene:suspect:match;

		values:S_hair_color:suspect:normalized:1,1,1,1;

		parents:S_HairFoundOnScene:suspect:MS_hair_on_scene$murder_scene,S_hair_color;

		parents:S_HairFoundOnVictim:suspect:S_hair_color,M_hair_color$murderer,MW_hair_on_victim$Is_murder_weapon;

		parents:S_murderer:suspect:S_sufficient_strength,S_ShoePrintFoundOnScene,S_HairFoundOnScene,S_HairFoundOnVictim;

		parents:S_relation:suspect:S_name,V_name$victim;

		parents:S_heir:suspect:S_relation;
		values:S_heir:suspect:twoColumns:0.98,0.1,0.95,0.15,0.2,0;

		values:S_rich:suspect:normalized:1,1;

		parents:S_indebt:suspect:S_rich;
		values:S_indebt:suspect:twoColumns:0,0.8;

		values:S_dark_secret:suspect:normalized:1,1;

		parents:S_victim_knows_of_dark_secret:suspect:S_dark_secret;
		values:S_victim_knows_of_dark_secret:suspect:twoColumns:0.7,0;

		parents:S_suspect_knows_of_dark_secret:suspect:V_victims_dark_secret$victim;
		values:S_suspect_knows_of_dark_secret:suspect:twoColumns:0.7,0;

		values:S_harmed_suspect_in_past:suspect:normalized:1,1;

		values:S_victim_threatened_to_expose_swindle:suspect:normalized:1,1;

		values:S_victim_threatened_to_expose_blackmail:suspect:normalized:1,1;

		values:S_lost_a_fortune:suspect:normalized:1,1;

		parents:S_inheritance:suspect:S_heir;
		values:S_inheritance:suspect:twoColumns:0.9,0;

		parents:S_blackmailer:suspect:S_suspect_knows_of_dark_secret,S_victim_threatened_to_expose_blackmail;
		values:S_blackmailer:suspect:twoColumns:1,0,0,0;

		parents:S_swindler:suspect:S_victim_threatened_to_expose_swindle;
		values:S_swindler:suspect:twoColumns:1,0;

		parents:S_adultery:suspect:S_relation;
		values:S_adultery:suspect:normalized:0,1,0,1,0.6,0.4,0,1,0,1,0,1;

		parents:S_isSwindled:suspect:S_lost_a_fortune;
		values:S_isSwindled:suspect:twoColumns:1,0;

		parents:S_revenge:suspect:S_isSwindled,S_harmed_suspect_in_past;
		values:S_revenge:suspect:twoColumns:1,1,1,0;

		parents:S_isBlackmailed:suspect:S_victim_knows_of_dark_secret;
		values:S_isBlackmailed:suspect:twoColumns:0.8,0;

		parents:S_motive:suspect:S_blackmailer,S_inheritance,S_swindler,S_adultery,S_indebt,S_isSwindled,S_revenge,S_isBlackmailed;

		values:S_HairFoundOnScene:suspect:match;
		values:S_HairFoundOnVictim:suspect:match;
		values:S_motive:suspect:allFalseMatch;
		values:W_Is_murder_weapon:weapon:first;
		values:M_murderer:murderer:first;
		values:S_murderer:suspect:first;

		values:O_opportunity:opportunity:normalized:1,1;

		sub:chat;

		values:C_greet:chat:normalized:1,0;

		values:C_party:chat:normalized:1,1,1;

		values:C_inquire:chat:normalized:1,0;

		values:C_introduction:chat:normalized:1,0;

		values:C_drink:chat:normalized:1,0;

		values:C_seat:chat:normalized:1,0;

		sub:reaction;

		values:R_look:reaction:normalized:1,0;

		values:R_react:reaction:normalized:1,1,1;
		*/

	};
}


#endif // !SETTINGS_H
