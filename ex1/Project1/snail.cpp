#include <iostream>
using namespace std;
#include <iomanip> //input output조작 
const int MAX = 100;
int board[MAX][MAX] = {};
int N;

void PrintBoard() {
	for (int y = 0; y < N; y++) {
		for (int x = 0; x < N; x++) {
			cout <<setfill('0')<<setw(2)<< board[y][x]<<" "; //두자리 고정
		}
		cout << endl;
	}
}

void SetBoard() {
	int num = 1;
	int y=0, x = 0;
	int k = N / 2+N%2;

	for (int j = 0; j < k;j++) {
		while (x<N-j) {
			board[y][x] = num;
			x++;
			num += 1;
		}

		x--;
		y++;
		while (y< N-j) {
			board[y][x] = num;
			y++;
			num += 1;
		}
		y--;
		x--;
		while (x >0+j) {
			board[y][x] = num;
			num += 1;
			x--;
		}
		while (y >0+j) {
			board[y][x] = num;
			num += 1;
			y--;
		}
		x++;
		y++;
	}
}
enum DIR {
	RIGHT=0,
	DOWN=1,
	LEFT=2,
	UP=3,
};

bool cango(int y, int x) {
	if (y < 0 || y >= N)
		return false;
	if (x < 0 || x >= N)
		return false;
	if (board[y][x] != 0)
		return false;
	return true;
}

void sboard() {
	int dir = RIGHT;
	int num = 1;
	int y = 0;
	int x = 0;

	int dy[] = { 0,1,0,-1 };
	int dx[] = { 1,0,-1,0 };

	while (true) {
		board[y][x] = num;


		if (num == N * N)
			break;

		int nextY;
		int nextX;

		//int nextY = y+dy[dir];         자료구조방식 아래 switch대신 사용가능
		//int nextX = x+dx[dir];

		switch (dir)
		{
		case RIGHT:
			nextY = y;
			nextX = x + 1;
			break;
		case DOWN:
			nextY = y+1;
			nextX = x;
			break;
		case LEFT:
			nextY = y;
			nextX = x-1;
			break;
		case UP:
			nextY = y-1;
			nextX = x ;
			break;
		
		}



		if (cango(nextY, nextX)) {
			y = nextY;
			x = nextX;
			num++;
		}
		else {

			//dir = (dir + 1) % 4; 스위치 대신 간략

			switch (dir)
			{
			case RIGHT:
				dir = DOWN;
				break;
			case DOWN:
				dir = LEFT;
				break;
			case LEFT:
				dir = UP;
				break;
			case UP:
				dir = RIGHT;
				break;
			default:
				break;
			}
		}

	}

}



int main() {
	cin >> N;
	//SetBoard();
	sboard();
	PrintBoard();
	return 0;
}