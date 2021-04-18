namespace MyLights.LightUdp
{
    public enum LightProperties
    {
        None = 0,
        Id = 100,
        Name = 101,
        Power = 20,
        Mode = 21,
        Brightness = 22,
        ColorTemp = 23,
        Color = 24,
        Hue = 240,
        Saturation = 241,
        Value = 242,
    }

    public enum DgramVerbs
    {
        None = 0,
        Tell = 1,       // Inform remote of resource property
        Wonder = 2,     // Express interest in remote resource property
        Wish = 3,       // Request a change in remote property
        Enloop = 4,     // Request to be updated of all changes in a remote resource property
    }
}
