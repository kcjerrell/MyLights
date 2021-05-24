namespace MyLights.Bridges.Udp
{
    public enum DgramVerbs
    {
        None = 0,
        Tell = 1,       // Inform remote of resource property
        Wonder = 2,     // Express interest in remote resource property
        Wish = 3,       // Request a change in remote property
        Enloop = 4,     // Request to be updated of all changes in a remote resource property
        Holler = 5,
        Nod = 6,
        Reload = 7,
    }
}