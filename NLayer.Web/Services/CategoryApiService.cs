using NLayer.Core.DTOs;

namespace NLayer.Web.Services
{
    //Mvc projemizi Api ye dahil etmek için oluşturduk.
    public class CategoryApiService
    {
        private readonly HttpClient _httpClient;

        public CategoryApiService(HttpClient httpClient)
        {
            //httpclient ı kendimiz üretmiyoruz DI Container'a üretecek şekilde ayarlamalıyız. Best practise budur.
            _httpClient = httpClient;
        }

        public async Task<List<CategoryDto>> GetAllAsync()
        {
            var response = await _httpClient.GetFromJsonAsync<CustomResponseDto<List<CategoryDto>>>("categories");
            return response.Data;
        }



    }
}
