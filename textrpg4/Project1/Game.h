#pragma once
//#include "Player.h" 
//���� ����: Player�� ���߿� ������ ���̴� �� �̸��� ���� ���͵� �����ض�
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

	//Player _player; �����Ҷ� _player�� ������ �ʿ�, ������ ����, ������ Player�� �ʿ�
	//Player* _player; �����Ҷ� �ּҹٱ��� ����, Player ���� �������� �ʴ��� �ٱ��ϸ� �����ϸ�Ǽ� ������

	Player* _player;
	Field* _field;
};

