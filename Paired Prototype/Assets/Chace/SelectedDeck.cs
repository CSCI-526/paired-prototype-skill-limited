using System.Collections.Generic;

public static class SelectedDeck
{
    public static readonly List<string> SelectedCardNames = new List<string>(5);

    public static void Set(IEnumerable<string> names)
    {
        SelectedCardNames.Clear();
        SelectedCardNames.AddRange(names);
    }
}
