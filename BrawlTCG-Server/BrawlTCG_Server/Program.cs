using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Net.Sockets;
using System.Threading.Tasks;

namespace BrawlTCG_Server
{
    class Program
    {
        static async Task Main(string[] args)
        {
            int port = 5000;
            TcpListener listener = new TcpListener(IPAddress.Any, port);
            listener.Start();
            Console.WriteLine($"Server listening on port {port}...");

            // Accept exactly 2 clients for a game
            Console.WriteLine("Waiting for player 1...");
            TcpClient client1 = await listener.AcceptTcpClientAsync();
            Console.WriteLine("Player 1 connected.");

            Console.WriteLine("Waiting for player 2...");
            TcpClient client2 = await listener.AcceptTcpClientAsync();
            Console.WriteLine("Player 2 connected.");

            // Setup streams
            StreamReader reader1 = new StreamReader(client1.GetStream());
            StreamWriter writer1 = new StreamWriter(client1.GetStream()) { AutoFlush = true };
            StreamReader reader2 = new StreamReader(client2.GetStream());
            StreamWriter writer2 = new StreamWriter(client2.GetStream()) { AutoFlush = true };

            // 1️⃣ Exchange initial player data
            string player1Data = await reader1.ReadLineAsync();
            string player2Data = await reader2.ReadLineAsync();

            // Send each player the other’s data
            await writer1.WriteLineAsync(player2Data);
            await writer2.WriteLineAsync(player1Data);

            Console.WriteLine("Player data exchanged. Waiting for turn distribution request...");

            // 2️⃣ Handle DIST_TURNS request
            var distTask1 = HandleDistTurns(reader1, writer1, isFirst: true);
            var distTask2 = HandleDistTurns(reader2, writer2, isFirst: false);
            await Task.WhenAll(distTask1, distTask2);

            Console.WriteLine("Turn distribution complete. Starting message relay...");

            // 3️⃣ Start relaying in-game messages
            var task1 = RelayMessages(reader1, writer2, "Player 1 -> Player 2");
            var task2 = RelayMessages(reader2, writer1, "Player 2 -> Player 1");

            await Task.WhenAny(task1, task2);
            Console.WriteLine("One of the players disconnected. Shutting down server.");

            client1.Close();
            client2.Close();
            listener.Stop();
        }

        static async Task HandleDistTurns(StreamReader reader, StreamWriter writer, bool isFirst)
        {
            try
            {
                string request = await reader.ReadLineAsync();
                if (request != null && request == "DIST_TURNS_REQUEST")
                {
                    // First connected player gets true, second gets false
                    await writer.WriteLineAsync(isFirst ? "DIST_TURNS:true" : "DIST_TURNS:false");
                    Console.WriteLine($"Sent turn info to {(isFirst ? "Player 1" : "Player 2")}: {isFirst}");
                }
            }
            catch
            {
                // Handle disconnect silently
            }
        }

        static async Task RelayMessages(StreamReader reader, StreamWriter writer, string direction)
        {
            try
            {
                while (true)
                {
                    string msg = await reader.ReadLineAsync();
                    if (msg == null) break; // client disconnected
                    await writer.WriteLineAsync(msg);
                    Console.WriteLine($"{direction}: {msg}");
                }
            }
            catch
            {
                // Client disconnected or network error
            }
        }
    }
}
