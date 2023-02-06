//textrpg
#include <iostream>
#include <string>
using namespace std;

//EnterLobby
//CreatePlayer
//EnterGame
//CresteMonsters
//EnterBattle

enum playertype {
	Knight=1,
	magition=2,
	dragonKnight=3
};
enum monstertype {
	oak = 1,
	skeleton = 2,
	dragon=3
};
struct StatInfo {
	int hp = 0;
	int att = 0;
	int df = 0;
};
void EnterLobby();
void CreatePlayer(StatInfo* player);
void EnterGame(StatInfo* player);
void CresteMonsters(StatInfo monster[], const int count);
void outputText(const char* msg);
void PrintStatInfo(StatInfo* info, const char* what);
void Enterbattle(StatInfo* player, StatInfo* monster, int& live);
bool check(int num, int live[]);
int main() {
	srand((unsigned)time(nullptr));

	EnterLobby();

	StatInfo player;
	CreatePlayer(&player);
	EnterGame(&player);
	
	return 0;
}
void EnterLobby() {
	outputText("로비에 입장하였습니다.");
}
void outputText(const char* msg) {
	cout <<"================================" << endl;
	cout << msg << endl;
	cout << "================================" << endl;
}
void CreatePlayer(StatInfo* player) {
	outputText("플레이어 생성");
	outputText("[1] 나이트 [2] 흑마 [3] 용기사");
	int input;
	cin >> input;

	switch (input) {
	case Knight:
		player->hp = 100;
		player->att = 10;
		player->df = 40;
		break;
	case magition:
		player->hp = 60;
		player->att = 40;
		player->df = 20;
		break;
	case dragonKnight:
		player->hp = 80;
		player->att = 30;
		player->df = 30;
		break;
	}
}
void EnterGame(StatInfo* player) {
	const int MONSTERCOUNT = 2;
	outputText("게임에 입장하였습니다.");
	StatInfo monsterinfo[MONSTERCOUNT];
	int live[MONSTERCOUNT] = { 1,1 };
	CresteMonsters(monsterinfo, MONSTERCOUNT);



	while (true) {
		bool o = check(MONSTERCOUNT, live);

		if (o == false) {
			outputText("몬스터 모두 사망 게임종료");
			return;
		}
		outputText("[1] 공격 [2] 도망");
		int input;
		cin >> input;
		if (input == 2)
			break;

		outputText("공격할 몬스터 선택!");
		for (int i = 0; i < MONSTERCOUNT; i++)
			PrintStatInfo(&monsterinfo[i], "monster");
		cout << "================================" << endl;
		cin >> input;
		cout<< "================================" << endl;
		cout << input << "번 공격!" << endl;
		
		Enterbattle(player, &monsterinfo[input-1], live[input-1]);

		if (player->hp <= 0)
		{
			outputText("플레이어 사망으로 게임 종료");
			break;
		}
		
	}

}
void CresteMonsters(StatInfo monster[], const int count) {
	outputText("몬스터 생성.");
	for (int i = 0; i < count; i++) {
		int num = 1 + rand() % 3;
		switch (num)
		{
		case oak:
			monster[i].hp = 3;
			monster[i].att = 1;
			monster[i].df = 1;
			break;
		case skeleton:
			monster[i].hp = 5;
			monster[i].att = 2;
			monster[i].df = 2;
			break;
		case dragon:
			monster[i].hp = 10;
			monster[i].att = 30;
			monster[i].df = 50;
			break;
		default:
			break;
		}
	}
}
void PrintStatInfo(StatInfo* info, const char* what) {
	cout << what << "의 info" << endl;
	cout << "HP: " << info->hp << "ATT: " << info->att << "DF: " << info->df << endl;
}
void Enterbattle(StatInfo* player, StatInfo* monster, int& live) {

	if (live == 0) {
		cout << "몬스터 이미 사망" << endl;
		return;
	}
	if (monster->df >= player->att) {
		monster->df -= player->att;
	}
	else {
		monster->hp -= (player->att - monster->df);
		monster->df = 0;
	}
	if (monster->hp <= 0) {
		live = 0;
		monster->hp = 0;
		cout << "몬스터 사망" << endl;
		return;
	}
	else {
		PrintStatInfo(monster, "monster");
	}

	if (player->df >= monster->att) {
		player->df -= monster->att;
	}
	else {
		player->hp -= (monster->att - player->df);
		player->df = 0;
	}
	if (player->hp <= 0) {
		cout << "플레이어 사망" << endl;
		outputText("게임 종료");
		return;
	}

}
bool check(int num, int live[]) {
	for (int i = 0; i < num; i++) {
		if (live[i] == 1)
			return true;
			
	}
	return false;
}