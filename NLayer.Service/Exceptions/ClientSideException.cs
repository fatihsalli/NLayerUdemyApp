using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NLayer.Service.Exceptions
{
    //Hata olduğu için Exception sınıfından miras aldık
    public class ClientSideException:Exception
    {
        //Uygulamada verilen exception'ın bizim ürettiğimiz mi ya da context'in kendi ürettiği bir extension mı bunu anlayabilmek ve yönetebilmek adına ClienSideException adında class tanımlayarak, bunu "UsecustomExceptionHandler" middleware'i içinde kullanıyoruz. Bu sayede extension'ın bizim gönderdiğimiz extension olduğunu anlayabiliyoruz.
        //Buradan alınan mesaj exception mesajına aktarılacak
        public ClientSideException(string message):base(message) //Exception constructorına gönderilir.
        {

        }

    }
}
