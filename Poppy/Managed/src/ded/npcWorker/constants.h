//constants.h

#ifndef NPC_CONSTANTS_H
#define NPC_CONSTANTS_H

#include <map>

using namespace std;

namespace NPCConstants {
	const int NumberOfMaleNames = 1177169;
	const int NumberOfFemaleNames = 1399571;

	map<string, int> maleNames{
		{ "John", 89950 },
		{ "William", 84881 },
		{ "James", 54056 },
		{ "George", 47651 },
		{ "Charles", 46656 },
		{ "Frank", 30967 },
		{ "Joseph", 26292 },
		{ "Henry", 24139 },
		{ "Robert", 24074 },
		{ "Thomas", 23750 },
		{ "Edward", 23133 },
		{ "Harry", 22649 },
		{ "Walter", 18186 },
		{ "Arthur", 16180 },
		{ "Fred", 15602 },
		{ "Albert", 14375 },
		{ "Samuel", 9129 },
		{ "Clarence", 8760 },
		{ "Louis", 8275 },
		{ "David", 7569 },
		{ "Joe", 7097 },
		{ "Charlie", 7091 },
		{ "Richard", 7010 },
		{ "Ernest", 6700 },
		{ "Roy", 6565 },
		{ "Will", 6196 },
		{ "Andrew", 6053 },
		{ "Jesse", 6003 },
		{ "Oscar", 5982 },
		{ "Willie", 5906 },
		{ "Daniel", 5680 },
		{ "Benjamin", 5407 },
		{ "Carl", 5227 },
		{ "Sam", 5069 },
		{ "Alfred", 4837 },
		{ "Earl", 4830 },
		{ "Peter", 4656 },
		{ "Elmer", 4627 },
		{ "Frederick", 4603 },
		{ "Howard", 4349 },
		{ "Lewis", 4342 },
		{ "Ralph", 4269 },
		{ "Herbert", 4257 },
		{ "Paul", 4197 },
		{ "Lee", 3945 },
		{ "Tom", 3786 },
		{ "Herman", 3689 },
		{ "Martin", 3595 },
		{ "Jacob", 3566 },
		{ "Michael", 3559 },
		{ "Jim", 3520 },
		{ "Claude", 3504 },
		{ "Ben", 3449 },
		{ "Eugene", 3430 },
		{ "Francis", 3244 },
		{ "Grover", 3184 },
		{ "Raymond", 3135 },
		{ "Harvey", 3123 },
		{ "Clyde", 3034 },
		{ "Edwin", 3011 },
		{ "Edgar", 3006 },
		{ "Ed", 2957 },
		{ "Lawrence", 2919 },
		{ "Bert", 2808 },
		{ "Chester", 2699 },
		{ "Jack", 2684 },
		{ "Otto", 2679 },
		{ "Luther", 2619 },
		{ "Charley", 2550 },
		{ "Guy", 2489 },
		{ "Floyd", 2476 },
		{ "Ira", 2379 },
		{ "Ray", 2371 },
		{ "Hugh", 2355 },
		{ "Isaac", 2311 },
		{ "Oliver", 2287 },
		{ "Patrick", 2285 },
		{ "Homer", 2213 },
		{ "Theodore", 2176 },
		{ "Leonard", 2144 },
		{ "Leo", 2098 },
		{ "Alexander", 2081 },
		{ "August", 2012 },
		{ "Harold", 2001 },
		{ "Allen", 1998 },
		{ "Jessie", 1843 },
		{ "Archie", 1816 },
		{ "Philip", 1741 },
		{ "Stephen", 1684 },
		{ "Horace", 1650 },
		{ "Marion", 1624 },
		{ "Bernard", 1616 },
		{ "Anthony", 1611 },
		{ "Julius", 1605 },
		{ "Warren", 1580 },
		{ "Leroy", 1574 },
		{ "Clifford", 1552 },
		{ "Eddie", 1526 },
		{ "Sidney", 1505 },
		{ "Milton", 1479 },
		{ "Leon", 1475 },
		{ "Alex", 1451 },
		{ "Lester", 1440 },
		{ "Emil", 1434 },
		{ "Dan", 1402 },
		{ "Willis", 1388 },
		{ "Everett", 1374 },
		{ "Dave", 1339 },
		{ "Leslie", 1292 },
		{ "Rufus", 1283 },
		{ "Alvin", 1256 },
		{ "Perry", 1255 },
		{ "Lloyd", 1254 },
		{ "Victor", 1242 },
		{ "Calvin", 1236 },
		{ "Harrison", 1235 },
		{ "Norman", 1235 },
		{ "Wesley", 1186 },
		{ "Jess", 1182 },
		{ "Percy", 1166 },
		{ "Amos", 1161 },
		{ "Dennis", 1107 },
		{ "Jerry", 1106 },
		{ "Nathan", 1106 },
		{ "Franklin", 1098 },
		{ "Alonzo", 1088 },
		{ "Matthew", 1068 },
		{ "Mack", 1062 },
		{ "Earnest", 1043 },
		{ "Gus", 1029 },
		{ "Russell", 1023 },
		{ "Adam", 1020 },
		{ "Jay", 1016 },
		{ "Wallace", 1009 },
		{ "Otis", 1006 },
		{ "Stanley", 997 },
		{ "Adolph", 994 },
		{ "Jake", 972 },
		{ "Roscoe", 969 },
		{ "Maurice", 961 },
		{ "Melvin", 960 },
		{ "Gilbert", 957 },
		{ "Ross", 951 },
		{ "Willard", 939 },
		{ "Mark", 928 },
		{ "Levi", 927 },
		{ "Wilbur", 914 },
		{ "Cornelius", 911 },
		{ "Jose", 911 },
		{ "Aaron", 910 },
		{ "Elbert", 905 },
		{ "Emmett", 901 },
		{ "Phillip", 898 },
		{ "Morris", 895 },
		{ "Noah", 895 },
		{ "Claud", 881 },
		{ "Clinton", 867 },
		{ "Felix", 863 },
		{ "Moses", 859 },
		{ "Elijah", 856 },
		{ "Nelson", 849 },
		{ "Simon", 841 },
		{ "Lonnie", 832 },
		{ "Virgil", 827 },
		{ "Hiram", 825 },
		{ "Jasper", 820 },
		{ "Marshall", 804 },
		{ "Manuel", 802 },
		{ "Sylvester", 796 },
		{ "Fredrick", 783 },
		{ "Mike", 779 },
		{ "Abraham", 776 },
		{ "Silas", 775 },
		{ "Irvin", 770 },
		{ "Max", 765 },
		{ "Owen", 758 },
		{ "Christopher", 752 },
		{ "Reuben", 742 },
		{ "Glenn", 731 },
		{ "Nicholas", 730 },
		{ "Ellis", 728 },
		{ "Marvin", 726 },
		{ "Wiley", 726 },
		{ "Eli", 721 },
		{ "Edmund", 708 },
		{ "Ollie", 706 },
		{ "Cecil", 705 },
		{ "Cleveland", 702 },
		{ "Curtis", 695 },
		{ "Timothy", 693 },
		{ "Harley", 685 },
		{ "Jeff", 685 },
		{ "Anton", 674 },
		{ "Alva", 663 },
		{ "Wilson", 660 },
		{ "Irving", 654 },
		{ "Clayton", 653 },
		{ "Rudolph", 652 },
		{ "Vernon", 636 },
		{ "Hubert", 634 }
	};

	map<string, int> femaleNames{

		{ "Mary", 91668 },
		{ "Anna", 38159 },
		{ "Emma", 25404 },
		{ "Elizabeth", 25006 },
		{ "Margaret", 21799 },
		{ "Minnie", 21724 },
		{ "Ida", 18283 },
		{ "Bertha", 18263 },
		{ "Clara", 17717 },
		{ "Alice", 17142 },
		{ "Annie", 17027 },
		{ "Florence", 16699 },
		{ "Bessie", 15373 },
		{ "Grace", 15227 },
		{ "Ethel", 14866 },
		{ "Sarah", 14715 },
		{ "Ella", 13936 },
		{ "Martha", 13911 },
		{ "Nellie", 13761 },
		{ "Mabel", 13096 },
		{ "Laura", 12806 },
		{ "Carrie", 12514 },
		{ "Cora", 11954 },
		{ "Helen", 11496 },
		{ "Maude", 11453 },
		{ "Lillian", 11270 },
		{ "Gertrude", 11100 },
		{ "Rose", 11064 },
		{ "Edna", 11020 },
		{ "Pearl", 10903 },
		{ "Edith", 10881 },
		{ "Jennie", 10220 },
		{ "Hattie", 10199 },
		{ "Mattie", 9844 },
		{ "Eva", 9800 },
		{ "Julia", 9800 },
		{ "Myrtle", 9769 },
		{ "Louise", 9596 },
		{ "Lillie", 9467 },
		{ "Jessie", 9186 },
		{ "Frances", 9182 },
		{ "Catherine", 8915 },
		{ "Lula", 8882 },
		{ "Lena", 8691 },
		{ "Marie", 8490 },
		{ "Ada", 8423 },
		{ "Josephine", 7732 },
		{ "Fannie", 7527 },
		{ "Lucy", 7520 },
		{ "Dora", 7288 },
		{ "Agnes", 7237 },
		{ "Maggie", 7072 },
		{ "Blanche", 6925 },
		{ "Katherine", 6922 },
		{ "Elsie", 6647 },
		{ "Nora", 6521 },
		{ "Mamie", 6330 },
		{ "Rosa", 6157 },
		{ "Stella", 6130 },
		{ "Daisy", 6054 },
		{ "May", 5881 },
		{ "Effie", 5850 },
		{ "Mae", 5647 },
		{ "Ellen", 5402 },
		{ "Nettie", 5313 },
		{ "Ruth", 5298 },
		{ "Alma", 5190 },
		{ "Della", 5048 },
		{ "Lizzie", 5035 },
		{ "Sadie", 4915 },
		{ "Sallie", 4840 },
		{ "Nancy", 4792 },
		{ "Susie", 4780 },
		{ "Maud", 4201 },
		{ "Flora", 4195 },
		{ "Irene", 4002 },
		{ "Etta", 3973 },
		{ "Katie", 3907 },
		{ "Lydia", 3902 },
		{ "Lottie", 3888 },
		{ "Viola", 3806 },
		{ "Caroline", 3743 },
		{ "Addie", 3739 },
		{ "Hazel", 3644 },
		{ "Georgia", 3540 },
		{ "Esther", 3476 },
		{ "Mollie", 3471 },
		{ "Olive", 3456 },
		{ "Willie", 3397 },
		{ "Harriet", 3369 },
		{ "Emily", 3368 },
		{ "Charlotte", 3311 },
		{ "Amanda", 3280 },
		{ "Kathryn", 3276 },
		{ "Lulu", 3253 },
		{ "Susan", 3231 },
		{ "Kate", 3194 },
		{ "Nannie", 3165 },
		{ "Jane", 2971 },
		{ "Amelia", 2957 },
		{ "Virginia", 2948 },
		{ "Mildred", 2919 },
		{ "Beulah", 2866 },
		{ "Eliza", 2851 },
		{ "Rebecca", 2849 },
		{ "Ollie", 2832 },
		{ "Belle", 2648 },
		{ "Ruby", 2603 },
		{ "Pauline", 2593 },
		{ "Matilda", 2533 },
		{ "Theresa", 2505 },
		{ "Hannah", 2480 },
		{ "Henrietta", 2480 },
		{ "Ora", 2391 },
		{ "Estella", 2321 },
		{ "Leona", 2151 },
		{ "Augusta", 2145 },
		{ "Eleanor", 2136 },
		{ "Rachel", 2128 },
		{ "Amy", 2127 },
		{ "Sara", 2127 },
		{ "Anne", 2126 },
		{ "Marion", 2109 },
		{ "Iva", 2108 },
		{ "Ann", 2104 },
		{ "Nina", 2093 },
		{ "Dorothy", 2088 },
		{ "Lola", 2054 },
		{ "Lela", 1915 },
		{ "Beatrice", 1900 },
		{ "Josie", 1884 },
		{ "Sophia", 1865 },
		{ "Estelle", 1841 },
		{ "Mayme", 1841 },
		{ "Barbara", 1820 },
		{ "Evelyn", 1817 },
		{ "Maria", 1732 },
		{ "Inez", 1633 },
		{ "Allie", 1606 },
		{ "Essie", 1554 },
		{ "Delia", 1546 },
		{ "Mable", 1544 },
		{ "Millie", 1527 },
		{ "Alta", 1523 },
		{ "Betty", 1519 },
		{ "Callie", 1515 },
		{ "Janie", 1491 },
		{ "Rosie", 1485 },
		{ "Victoria", 1440 },
		{ "Ola", 1421 },
		{ "Gladys", 1420 },
		{ "Louisa", 1420 },
		{ "Ina", 1406 },
		{ "Eula", 1396 },
		{ "Luella", 1384 },
		{ "Vera", 1365 },
		{ "Lou", 1364 },
		{ "Celia", 1363 },
		{ "Nell", 1345 },
		{ "Goldie", 1327 },
		{ "Winifred", 1309 },
		{ "Bettie", 1298 },
		{ "Hilda", 1280 },
		{ "Sophie", 1267 },
		{ "Christine", 1266 },
		{ "Marguerite", 1256 },
		{ "Tillie", 1254 },
		{ "Birdie", 1237 },
		{ "Rena", 1219 },
		{ "Eunice", 1216 },
		{ "Bertie", 1153 },
		{ "Olga", 1116 },
		{ "Sylvia", 1109 },
		{ "Lucille", 1108 },
		{ "Bess", 1099 },
		{ "Isabelle", 1092 },
		{ "Genevieve", 1088 },
		{ "Leila", 1079 },
		{ "Mathilda", 1065 },
		{ "Dollie", 1063 },
		{ "Isabel", 1053 },
		{ "Verna", 1052 },
		{ "Bernice", 1037 },
		{ "Loretta", 1036 },
		{ "Rhoda", 1024 },
		{ "Cornelia", 1020 },
		{ "Sally", 1010 },
		{ "Jean", 1000 },
		{ "Alberta", 992 },
		{ "Winnie", 989 },
		{ "Lelia", 987 },
		{ "Lois", 954 },
		{ "Myra", 953 },
		{ "Harriett", 938 },
		{ "Roxie", 933 },
		{ "Adeline", 932 },
		{ "Abbie", 929 },
		{ "Flossie", 924 },
		{ "Sue", 918 },
		{ "Christina", 916 }
	};

	//origin, name, frequency
	map<string, map<string, int>> surnames{
		{ "Occupation", {
				{ "Smith", 253600 },
				{ "Taylor", 124400 },
				{ "Wright", 62700 },
				{ "Walker", 59300 },
				{ "Turner", 56300 },
				{ "Clark", 50700 },
				{ "Cooper", 48400 },
				{ "Ward", 45700 },
				{ "Baker", 43600 },
				{ "Clarke", 38100 },
				{ "Cook", 38100 },
				{ "Parker", 39100 },
				{ "Carter", 33400 }
			}
		},
		{
			"Christian / Forename", {
				{ "Jones", 242100 },
				{ "Williams", 159900 },
				{ "Davies", 113600 },
				{ "Thomas", 94000 },
				{ "Evans", 93000 },
				{ "Roberts", 78400 },
				{ "Johnson", 69500 },
				{ "Robinson", 66700 },
				{ "Wilson", 66800 },
				{ "Hughes", 59000 },
				{ "Lewis", 58000 },
				{ "Edwards", 58100 },
				{ "Thompson", 606000 },
				{ "Jackson", 55800 },
				{ "Harris", 51900 },
				{ "Harrison", 47200 },
				{ "Davis", 43700 },
				{ "Martin", 43900 },
				{ "Morris", 43400 },
				{ "James", 43100 },
				{ "Morgan", 41000 },
				{ "Allen", 40500 },
				{ "Price", 37900 },
				{ "Phillips", 37900 }
			}
		},
		{
			"Peculiarities", {
				{ "Brown", 105600 },
				{ "White", 56900 }
			}
		},
		{
			"Locality", {
				{ "Wood", 61200 },
				{ "Hall", 60400 },
				{ "Green", 59400 },
				{ "Hill", 52200 },
				{ "Moore", 39300 },
				{ "Shaw", 36500 },
				{ "Lee", 35200 }
			}
		},
		{
			"Other", {
				{ "King", 42300 },
			}
		},
		{
			"Christian", {
				{ "Watson", 34800 },
				{ "Bennett", 35800 },
				{ "Griffiths", 34800 }
			}
		}

	};
}


#endif // !NPC_CONSTANTS_H