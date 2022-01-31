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

        [HttpGet("Personagens")]
        [ProducesResponseType(typeof(Personagem), (int)StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(Retorno), (int)StatusCodes.Status204NoContent)]
        [ProducesResponseType(typeof(Retorno), (int)StatusCodes.Status400BadRequest)]

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
                            foreach (var character in result.data.results)
                            {
                                arq.Write("ID:" + character.id + "\n" + "Nome:" + character.name + "\n" + "Descricao:" + character.description + "\n");

                                foreach (var comic in character.comics.items)
                                {
                                    arq.Write("Comics:" + comic.name + "\n");
                                }
                                foreach (var series in character.series.items)
                                {
                                    arq.Write("Series:" + series.name + "\n");
                                }
                                foreach (var story in character.stories.items)
                                {
                                    arq.Write("Stories:" + story.name + "\n");
                                }
                                foreach (var events in character.events.items)
                                {
                                    arq.Write("Events:" + events.name + "\n");
                                }

                                arq.Write("----------------------------------------------------------------------------------- \n");

                            }
                            arq.Close();
                        }

                        return result.data.results;
                    }
                    else
                    {
                        return new Retorno { code = StatusCodes.Status200OK, mensagem = "Nenhum item encontrado!!!"};
                    }
                }
            }
            catch (Exception e)
            {
                return new Retorno { code = StatusCodes.Status400BadRequest, mensagem = "Aconteceu um erro inesperado!!!" };
            }




        }

        [ApiExplorerSettings(IgnoreApi = true)]
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
