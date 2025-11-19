using AutoPartesRazor.Models;

namespace AutoPartesRazor.Interfaces;

public interface IPdfService
{
    byte[] GenerateOrderPdf(Order order);
}
