namespace mongoDb_CRUD_poc.ViewModels.DisplayModels;

/// <summary>
/// Light weight model to help with time conversions
/// (Data should be stored in UTC ALWAYS)
/// This means we need to convert it to local time in the display
/// </summary>
public class PrintDisplayModel
{
    public string? id { get; set; }
    public string? directoryPath { get; set; }
    public string? duration { get; set; }
    public string? startTimeLocal { get; set; }
    public string? endTimeLocal { get; set; }
    public int totalSlices { get; set; }
    public bool complete { get; set; }

}

