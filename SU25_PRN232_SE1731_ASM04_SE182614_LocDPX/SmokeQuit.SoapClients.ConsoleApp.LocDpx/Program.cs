using ChatsLocDpxServiceReference;
using CoachLocDpxServiceReference;

Console.WriteLine("=== SOAP Service Client ===");

// Create SOAP clients
var chatClient = new ChatsLocDpxSoapServiceClient(
    ChatsLocDpxSoapServiceClient.EndpointConfiguration.BasicHttpBinding_IChatsLocDpxSoapService);

var coachClient = new CoachLocDpxSoapServiceClient(
    CoachLocDpxSoapServiceClient.EndpointConfiguration.BasicHttpBinding_ICoachLocDpxSoapService);

try
{
    while (true)
    {
        Console.WriteLine("\n=== MAIN MENU ===");
        Console.WriteLine("1. Chat Management");
        Console.WriteLine("2. Coach Management");
        Console.WriteLine("3. Exit");
        Console.Write("Choose option: ");

        var choice = Console.ReadLine();

        switch (choice)
        {
            case "1":
                await ChatMenu(chatClient);
                break;
            case "2":
                await CoachMenu(coachClient);
                break;
            case "3":
                return;
            default:
                Console.WriteLine("Invalid option!");
                break;
        }
    }
}
catch (Exception ex)
{
    Console.WriteLine($"Error: {ex.Message}");
}

static async Task ChatMenu(ChatsLocDpxSoapServiceClient client)
{
    while (true)
    {
        Console.WriteLine("\n=== CHAT MANAGEMENT ===");
        Console.WriteLine("1. Get All Chats");
        Console.WriteLine("2. Get Chat by ID");
        Console.WriteLine("3. Create New Chat");
        Console.WriteLine("4. Update Chat");
        Console.WriteLine("5. Delete Chat");
        Console.WriteLine("6. Back to Main Menu");
        Console.Write("Choose option: ");

        var choice = Console.ReadLine();

        switch (choice)
        {
            case "1":
                await GetAllChats(client);
                break;
            case "2":
                await GetChatById(client);
                break;
            case "3":
                await CreateChat(client);
                break;
            case "4":
                await UpdateChat(client);
                break;
            case "5":
                await DeleteChat(client);
                break;
            case "6":
                return;
            default:
                Console.WriteLine("Invalid option!");
                break;
        }
    }
}

static async Task CoachMenu(CoachLocDpxSoapServiceClient client)
{
    while (true)
    {
        Console.WriteLine("\n=== COACH MANAGEMENT ===");
        Console.WriteLine("1. Get All Coaches");
        Console.WriteLine("2. Back to Main Menu");
        Console.Write("Choose option: ");

        var choice = Console.ReadLine();

        switch (choice)
        {
            case "1":
                await GetAllCoaches(client);
                break;
            case "2":
                return;
            default:
                Console.WriteLine("Invalid option!");
                break;
        }
    }
}

// Chat CRUD Operations
static async Task GetAllChats(ChatsLocDpxSoapServiceClient client)
{
    try
    {
        var chats = await client.GetAllAsync();

        Console.WriteLine($"\n=== ALL CHATS ({chats.Length} found) ===");
        if (chats.Length == 0)
        {
            Console.WriteLine("No chats found.");
            return;
        }

        foreach (var chat in chats)
        {
            Console.WriteLine($"ID: {chat.ChatsLocDpxid} | User: {chat.UserId} | Coach: {chat.CoachId}");
            Console.WriteLine($"Message: {chat.Message}");
            Console.WriteLine($"Sent By: {chat.SentBy} | Type: {chat.MessageType} | Read: {chat.IsRead}");
            Console.WriteLine($"Created: {chat.CreatedAt:yyyy-MM-dd HH:mm:ss}");
            Console.WriteLine(new string('-', 50));
        }
    }
    catch (Exception ex)
    {
        Console.WriteLine($"Error getting chats: {ex.Message}");
    }
}

static async Task GetChatById(ChatsLocDpxSoapServiceClient client)
{
    try
    {
        Console.Write("Enter Chat ID: ");
        var chatId = int.Parse(Console.ReadLine() ?? "0");

        var chat = await client.GetByIdAsync(chatId);

        if (chat != null && chat.ChatsLocDpxid > 0)
        {
            Console.WriteLine("\n=== CHAT DETAILS ===");
            Console.WriteLine($"ID: {chat.ChatsLocDpxid}");
            Console.WriteLine($"User ID: {chat.UserId}");
            Console.WriteLine($"Coach ID: {chat.CoachId}");
            Console.WriteLine($"Message: {chat.Message}");
            Console.WriteLine($"Sent By: {chat.SentBy}");
            Console.WriteLine($"Message Type: {chat.MessageType}");
            Console.WriteLine($"Is Read: {chat.IsRead}");
            Console.WriteLine($"Created At: {chat.CreatedAt:yyyy-MM-dd HH:mm:ss}");
        }
        else
        {
            Console.WriteLine("Chat not found!");
        }
    }
    catch (Exception ex)
    {
        Console.WriteLine($"Error getting chat: {ex.Message}");
    }
}

static async Task CreateChat(ChatsLocDpxSoapServiceClient client)
{
    try
    {
        Console.WriteLine("\n=== CREATE NEW CHAT ===");

        Console.Write("Enter User ID: ");
        var userId = int.Parse(Console.ReadLine() ?? "0");

        Console.Write("Enter Coach ID: ");
        var coachId = int.Parse(Console.ReadLine() ?? "0");

        Console.Write("Enter Message: ");
        var message = Console.ReadLine() ?? "";

        Console.Write("Enter Sent By (User/Coach): ");
        var sentBy = Console.ReadLine() ?? "";

        Console.Write("Enter Message Type (text/image/file): ");
        var messageType = Console.ReadLine() ?? "text";

        Console.Write("Is Read (true/false): ");
        var isRead = bool.Parse(Console.ReadLine() ?? "false");

        var newChat = new ChatsLocDpx
        {
            UserId = userId,
            CoachId = coachId,
            Message = message,
            SentBy = sentBy,
            MessageType = messageType,
            IsRead = isRead,
            CreatedAt = DateTime.Now
        };

        var result = await client.CreateAsync(newChat);

        if (result > 0)
        {
            Console.WriteLine($"✅ Chat created successfully! Result: {result}");
        }
        else
        {
            Console.WriteLine("❌ Failed to create chat.");
        }
    }
    catch (Exception ex)
    {
        Console.WriteLine($"Error creating chat: {ex.Message}");
    }
}

static async Task UpdateChat(ChatsLocDpxSoapServiceClient client)
{
    try
    {
        Console.WriteLine("\n=== UPDATE CHAT ===");

        Console.Write("Enter Chat ID to update: ");
        var chatId = int.Parse(Console.ReadLine() ?? "0");

        // First get the existing chat
        var existingChat = await client.GetByIdAsync(chatId);
        if (existingChat == null || existingChat.ChatsLocDpxid == 0)
        {
            Console.WriteLine("Chat not found!");
            return;
        }

        Console.WriteLine($"Current Message: {existingChat.Message}");
        Console.Write("Enter new Message (or press Enter to keep current): ");
        var newMessage = Console.ReadLine();
        if (!string.IsNullOrEmpty(newMessage))
            existingChat.Message = newMessage;

        Console.WriteLine($"Current Sent By: {existingChat.SentBy}");
        Console.Write("Enter new Sent By (or press Enter to keep current): ");
        var newSentBy = Console.ReadLine();
        if (!string.IsNullOrEmpty(newSentBy))
            existingChat.SentBy = newSentBy;

        Console.WriteLine($"Current Is Read: {existingChat.IsRead}");
        Console.Write("Update Is Read (true/false or press Enter to keep current): ");
        var newIsReadInput = Console.ReadLine();
        if (!string.IsNullOrEmpty(newIsReadInput))
            existingChat.IsRead = bool.Parse(newIsReadInput);

        var updatedChat = await client.UpdateAsync(existingChat);

        if (updatedChat != null && updatedChat.ChatsLocDpxid > 0)
        {
            Console.WriteLine("✅ Chat updated successfully!");
            Console.WriteLine($"Updated Message: {updatedChat.Message}");
        }
        else
        {
            Console.WriteLine("❌ Failed to update chat.");
        }
    }
    catch (Exception ex)
    {
        Console.WriteLine($"Error updating chat: {ex.Message}");
    }
}

static async Task DeleteChat(ChatsLocDpxSoapServiceClient client)
{
    try
    {
        Console.WriteLine("\n=== DELETE CHAT ===");

        Console.Write("Enter Chat ID to delete: ");
        var chatId = int.Parse(Console.ReadLine() ?? "0");

        Console.Write($"Are you sure you want to delete chat ID {chatId}? (y/n): ");
        var confirm = Console.ReadLine()?.ToLower();

        if (confirm != "y" && confirm != "yes")
        {
            Console.WriteLine("Delete cancelled.");
            return;
        }

        var result = await client.DeleteAsync(chatId);

        if (result > 0)
        {
            Console.WriteLine("✅ Chat deleted successfully!");
        }
        else
        {
            Console.WriteLine("❌ Failed to delete chat or chat not found.");
        }
    }
    catch (Exception ex)
    {
        Console.WriteLine($"Error deleting chat: {ex.Message}");
    }
}

// Coach Operations
static async Task GetAllCoaches(CoachLocDpxSoapServiceClient client)
{
    try
    {
        var coaches = await client.GetAllAsync();

        Console.WriteLine($"\n=== ALL COACHES ({coaches.Length} found) ===");
        if (coaches.Length == 0)
        {
            Console.WriteLine("No coaches found.");
            return;
        }

        foreach (var coach in coaches)
        {
            Console.WriteLine($"ID: {coach.CoachesLocDpxid}");
            Console.WriteLine($"Name: {coach.FullName}");
            Console.WriteLine($"Email: {coach.Email}");
            Console.WriteLine($"Phone: {coach.PhoneNumber ?? "N/A"}");
            Console.WriteLine($"Bio: {coach.Bio ?? "N/A"}");
            Console.WriteLine($"Created: {coach.CreatedAt:yyyy-MM-dd HH:mm:ss}");
            Console.WriteLine(new string('-', 50));
        }
    }
    catch (Exception ex)
    {
        Console.WriteLine($"Error getting coaches: {ex.Message}");
    }
}