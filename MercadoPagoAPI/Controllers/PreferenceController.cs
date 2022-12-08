using MercadoPago.Client;
using MercadoPago.Client.Payment;
using MercadoPago.Client.Preference;
using MercadoPago.Config;
using MercadoPago.Resource;
using MercadoPago.Resource.Payment;
using MercadoPago.Resource.Preference;
using Newtonsoft.Json;
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
    [Route("api/preference")]
    public class PreferenceController : ApiController
    {
        public PreferenceController()
        {
            MercadoPagoConfig.AccessToken = ConfigurationManager.AppSettings["MercadoPagoAccessToken"];
        }


        [HttpGet]
        [Route("api/preference/criar")]
        public async Task<HttpResponseMessage> Criar(decimal valor, string produto)
        {
            var idPagamento = Guid.NewGuid();

            var metaDados = new Dictionary<string, object>();

            metaDados.Add("idpagamento", idPagamento);

            var request = new PreferenceRequest
            {
                ExternalReference = idPagamento.ToString(),
                Items = new List<PreferenceItemRequest>
                {
                    new PreferenceItemRequest
                    {
                        Description = produto,
                        Id = idPagamento.ToString(),
                        Quantity = 1,
                        Title = produto,
                        UnitPrice = valor,
                    },
                },
                Metadata = metaDados,

                Payer = new PreferencePayerRequest
                {
                },
                //Metadata = metaDados,
                PaymentMethods = new PreferencePaymentMethodsRequest
                {
                    //ExcludedPaymentMethods = new List<PreferencePaymentMethodRequest>
                    //{
                    //    new PreferencePaymentMethodRequest
                    //    {
                    //        Id = "pix",
                    //    },
                    //},
                },
            };

            var client = new PreferenceClient();
            Preference preference = await client.CreateAsync(request);

            var result = new
            {
                Id = idPagamento,
                DataCriacao = DateTime.Now,
                Produto = produto,
                Valor = valor,
                IdPreference = preference.Id,
                UrlPagamento = preference.InitPoint,
            };

            return Request.CreateResponse(HttpStatusCode.OK, result);

        }

        [HttpGet]
        [Route("api/preference/consultar")]
        public async Task<HttpResponseMessage> Consultar(Guid id)
        {
            try
            {
                var searchRequest = new AdvancedSearchRequest
                {
                    Limit = 1,
                    Offset = 0,
                    Sort = "date_created",
                    Criteria = "desc",
                    Range = "date_created",
                    BeginDate = DateTime.Now.AddYears(-1),
                    EndDate = DateTime.Now.AddDays(1).AddMilliseconds(-1),
                    Filters = new Dictionary<string, object>
                    {
                        ["external_reference"] = id,
                    },
                };

                var client2 = new PaymentClient();
                ResultsResourcesPage<Payment> results = await client2.SearchAsync(searchRequest);

                return Request.CreateResponse(HttpStatusCode.OK, results);

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

        [HttpGet]
        [Route("api/preference/consultar2")]
        public async Task<HttpResponseMessage> Consultar2(long id)
        {
            try
            {
                //FAZ REQUEST NO ID DE PAGAMENTO
                var client = new PaymentClient();
                Payment payment = await client.CaptureAsync(id);

                var result =  payment.Metadata["idpagamento"].ToString();

                //var paymentJson = JsonConvert.SerializeObject(payment);

                return Request.CreateResponse(HttpStatusCode.OK, new { id = result, status = payment.Status.ToUpper() });

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

        [HttpGet]
        [Route("api/preference/ConsultarPreferencia")]
        public async Task<HttpResponseMessage> ConsultarPreferencia(Guid id)
        {
            try
            {
                var client = new PreferenceClient();
                var preference = await client.GetAsync(id.ToString());

                return Request.CreateResponse(HttpStatusCode.OK, preference);

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

        
    }
}
