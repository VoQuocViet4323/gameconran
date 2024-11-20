using System;
using System.Collections.Generic;
using System.Threading;

class Game
{
    private int screenWidth = 40;
    private int screenHeight = 20;
    private int score = 0;
    private bool gameOver = false; //false: trò chơi đang chạy, true: trò chơi kết thúc
    private bool canPassThroughWalls = false; // false: ko thể xuyên tường, true: có xuyên tường
    private ConsoleKey direction = ConsoleKey.RightArrow;
    private int speed = 100;
    private char[,] currentMap;
    private List<Position> snake;
    private Position food;
    private Random random;

    public Game()
    {
        snake = new List<Position>();
        random = new Random();
    }

    public void ShowMenu()
    {
        bool exitMenu = false;

        while (!exitMenu)
        {
            Console.Clear();
            Console.WriteLine("Chào mừng đến với trò chơi con rắn!");
            Console.WriteLine("Chọn map:");
            Console.WriteLine("1. Map cơ bản (không chướng ngại)");
            Console.WriteLine("2. Map có tường bên trong");
            Console.WriteLine("3. Thoát");

            string choice = Console.ReadLine();
            switch (choice)
            {
                case "1":
                    MapLoader.LoadBasicMap(ref currentMap, screenWidth, screenHeight);
                    break;
                case "2":
                    MapLoader.LoadComplexMap(ref currentMap, screenWidth, screenHeight);
                    break;
                case "3":
                    exitMenu = true;
                    continue; // Quay lại đầu vòng lặp để thoát khỏi menu
                default:
                    Console.WriteLine("Lựa chọn không hợp lệ. Hãy thử lại.");
                    continue;
            }

            Console.Clear();
            Console.WriteLine("Chọn chế độ:");
            Console.WriteLine("1. Không xuyên tường");
            Console.WriteLine("2. Xuyên tường");
            Console.WriteLine("3. Quay lại");

            bool validMode = false;
            while (!validMode)
            {
                string modeChoice = Console.ReadLine();
                switch (modeChoice)
                {
                    case "1":
                        canPassThroughWalls = false;
                        validMode = true;
                        break;
                    case "2":
                        canPassThroughWalls = true;
                        validMode = true;
                        break;
                    case "3":
                        goto RestartMenu; // Quay lại menu map
                    default:
                        Console.WriteLine("Lựa chọn không hợp lệ. Hãy thử lại.");
                        break;
                }
            }

            Console.Clear();
            Console.WriteLine("Chọn chế độ tốc độ:");
            Console.WriteLine("1. Chậm (200ms)");
            Console.WriteLine("2. Trung bình (100ms)");
            Console.WriteLine("3. Nhanh (50ms)");
            Console.WriteLine("4. Quay lại");

            bool validSpeed = false;
            while (!validSpeed)
            {
                string speedChoice = Console.ReadLine();
                switch (speedChoice)
                {
                    case "1":
                        speed = 200;
                        validSpeed = true;
                        break;
                    case "2":
                        speed = 100;
                        validSpeed = true;
                        break;
                    case "3":
                        speed = 50;
                        validSpeed = true;
                        break;
                    case "4":
                        goto RestartMode; // Quay lại menu chế độ
                    default:
                        Console.WriteLine("Lựa chọn không hợp lệ. Hãy thử lại.");
                        break;
                }
            }

            exitMenu = true; // Kết thúc menu sau khi chọn xong tất cả
            continue;

        RestartMode:
            continue;

        RestartMenu:
            continue;
        }
    }


    public void Start()
    {
        screenWidth = Math.Min(40, Console.WindowWidth - 1);
        screenHeight = Math.Min(20, Console.WindowHeight - 2);

        Console.CursorVisible = false;

        snake.Add(new Position(screenWidth / 2, screenHeight / 4));
        GenerateFood();

        while (!gameOver)
        {
            DrawScreen();
            Input();
            Update();
            Thread.Sleep(speed);
        }

        Console.Clear();
        Console.WriteLine("Game Over! Điểm của bạn: " + score);
    }

    private void Input()
    {
        if (Console.KeyAvailable)
        {
            var key = Console.ReadKey(true).Key; //đọc phím nhấn

            if (key == ConsoleKey.Escape)
            {
                ShowPauseMenu();
            }
            else if (key == ConsoleKey.UpArrow || key == ConsoleKey.DownArrow ||
                     key == ConsoleKey.LeftArrow || key == ConsoleKey.RightArrow)
            {
                // Ngăn di chuyển ngược chiều
                if ((key == ConsoleKey.UpArrow && direction != ConsoleKey.DownArrow) ||
                    (key == ConsoleKey.DownArrow && direction != ConsoleKey.UpArrow) ||
                    (key == ConsoleKey.LeftArrow && direction != ConsoleKey.RightArrow) ||
                    (key == ConsoleKey.RightArrow && direction != ConsoleKey.LeftArrow))
                {
                    direction = key;
                }
            }
        }
    }

    private void ShowPauseMenu()
    {
        Console.Clear();
        Console.WriteLine("=== Tạm dừng trò chơi ===");
        Console.WriteLine("1. Tiếp tục");
        Console.WriteLine("2. Thoát trò chơi");

        bool validChoice = false;
        while (!validChoice)
        {
            Console.Write("Lựa chọn của bạn (1-2): ");
            string choice = Console.ReadLine();
            switch (choice)
            {
                case "1":
                    validChoice = true;
                    Console.Clear();
                    break;
                case "2":
                    validChoice = true;
                    gameOver = true;
                    break;
                default:
                    Console.WriteLine("Lựa chọn không hợp lệ. Hãy thử lại.");
                    break;
            }
        }
    }

    private void PauseGame()
    {
        Console.SetCursorPosition(0, screenHeight + 1);
        Console.WriteLine("Game paused. Press any key to continue...");
        Console.ReadKey(true);
    }


    private void Update()
    {
        Position head = snake[^1]; //snake[^1] là phần tử cuối cùng trong danh sách snake
        Position newHead = head; //Biến 'newHead' sẽ chứa vị trí mới của đầu rắn sau khi di chuyển.

        switch (direction)
        {
            case ConsoleKey.UpArrow:
                newHead = new Position(head.X, head.Y - 1);
                break;
            case ConsoleKey.DownArrow:
                newHead = new Position(head.X, head.Y + 1);
                break;
            case ConsoleKey.LeftArrow:
                newHead = new Position(head.X - 1, head.Y);
                break;
            case ConsoleKey.RightArrow:
                newHead = new Position(head.X + 1, head.Y);
                break;
        }

        if (canPassThroughWalls)
        {
            if (newHead.X <= 0) newHead = new Position(screenWidth - 2, newHead.Y);
            else if (newHead.X >= screenWidth - 1) newHead = new Position(1, newHead.Y);
            if (newHead.Y <= 0) newHead = new Position(newHead.X, screenHeight - 2);
            else if (newHead.Y >= screenHeight - 1) newHead = new Position(newHead.X, 1);
        }
        else
        {
            if (newHead.X <= 0 || newHead.X >= screenWidth - 1 || newHead.Y <= 0 || newHead.Y >= screenHeight - 1 || currentMap[newHead.Y, newHead.X] == '#')
            {
                gameOver = true;
                return;
            }
        }

        if (snake.Contains(newHead))
        {
            gameOver = true;
            return;
        }

        snake.Add(newHead);

        if (newHead.Equals(food))
        {
            score++;
            GenerateFood();
        }
        else
        {
            snake.RemoveAt(0);
        }
    }

    private void GenerateFood()
    {
        do
        {
            food = new Position(random.Next(1, screenWidth - 1), random.Next(1, screenHeight - 1));
        }
        while (snake.Contains(food) || currentMap[food.Y, food.X] == '#');
    }

    private void DrawScreen()
    {
        Console.Clear();
        //Vẽ bản đồ
        for (int y = 0; y < screenHeight; y++)
        {
            for (int x = 0; x < screenWidth; x++)
            {
                Console.SetCursorPosition(x, y);
                Console.Write(currentMap[y, x]);
            }
        }

        //Vẽ rắn
        Console.ForegroundColor = ConsoleColor.Green;
        foreach (Position pos in snake)
        {
            Console.SetCursorPosition(pos.X, pos.Y);
            Console.Write("O");
        }

        //Vẽ thức ăn
        Console.ForegroundColor = ConsoleColor.Red;
        Console.SetCursorPosition(food.X, food.Y);
        Console.Write("X");
        Console.ResetColor();

        // Hiển thị thông tin điểm và tốc độ
        Console.SetCursorPosition(0, screenHeight);
        Console.Write($"Score: {score} | Speed: {speed}ms");
    }
}