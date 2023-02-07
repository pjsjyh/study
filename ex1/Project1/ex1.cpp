#include <iostream>
using namespace std;
//문자열 길이
int Strlen(const char* str) {
	int i = 0;

	while (str[i] != '\0') {
		i++;
	}
	return i;
}
//문자열 복사
void Strcpy(char* b, const char* a) {
	int i = 0;
	while (a[i] != '\0') {
		b[i] = a[i];
		i++;
	}
	b[i] = '\0';
	return;


	///포인트 버젼
	char* ret = b;
	while (*a != '\0') {
		*b++ = *a++;

	}
	*b = '\0';
	//return ret;  포인트 처음값으로 돌려 반환

}


void Strcat(char* c, const char* a) {
	int i = 0;
	int len = Strlen(c);
	while (a[i] != '\0'){
		c[len+i] = a[i];
		i++;

	}
	c[len + i] = '\0';
}

int Strcmp(const char* a, const char* b) {
	int i = 0;
	while (a[i] != '\0' || b[i] != '\0') {
		if (a[i] > b[i])
			return 1;
		if (a[i] < b[i])
			return -1;
		i++;
	}
	return 0;
}

void ReverseStr(char* a) {
	int i = 0;
	int len = Strlen(a);
	
	for (int i = 0; i < len / 2; i++) {
		int temp = a[i];
		a[i] = a[len - 1 - i];
		a[len - i - 1] = temp;
	}
}

int main() {
	const int BUF_SIZE = 100;

	char a[BUF_SIZE] = "Hello";
	char b[BUF_SIZE];
	char c[BUF_SIZE]="World";
	int len = Strlen(a);
	//cout << len;

	Strcpy(b, a);
	//cout << b;

	//Strcat(c, a);
	//cout << c;

	//int value = Strcmp(a, c);
	//cout << value;

	ReverseStr(a);
	cout << a << endl;
	return 0;
}