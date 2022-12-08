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
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using System.Web.Http;

namespace MercadoPagoAPI.Controllers
{
    public class CriptController : ApiController
    {


        //api/criar
        [HttpGet]
        [Route("api/cript/encrypt")]
        public HttpResponseMessage Encrypt(string text)
        {
            try
            {
                return Request.CreateResponse(HttpStatusCode.OK, new 
                {
                    resultado = Encryptxx(text)
                });
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
        [Route("api/cript/decrypt")]
        public async Task<HttpResponseMessage> Decrypt(string text)
        {
            try
            {
                return Request.CreateResponse(HttpStatusCode.OK, new
                {
                    resultado = Decryptxx(text)
                });
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


        public string Encryptxx(string text)
        {

            byte[] src = Encoding.UTF8.GetBytes(text);
            byte[] key = Encoding.ASCII.GetBytes("0123456789abcdef");
            RijndaelManaged aes = new RijndaelManaged();
            aes.Mode = CipherMode.ECB;
            aes.Padding = PaddingMode.PKCS7;
            aes.KeySize = 128;

            using (ICryptoTransform encrypt = aes.CreateEncryptor(key, null))
            {
                byte[] dest = encrypt.TransformFinalBlock(src, 0, src.Length);
                encrypt.Dispose();
                //return Convert.ToBase64String(dest);
                return ByteArrayToString(dest);
            }
        }

        public string Decryptxx(string text)
        {

            //byte[] src = Convert.FromBase64String(text);
            byte[] src = StringToByteArray(text);

            RijndaelManaged aes = new RijndaelManaged();
            byte[] key = Encoding.ASCII.GetBytes("0123456789abcdef");
            aes.KeySize = 128;
            aes.Padding = PaddingMode.PKCS7;
            aes.Mode = CipherMode.ECB;
            using (ICryptoTransform decrypt = aes.CreateDecryptor(key, null))
            {
                byte[] dest = decrypt.TransformFinalBlock(src, 0, src.Length);
                decrypt.Dispose();
                return Encoding.UTF8.GetString(dest);
            }
        }



        public string ByteArrayToString(byte[] ba)
        {
            return BitConverter.ToString(ba).Replace("-", "");
        }

        public static byte[] StringToByteArray(String hex)
        {
            int NumberChars = hex.Length;
            byte[] bytes = new byte[NumberChars / 2];
            for (int i = 0; i < NumberChars; i += 2)
                bytes[i / 2] = Convert.ToByte(hex.Substring(i, 2), 16);
            return bytes;
        }
    }
}
