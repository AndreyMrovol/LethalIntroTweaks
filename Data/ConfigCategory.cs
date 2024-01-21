namespace IntroTweaks.Data;

public struct Category(string value) {
    public static Category GENERAL => new("0 >> General << 0");
    public static Category INTRO_TWEAKS => new("1 >> Intro << 1");
    public static Category MENU_TWEAKS => new("2 >> Main Menu << 2");
    public static Category VERSION_TEXT => new("3 >> Custom Version Text << 3");
    public static Category MISC => new("4 >> Miscellaneous << 4");

    public string Value { get; private set; } = value;
}