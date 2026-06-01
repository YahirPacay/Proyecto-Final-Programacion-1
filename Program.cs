using System;
using System.Text.Json;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

class Program
{
    static async Task Main()
    {
        
        string apiKey = Environment.GetEnvironmentVariable("GEMINI_API_KEY2");
        
        if (string.IsNullOrEmpty(apiKey))
        {
            Console.WriteLine("Error: No se encontró la API Key en el sistema.");
            return;
        }
        Console.WriteLine("API Key cargada con éxito de forma segura.");
        

        string urlDelEndpointDeGoogle = $"https://generativelanguage.googleapis.com/v1beta/models/gemini-2.5-flash:generateContent?key={apiKey}";        
        

        while (true)
        {
            Console.Write("Escribe tu consulta para Gemini: ");
            string instruccionDelUsuario = Console.ReadLine().ToLower() ?? "";

            if (string.IsNullOrWhiteSpace(instruccionDelUsuario))
                continue;

            if (instruccionDelUsuario.Equals("salir", StringComparison.OrdinalIgnoreCase))
                break;

            // Documentacion de api Gemini estructura
            var estructuraPeticionGemini = new
            {
                contents = new[]
                {
                    new 
                    { 
                        parts = new[] 
                        { 
                            new { text = instruccionDelUsuario } 
                        } 
                    }
                }
            };

            //Convierte a Json
            string jsonCuerpoPeticion = JsonSerializer.Serialize(estructuraPeticionGemini);  
            
            //Petricion rest 
            using (var clienteHttp = new HttpClient())
            {
                // Definimos el contenido HTTP y especificamos que es un JSON codificado en UTF-8
                var contenidoHttpAEnviar = new StringContent(jsonCuerpoPeticion, Encoding.UTF8, "application/json");

                try
                {
                    //Console.WriteLine($"Enviando petición REST a Gemini ({urlDelEndpointDeGoogle})...\n");
        
                    // HttpResponseMessage respuestaBrutaDelServidor = await clienteHttp.PostAsync(urlDelEndpointDeGoogle, contenidoHttpAEnviar);
                    // string respuesta = await respuestaBrutaDelServidor.Content.ReadAsStringAsync();
                    // Console.WriteLine($"Status: {(int)respuestaBrutaDelServidor.StatusCode}");
                    // Console.WriteLine(respuesta);

                    // Realizamos la solicitud POST al servidor de Google
                    HttpResponseMessage respuestaBrutaDelServidor = await clienteHttp.PostAsync(urlDelEndpointDeGoogle, contenidoHttpAEnviar);
                    
                    // Si el código HTTP no es exitoso lanzará una excepción
                    respuestaBrutaDelServidor.EnsureSuccessStatusCode();

                    //respuesta del servidor en formato de texto JSON
                    string jsonRespuestaRecibido = await respuestaBrutaDelServidor.Content.ReadAsStringAsync();

                    
                    //Console.WriteLine(" Restpuesta recibida JSON");
                    //Console.WriteLine(jsonRespuestaRecibido);

                    using JsonDocument documento = JsonDocument.Parse(jsonRespuestaRecibido);

                    //
                    string textoGenerado = documento.RootElement
                        .GetProperty("candidates")[0]
                        .GetProperty("content")
                        .GetProperty("parts")[0]
                        .GetProperty("text")
                        .GetString() ?? "Sin respuesta";

                    Console.WriteLine("\n================ Respuesta de Gemini ================");
                    Console.WriteLine(textoGenerado);
                    Console.WriteLine("=====================================================");
                    
                    //
                    Console.WriteLine("=================================================================");
                }
                
                catch (HttpRequestException exHttp)
                {
                    Console.WriteLine($" Error de comunicación HTTP: {exHttp.Message}");
                }
                catch (Exception exInesperado)
                {
                    Console.WriteLine($" Ocurrió un error inesperado: {exInesperado.Message}");
                }
                
            }  
        }

    }
}