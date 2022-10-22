using AdaYazilimCase.API.Models.RequestModels;
using AdaYazilimCase.API.Models.ResponseModels;
using Microsoft.AspNetCore.Mvc;

namespace AdaYazilimCase.API.Controllers;

[Route("api/[controller]")]
[ApiController]
public class ReservationController : ControllerBase
{
    public ReservationController()
    {

    }

    [HttpPost]
    public IActionResult Reservation(ReservationRequestModel reservation)
    {
        ReservationResponseModel response = new ReservationResponseModel();

        if (reservation == null)
            return NotFound(reservation);

        int kalanYolcuSayisi = reservation.PersonReservationCount;

        if (reservation.Train == null || reservation.Train.Carriages == null)
            return NotFound(response);

        foreach (var item in reservation.Train.Carriages)
        {
            // Vagon'un Yüzde 70 doluluk oranında kaç koltuk kapasitesi olduğunu buluyoruz.
            int dolulukOrani = (item.Capacity * 70) / 100;

            // Vagon'a kaç kişi yerleşebilir sayısı
            int vagonKalanKoltukSayisi = dolulukOrani - item.Fullness;

            // Doluluk oranı %70 altında ise işlemler gerçekleşir.
            if (item.Fullness < dolulukOrani)
            {
                ReservationWagonSettlement vagon;
                //  Yolcular farklı vagona yerleşmek istemiyorlarsa ve yeterli yer yoksa sonraki vagona bak ->
                if (!reservation.AnotherCarriage && vagonKalanKoltukSayisi < kalanYolcuSayisi)
                    continue;
                // Vagon'un %70'i kalan yolcu sayısından büyük veya eşit olma durumu
                if (vagonKalanKoltukSayisi >= kalanYolcuSayisi)
                {
                    vagon = new ReservationWagonSettlement()
                    {
                        PersonCount = kalanYolcuSayisi,
                        WagonName = item.Name
                    };
                    response.ReservationWagonSettlements.Add(vagon);
                    return Ok(response);
                }
                // Vagon'un %70'i kalan yolcu sayısından büyük veya eşit olmama durumu
                else
                {
                    kalanYolcuSayisi = kalanYolcuSayisi - vagonKalanKoltukSayisi;
                    vagon = new ReservationWagonSettlement()
                    {
                        PersonCount = vagonKalanKoltukSayisi,
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
        if (kalanYolcuSayisi > 0)
        {
            response.ReservationWagonSettlements.Clear();
            return NotFound(response);
        }
        return Ok(response);
    }
}
