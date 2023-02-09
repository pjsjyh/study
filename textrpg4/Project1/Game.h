#pragma once
//#include "Player.h" 
//전방 선언: Player가 나중에 등장할 것이니 이 이름이 먼저 나와도 감안해라
class Player;
class Field;
class Game
{
public:
	Game();
	~Game();

	void Init();
	void Update();
	void CreatePlayer();
private:

	//Player _player; 생성할때 _player값 무조건 필요, 안좋은 설계, 완전한 Player가 필요
	//Player* _player; 생성할때 주소바구니 생성, Player 값이 완전하지 않더라도 바구니만 생성하면되서 괜찮다

	Player* _player;
	Field* _field;
};

