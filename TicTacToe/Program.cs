namespace TicTacToe
{
    class Program
    {
        // Main
        static void Main(string[] args)
        {
            Console.WriteLine(3 & 1);
            while (true)
            {
                var controller = new GameController();
                controller.Run();
                if (!GameController.Confirm("Try Again?(y/n):"))
                    break;
            }
        }
    }
}
