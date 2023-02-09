#pragma once

enum CreatureType {
	CT_PLAYER=1,
	CT_MONSTER=1
};

class Creature
{
public:
	

	Creature(int creature) 
		:_creatureType(creature),_hp(0),_attack(0),_defence(0) 
	{

	}
	virtual ~Creature() {

	}
	virtual void PrintInfo() = 0;//추상가상함수 반드시 다른곳에서 재정의해서 사용해야됨

	void OnAttacked(Creature* attacker);
	bool IsDead() { return _hp <= 0; }
protected:
	int _creatureType;
	int _hp;
	int _attack;
	int _defence;
};

