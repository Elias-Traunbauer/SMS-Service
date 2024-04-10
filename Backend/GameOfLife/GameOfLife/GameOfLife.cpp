#include <iostream>
#include <chrono>
#include <thread>

const int rows = 51;
const int cols = 51;

struct Cell {
	int row;
	int col;
};

void set(int* board, int row, int col, int value) {
	board[row * cols + col] = value;
}

int get(int* board, int row, int col) {
	return board[row * cols + col];
}

int countNeighbors(int* board, int row, int col) {
	int count = 0;
	for (int i = row - 1; i <= row + 1; i++) {
		for (int j = col - 1; j <= col + 1; j++) {
			if (i >= 0 && i < rows && j >= 0 && j < cols) {
				if (i != row || j != col) {
					count += get(board, i, j);
				}
			}
		}
	}
	return count;
}

void logic(int* board) {
	int newBoard[rows * cols];
	for (int i = 0; i < rows; i++) {
		for (int j = 0; j < cols; j++) {
		int neighbors = countNeighbors(board, i, j);
		if (get(board, i, j)) {
			if (neighbors < 2) {
				set(newBoard, i, j, 0);
			}
			else if (neighbors == 2 || neighbors == 3) {
				set(newBoard, i, j, 1);
			}
			else {
				set(newBoard, i, j, 0);
			}
		}
		else {
			if (neighbors == 3) {
				set(newBoard, i, j, 1);
			}
			else {
				set(newBoard, i, j, 0);
			}
		}
	}
	}
		for (int i = 0; i < rows * cols; i++) {
		board[i] = newBoard[i];
	}
}

void draw(int* board) {
	system("CLS");
	for (int i = 0; i < rows; i++) {
		for (int j = 0; j < cols; j++) {
			if (get(board, i, j)) {
				std::cout << "##";
			}
			else {
				std::cout << "  ";
			}
		}
		std::cout << std::endl;
	}
}

int main()
{
	Cell cell = { 0, 0 };

	cell.row = 1;
	int* cellPtr = &cell.row;
	int value = *cellPtr;

	int board[rows * cols];

	for (int i = 0; i < rows * cols; i++) {
		board[i] = 0;
	}

	board[cols * (rows / 2) + cols / 2 + 1] = 1;
	board[cols * (rows / 2) + cols / 2] = 1;
	board[cols * (rows / 2) + cols / 2 - 1] = 1;
	board[cols * (rows / 2 + 1) + cols / 2] = 1;
	board[cols * (rows / 2 - 1) + cols / 2] = 1;
	board[cols * (rows / 2 - 3) + cols / 2] = 1;
	board[cols * (rows / 2 - 4) + cols / 2] = 1;
	board[cols * (rows / 2 - 5) + cols / 2] = 1;

	int* boardPtr = &board[0];

	for (;;) {
		draw(boardPtr);
		logic(boardPtr);
	}
}