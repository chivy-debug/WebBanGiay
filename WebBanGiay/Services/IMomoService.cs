using WebBanGiay.Models.Momo;
using WebBanGiay.Models.OrderMomo;

namespace WebBanGiay.Services;

public interface IMomoService
{
    Task<MomoCreatePaymentResponseModel> CreatePaymentAsync(OrderInfoModel model);
    MomoExecuteResponseModel PaymentExecuteAsync(IQueryCollection collection);
}