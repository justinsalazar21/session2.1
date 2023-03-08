using System.Net.Http;
using System.Numerics;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace HomeWork2._1_Pet
{
    [TestClass]
    public class Pets
    {
        private static HttpClient client;

        private static readonly string BaseUrl = "https://petstore.swagger.io/v2/";

        private static readonly string PetsEndpoint = "pet";
        private static string GetURL(string enpoint) => $"{BaseUrl}{enpoint}";
        private static Uri GetURI(string endpoint) => new Uri(GetURL(endpoint));

        private readonly List<PetInfo> petListCleanUp = new List<PetInfo>();

        //test initialize
        [TestInitialize]
        public void TestInitialize()
        {
            client = new HttpClient();
        }

        //test cleanup-delete
        [TestCleanup]
        public async Task Delete()
        {
            foreach (var data in petListCleanUp)
            {
                var httpResponse = await client.DeleteAsync(GetURL($"{PetsEndpoint}/{data.Id}"));
            }

        }

        [TestMethod]
        public async Task PutMethod()
        {
            //declaration of values for category, PhotoURLs, tags
            Category petCategory = new Category(1, "House Pet");

            List<string> PhotoUrls = new List<string>();
            PhotoUrls.Add("petURLtest");

            List<CategoryT> tags = new List<CategoryT>();
            tags.Add(new CategoryT(1, "Chihuahua Smooth Coat"));

            //create json object
            PetInfo petModels = new PetInfo()
            {
                Id = 1001,
                Category = petCategory,
                Name = "Harvey",
                PhotoUrls = PhotoUrls,
                Tags = tags,
                Status = "available"
            };

            // Serialize Content
            var request = JsonConvert.SerializeObject(petModels);
            var postRequest = new StringContent(request, Encoding.UTF8, "application/json");

            // Send Post Request
            await client.PostAsync(GetURL(PetsEndpoint), postRequest);

            // Get Request
            var getResponse = await client.GetAsync(GetURI($"{PetsEndpoint}/{petModels.Id}"));

            // Deserialize Content
            var petInformation = JsonConvert.DeserializeObject<PetInfo>(getResponse.Content.ReadAsStringAsync().Result);
    
            // Update value of userData
            // assigning of new values to the previous json object
            petInformation.Category.Name = "Outside Pet";
            petInformation.Category.Id = 1002;

            petInformation.Tags.Add(new CategoryT(1002, "Chihuahua Long Coat"));
            petInformation.PhotoUrls.Add("new urlsssssss");

            petModels = new PetInfo()
            {
                Id = petInformation.Id,
                Category = petInformation.Category,
                Name = "Harvey Boy",
                PhotoUrls = petInformation.PhotoUrls,
                Tags = petInformation.Tags,
                Status = "Sold"
            };

            // Serialize Content
            request = JsonConvert.SerializeObject(petModels);
            postRequest = new StringContent(request, Encoding.UTF8, "application/json");

            // Send Put Request
            var httpResponse = await client.PutAsync(GetURL($"{PetsEndpoint}"), postRequest);

            // Get Status Code
            var statusCode = httpResponse.StatusCode;

            // Get Request
            getResponse = await client.GetAsync(GetURI($"{PetsEndpoint}/{petModels.Id}"));

            // Deserialize Content
            petInformation = JsonConvert.DeserializeObject<PetInfo>(getResponse.Content.ReadAsStringAsync().Result);

            // Add data to cleanup list
            petListCleanUp.Add(petInformation);

            // Assertion
            Assert.AreEqual(HttpStatusCode.OK, statusCode, "Status code is not equal to 201");
            Assert.AreEqual(petModels.Id, petInformation.Id, "ID doesn't match");
            Assert.AreEqual(petModels.Category.Id, petInformation.Category.Id, "Category ID doesn't match");
            Assert.AreEqual(petModels.Category.Name, petInformation.Category.Name, "Category ID doesn't match");
            Assert.AreEqual(petModels.Name, petInformation.Name, "ID doesn't match");
            Assert.AreEqual(petModels.Status, petInformation.Status, " doesn't match");
            
            ////looping to show the list of photoURLs
            //for (int i = 0; i < petModels.PhotoUrls.Count; i++)
            //{
            //    Assert.AreEqual(petModels.PhotoUrls[i], petInformation.PhotoUrls[i], "PhotoURLS are not equal");
            //}

            ////looping to show the list of tags
            //for (int i = 0; i < petModels.Tags.Count; i++)
            //{
            //    Assert.AreEqual(petModels.Tags[i], petInformation.Tags[i], "Tags are not equal");
            //}

        }

    }

}
