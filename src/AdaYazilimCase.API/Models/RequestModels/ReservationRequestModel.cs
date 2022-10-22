namespace AdaYazilimCase.API.Models.RequestModels
{
    public class ReservationRequestModel
    {
        public Train? Train { get; set; }
        public int PersonReservationCount { get; set; }
        public bool AnotherCarriage { get; set; }
    }
}
