namespace AdaYazilimCase.API.Models.RequestModels;

public class Train
{
    public Train()
    {
        Carriages = new List<CarriageRequestModel>();
    }
    
    public string? Name { get; set; }
    public List<CarriageRequestModel>? Carriages { get; set; }
}
