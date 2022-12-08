using MercadoPago.Client.Common;
using MercadoPago.Client.Payment;
using MercadoPago.Config;
using MercadoPago.Resource.Payment;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web.Http;

namespace MercadoPagoAPI.Controllers
{
    public class PixController : ApiController
    {

        public PixController()
        {
            MercadoPagoConfig.AccessToken = ConfigurationManager.AppSettings["MercadoPagoAccessToken"];
        }

        //api/criar
        [HttpGet]
        public async Task<HttpResponseMessage> Criar(decimal valor, string produto)
        {
            try
            {
                var request = new PaymentCreateRequest
                {
                    TransactionAmount = valor,
                    Description = produto,
                    PaymentMethodId = "pix",
                    DateOfExpiration = DateTime.Now.AddDays(7).Date.AddHours(23),
                    Payer = new PaymentPayerRequest
                    {
                        Email = "test@test.com",
                        FirstName = "Test",
                        LastName = "User",
                        Identification = new IdentificationRequest
                        {
                            Type = "CHAVE ALEATORIA",
                            Number = "8fe20dbb-9920-4896-8dd5-090e98c84aa6",
                        },
                    },
                };

                var client = new PaymentClient();
                Payment payment = await client.CreateAsync(request);

                var result = new
                {
                    payment.Id,
                    payment.Status,
                    payment.StatusDetail,
                    CopiaCola = payment.PointOfInteraction.TransactionData.QrCode,
                    QrCode = payment.PointOfInteraction.TransactionData.QrCodeBase64,
                    payment.PointOfInteraction.TransactionData.TicketUrl,
                    DataExpiracao = payment.DateOfExpiration,
                    Produto = payment.Description,
                    Valor = payment.TransactionAmount
                };

                return Request.CreateResponse(HttpStatusCode.OK, result);

                //return Ok(result);
            }
            catch (Exception ex)
            {
                return this.Request.CreateResponse(HttpStatusCode.InternalServerError,
                    new
                    {
                        Message = ex.Message.ToString()
                    });
            }
        }


        [HttpGet]
        public async Task<HttpResponseMessage> Consultar(long id)
        {
            try
            {
                var client = new PaymentClient();
                Payment payment = await client.GetAsync(id);

                var result = new
                {
                    payment.Id,
                    payment.Status,
                    payment.StatusDetail,
                    CopiaCola = payment.PointOfInteraction.TransactionData.QrCode,
                    QrCode = payment.PointOfInteraction.TransactionData.QrCodeBase64,
                    payment.PointOfInteraction.TransactionData.TicketUrl,
                    DataExpiracao = payment.DateOfExpiration,
                    Produto = payment.Description,
                    Valor = payment.TransactionAmount
                };

                return Request.CreateResponse(HttpStatusCode.OK, result);
            }
            catch (Exception ex)
            {
                return this.Request.CreateResponse(HttpStatusCode.InternalServerError,
                                    new
                                    {
                                        Message = ex.InnerException.Message.ToString()
                                    });
            }
        }

        //// GET: api/Pix
        //public IEnumerable<string> Get()
        //{
        //    return new string[] { "value1", "value2" };
        //}

        //// GET: api/Pix/5
        //public string Get(int id)
        //{
        //    return "value";
        //}

        //// POST: api/Pix
        //public void Post([FromBody] string value)
        //{
        //}

        //// PUT: api/Pix/5
        //public void Put(int id, [FromBody] string value)
        //{
        //}

        //// DELETE: api/Pix/5
        //public void Delete(int id)
        //{
        //}
    }
}
