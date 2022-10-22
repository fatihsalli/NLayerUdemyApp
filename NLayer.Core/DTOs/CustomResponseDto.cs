using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace NLayer.Core.DTOs
{
    public class CustomResponseDto<T>
    {
        //"CustomResponseDto" adında bir class oluşturduk. Endpointlerden geri dönüşleri tek bir class üzerinden yapmak için "Success" ve "Fail" durumlarını metot içerisine ekledik. Sonuç başarılı ise datayı, sonuç fail ise hataları geri dönüyoruz. Success ve Fail için iki ayrı class da oluşturabilirdik ancak bu durum client tarafında gereksiz olarak 2 adet implementasyon yapılmasına yol açmaktadır.
        //Geriye bir data dönmek istemediğim durum için (örneğin Success(int statusCode) methodundaki gibi) bir class daha oluşturuyoruz. (NoContentDto)
        public T Data { get; set; }

        //JsonIgnore diyerek client tarafına StatusCode'u dönmemek için yaptık. Zaten istek yaptıklarında StatucCode'u görüyorlar
        [JsonIgnore]
        public int StatusCode { get; set; }
        //Birden fazla hata olabileceği için List<string> tanımladık.
        public List<string> Errors { get; set; }

        //Static factory method tanımladık factory desing patternden gelir. Success ve Fail durumlarına göre geriye new instance lar dönüyoruz. (Nesne üretme yolu)
        public static CustomResponseDto<T> Success(int statusCode,T data)
        {
            //return new CustomResponseDto<T> { StatusCode = statusCode, Data = data, Errors=null};=> Errors=null da diyebilirdik ancak zaten belirtmeye gerek olmaksızın Errors=null yapacaktır. Ekstra belirtmeye gerek yoktur.
            return new CustomResponseDto<T> { StatusCode = statusCode, Data = data};
        }

        public static CustomResponseDto<T> Success(int statusCode)
        {
            return new CustomResponseDto<T> { StatusCode = statusCode};
        }

        public static CustomResponseDto<T> Fail(int statusCode, List<string> errors)
        {
            return new CustomResponseDto<T> { StatusCode = statusCode,Errors=errors };
        }

        public static CustomResponseDto<T> Fail(int statusCode, string error)
        {
            return new CustomResponseDto<T> { StatusCode = statusCode, Errors = new List<string> { error } };
        }


    }
}
