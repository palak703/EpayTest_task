using Newtonsoft.Json;

namespace Simulator
{
    internal class Program
    {
        public class Customer
        {
            public string FirstName { get; set; }
            public string LastName { get; set; }
            public int Age { get; set; }
            public int Id { get; set; }

            public override string ToString()
            {
                return $"{{ firstName: '{FirstName}', lastName: '{LastName}', age: {Age}, id: {Id} }}";
            }
        }
        async static void Main(string[] args)
        {
            var simulator = new RequestSimulator();

            await simulator.SimulateRequests("https://localhost:44357", 5);
        }

        class RequestSimulator
        {
            private readonly string[] FirstNames = { "Leia", "Sadie", "Jose", "Sara", "Frank", "Dewey", "Tomas", "Joel", "Lukas", "Carlos" };
            private readonly string[] LastNames = { "Liberty", "Ray", "Harrison", "Ronan", "Drew", "Powell", "Larsen", "Chan", "Anderson", "Lane" };

            public async Task SimulateRequests(string baseUrl, int numberOfRequests)
            {
                var tasks = new List<Task>();
                var random = new Random();

                for (int i = 0; i < numberOfRequests; i++)
                {
                    var customers = new List<Customer>();
                    for (int j = 0; j < 2; j++)
                    {
                        var customer = new Customer
                        {
                            FirstName = FirstNames[random.Next(FirstNames.Length)],
                            LastName = LastNames[random.Next(LastNames.Length)],
                            Age = random.Next(18, 90),
                            Id = i * 2 + j + 1 // Sequential ID
                        };
                        customers.Add(customer);
                    }

                    var task = Task.Run(async () =>
                    {
                        await SendPostRequest(baseUrl + "/customers", customers);
                        await Task.Delay(100); // Simulate slight delay between requests
                    });
                    tasks.Add(task);
                }

                await Task.WhenAll(tasks);
            }

            private async Task SendPostRequest(string url, List<Customer> customers)
            {
                using var httpClient = new System.Net.Http.HttpClient();
                var jsonContent = JsonConvert.SerializeObject(customers);
                var httpContent = new StringContent(jsonContent, System.Text.Encoding.UTF8, "application/json");
                var response = await httpClient.PostAsync(url, httpContent);
                var responseContent = await response.Content.ReadAsStringAsync();
                Console.WriteLine(responseContent);
            }
        }



    }
}
