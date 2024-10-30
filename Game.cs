using System;
using System.Collections.Generic;
using System.Threading;

class Game
{
    private int screenWidth = 40;
    private int screenHeight = 20;
    private int score = 0;
    private bool gameOver = false;
    private bool canPassThroughWalls = false;
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
        Console.Clear();
        Console.WriteLine("Chào mừng đến với trò chơi con rắn!");
        Console.WriteLine("Chọn map:");
        Console.WriteLine("1. Map cơ bản (không chướng ngại)");
        Console.WriteLine("2. Map có tường bên trong");

        bool validChoice = false;
        while (!validChoice)
        {
            Console.Write("Lựa chọn của bạn (1-2): ");
            string choice = Console.ReadLine();
            switch (choice)
            {
                case "1":
                    MapLoader.LoadBasicMap(ref currentMap, screenWidth, screenHeight);
                    validChoice = true;
                    break;
                case "2":
                    MapLoader.LoadComplexMap(ref currentMap, screenWidth, screenHeight);
                    validChoice = true;
                    break;
                default:
                    Console.WriteLine("Lựa chọn không hợp lệ. Hãy thử lại.");
                    break;
            }
        }

        Console.WriteLine("Chọn chế độ:");
        Console.WriteLine("1. Không xuyên tường");
        Console.WriteLine("2. Xuyên tường");

        validChoice = false;
        while (!validChoice)
        {
            Console.Write("Lựa chọn của bạn (1-2): ");
            string modeChoice = Console.ReadLine();
            switch (modeChoice)
            {
                case "1":
                    canPassThroughWalls = false;
                    validChoice = true;
                    break;
                case "2":
                    canPassThroughWalls = true;
                    validChoice = true;
                    break;
                default:
                    Console.WriteLine("Lựa chọn không hợp lệ. Hãy thử lại.");
                    break;
            }
        }

        Console.WriteLine("Chọn chế độ tốc độ:");
        Console.WriteLine("1. Chậm (200ms)");
        Console.WriteLine("2. Trung bình (100ms)");
        Console.WriteLine("3. Nhanh (50ms)");

        validChoice = false;
        while (!validChoice)
        {
            Console.Write("Lựa chọn của bạn (1-3): ");
            string speedChoice = Console.ReadLine();
            switch (speedChoice)
            {
                case "1":
                    speed = 200;
                    validChoice = true;
                    break;
                case "2":
                    speed = 100;
                    validChoice = true;
                    break;
                case "3":
                    speed = 50;
                    validChoice = true;
                    break;
                default:
                    Console.WriteLine("Lựa chọn không hợp lệ. Hãy thử lại.");
                    break;
            }
        }
    }

    public void Start()
    {
        screenWidth = Math.Min(40, Console.WindowWidth - 1);
        screenHeight = Math.Min(20, Console.WindowHeight - 2);

        Console.CursorVisible = false;

        snake.Add(new Position(screenWidth / 2, screenHeight / 2));
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
            var key = Console.ReadKey(true).Key;
            if (key == ConsoleKey.UpArrow || key == ConsoleKey.DownArrow ||
                key == ConsoleKey.LeftArrow || key == ConsoleKey.RightArrow)
            {
                direction = key;
            }
        }
    }

    private void Update()
    {
        Position head = snake[^1];
        Position newHead = head;

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

        for (int y = 0; y < screenHeight; y++)
        {
            for (int x = 0; x < screenWidth; x++)
            {
                Console.SetCursorPosition(x, y);
                Console.Write(currentMap[y, x]);
            }
        }

        foreach (Position pos in snake)
        {
            Console.SetCursorPosition(pos.X, pos.Y);
            Console.Write("O");
        }

        Console.SetCursorPosition(food.X, food.Y);
        Console.Write("X");
    }
}
