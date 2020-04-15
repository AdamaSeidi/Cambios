using System.Net;
namespace Cambios.Servicos
{
    using Modelos;


    public class NetworkService // este classe vai disponibilizar a ligação de internet
    {
        public Response CheckConnection()
        {
            var client = new WebClient();
            try
            {
                using (client.OpenRead("http://clients3.google.com/generate_204"))
                {
                    return new Response
                    {
                        IsSuccess = true
                    };

                }
            }
            catch 
            {
                return new Response 
                { 
                    IsSuccess  = false,
                    Message = " Configure a sua Ligação a Internet "      
                };

            }
        }
    }
}
