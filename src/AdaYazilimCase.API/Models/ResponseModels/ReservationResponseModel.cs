namespace AdaYazilimCase.API.Models.ResponseModels;

public class ReservationResponseModel
{
    public ReservationResponseModel()
    {
        ReservationWagonSettlements = new List<ReservationWagonSettlement>();
    }

    public bool OrderAvailable { get; set; } = true;
    public List<ReservationWagonSettlement> ReservationWagonSettlements { get; set; }
}
