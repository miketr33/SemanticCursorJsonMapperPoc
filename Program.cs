// See https://aka.ms/new-console-template for more information

using SemanticCursorJsonMapper;

Console.WriteLine(
    "Hello, this POC is going to take an set of example json documents, each are slightly different to each other to represent unreliable data. We are going to use Semantic Cursor to identify the most relevant/likely fields and map them to a static model of our choosing. This will mean properties such as firstName could be found whatever level they are at or change to in the future");

// Example of an unreliable JSON document
var jsonExample1 = @"{
                ""firstName"": ""John"",
                ""lastName"": ""Doe"",
                ""email"": ""john.doe@example.com"",
                ""phone"": ""555-123-4567"",
                ""primaryAddress"": {
                    ""street"": ""123 Main St"",
                    ""city"": ""Springfield"",
                    ""state"": ""IL"",
                    ""postalCode"": ""62701"",
                    ""country"": ""USA""
                },
                ""previousAddresses"": [
                    {
                        ""street"": ""456 Oak Ave"",
                        ""city"": ""Chicago"",
                        ""state"": ""IL"",
                        ""postalCode"": ""60601"",
                        ""country"": ""USA""
                    },
                    {
                        ""street"": ""789 Pine Blvd"",
                        ""city"": ""New York"",
                        ""state"": ""NY"",
                        ""postalCode"": ""10001"",
                        ""country"": ""USA""
                    }
                ],
                ""isMatch"": true,
                ""referenceId"": ""REF123456""
            }";

// Example of same data with different structure
var jsonExample2 = @"{
                ""person"": {
                    ""first_name"": ""John"",
                    ""last_name"": ""Doe"",
                    ""contact"": {
                        ""phone"": ""555-123-4567""
                    }
                },
                ""address"": {
                    ""line1"": ""123 Main St"",
                    ""city"": ""Springfield"",
                    ""state"": ""IL"",
                    ""zipCode"": ""62701"",
                    ""country"": ""USA"",
                    ""email"": ""john.doe@example.com""
                },
                ""addressHistory"": [
                    {
                        ""line1"": ""456 Oak Ave"",
                        ""city"": ""Chicago"",
                        ""state"": ""IL"",
                        ""zipCode"": ""60601"",
                        ""country"": ""USA""
                    },
                    {
                        ""line1"": ""789 Pine Blvd"",
                        ""city"": ""New York"",
                        ""state"": ""NY"",
                        ""zipCode"": ""10001"",
                        ""country"": ""USA""
                    }
                ],
                ""verification"": {
                    ""match"": true
                },
                ""metadata"": {
                    ""refId"": ""REF123456""
                },
                ""referenceId"": ""DUPLICATE_REF789""
            }";

var jsonExample3 = File.ReadAllText("C:\\Users\\mikej\\RiderProjects\\CPD\\SemanticCursorExperiment\\SemanticCursorJsonMapper\\ExampleJsonFiles\\Example3.json");
var mapper = new PersonMapper();

Console.WriteLine("Example 1 Parsing:");
var person1 = mapper.MapFromJson(jsonExample1);
PrintPersonInfo(person1);


Console.WriteLine("\nExample 2 Parsing:");
var person2 = mapper.MapFromJson(jsonExample2);
PrintPersonInfo(person2);

Console.WriteLine("\nExample 3 Parsing:"); // Kind of works but needs adjustments in the mapper to cover the email and phone in the example3.
var person3 = mapper.MapFromJson(jsonExample3);
PrintPersonInfo(person3);


static void PrintPersonInfo(PersonModel person)
{
    Console.WriteLine($"Name: {person.FirstName} {person.LastName}");
    Console.WriteLine($"Email: {person.Email}");
    Console.WriteLine($"Phone: {person.Phone}");
    Console.WriteLine($"Reference ID: {person.ReferenceId}");
    Console.WriteLine($"Is Match: {person.IsMatch}");
            
    if (person.PrimaryAddress != null)
    {
        Console.WriteLine("Primary Address:");
        Console.WriteLine($"  {person.PrimaryAddress.Street}");
        Console.WriteLine($"  {person.PrimaryAddress.City}, {person.PrimaryAddress.State} {person.PrimaryAddress.PostalCode}");
        Console.WriteLine($"  {person.PrimaryAddress.Country}");
    }
            
    if (person.PreviousAddresses?.Count > 0)
    {
        Console.WriteLine("Previous Addresses:");
        foreach (var addr in person.PreviousAddresses)
        {
            Console.WriteLine($"  {addr.Street}");
            Console.WriteLine($"  {addr.City}, {addr.State} {addr.PostalCode}");
            Console.WriteLine($"  {addr.Country}");
            Console.WriteLine();
        }
    }
}