using Grpc.Net.Client;
using SmokeQuit.GrpcService.LocDpx.Protos;
using System;
using System.Threading.Tasks;
using static SmokeQuit.GrpcService.LocDpx.Protos.ChatsLocDpxGRPC;

namespace SmokeQuit.GrpcClients.ConsoleApp.LocDpx
{
    class Program
    {
        private static ChatsLocDpxGRPCClient? _client;
        private static GrpcChannel? _channel;

        static async Task Main(string[] args)
        {
            Console.WriteLine("=== SmokeQuit gRPC Client - ChatsLocDpx CRUD ===");
            Console.WriteLine();

            // Initialize gRPC channel and client
            await InitializeGrpcClient();

            // Main menu loop
            bool exit = false;
            while (!exit)
            {
                ShowMainMenu();
                var choice = Console.ReadLine();

                switch (choice)
                {
                    case "1":
                        await GetAllChatsAsync();
                        break;
                    case "2":
                        await GetChatByIdAsync();
                        break;
                    case "3":
                        await CreateChatAsync();
                        break;
                    case "4":
                        await UpdateChatAsync();
                        break;
                    case "5":
                        await DeleteChatAsync();
                        break;
                    case "6":
                        exit = true;
                        break;
                    default:
                        Console.WriteLine("Invalid choice. Please try again.");
                        break;
                }

                if (!exit)
                {
                    Console.WriteLine("\nPress any key to continue...");
                    Console.ReadKey();
                    Console.Clear();
                }
            }

            // Cleanup
            await _channel?.ShutdownAsync();
            Console.WriteLine("Goodbye!");
        }

        private static async Task InitializeGrpcClient()
        {
            try
            {
                var channel = GrpcChannel.ForAddress("https://localhost:7184"); // Adjust port as needed
                _channel = channel;
                _client = new ChatsLocDpxGRPCClient(channel);
                Console.WriteLine("✅ Connected to gRPC server at https://localhost:7184");
                Console.WriteLine();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"❌ Failed to connect to gRPC server: {ex.Message}");
                Console.WriteLine("Press any key to exit...");
                Console.ReadKey();
                Environment.Exit(1);
            }
        }

        private static void ShowMainMenu()
        {
            Console.WriteLine("=== ChatsLocDpx gRPC Operations ===");
            Console.WriteLine("1. Get All Chats");
            Console.WriteLine("2. Get Chat by ID");
            Console.WriteLine("3. Create New Chat");
            Console.WriteLine("4. Update Chat");
            Console.WriteLine("5. Delete Chat");
            Console.WriteLine("6. Exit");
            Console.WriteLine();
            Console.Write("Select an option (1-6): ");
        }

        #region Get All Chats
        private static async Task GetAllChatsAsync()
        {
            try
            {
                Console.WriteLine("\n=== Getting All Chats ===");
                var request = new EmptyRequest();
                var response =  _client.GetAllAsync(request);

                if (response.Items != null && response.Items.Count > 0)
                {
                    Console.WriteLine($"Found {response.Items.Count} chats:");
                    Console.WriteLine();

                    foreach (var chat in response.Items)
                    {
                        Console.WriteLine($"Chat ID: {chat.ChatsLocDpxid}");
                        Console.WriteLine($"User ID: {chat.UserId}");
                        Console.WriteLine($"Coach ID: {chat.CoachId}");
                        Console.WriteLine($"Message: {chat.Message}");
                        Console.WriteLine($"Sent By: {chat.SentBy}");
                        Console.WriteLine($"Message Type: {chat.MessageType}");
                        Console.WriteLine($"Is Read: {chat.IsRead}");
                        Console.WriteLine($"Attachment URL: {chat.AttachmentUrl}");
                        Console.WriteLine($"Response Time: {chat.ResponseTime}");
                        Console.WriteLine($"Created At: {chat.CreatedAt}");
                        Console.WriteLine(new string('-', 50));
                    }
                }
                else
                {
                    Console.WriteLine("No chats found.");
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"❌ Error getting chats: {ex.Message}");
            }
        }
        #endregion

        #region Get Chat by ID
        private static async Task GetChatByIdAsync()
        {
            try
            {
                Console.WriteLine("\n=== Get Chat by ID ===");
                Console.Write("Enter Chat ID: ");

                if (int.TryParse(Console.ReadLine(), out int chatId))
                {
                    var request = new ChatsLocDpxIdRequest { Id = chatId };
                    var chat =  _client.GetByIdAsync(request);

                    if (chat.ChatsLocDpxid > 0)
                    {
                        Console.WriteLine($"\nChat Details:");
                        Console.WriteLine($"Chat ID: {chat.ChatsLocDpxid}");
                        Console.WriteLine($"User ID: {chat.UserId}");
                        Console.WriteLine($"Coach ID: {chat.CoachId}");
                        Console.WriteLine($"Message: {chat.Message}");
                        Console.WriteLine($"Sent By: {chat.SentBy}");
                        Console.WriteLine($"Message Type: {chat.MessageType}");
                        Console.WriteLine($"Is Read: {chat.IsRead}");
                        Console.WriteLine($"Attachment URL: {chat.AttachmentUrl}");
                        Console.WriteLine($"Response Time: {chat.ResponseTime}");
                        Console.WriteLine($"Created At: {chat.CreatedAt}");
                    }
                    else
                    {
                        Console.WriteLine($"Chat with ID {chatId} not found.");
                    }
                }
                else
                {
                    Console.WriteLine("Invalid Chat ID format.");
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"❌ Error getting chat: {ex.Message}");
            }
        }
        #endregion

        #region Create Chat
        private static async Task CreateChatAsync()
        {
            try
            {
                Console.WriteLine("\n=== Create New Chat ===");

                var chat = new ChatsLocDpx();

                Console.Write("Enter User ID: ");
                if (int.TryParse(Console.ReadLine(), out int userId))
                    chat.UserId = userId;

                Console.Write("Enter Coach ID: ");
                if (int.TryParse(Console.ReadLine(), out int coachId))
                    chat.CoachId = coachId;

                Console.Write("Enter Message: ");
                chat.Message = Console.ReadLine() ?? "";

                Console.Write("Enter Sent By: ");
                chat.SentBy = Console.ReadLine() ?? "";

                Console.Write("Enter Message Type: ");
                chat.MessageType = Console.ReadLine() ?? "";

                Console.Write("Is Read (true/false): ");
                if (bool.TryParse(Console.ReadLine(), out bool isRead))
                    chat.IsRead = isRead;

                Console.Write("Enter Attachment URL (optional): ");
                chat.AttachmentUrl = Console.ReadLine() ?? "";

                // Set current time for creation
                chat.CreatedAt = DateTime.Now.ToString("yyyy-MM-ddTHH:mm:ss");

                var result =  _client.CreateAsync(chat);

                if (result.Result > 0)
                {
                    Console.WriteLine($"✅ Chat created successfully! Result: {result.Result}");
                }
                else
                {
                    Console.WriteLine($"❌ Failed to create chat. Result: {result.Result}");
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"❌ Error creating chat: {ex.Message}");
            }
        }
        #endregion

        #region Update Chat
        private static async Task UpdateChatAsync()
        {
            try
            {
                Console.WriteLine("\n=== Update Chat ===");
                Console.Write("Enter Chat ID to update: ");

                if (int.TryParse(Console.ReadLine(), out int chatId))
                {
                    // First get the existing chat
                    var getRequest = new ChatsLocDpxIdRequest { Id = chatId };
                    var existingChat =  _client.GetByIdAsync(getRequest);

                    if (existingChat.ChatsLocDpxid > 0)
                    {
                        Console.WriteLine($"Current chat details:");
                        Console.WriteLine($"Message: {existingChat.Message}");
                        Console.WriteLine($"Sent By: {existingChat.SentBy}");
                        Console.WriteLine($"Message Type: {existingChat.MessageType}");
                        Console.WriteLine($"Is Read: {existingChat.IsRead}");
                        Console.WriteLine();

                        // Update fields
                        Console.Write($"Enter new Message (current: {existingChat.Message}): ");
                        var newMessage = Console.ReadLine();
                        if (!string.IsNullOrEmpty(newMessage))
                            existingChat.Message = newMessage;

                        Console.Write($"Enter new Sent By (current: {existingChat.SentBy}): ");
                        var newSentBy = Console.ReadLine();
                        if (!string.IsNullOrEmpty(newSentBy))
                            existingChat.SentBy = newSentBy;

                        Console.Write($"Enter new Message Type (current: {existingChat.MessageType}): ");
                        var newMessageType = Console.ReadLine();
                        if (!string.IsNullOrEmpty(newMessageType))
                            existingChat.MessageType = newMessageType;

                        Console.Write($"Is Read (current: {existingChat.IsRead}) - true/false: ");
                        var newIsReadStr = Console.ReadLine();
                        if (bool.TryParse(newIsReadStr, out bool newIsRead))
                            existingChat.IsRead = newIsRead;

                        var result =  _client.UpdateAsync(existingChat);

                        if (result.Result > 0)
                        {
                            Console.WriteLine($"✅ Chat updated successfully! Result: {result.Result}");
                        }
                        else
                        {
                            Console.WriteLine($"❌ Failed to update chat. Result: {result.Result}");
                        }
                    }
                    else
                    {
                        Console.WriteLine($"Chat with ID {chatId} not found.");
                    }
                }
                else
                {
                    Console.WriteLine("Invalid Chat ID format.");
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"❌ Error updating chat: {ex.Message}");
            }
        }
        #endregion

        #region Delete Chat
        private static async Task DeleteChatAsync()
        {
            try
            {
                Console.WriteLine("\n=== Delete Chat ===");
                Console.Write("Enter Chat ID to delete: ");

                if (int.TryParse(Console.ReadLine(), out int chatId))
                {
                    // First show the chat to be deleted
                    var getRequest = new ChatsLocDpxIdRequest { Id = chatId };
                    var existingChat =  _client.GetByIdAsync(getRequest);

                    if (existingChat.ChatsLocDpxid > 0)
                    {
                        Console.WriteLine($"\nChat to be deleted:");
                        Console.WriteLine($"Chat ID: {existingChat.ChatsLocDpxid}");
                        Console.WriteLine($"Message: {existingChat.Message}");
                        Console.WriteLine($"Sent By: {existingChat.SentBy}");
                        Console.WriteLine();

                        Console.Write("Are you sure you want to delete this chat? (y/N): ");
                        var confirmation = Console.ReadLine();

                        if (confirmation?.ToLower() == "y" || confirmation?.ToLower() == "yes")
                        {
                            var deleteRequest = new ChatsLocDpxIdRequest { Id = chatId };
                            var result =  _client.DeleteAsync(deleteRequest);

                            if (result.Result > 0)
                            {
                                Console.WriteLine($"✅ Chat deleted successfully! Result: {result.Result}");
                            }
                            else
                            {
                                Console.WriteLine($"❌ Failed to delete chat. Result: {result.Result}");
                            }
                        }
                        else
                        {
                            Console.WriteLine("Delete operation cancelled.");
                        }
                    }
                    else
                    {
                        Console.WriteLine($"Chat with ID {chatId} not found.");
                    }
                }
                else
                {
                    Console.WriteLine("Invalid Chat ID format.");
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"❌ Error deleting chat: {ex.Message}");
            }
        }
        #endregion
    }
}