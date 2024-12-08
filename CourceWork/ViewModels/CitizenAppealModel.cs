using CourceWork.Models;

namespace CourceWork.ViewModels
{
    public class CitizenAppealModel
    {
        public CitizenAppeal CitizenAppeal { get; set; } = new CitizenAppeal();
        public int ShowId { get; set; }
        public AllShowsModel AllShows { get; set; } = new AllShowsModel();
    }
}
