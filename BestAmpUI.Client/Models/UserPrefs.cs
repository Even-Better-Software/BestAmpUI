namespace BestAmpUI.Client.Models;

public class UserPrefs
{
    /*
    DEV NOTE: the request classes happen to hold the same values that we want
    the user to be able to save, so I'll just compose the UserPrefs class out of them.
    */  

    public UpdatePrefsRequest? Settings { get; set; }
    public UpdateAmpValuesRequest? Dials { get; set; }
}
