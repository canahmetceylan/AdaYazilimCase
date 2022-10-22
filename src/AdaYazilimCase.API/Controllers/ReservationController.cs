using AdaYazilimCase.API.Models.RequestModels;
using AdaYazilimCase.API.Models.ResponseModels;
using Microsoft.AspNetCore.Mvc;

namespace AdaYazilimCase.API.Controllers;

[Route("api/[controller]")]
[ApiController]
public class ReservationController : ControllerBase
{
    private readonly ILogger<ReservationController> _logger;

    public ReservationController(ILogger<ReservationController> logger)
    {
        _logger = logger;
    }

    [HttpPost]
    public IActionResult Reservation(ReservationRequestModel reservation)
    {
        ReservationResponseModel response = new ReservationResponseModel();

        if (reservation == null)
            return NotFound(reservation);

        int remainingPersonCount = reservation.PersonReservationCount;

        if (reservation.Train == null || reservation.Train.Carriages == null)
            return NotFound(response);

        foreach (var item in reservation.Train.Carriages)
        {
            // Vagon'un Yüzde 70 doluluk oranında kaç koltuk kapasitesi olduğunu buluyoruz.
            int fullnessPercentage = (item.Capacity * 70) / 100;

            // Vagon'a kaç kişi yerleşebilir sayısı
            int wagonRemainingSeatsCount = fullnessPercentage - item.Fullness;

            // Doluluk oranı %70 altında ise işlemler gerçekleşir.
            if (item.Fullness < fullnessPercentage)
            {
                ReservationWagonSettlement vagon;
                //  Yolcular farklı vagona yerleşmek istemiyorlarsa ve yeterli yer yoksa sonraki vagona bak ->
                if (!reservation.AnotherCarriage && wagonRemainingSeatsCount < remainingPersonCount)
                    continue;
                // Vagon'un %70'i kalan yolcu sayısından büyük veya eşit olma durumu
                if (wagonRemainingSeatsCount >= remainingPersonCount)
                {
                    vagon = new ReservationWagonSettlement()
                    {
                        PersonCount = remainingPersonCount,
                        WagonName = item.Name
                    };
                    response.ReservationWagonSettlements.Add(vagon);
                    break;
                }
                // Vagon'un %70'i kalan yolcu sayısından büyük veya eşit olmama durumu
                else
                {
                    remainingPersonCount = remainingPersonCount - wagonRemainingSeatsCount;
                    vagon = new ReservationWagonSettlement()
                    {
                        PersonCount = wagonRemainingSeatsCount,
                        WagonName = item.Name
                    };
                    response.ReservationWagonSettlements.Add(vagon);
                    continue;
                }
            }
            // Doluluk oranı %70 üzeri ise vagon atlanır.
            else
                continue;

        }
        if (remainingPersonCount > 0)
        {
            response.ReservationWagonSettlements.Clear();
            return NotFound(response);
        }
        return Ok(response);
    }
}
