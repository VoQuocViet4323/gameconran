class MapLoader
{
    public static void LoadBasicMap(ref char[,] map, int width, int height)
    {
        map = new char[height, width];
        for (int y = 0; y < height; y++)
        {
            for (int x = 0; x < width; x++)
            {
                map[y, x] = ' ';
            }
        }
    }

    public static void LoadComplexMap(ref char[,] map, int width, int height)
    {
        map = new char[height, width];
        for (int y = 0; y < height; y++)
        {
            for (int x = 0; x < width; x++)
            {
                if (y == 0 || y == height - 1 || x == 0 || x == width - 1)
                {
                    map[y, x] = '#'; // Tường ngoài
                }
                else if (y == height / 2 && x > 5 && x < width - 5)
                {
                    map[y, x] = '#'; // Tường bên trong
                }
                else
                {
                    map[y, x] = ' ';
                }
            }
        }
    }
}
