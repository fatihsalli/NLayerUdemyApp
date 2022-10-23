namespace NLayer.Core.UnitOfWorks
{
    public interface IUnitOfWork
    {
        //Neden SaveChange görevini ayrı tanımladık? Her repositorydeki işlem sonrası SaveChange tanımlasaydık sıralı işlemlerde sorun çıkabilirdi. Örneğin 1.işlemi yaptık kaydettik sıra 2.işlemde kaydederken hata aldık ama 1.işlemi kaydettiğimiz için bu durum sorun teşkil etmektedir. Bu sebeple daha kontrollü bir şekilde IUnitOfWork Interface'i ile SaveChange metodunu çağıracağız.

        //SaveChangeAsync
        Task CommitAsync();

        //SaveChange
        void Commit();


    }
}
