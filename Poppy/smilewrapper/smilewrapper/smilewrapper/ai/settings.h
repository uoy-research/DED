// settings.h
#ifndef SETTINGS_H
#define SETTINGS_H

using namespace AI;

namespace dpge {

	class Settings {
	public:


		map<string, Node> Nodes( ) {
			string name;
			map<string, Node> nodes;
			//Weapon
			name = "Weapon";
			nodes[name] = Node( name, list<string>{ "Revolver", "Rifle", "Shotgun", "Spanner", "Club", "Robe", "String", "Hands", "Arsenic", "Cyanide", "Water" }, PLOT_TYPE_NAME[PlotEnum::WEAPON] );
			name = "Weapon_container";
			nodes[name] = Node( name, list<string>{ "Object", "Suspect" }, PLOT_TYPE_NAME[PlotEnum::WEAPON] );
			name = "Weapon_contained_in_suspect";
			nodes[name] = Node( name, list<string>{ "Suspects", "None" }, PLOT_TYPE_NAME[PlotEnum::WEAPON] );
			name = "Weapon_contained_in_object";
			nodes[name] = Node( name, list<string>{ "Desk", "Chair", "Sink", "FilingCabinet", "Wastbasket", "Table", "Shelf", "Door", "Window", "None" }, PLOT_TYPE_NAME[PlotEnum::WEAPON] );
			name = "Weapon_marks";
			nodes[name] = Node( name, list<string>{ "Square", "Oval", "Cut", "RobeMarks", "Bullethole", "Bulletholes", "Stringmarks", "None" }, PLOT_TYPE_NAME[PlotEnum::WEAPON] );
			name = "Weapon_meathod";
			nodes[name] = Node( name, list<string>{ "Strangled", "Stabbed", "Drowned", "Shot", "Bludgeoned", "Poisoned" }, PLOT_TYPE_NAME[PlotEnum::WEAPON] );
			name = "Weapon_material";
			nodes[name] = Node( name, list<string>{ "Iron", "Wood", "Steal", "Rubber", "Smell" }, PLOT_TYPE_NAME[PlotEnum::WEAPON] );
			name = "Weapon_needs_strength";
			nodes[name] = Node( name, list<string>{ "True", "False" }, PLOT_TYPE_NAME[PlotEnum::WEAPON] );
			name = "Weapon_wound_type";
			nodes[name] = Node( name, list<string>{ "Blunt wound", "Open wound", "None" }, PLOT_TYPE_NAME[PlotEnum::WEAPON] );
			name = "Victim_wound_type";
			nodes[name] = Node( name, list<string>{ "Blunt wound", "Open wound", "None" }, PLOT_TYPE_NAME[PlotEnum::WEAPON] );
			name = "Is_murder_weapon";
			nodes[name] = Node( name, list<string>{ "True", "False" }, PLOT_TYPE_NAME[PlotEnum::WEAPON] );
			name = "blood_on_weapon";
			nodes[name] = Node( name, list<string>{ "True", "False" }, PLOT_TYPE_NAME[PlotEnum::WEAPON] );
			name = "Hair_on_weapon";
			nodes[name] = Node( name, list<string>{ "True", "False" }, PLOT_TYPE_NAME[PlotEnum::WEAPON] );
			name = "Hair_on_victim";
			nodes[name] = Node( name, list<string>{ "True", "False" }, PLOT_TYPE_NAME[PlotEnum::WEAPON] );
			name = "Is_strength";
			nodes[name] = Node( name, list<string>{ "True", "False" }, PLOT_TYPE_NAME[PlotEnum::WEAPON] );
			name = "Is_marks";
			nodes[name] = Node( name, list<string>{ "True", "False" }, PLOT_TYPE_NAME[PlotEnum::WEAPON] );
			name = "Is_material";
			nodes[name] = Node( name, list<string>{ "True", "False" }, PLOT_TYPE_NAME[PlotEnum::WEAPON] );
			name = "Is_wound";
			nodes[name] = Node( name, list<string>{ "True", "False" }, PLOT_TYPE_NAME[PlotEnum::WEAPON] );
			name = "Is_weapons_hair";
			nodes[name] = Node( name, list<string>{ "True", "False" }, PLOT_TYPE_NAME[PlotEnum::WEAPON] );
			name = "Is_blood";
			nodes[name] = Node( name, list<string>{ "True", "False" }, PLOT_TYPE_NAME[PlotEnum::WEAPON] );
			name = "Weapons_hair_colour";
			nodes[name] = Node( name, list<string>{ "Blond", "Brown", "Black", "Red", "None" }, PLOT_TYPE_NAME[PlotEnum::WEAPON] );

			//murder scene
			name = "Scene_room";
			nodes[name] = Node( name, list<string>{ "Dining room", "Living room", "Kitchen", "Lounge", "Hall", "Library" }, PLOT_TYPE_NAME[PlotEnum::SCENE] ); //todo supply rooms
			name = "Scene_shoe_print";
			nodes[name] = Node( name, list<string>{ "37", "38", "39", "40", "41", "42", "43", "44", "45", "None" }, PLOT_TYPE_NAME[PlotEnum::SCENE] );
			name = "Scene_glass";
			nodes[name] = Node( name, list<string>{ "Inside", "Outside", "None" }, PLOT_TYPE_NAME[PlotEnum::SCENE] );
			name = "Hair_on_scene";
			nodes[name] = Node( name, list<string>{ "Blond", "Brown", "Black", "Red", "None" }, PLOT_TYPE_NAME[PlotEnum::SCENE] );

			//object
			name = "Object_name";
			nodes[name] = Node( name, list<string>{ "Desk", "Chair", "Sink", "FilingCabinet", "Wastebasket", "Table", "Shelf", "Door", "Window" }, PLOT_TYPE_NAME[PlotEnum::OBJECT] ); //TODO supply objects
			name = "Object_position";
			nodes[name] = Node( name, list<string>{ "Middle of the room", "Up by a wall", "In a corner", "On the floor" }, PLOT_TYPE_NAME[PlotEnum::OBJECT] );
			name = "Object_is_container";
			nodes[name] = Node( name, list<string>{ "True", "False" }, PLOT_TYPE_NAME[PlotEnum::OBJECT] );
			name = "Object_is_breakable";
			nodes[name] = Node( name, list<string>{ "True", "False" }, PLOT_TYPE_NAME[PlotEnum::OBJECT] );
			name = "Object_glass";
			nodes[name] = Node( name, list<string>{ "True", "False" }, PLOT_TYPE_NAME[PlotEnum::OBJECT] );
			name = "Object_locked";
			nodes[name] = Node( name, list<string>{ "True", "False", "None" }, PLOT_TYPE_NAME[PlotEnum::OBJECT] );
			name = "Object_size";
			nodes[name] = Node( name, list<string>{ "Large", "Tall", "Medium", "Small" }, PLOT_TYPE_NAME[PlotEnum::OBJECT] );
			name = "Object_lock";
			nodes[name] = Node( name, list<string>{ "Key lock", "Non key lock", "None" }, PLOT_TYPE_NAME[PlotEnum::OBJECT] );
			name = "Object_broken";
			nodes[name] = Node( name, list<string>{ "True", "False" }, PLOT_TYPE_NAME[PlotEnum::OBJECT] );
			name = "Object_list_of_suspects";
			nodes[name] = Node( name, list<string>{ "Notebook", "Calendar", "Casefile" }, PLOT_TYPE_NAME[PlotEnum::OBJECT] );
			name = "Object_key_in_lock";
			nodes[name] = Node( name, list<string>{ "True", "False" }, PLOT_TYPE_NAME[PlotEnum::OBJECT] );

			//suspect
			name = "Suspect_sex";
			nodes[name] = Node( name, list<string>{ "Male", "Female" }, PLOT_TYPE_NAME[PlotEnum::SUSPECT] );
			name = "Suspect_occupation";
			nodes[name] = Node( name, list<string>{ "Teacher", "Nurse", "Botanist", "Dentist", "Realestate agent", "Secratery", "Judge", "Lawyer", "GP" }, PLOT_TYPE_NAME[PlotEnum::SUSPECT] );
			name = "Suspect_strength";
			nodes[name] = Node( name, list<string>{ "Strong", "Weak" }, PLOT_TYPE_NAME[PlotEnum::SUSPECT] );
			name = "Suspect_size";
			nodes[name] = Node( name, list<string>{ "Tall", "Medium", "Small" }, PLOT_TYPE_NAME[PlotEnum::SUSPECT] );
			name = "Suspect_shoe_size";
			nodes[name] = Node( name, list<string>{ "37", "38", "39", "40", "41", "42", "43", "44", "45" }, PLOT_TYPE_NAME[PlotEnum::SUSPECT] );
			name = "Suspect_is_sufficient_strength";
			nodes[name] = Node( name, list<string>{ "True", "False" }, PLOT_TYPE_NAME[PlotEnum::SUSPECT] );
			name = "Suspect_is_shoe_print_on_scene";
			nodes[name] = Node( name, list<string>{ "Match", "No match", "None" }, PLOT_TYPE_NAME[PlotEnum::SUSPECT] );
			name = "Suspect_hair_colour";
			nodes[name] = Node( name, list<string>{ "Blond", "Brown", "Black", "Red" }, PLOT_TYPE_NAME[PlotEnum::SUSPECT] );
			name = "Suspect_is_hair_found_on_scene";
			nodes[name] = Node( name, list<string>{ "Match", "No match", "None" }, PLOT_TYPE_NAME[PlotEnum::SUSPECT] );
			name = "Suspect_is_hair_found_on_victim";
			nodes[name] = Node( name, list<string>{ "Match", "No match", "None" }, PLOT_TYPE_NAME[PlotEnum::SUSPECT] );
			name = "Is_suspect_murderer";
			nodes[name] = Node( name, list<string>{ "True", "False" }, PLOT_TYPE_NAME[PlotEnum::SUSPECT] );
			name = "Victim_relation";
			nodes[name] = Node( name, list<string>{ "Child", "Parent", "Spouce", "Friends", "Adulterer", "None" }, PLOT_TYPE_NAME[PlotEnum::SUSPECT] );
			name = "Suspect_heir";
			nodes[name] = Node( name, list<string>{ "True", "False" }, PLOT_TYPE_NAME[PlotEnum::SUSPECT] );
			name = "Suspect_rich";
			nodes[name] = Node( name, list<string>{ "True", "False" }, PLOT_TYPE_NAME[PlotEnum::SUSPECT] );
			name = "Suspect_in_debt";
			nodes[name] = Node( name, list<string>{ "True", "False" }, PLOT_TYPE_NAME[PlotEnum::SUSPECT] );
			name = "Suspect_has_dark_secret";
			nodes[name] = Node( name, list<string>{ "True", "False" }, PLOT_TYPE_NAME[PlotEnum::SUSPECT] );
			name = "Victim_knows_of_dark_secret";
			nodes[name] = Node( name, list<string>{ "True", "False" }, PLOT_TYPE_NAME[PlotEnum::SUSPECT] );
			name = "Suspect_knows_of_dark_secret";
			nodes[name] = Node( name, list<string>{ "True", "False" }, PLOT_TYPE_NAME[PlotEnum::SUSPECT] );
			name = "Harmed_suspect_in_past";
			nodes[name] = Node( name, list<string>{ "True", "False" }, PLOT_TYPE_NAME[PlotEnum::SUSPECT] );
			name = "Victim_threatened_to_expose_swindle";
			nodes[name] = Node( name, list<string>{ "True", "False" }, PLOT_TYPE_NAME[PlotEnum::SUSPECT] );
			name = "Victim_threatened_to_expose_blackmail";
			nodes[name] = Node( name, list<string>{ "True", "False" }, PLOT_TYPE_NAME[PlotEnum::SUSPECT] );
			name = "Suspect_lost_a_fortune";
			nodes[name] = Node( name, list<string>{ "True", "False" }, PLOT_TYPE_NAME[PlotEnum::SUSPECT] );
			name = "Suspect_inheritance";
			nodes[name] = Node( name, list<string>{ "True", "False" }, PLOT_TYPE_NAME[PlotEnum::SUSPECT] );
			name = "Suspect_is_blackmailer";
			nodes[name] = Node( name, list<string>{ "True", "False" }, PLOT_TYPE_NAME[PlotEnum::SUSPECT] );
			name = "Suspect_is_wedlocked";
			nodes[name] = Node( name, list<string>{ "True", "False" }, PLOT_TYPE_NAME[PlotEnum::SUSPECT] );
			name = "Suspect_is_swindler";
			nodes[name] = Node( name, list<string>{ "True", "False" }, PLOT_TYPE_NAME[PlotEnum::SUSPECT] );
			name = "Suspect_is_adulterer";
			nodes[name] = Node( name, list<string>{ "True", "False" }, PLOT_TYPE_NAME[PlotEnum::SUSPECT] );
			name = "Victim_is_adulterer";
			nodes[name] = Node( name, list<string>{ "True", "False" }, PLOT_TYPE_NAME[PlotEnum::SUSPECT] );
			name = "Suspect_is_swindled";
			nodes[name] = Node( name, list<string>{ "True", "False" }, PLOT_TYPE_NAME[PlotEnum::SUSPECT] );
			name = "Suspect_wanted_revenge";
			nodes[name] = Node( name, list<string>{ "True", "False" }, PLOT_TYPE_NAME[PlotEnum::SUSPECT] );
			name = "Suspect_was_blackmailed";
			nodes[name] = Node( name, list<string>{ "True", "False" }, PLOT_TYPE_NAME[PlotEnum::SUSPECT] );
			name = "Suspect_has_motive";
			nodes[name] = Node( name, list<string>{ "True", "False" }, PLOT_TYPE_NAME[PlotEnum::SUSPECT] );
			name = "Suspect_has_opportunity";
			nodes[name] = Node( name, list<string>{ "True", "False" }, PLOT_TYPE_NAME[PlotEnum::SUSPECT] );
			name = "Suspect_has_means";
			nodes[name] = Node( name, list<string>{ "True", "False" }, PLOT_TYPE_NAME[PlotEnum::SUSPECT] );
			name = "Suspect_is_murderer";
			nodes[name] = Node( name, list<string>{ "True", "False" }, PLOT_TYPE_NAME[PlotEnum::SUSPECT] );

			//chat
			name = "Greet";
			nodes[name] = Node( name, list<string>{ "True", "False" }, PLOT_TYPE_NAME[PlotEnum::CHAT] );
			name = "Party";
			nodes[name] = Node( name, list<string>{ "True", "False" }, PLOT_TYPE_NAME[PlotEnum::CHAT] );
			name = "Inquire";
			nodes[name] = Node( name, list<string>{ "True", "False" }, PLOT_TYPE_NAME[PlotEnum::CHAT] );
			name = "Introduction";
			nodes[name] = Node( name, list<string>{ "True", "False" }, PLOT_TYPE_NAME[PlotEnum::CHAT] );
			name = "Drink";
			nodes[name] = Node( name, list<string>{ "True", "False" }, PLOT_TYPE_NAME[PlotEnum::CHAT] );
			name = "Seat";
			nodes[name] = Node( name, list<string>{ "True", "False" }, PLOT_TYPE_NAME[PlotEnum::CHAT] );

			//reaction;
			name = "Reaction_look_at";
			nodes[name] = Node( name, list<string>{ "True", "False" }, PLOT_TYPE_NAME[PlotEnum::REACTION] );
			name = "Reaction_react";
			nodes[name] = Node( name, list<string>{ "True", "False" }, PLOT_TYPE_NAME[PlotEnum::REACTION] );

			return nodes;
		}

		list<Arc> InitialPlotArcs( ) {
			list<Arc> arcs;

			//Weapon

			arcs.push_back( Arc( "Weapon_container", PLOT_TYPE_NAME[PlotEnum::WEAPON], "Weapon", PLOT_TYPE_NAME[PlotEnum::WEAPON] ) );
			arcs.push_back( Arc( "Weapon_contained_in_suspect", PLOT_TYPE_NAME[PlotEnum::WEAPON], "Weapon_container", PLOT_TYPE_NAME[PlotEnum::WEAPON] ) );
			arcs.push_back( Arc( "Weapon_contained_in_object", PLOT_TYPE_NAME[PlotEnum::WEAPON], "Weapon_container", PLOT_TYPE_NAME[PlotEnum::WEAPON] ) );
			arcs.push_back( Arc( "Weapon_marks", PLOT_TYPE_NAME[PlotEnum::WEAPON], "Weapon", PLOT_TYPE_NAME[PlotEnum::WEAPON] ) );
			arcs.push_back( Arc( "Weapon_meathod", PLOT_TYPE_NAME[PlotEnum::WEAPON], "Weapon", PLOT_TYPE_NAME[PlotEnum::WEAPON] ) );
			arcs.push_back( Arc( "Weapon_material", PLOT_TYPE_NAME[PlotEnum::WEAPON], "Weapon", PLOT_TYPE_NAME[PlotEnum::WEAPON] ) );
			arcs.push_back( Arc( "Weapon_needs_strength", PLOT_TYPE_NAME[PlotEnum::WEAPON], "Weapon_meathod", PLOT_TYPE_NAME[PlotEnum::WEAPON] ) );
			arcs.push_back( Arc( "Weapon_needs_strength", PLOT_TYPE_NAME[PlotEnum::WEAPON], "Victim_wound_type", PLOT_TYPE_NAME[PlotEnum::WEAPON] ) );
			arcs.push_back( Arc( "Victim_wound_type", PLOT_TYPE_NAME[PlotEnum::WEAPON], "Weapon_meathod", PLOT_TYPE_NAME[PlotEnum::WEAPON] ) );

			arcs.push_back( Arc( "Is_murder_weapon", PLOT_TYPE_NAME[PlotEnum::WEAPON], "Is_strength", PLOT_TYPE_NAME[PlotEnum::WEAPON] ) );
			arcs.push_back( Arc( "Is_murder_weapon", PLOT_TYPE_NAME[PlotEnum::WEAPON], "Is_marks", PLOT_TYPE_NAME[PlotEnum::WEAPON] ) );
			arcs.push_back( Arc( "Is_murder_weapon", PLOT_TYPE_NAME[PlotEnum::WEAPON], "Is_material", PLOT_TYPE_NAME[PlotEnum::WEAPON] ) );
			arcs.push_back( Arc( "Is_murder_weapon", PLOT_TYPE_NAME[PlotEnum::WEAPON], "Is_wound", PLOT_TYPE_NAME[PlotEnum::WEAPON] ) );
			arcs.push_back( Arc( "Is_murder_weapon", PLOT_TYPE_NAME[PlotEnum::WEAPON], "Is_weapons_hair", PLOT_TYPE_NAME[PlotEnum::WEAPON] ) );
			arcs.push_back( Arc( "Is_murder_weapon", PLOT_TYPE_NAME[PlotEnum::WEAPON], "Is_blood", PLOT_TYPE_NAME[PlotEnum::WEAPON] ) );

			arcs.push_back( Arc( "blood_on_weapon", PLOT_TYPE_NAME[PlotEnum::WEAPON], "Weapon_meathod", PLOT_TYPE_NAME[PlotEnum::WEAPON] ) );
			arcs.push_back( Arc( "Weapons_hair_colour", PLOT_TYPE_NAME[PlotEnum::WEAPON], "Hair_on_weapon", PLOT_TYPE_NAME[PlotEnum::WEAPON] ) );
			arcs.push_back( Arc( "Weapons_hair_colour", PLOT_TYPE_NAME[PlotEnum::WEAPON], "Suspect_hair_colour", PLOT_TYPE_NAME[PlotEnum::MURDERER] ) );
			arcs.push_back( Arc( "Weapons_hair_colour", PLOT_TYPE_NAME[PlotEnum::WEAPON], "Suspect_hair_colour", PLOT_TYPE_NAME[PlotEnum::VICTIM] ) );

			arcs.push_back( Arc( "Is_strength", PLOT_TYPE_NAME[PlotEnum::WEAPON], "Weapon_needs_strength", PLOT_TYPE_NAME[PlotEnum::WEAPON] ) );
			arcs.push_back( Arc( "Is_strength", PLOT_TYPE_NAME[PlotEnum::WEAPON], "Weapon_needs_strength", PLOT_TYPE_NAME[PlotEnum::MURDER_WEAPON] ) );
			arcs.push_back( Arc( "Is_marks", PLOT_TYPE_NAME[PlotEnum::WEAPON], "Weapon_marks", PLOT_TYPE_NAME[PlotEnum::WEAPON] ) );
			arcs.push_back( Arc( "Is_marks", PLOT_TYPE_NAME[PlotEnum::WEAPON], "Weapon_marks", PLOT_TYPE_NAME[PlotEnum::MURDER_WEAPON] ) );
			arcs.push_back( Arc( "Is_material", PLOT_TYPE_NAME[PlotEnum::WEAPON], "Weapon_material", PLOT_TYPE_NAME[PlotEnum::WEAPON] ) );
			arcs.push_back( Arc( "Is_material", PLOT_TYPE_NAME[PlotEnum::WEAPON], "Weapon_material", PLOT_TYPE_NAME[PlotEnum::MURDER_WEAPON] ) );
			arcs.push_back( Arc( "Is_wound", PLOT_TYPE_NAME[PlotEnum::WEAPON], "Weapon_wound_type", PLOT_TYPE_NAME[PlotEnum::WEAPON] ) );
			arcs.push_back( Arc( "Is_wound", PLOT_TYPE_NAME[PlotEnum::WEAPON], "Weapon_wound_type", PLOT_TYPE_NAME[PlotEnum::MURDER_WEAPON] ) );
			arcs.push_back( Arc( "Is_wound", PLOT_TYPE_NAME[PlotEnum::WEAPON], "Victim_wound_type", PLOT_TYPE_NAME[PlotEnum::WEAPON] ) );
			arcs.push_back( Arc( "Is_weapons_hair", PLOT_TYPE_NAME[PlotEnum::WEAPON], "Weapons_hair_colour", PLOT_TYPE_NAME[PlotEnum::WEAPON] ) );
			arcs.push_back( Arc( "Is_weapons_hair", PLOT_TYPE_NAME[PlotEnum::WEAPON], "Weapons_hair_colour", PLOT_TYPE_NAME[PlotEnum::MURDER_WEAPON] ) );
			arcs.push_back( Arc( "Is_blood", PLOT_TYPE_NAME[PlotEnum::WEAPON], "blood_on_weapon", PLOT_TYPE_NAME[PlotEnum::WEAPON] ) );
			arcs.push_back( Arc( "Is_blood", PLOT_TYPE_NAME[PlotEnum::WEAPON], "blood_on_weapon", PLOT_TYPE_NAME[PlotEnum::MURDER_WEAPON] ) );

			//Object 
			arcs.push_back( Arc( "Object_name", PLOT_TYPE_NAME[PlotEnum::OBJECT], "Object_position", PLOT_TYPE_NAME[PlotEnum::OBJECT] ) );
			arcs.push_back( Arc( "Object_lock", PLOT_TYPE_NAME[PlotEnum::OBJECT], "Object_name", PLOT_TYPE_NAME[PlotEnum::OBJECT] ) );
			arcs.push_back( Arc( "Object_key_in_lock", PLOT_TYPE_NAME[PlotEnum::OBJECT], "Object_lock", PLOT_TYPE_NAME[PlotEnum::OBJECT] ) );
			arcs.push_back( Arc( "Object_is_breakable", PLOT_TYPE_NAME[PlotEnum::OBJECT], "Object_name", PLOT_TYPE_NAME[PlotEnum::OBJECT] ) );
			arcs.push_back( Arc( "Object_broken", PLOT_TYPE_NAME[PlotEnum::OBJECT], "Object_is_breakable", PLOT_TYPE_NAME[PlotEnum::OBJECT] ) );
			arcs.push_back( Arc( "Object_glass", PLOT_TYPE_NAME[PlotEnum::OBJECT], "Object_broken", PLOT_TYPE_NAME[PlotEnum::OBJECT] ) );
			arcs.push_back( Arc( "Object_glass", PLOT_TYPE_NAME[PlotEnum::OBJECT], "Object_name", PLOT_TYPE_NAME[PlotEnum::OBJECT] ) );
			arcs.push_back( Arc( "Object_size", PLOT_TYPE_NAME[PlotEnum::OBJECT], "Object_name", PLOT_TYPE_NAME[PlotEnum::OBJECT] ) );
			arcs.push_back( Arc( "Object_is_container", PLOT_TYPE_NAME[PlotEnum::OBJECT], "Object_name", PLOT_TYPE_NAME[PlotEnum::OBJECT] ) );

			//Suspect
			arcs.push_back( Arc( "Suspect_size", PLOT_TYPE_NAME[PlotEnum::SUSPECT], "Suspect_sex", PLOT_TYPE_NAME[PlotEnum::SUSPECT] ) );
			arcs.push_back( Arc( "Suspect_strength", PLOT_TYPE_NAME[PlotEnum::SUSPECT], "Suspect_sex", PLOT_TYPE_NAME[PlotEnum::SUSPECT] ) );
			arcs.push_back( Arc( "Suspect_shoe_size", PLOT_TYPE_NAME[PlotEnum::SUSPECT], "Suspect_sex", PLOT_TYPE_NAME[PlotEnum::SUSPECT] ) );
			arcs.push_back( Arc( "Suspect_shoe_size", PLOT_TYPE_NAME[PlotEnum::SUSPECT], "Suspect_size", PLOT_TYPE_NAME[PlotEnum::SUSPECT] ) );

			arcs.push_back( Arc( "Suspect_is_sufficient_strength", PLOT_TYPE_NAME[PlotEnum::SUSPECT], "Suspect_strength", PLOT_TYPE_NAME[PlotEnum::SUSPECT] ) );
			arcs.push_back( Arc( "Suspect_is_sufficient_strength", PLOT_TYPE_NAME[PlotEnum::SUSPECT], "Weapon_needs_strength", PLOT_TYPE_NAME[PlotEnum::MURDER_WEAPON] ) );
			arcs.push_back( Arc( "Suspect_is_shoe_print_on_scene", PLOT_TYPE_NAME[PlotEnum::SUSPECT], "Suspect_shoe_size", PLOT_TYPE_NAME[PlotEnum::SUSPECT] ) );
			arcs.push_back( Arc( "Suspect_is_shoe_print_on_scene", PLOT_TYPE_NAME[PlotEnum::SUSPECT], "Scene_shoe_print", PLOT_TYPE_NAME[PlotEnum::MURDER_SCENE] ) );
			arcs.push_back( Arc( "Suspect_is_hair_found_on_victim", PLOT_TYPE_NAME[PlotEnum::SUSPECT], "Suspect_hair_colour", PLOT_TYPE_NAME[PlotEnum::SUSPECT] ) );
			arcs.push_back( Arc( "Suspect_is_hair_found_on_victim", PLOT_TYPE_NAME[PlotEnum::SUSPECT], "Suspect_hair_colour", PLOT_TYPE_NAME[PlotEnum::VICTIM] ) );
			arcs.push_back( Arc( "Suspect_is_blackmailer", PLOT_TYPE_NAME[PlotEnum::SUSPECT], "Suspect_knows_of_dark_secret", PLOT_TYPE_NAME[PlotEnum::SUSPECT] ) );
			arcs.push_back( Arc( "Suspect_is_blackmailer", PLOT_TYPE_NAME[PlotEnum::SUSPECT], "Victim_threatened_to_expose_blackmail", PLOT_TYPE_NAME[PlotEnum::SUSPECT] ) );
			arcs.push_back( Arc( "Suspect_was_blackmailed", PLOT_TYPE_NAME[PlotEnum::SUSPECT], "Suspect_has_dark_secret", PLOT_TYPE_NAME[PlotEnum::SUSPECT] ) );
			arcs.push_back( Arc( "Suspect_was_blackmailed", PLOT_TYPE_NAME[PlotEnum::SUSPECT], "Suspect_knows_of_dark_secret", PLOT_TYPE_NAME[PlotEnum::VICTIM] ) );
			arcs.push_back( Arc( "Suspect_knows_of_dark_secret", PLOT_TYPE_NAME[PlotEnum::SUSPECT], "Suspect_has_dark_secret", PLOT_TYPE_NAME[PlotEnum::VICTIM] ) );
			arcs.push_back( Arc( "Suspect_inheritance", PLOT_TYPE_NAME[PlotEnum::SUSPECT], "Suspect_heir", PLOT_TYPE_NAME[PlotEnum::SUSPECT] ) );
			arcs.push_back( Arc( "Suspect_inheritance", PLOT_TYPE_NAME[PlotEnum::SUSPECT], "Suspect_rich", PLOT_TYPE_NAME[PlotEnum::SUSPECT] ) );
			arcs.push_back( Arc( "Suspect_heir", PLOT_TYPE_NAME[PlotEnum::SUSPECT], "Victim_relation", PLOT_TYPE_NAME[PlotEnum::SUSPECT] ) );
			arcs.push_back( Arc( "Suspect_is_wedlocked", PLOT_TYPE_NAME[PlotEnum::SUSPECT], "Victim_relation", PLOT_TYPE_NAME[PlotEnum::SUSPECT] ) );
			arcs.push_back( Arc( "Suspect_is_adulterer", PLOT_TYPE_NAME[PlotEnum::SUSPECT], "Victim_relation", PLOT_TYPE_NAME[PlotEnum::SUSPECT] ) );
			arcs.push_back( Arc( "Victim_is_adulterer", PLOT_TYPE_NAME[PlotEnum::SUSPECT], "Victim_relation", PLOT_TYPE_NAME[PlotEnum::SUSPECT] ) );
			arcs.push_back( Arc( "Suspect_in_debt", PLOT_TYPE_NAME[PlotEnum::SUSPECT], "Suspect_rich", PLOT_TYPE_NAME[PlotEnum::SUSPECT] ) );
			arcs.push_back( Arc( "Suspect_is_swindled", PLOT_TYPE_NAME[PlotEnum::SUSPECT], "Suspect_is_swindler", PLOT_TYPE_NAME[PlotEnum::VICTIM] ) );
			arcs.push_back( Arc( "Suspect_is_swindled", PLOT_TYPE_NAME[PlotEnum::SUSPECT], "Suspect_lost_a_fortune", PLOT_TYPE_NAME[PlotEnum::VICTIM] ) );
			arcs.push_back( Arc( "Suspect_is_swindler", PLOT_TYPE_NAME[PlotEnum::SUSPECT], "Victim_threatened_to_expose_swindle", PLOT_TYPE_NAME[PlotEnum::SUSPECT] ) );
			arcs.push_back( Arc( "Suspect_wanted_revenge", PLOT_TYPE_NAME[PlotEnum::SUSPECT], "Suspect_lost_a_fortune", PLOT_TYPE_NAME[PlotEnum::SUSPECT] ) );
			arcs.push_back( Arc( "Suspect_wanted_revenge", PLOT_TYPE_NAME[PlotEnum::SUSPECT], "Suspect_sex", PLOT_TYPE_NAME[PlotEnum::SUSPECT] ) );
			arcs.push_back( Arc( "Suspect_has_motive", PLOT_TYPE_NAME[PlotEnum::SUSPECT], "Suspect_is_blackmailer", PLOT_TYPE_NAME[PlotEnum::SUSPECT] ) );
			arcs.push_back( Arc( "Suspect_has_motive", PLOT_TYPE_NAME[PlotEnum::SUSPECT], "Suspect_was_blackmailed", PLOT_TYPE_NAME[PlotEnum::SUSPECT] ) );
			arcs.push_back( Arc( "Suspect_has_motive", PLOT_TYPE_NAME[PlotEnum::SUSPECT], "Suspect_inheritance", PLOT_TYPE_NAME[PlotEnum::SUSPECT] ) );
			arcs.push_back( Arc( "Suspect_has_motive", PLOT_TYPE_NAME[PlotEnum::SUSPECT], "Suspect_is_adulterer", PLOT_TYPE_NAME[PlotEnum::SUSPECT] ) );
			arcs.push_back( Arc( "Suspect_has_motive", PLOT_TYPE_NAME[PlotEnum::SUSPECT], "Victim_is_adulterer", PLOT_TYPE_NAME[PlotEnum::SUSPECT] ) );
			arcs.push_back( Arc( "Suspect_has_motive", PLOT_TYPE_NAME[PlotEnum::SUSPECT], "Suspect_in_debt", PLOT_TYPE_NAME[PlotEnum::SUSPECT] ) );
			arcs.push_back( Arc( "Suspect_has_motive", PLOT_TYPE_NAME[PlotEnum::SUSPECT], "Suspect_is_swindled", PLOT_TYPE_NAME[PlotEnum::SUSPECT] ) );
			arcs.push_back( Arc( "Suspect_has_motive", PLOT_TYPE_NAME[PlotEnum::SUSPECT], "Suspect_is_swindler", PLOT_TYPE_NAME[PlotEnum::SUSPECT] ) );
			arcs.push_back( Arc( "Suspect_is_murderer", PLOT_TYPE_NAME[PlotEnum::SUSPECT], "Suspect_has_motive", PLOT_TYPE_NAME[PlotEnum::SUSPECT] ) );
			arcs.push_back( Arc( "Suspect_is_murderer", PLOT_TYPE_NAME[PlotEnum::SUSPECT], "Suspect_has_opportunity", PLOT_TYPE_NAME[PlotEnum::SUSPECT] ) );
			arcs.push_back( Arc( "Suspect_is_murderer", PLOT_TYPE_NAME[PlotEnum::SUSPECT], "Suspect_has_means", PLOT_TYPE_NAME[PlotEnum::SUSPECT] ) );

			return arcs;
		}

		list<pair<string, PlotEnum>> TotalogyDefinitions( ) {
			list<pair<string, PlotEnum>> totologies;
			totologies.push_back( make_pair( "Suspect_is_murderer", PlotEnum::SUSPECT ) );
			totologies.push_back( make_pair( "Suspect_has_motive", PlotEnum::SUSPECT ) );
			totologies.push_back( make_pair( "Suspect_has_opportunity", PlotEnum::SUSPECT ) );
			totologies.push_back( make_pair( "Suspect_has_means", PlotEnum::SUSPECT ) );
			totologies.push_back( make_pair( "Is_murder_weapon", PlotEnum::WEAPON ) );
			return totologies;
		}
	};
}


#endif // !SETTINGS_H
