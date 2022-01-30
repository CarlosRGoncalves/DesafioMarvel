using ApiMarvel.DTO;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Security.Cryptography;
using System.Text;
using System.IO;
using Microsoft.Extensions.Configuration;
using System.Net.Http.Json;
using Newtonsoft.Json;

namespace ApiMarvel.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class MarvelController : ControllerBase
    {
        private readonly IConfiguration Configuration;

        public MarvelController(IConfiguration configuration) => Configuration = configuration;

        [HttpGet]
        public async Task<object> ConsumoApi()

        {
            try
            {
                using (var client = new HttpClient())
                {

                    var Url = Configuration["ConfigsAPI:BaseURL"];
                    string apikey = Configuration["ConfigsAPI:PublicKey"];
                    string privatekey = Configuration["ConfigsAPI:PrivateKey"]; //colocar .env
                    string ts = DateTime.Now.Ticks.ToString();
                    string hash = GerarHash(ts, apikey,
                                privatekey);

                    var directory = Directory.GetCurrentDirectory();

                    var response = await client.GetStringAsync(Url + "?apikey=" + apikey + "&ts=" + ts + "&hash=" + hash);

                    if (response != null)
                    {
                        var result = JsonConvert.DeserializeObject<Personagem>(response);

                        using (StreamWriter arq = new StreamWriter(directory + "\\personagensmarvel.txt"))
                        {
                            foreach (var pertxt in result.data.results)
                            {
                                arq.Write("ID:" + pertxt.id + "\n" + "Nome:" + pertxt.name + "\n" + "Descricao:" + pertxt.description + "\n");

                                foreach (var pertxt2 in pertxt.comics.items)
                                {
                                    arq.Write("Comics:" + pertxt2.name + "\n");
                                }
                                foreach (var pertxt3 in pertxt.series.items)
                                {
                                    arq.Write("Series:" + pertxt3.name + "\n");
                                }
                                foreach (var pertxt4 in pertxt.stories.items)
                                {
                                    arq.Write("Stories:" + pertxt4.name + "\n");
                                }
                                foreach (var pertxt5 in pertxt.events.items)
                                {
                                    arq.Write("Events:" + pertxt5.name + "\n");
                                }

                                arq.Write("----------------------------------------------------------------------------------- \n");

                            }
                            arq.Close();
                        }

                        return result.data.results;
                    }
                    else
                    {
                        return StatusCodes.Status200OK;
                    }




                }
            }
            catch (Exception e)
            {
                return StatusCodes.Status400BadRequest;
            }




        }
        public string GerarHash(string ts, string apikey, string privatekey)
        {

            byte[] bytes =
                Encoding.UTF8.GetBytes(ts + privatekey + apikey);

            var gerador = MD5.Create();


            byte[] hash = gerador.ComputeHash(bytes);

            return BitConverter.ToString(hash)
                .ToLower().Replace("-", String.Empty);
        }
    }
}
