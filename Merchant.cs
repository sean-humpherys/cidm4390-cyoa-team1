using CYOA;

namespace cidm4390_cyoa_team1;

public class Merchant
{
    private readonly Messages _messages;

    public Merchant(Messages messages)
    {
        _messages = messages;
    }

    public void Encounter(Player player)
    {
        Console.WriteLine(_messages.GetMessage(59));
        if (player.merchantGiftClaimed)
        {
            Console.WriteLine(_messages.GetMessage(64));
            return;
        }

        while (true)
        {
            Console.WriteLine(_messages.GetMessage(60));
            string? choice = Console.ReadLine()?.Trim();
            switch (choice)
            {
                case "1":
                    player.armorProtection = 2;
                    player.merchantGiftClaimed = true;
                    Console.WriteLine(_messages.GetMessage(65));
                    return;
                case "2":
                    player.weapon.maxDamage += 4;
                    player.merchantGiftClaimed = true;
                    Console.WriteLine(_messages.GetMessage(61));
                    return;
                case "3":
                case null:
                    Console.WriteLine(_messages.GetMessage(62));
                    return;
                default:
                    Console.WriteLine(_messages.GetMessage(63));
                    break;
            }
        }
    }
}
