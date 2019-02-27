using System.Threading.Tasks;

namespace HeartsApp.Seeders
{
    public interface ISeeder
    {
        string Name { get; }
        Task SeedDb(HeartsContext context);
    }
}