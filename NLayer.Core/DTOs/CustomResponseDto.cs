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
        //Geriye bir data dönmek istemediğim durum için (örneğin Success(int statusCode) methodundaki gibi) bir class daha oluşturuyoruz. (NoContentDto)
        public T Data { get; set; }

        //JsonIgnore diyerek client tarafına StatusCode'u dönmemek için yaptık.
        [JsonIgnore]
        public int StatusCode { get; set; }
        public List<string> Errors { get; set; }

        //Static factory method tanımladık factory desing patternden gelir. Success ve Fail durumlarına göre geriye new instance lar dönüyoruz. (Nesne üretme yolu)
        public static CustomResponseDto<T> Success(int statusCode,T data)
        {
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
